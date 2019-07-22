namespace SIS.MVC.Services
{
    using SIS.MVC.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ServiceCollection : IServiceCollection
    {
        private IDictionary<Type, TypeAndParameters> container = new Dictionary<Type, TypeAndParameters>();
        private IDictionary<Type, Func<object>> containerFunc = new Dictionary<Type, Func<object>>();

        public void AddService<T1, T2>(params object[] parameters)
        {
            container[typeof(T1)] = new TypeAndParameters(typeof(T2), parameters);
        }

        public void AddService<T1>(Func<object> action)
        {
            containerFunc[typeof(T1)] = action;
        }

        public object CreateInstance(Type t1)
        {
            Type instanceType = null;
            object[] parametersProvided = null;

            if (containerFunc.ContainsKey(t1))
            {
                return containerFunc[t1].Invoke();
            }
            if (container.ContainsKey(t1))
            {
                instanceType = container[t1].Type;
                if (container[t1].Parameters.Any())
                {
                    parametersProvided = container[t1].Parameters.ToArray();
                }
            }
            else if (t1.IsAbstract || t1.IsInterface || t1.IsValueType)
            {
                Console.WriteLine($"Type {t1.Name} can not be instantanced, a null value is set for it!");
                return null;
            }
            else
            {
                instanceType = t1;
            }

            if (parametersProvided != null)
            {
                return Activator.CreateInstance(instanceType, parametersProvided);
            }

            Queue<object> parameters = new Queue<object>();
            ConstructorInfo ctor = GetBestConstructor(instanceType);
            var parameterTypes = ctor.GetParameters().Select(x => x.ParameterType).ToArray();
            foreach (var parameter in parameterTypes)
            {
                object parameterInstance = CreateInstance(parameter);
                parameters.Enqueue(parameterInstance);
            }
            object instance = ctor.Invoke(parameters.ToArray());
            return instance;
        }

        private ConstructorInfo GetBestConstructor(Type type)
        {
            return type.GetConstructors().OrderBy(x => x.GetParameters()
                                       .All(p => container.ContainsKey(p.ParameterType)))
                                       .OrderBy(x => x.GetParameters().Count())
                                       .First();
        }

        private class TypeAndParameters
        {
            private ICollection<object> parameters;
            public TypeAndParameters(Type type, object[] parameters)
            {
                this.Type = type;
                this.parameters = parameters;
            }
            public IReadOnlyCollection<object> Parameters => parameters.ToArray();
            public Type Type { get; }
        }
    }
}