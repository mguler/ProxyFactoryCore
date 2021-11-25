using System;

namespace ProxyFactoryCore.Abstract
{
    public interface IProxyBuilder
    {
        Type CreateProxyType<T1>(IInterceptorConfiguration config);
        Type CreateProxyType(Type baseType, IInterceptorConfiguration config);
    }
}
