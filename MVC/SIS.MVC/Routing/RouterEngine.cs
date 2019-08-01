namespace SIS.MVC.Routing
{
    using SIS.HTTP.Enums;
    using SIS.HTTP.Headers;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using SIS.MVC.Contracts;
    using SIS.MVC.Services;
    using SIS.WebServer.Routing;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    internal class RouterEngine
    {
        internal static ILogger Logger { get; set; }

        internal static void ConfigureRoutesByNamesConvention(ServerRoutingTable serverRoutingTable)
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

                    EnlistRoute(defaultMethod, path, controllerType, methodInfo, serverRoutingTable);
                }

                foreach (MethodInfo methodInfo in controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(m => m.ReturnType == typeof(IHttpResponse) && m.GetCustomAttributes<HttpAttribute>().Any(x => x.Path == null)))
                {//case of pathless HttpAttribute (HttpPostMost likely)
                    string path = "/" + viewFolderName + "/" + methodInfo.Name;
                    foreach (var methodType in methodInfo.GetCustomAttributes<HttpAttribute>().Where(x => x.Path == null).Select(x => x.MethodType))
                    {
                        if (serverRoutingTable.Routes[methodType].Keys.Any(x => x == path))
                        {
                            continue;
                        } //the route is registered manually so it is skipped!
                        EnlistRoute(methodType, path, controllerType, methodInfo, serverRoutingTable);
                    }
                }
            }
        }

        internal static void ConfigureRoutingFromAttributes(ServerRoutingTable serverRoutingTable)
        {
            Type[] controllerClasses = Assembly.GetEntryAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Controller)) && !x.IsAbstract).ToArray();

            foreach (Type controller in controllerClasses)
            {
                MethodInfo[] actionMethods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                                       .Where(x => x.GetCustomAttributes<HttpAttribute>().Where(at => at.Path != null).Any()).ToArray();

                foreach (MethodInfo methodInfo in actionMethods)
                {
                    HttpAttribute[] attributes = methodInfo.GetCustomAttributes<HttpAttribute>().ToArray();

                    foreach (HttpAttribute attribute in attributes.Where(at => at.Path != null))
                    {
                        HttpRequestMethod methodType = attribute.MethodType;
                        string path = attribute.Path;
                        EnlistRoute(methodType, path, controller, methodInfo, serverRoutingTable);
                    }
                }
            }
        }

        internal static void RegisterStaticFiles(ServerRoutingTable serverRoutingTable)
        {
            string location = Locator.GetPathOfFolder(WebHost.Configurations.LocationOfRootFolder);
            var filePaths = Directory.EnumerateFiles(location, "*.*", SearchOption.AllDirectories).ToArray();
            foreach (var file in filePaths)
            {
                string fileName = Regex.Match(file, @".+[\/\\](.+)").Groups[1].Value;
                if (serverRoutingTable.Routes[HttpRequestMethod.Get].ContainsKey(fileName))
                {
                    Logger.Log($"File with name <{fileName}> occures more than once in the root folder. If content of it differs change its name!");
                }

                serverRoutingTable.Routes[HttpRequestMethod.Get]["/" + fileName] = (request) =>
                  {
                      IHttpResponse response = new HttpResponse(System.Net.HttpStatusCode.OK);
                      response.Content = File.ReadAllBytes(file);
                      response.Headers.Add(new HttpHeader(HttpHeader.ContentLengthKey, response.Content.Length.ToString()));
                      response.Headers.Add(new HttpHeader(HttpHeader.ContentDispositionKey, "inline"));
                      return response;
                  };
            }
        }

        private static void EnlistRoute(HttpRequestMethod methodType, string path, Type controllerType, MethodInfo methodInfo, ServerRoutingTable serverRoutingTable)
        {
            serverRoutingTable.Routes[methodType][path] = (IHttpRequest request) =>
            {
                var controllerInstance = WebHost.ServiceContainer.CreateInstance(controllerType);
                PropertyInfo requestProperty = controllerType.GetProperty("Request");
                requestProperty.SetValue(controllerInstance, request);

                #region AuthorisedAttributeCheckAndRedirect
                Type baseControllerType = typeof(Controller);
                AuthorisedAttribute attributeFound = methodInfo.GetCustomAttributes<AuthorisedAttribute>().FirstOrDefault();
                bool noLoggedInUser = baseControllerType.GetProperty("CurentUser", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(controllerInstance) is null;
                if (attributeFound != null && noLoggedInUser)
                {
                    string redirectString = attributeFound.AltPath;
                    if (redirectString is null)
                    {
                        redirectString = baseControllerType.GetField("defaultAuthorisedRedirectAdress", BindingFlags.Static | BindingFlags.NonPublic).GetValue(controllerInstance) as string;
                    }
                    MethodInfo redirectMethod = controllerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.Name == "RedirectResult");
                    return (IHttpResponse)redirectMethod.Invoke(controllerInstance, new object[] { redirectString });
                }
                #endregion

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
                Console.WriteLine();

                if (requiredParam.ParameterType.GetInterfaces().Contains(typeof(IEnumerable<string>)))
                {
                    if (!providedParameters.ContainsKey(requiredParam.Name))
                    {
                        parametersData.Add(new string[0]);
                    }
                    else
                    {
                        parametersData.Add(providedParameters[requiredParam.Name]);
                    }
                    continue;
                }

                else if (requiredParam.ParameterType.IsValueType || requiredParam.ParameterType == typeof(string))
                {
                    if (!providedParameters.ContainsKey(requiredParam.Name))
                    {
                        Logger.Log($"parameter {requiredParam.Name} was not provided. The value in method is set to null!");
                        parametersData.Add(null);
                        continue;
                    }
                    else if (providedParameters[requiredParam.Name] is null || providedParameters[requiredParam.Name].GetType() != requiredParam.ParameterType)
                    {
                        try
                        {
                            parametersData.Add(Convert.ChangeType(providedParameters[requiredParam.Name], requiredParam.ParameterType));

                        }
                        catch (Exception)
                        {
                            Logger.Log($"parameter {requiredParam.Name} was provided provided but was unconvertable to {requiredParam.ParameterType.FullName}. The value in method is set to null!");
                            parametersData.Add(null);
                        }
                        continue;
                    }
                    parametersData.Add(providedParameters[requiredParam.Name]);
                    continue;
                }
                ConstructorInfo subClassConstructor = requiredParam.ParameterType.GetConstructors()
                                                         .Where(x => x.GetParameters().All(p => providedParameters.ContainsKey(p.Name) || p.ParameterType.GetInterfaces().Contains(typeof(IEnumerable<string>))))
                                                         .OrderByDescending(x => x.GetParameters().Count())
                                                         .FirstOrDefault();

                if (subClassConstructor is null)
                {
                    Logger.Log($"Parameter {requiredParam.Name} was a class and had not all parameters provided.The value in method is set to null!");
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
