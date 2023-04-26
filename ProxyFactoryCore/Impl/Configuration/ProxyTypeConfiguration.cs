using ProxyFactoryCore.Abstract.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProxyFactoryCore.Impl.Configuration
{
    public class ProxyTypeConfiguration : IProxyTypeConfiguration
    {
        public Type Type { get;  }
        public Type ProxyType { get; set; }
        public Type ProxyBuilder { get; set; }
        public Dictionary<MethodInfo,IMethodInterceptionConfiguration> MethodInterceptionConfiguration { get; set; } = new Dictionary<MethodInfo, IMethodInterceptionConfiguration>();

        public ProxyTypeConfiguration(Type type)
        {
            Type = type;
        }
    }
}
