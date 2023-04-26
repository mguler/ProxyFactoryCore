using System;
using System.Linq.Expressions;

namespace ProxyFactoryCore.Abstract.Configuration
{
    public interface IProxyTypeConfigurationBuilder<T>
    {
        IProxyTypeConfigurationBuilder<T> SetProxyBuilder<TProxyBuilder>();
        IProxyTypeConfigurationBuilder<T> SetProxyBuilder(Type proxyBuilderType);
        IProxyTypeConfigurationBuilder<T> ConfigureMethod<TResult>(Expression<Func<T, TResult>> expression, Expression<Action<IMethodInterceptionConfigurationBuilder>> methodInterceptionConfigurationBuilder);
    }
}
