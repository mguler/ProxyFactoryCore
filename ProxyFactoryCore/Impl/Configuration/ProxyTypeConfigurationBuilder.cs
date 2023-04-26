using ProxyFactoryCore.Abstract.Configuration;
using System;
using System.Linq.Expressions;

namespace ProxyFactoryCore.Impl.Configuration
{
    public class ProxyTypeConfigurationBuilder<T>: IProxyTypeConfigurationBuilder<T>
    {
        private readonly IProxyTypeConfiguration _proxyTypeConfiguration;
        internal ProxyTypeConfigurationBuilder(IProxyTypeConfiguration proxyTypeConfiguration)
        {
            _proxyTypeConfiguration= proxyTypeConfiguration;
        }
        public IProxyTypeConfigurationBuilder<T> ConfigureMethod<TResult>(Expression<Func<T, TResult>> expression, Expression<Action<IMethodInterceptionConfigurationBuilder>> func)
        {
            var methodInfo = (expression.Body as MethodCallExpression).Method;
            var methodInterceptionConfiguration = default(IMethodInterceptionConfiguration);

            if (_proxyTypeConfiguration.MethodInterceptionConfiguration.ContainsKey(methodInfo))
            {
                methodInterceptionConfiguration = _proxyTypeConfiguration.MethodInterceptionConfiguration[methodInfo];
            }
            else 
            {
                methodInterceptionConfiguration = new MethodInterceptionConfiguration(methodInfo);
                _proxyTypeConfiguration.MethodInterceptionConfiguration.Add(methodInfo, methodInterceptionConfiguration);
            }
            func.Compile().Invoke(new MethodInterceptionConfigurationBuilder(methodInterceptionConfiguration));
            return this;
        }

        public IProxyTypeConfigurationBuilder<T> SetProxyBuilder<TProxyBuilder>()
        {
            throw new NotImplementedException();
        }

        public IProxyTypeConfigurationBuilder<T> SetProxyBuilder(Type proxyBuilderType)
        {
            throw new NotImplementedException();
        }
    }
}
