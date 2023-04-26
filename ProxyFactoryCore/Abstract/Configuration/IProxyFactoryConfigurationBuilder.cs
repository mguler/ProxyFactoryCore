using System;
using System.Linq.Expressions;

namespace ProxyFactoryCore.Abstract.Configuration
{
    public interface IProxyFactoryConfigurationBuilder
    {
        IProxyFactoryConfigurationBuilder Configure<T>(Expression<Action<IProxyTypeConfigurationBuilder<T>>> expression) where T : class;

    }
}
