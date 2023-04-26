using ProxyFactoryCore.Abstract.Configuration;
using ProxyFactoryCore.Abstract.Factory;
using System;

namespace ProxyFactoryCore.Impl.Configuration
{
    public class MethodInterceptionConfigurationBuilder : IMethodInterceptionConfigurationBuilder
    {
        private Type _exceptionHandler;
        private readonly IMethodInterceptionConfiguration _methodInterceptionConfiguration;
        internal MethodInterceptionConfigurationBuilder(IMethodInterceptionConfiguration methodInterceptionConfiguration)
        {
            _methodInterceptionConfiguration = methodInterceptionConfiguration;
        }
        public IMethodInterceptionConfigurationBuilder AddInterceptor<TInterceptor>() where TInterceptor : IInterceptor
        {
            _methodInterceptionConfiguration.Interceptors.Add(typeof(TInterceptor));
            return this;
        }

        public IMethodInterceptionConfigurationBuilder SetExceptionHandler<TExceptionHandler>() where TExceptionHandler : IExceptionHandler
        {
            _exceptionHandler = typeof(TExceptionHandler);
            return this;
        }
    }
}
