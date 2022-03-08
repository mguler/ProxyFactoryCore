using ProxyFactoryCore.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProxyFactoryCore.Impl
{
    public class DefaultProxyFactory:IDynamicProxyFactory
    {
        private readonly Dictionary<Type, IInterceptorConfiguration> _cache = new Dictionary<Type, IInterceptorConfiguration>();
        public IInterceptorConfiguration<T> Register<T>()
        {
            var interceptionConfiguration = new InterceptionConfiguration<T>(this);
            return interceptionConfiguration;
        }

        public T Create<T>(params object[] args)
        {
            var interceptorConfiguration = _cache.SingleOrDefault(interceptorConfiguration => interceptorConfiguration.Key == typeof(T)).Value
                as IInterceptorConfiguration<T>;

            var instance = Create(interceptorConfiguration, args);
            return instance;
        }
        public object Create<T>(object obj) => throw new NotImplementedException();
        public void Add<T>(IInterceptorConfiguration<T> interceptorConfiguration)
        {
            _cache.Add(typeof(T), interceptorConfiguration);
        }
        internal Type Build<T>(IInterceptorConfiguration<T> interceptorConfiguration)
        {
            if (interceptorConfiguration.ProxyBuilderType == null)
            {
                throw new Exception("Proxy builder type must be set");
            }

            var proxyBuilder = interceptorConfiguration.ProxyBuilderType.GetConstructor(Type.EmptyTypes)
                .Invoke(null) as IProxyBuilder;

            var proxyType = proxyBuilder.CreateProxyType(typeof(T), interceptorConfiguration);
            return proxyType;
        }
        internal T Create<T>(IInterceptorConfiguration<T> interceptorConfiguration, params object[] args)
        {
            var argumentTypes = args.Select(arg => arg.GetType()).ToArray();

            if (interceptorConfiguration.ProxyType == null)
            {
                interceptorConfiguration.ProxyType = Build(interceptorConfiguration);
            }

            var proxyInstance = interceptorConfiguration.ProxyType.GetConstructor(argumentTypes).Invoke(args);
            return (T)proxyInstance;
        }

        public Type GetProxyType<T>()
        {
            var interceptorConfiguration = _cache.SingleOrDefault(interceptorConfiguration => interceptorConfiguration.Key == typeof(T)).Value
                as IInterceptorConfiguration<T>;
            if (interceptorConfiguration.ProxyType == null)
            {
                interceptorConfiguration.ProxyType = Build(interceptorConfiguration);
            }
            return interceptorConfiguration.ProxyType;
        }
    }
}
