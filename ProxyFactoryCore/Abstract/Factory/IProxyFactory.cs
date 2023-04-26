using ProxyFactoryCore.Abstract.Configuration;
using System;
using System.Linq.Expressions;

namespace ProxyFactoryCore.Abstract.Factory
{
    public interface IProxyFactory
    {
        object Create(Type type);
        T GetProxyType<T>() where T : class;
        Type GetProxyType(Type type);
        
    }
}
