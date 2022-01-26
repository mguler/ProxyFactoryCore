using System;

namespace ProxyFactoryCore.Abstract
{
    public interface IDynamicProxyFactory
    {
        IInterceptorConfiguration<T> Register<T>();

        T Create<T>(params object[] args);
        object Create<T>(object obj);
        void Add<T>(IInterceptorConfiguration<T> interceptorConfiguration);
        Type GetProxyType<T>();
    }
}
