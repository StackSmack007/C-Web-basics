namespace SIS.MVC.Contracts
{
using System;
    public interface IServiceCollection
    {
        void AddService<T1, T2>(params object[] parameters);

        void AddService<T1>(Func<object> action);

        object CreateInstance(Type t1);
    }
}
