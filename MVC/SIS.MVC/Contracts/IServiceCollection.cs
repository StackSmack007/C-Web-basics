namespace SIS.MVC.Contracts
{
using System;
    public interface IServiceCollection
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T1">InterfaceName</typeparam>
        /// <typeparam name="T2">ClassName</typeparam>
        /// <param name="parameters">Parameters if ctor needs them!</param>
        void AddService<T1, T2>(params object[] parameters);

        void AddService<T1>(Func<object> action);

        object CreateInstance(Type t1);
    }
}
