using ProxyFactoryCore.Abstract.Builders;
using ProxyFactoryCore.Abstract.Configuration;
using ProxyFactoryCore.Abstract.Factory;
using ProxyFactoryCore.Impl.Builders;
using ProxyFactoryCore.Impl.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ProxyFactoryCore.Impl.Factory
{
    public class ProxyFactoryDefaultImpl : IProxyFactory, IProxyFactoryConfigurationBuilder
    {
        private readonly Dictionary<Type, IProxyTypeConfiguration> _proxyTypeConfigurations = new Dictionary<Type, IProxyTypeConfiguration>();
        public IProxyFactoryConfigurationBuilder Configure<T>(Expression<Action<IProxyTypeConfigurationBuilder<T>>> func) where T : class
        {
            var type = typeof(T);
            var configuration = default(IProxyTypeConfiguration);

            if (_proxyTypeConfigurations.ContainsKey(type))
            {
                configuration = _proxyTypeConfigurations[type];
            }
            else
            {
                configuration = new ProxyTypeConfiguration(type);
                _proxyTypeConfigurations.Add(type, configuration);
            }
                
            var builder = new ProxyTypeConfigurationBuilder<T>(configuration);
            func.Compile().DynamicInvoke(builder);

            if (configuration.ProxyBuilder == null)
            {
                configuration.ProxyBuilder = typeof(DerivedProxyBuilder);
            }

            var proxyBuilder = configuration.ProxyBuilder.GetConstructor(Type.EmptyTypes).Invoke(null) as IProxyBuilder;
            configuration.ProxyType = proxyBuilder.Build(configuration);

            return this;
        }

        public T Create<T>() where T : class
        {
            return Create(typeof(T)) as T;
        }

        public object Create(Type type)
        {
            var configuration = _proxyTypeConfigurations[type];
            if (configuration == null) 
            {
                throw new Exception($"There is no configurtion for the type {type.Name}");
            }

            var ctor = configuration.ProxyType.GetConstructor(Type.EmptyTypes);
            var instance = ctor.Invoke(null);
            return instance;
        }

        public T GetProxyType<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public Type GetProxyType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
