using System;

namespace ProxyFactoryCore.Abstract
{
    public interface IDynamicProxyFactory
    {
        IInterceptorConfiguration<T> Register<T>();

        T Create<T>();
        object Create<T>(object obj);
        void Add<T>(IInterceptorConfiguration<T> interceptorConfiguration);
    }
}
