namespace SIS.MVC
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using SIS.MVC.Contracts;
    using SIS.MVC.Loggers;
    using SIS.MVC.Services;
    using SIS.WebServer;
    using SIS.WebServer.Routing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using SIS.MVC.ViewEngine;
    using System.IO;

    /// <summary>
    ///  Every DTO must have constructor (Non empty!) accepting parameters exact number,
    /// <para> types and naming (Case Sensitive) as the dada provided by Views!!! </para>
    /// Overloading of constructors in DTOs is an option.
    /// <para> Nesting of complex types is an option but there must be unique names for every parameter provided by request! </para>
    /// </summary>
    public class WebHost
    {
        private static ServerRoutingTable serverRoutingTable;
        private static IServiceCollection serviceContainer;
        private static ILogger logger;

        static WebHost()
        {
            ClearTempFolder(@"SIS.MVC/ViewEngine/GeneratedModels");
            serverRoutingTable = new ServerRoutingTable();
            serviceContainer = new ServiceCollection();
            logger = new ConsoleLogger();
        }

        private static void ClearTempFolder(string subPath)
        {
            string destinationFolderPath = Locator.GetPathOfFolder(subPath);

            if (Directory.Exists(destinationFolderPath))
            {
                var files = Directory.EnumerateFiles(destinationFolderPath).ToHashSet();
                foreach (var filePath in files)
                {
                    File.Delete(filePath);
                }
            }
        }

        public static IServiceCollection ServiceContainer => serviceContainer;

        public static void Start(int port, IMvcApplication application)
        {
            ConfigureRoutingFromAttributes();
            application.ConfigureRouting(serverRoutingTable);
            ConfigureRoutesByNamesConvention();
            application.ConfigureServices(serviceContainer);
            serviceContainer.AddService<IViewEngine, View_Engine>();
            Server server = new Server(port, serverRoutingTable);
            server.Run();
        }

        /// <summary>
        /// Makes Routings via controller and method Names if possible if no other route is given!
        /// </summary>
        private static void ConfigureRoutesByNamesConvention()
        {
            Type[] controllerClassesWithUnregistredMethods = Assembly.GetEntryAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Controller)) && !x.IsAbstract)
                .Where(x => x.GetMethods().Any(m => m.ReturnType == typeof(IHttpResponse) &&
                      (!m.GetCustomAttributes<HttpAttribute>().Any() || m.GetCustomAttributes<HttpAttribute>().Any(at => at.Path == null)))).ToArray();

            if (!controllerClassesWithUnregistredMethods.Any()) return;

            foreach (Type controllerType in controllerClassesWithUnregistredMethods)
            {
                string viewFolderName = controllerType.Name.Replace("Controller", "");
                foreach (MethodInfo methodInfo in controllerType.GetMethods().Where(m => m.ReturnType == typeof(IHttpResponse) && !m.GetCustomAttributes<HttpAttribute>().Any()))
                {//case of no HttpAttribute
                    HttpRequestMethod defaultMethod = HttpRequestMethod.Get;
                    string path = "/" + viewFolderName + "/" + methodInfo.Name;
                    if (serverRoutingTable.Routes[defaultMethod].Keys.Any(x => x == path)) continue; //the route is registered manually so it is skipped!

                    EnlistRoute(defaultMethod, path, controllerType, methodInfo);
                }

                foreach (MethodInfo methodInfo in controllerType.GetMethods().Where(m => m.ReturnType == typeof(IHttpResponse) && m.GetCustomAttributes<HttpAttribute>().Any(x=>x.Path == null)))
                {//case of pathless HttpAttribute (HttpPostMost likely)
                    string path = "/" + viewFolderName + "/" + methodInfo.Name;               
                    foreach (var methodType in methodInfo.GetCustomAttributes<HttpAttribute>().Where(x => x.Path == null).Select(x=>x.MethodType))
                    {
                        if (serverRoutingTable.Routes[methodType].Keys.Any(x => x == path))
                        {
                            continue;
                        } //the route is registered manually so it is skipped!
                        EnlistRoute(methodType, path, controllerType, methodInfo);
                    }
                }
            }
        }

        private static void ConfigureRoutingFromAttributes()
        {
            Type[] controllerClasses = Assembly.GetEntryAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Controller)) && !x.IsAbstract).ToArray();

            foreach (Type controller in controllerClasses)
            {
                MethodInfo[] actionMethods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                                       .Where(x => x.GetCustomAttributes<HttpAttribute>().Where(at=>at.Path!=null).Any()).ToArray();

                foreach (MethodInfo methodInfo in actionMethods)
                {
                    HttpAttribute[] attributes = methodInfo.GetCustomAttributes<HttpAttribute>().ToArray();

                    foreach (HttpAttribute attribute in attributes.Where(at => at.Path != null))
                    {
                        HttpRequestMethod methodType = attribute.MethodType;
                        string path = attribute.Path;
                        EnlistRoute(methodType, path, controller, methodInfo);
                    }
                }
            }
        }

        private static void EnlistRoute(HttpRequestMethod methodType, string path, Type controllerType, MethodInfo methodInfo)
        {
            serverRoutingTable.Routes[methodType][path] = (IHttpRequest request) =>
            {
                var controllerInstance = serviceContainer.CreateInstance(controllerType);

                PropertyInfo requestProperty = controllerType.GetProperty("Request");
                requestProperty.SetValue(controllerInstance, request);

                #region ModelBinding
                Queue<ParameterInfo> actionParameters = new Queue<ParameterInfo>(methodInfo.GetParameters());
                List<object> parametersData = new List<object>();
                if (actionParameters.Any())
                {
                    Dictionary<string, object> providedParameters;

                    if (methodType is HttpRequestMethod.Get)
                    {
                        providedParameters = request.QueryData;
                    }
                    else if (methodType is HttpRequestMethod.Post)
                    {
                        providedParameters = request.FormData;
                    }
                    else
                    {
                        throw new InvalidOperationException($"No data provided for action {methodInfo.Name} requiring parameters!");
                    }
                    FillData(parametersData, actionParameters, providedParameters);
                }
                #endregion

                return (IHttpResponse)methodInfo.Invoke(controllerInstance, parametersData.ToArray());
            };
        }

        private static void FillData(List<object> parametersData, Queue<ParameterInfo> requiredParametersByMethod, Dictionary<string, object> providedParameters)
        {
            while (requiredParametersByMethod.Any())
            {
                ParameterInfo requiredParam = requiredParametersByMethod.Dequeue();

                if (requiredParam.ParameterType.IsValueType || requiredParam.ParameterType == typeof(string))
                {
                    if (!providedParameters.ContainsKey(requiredParam.Name))
                    {
                        logger.Log($"parameter {requiredParam.Name} was not provided. The value in method is set to null!");
                        parametersData.Add(null);
                        continue;
                    }
                    else if (providedParameters[requiredParam.Name].GetType() != requiredParam.ParameterType)
                    {
                        try
                        {
                            parametersData.Add(Convert.ChangeType(providedParameters[requiredParam.Name], requiredParam.ParameterType));

                        }
                        catch (Exception)
                        {
                            logger.Log($"parameter {requiredParam.Name} was provided provided but was unconvertable to {requiredParam.ParameterType.FullName}. The value in method is set to null!");
                            parametersData.Add(null);
                        }
                        continue;
                    }

                    parametersData.Add(providedParameters[requiredParam.Name]);
                    continue;
                }

                ConstructorInfo subClassConstructor = requiredParam.ParameterType.GetConstructors()
                                                         .Where(x => x.GetParameters().All(p => providedParameters.ContainsKey(p.Name)))
                                                         .OrderByDescending(x => x.GetParameters().Count())
                                                         .FirstOrDefault();

                if (subClassConstructor is null)
                {
                    logger.Log($"Parameter {requiredParam.Name} was a class and had not all parameters provided.The value in method is set to null!");
                    parametersData.Add(null);
                    continue;
                }
                List<object> subParametersData = new List<object>();
                Queue<ParameterInfo> subRequestParameters = new Queue<ParameterInfo>(subClassConstructor.GetParameters());
                FillData(subParametersData, subRequestParameters, providedParameters);

                var subInstance = subClassConstructor.Invoke(subParametersData.ToArray());
                parametersData.Add(subInstance);
            }
        }
    }
}