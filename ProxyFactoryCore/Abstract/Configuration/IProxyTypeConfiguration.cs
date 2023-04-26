using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProxyFactoryCore.Abstract.Configuration
{
    public interface IProxyTypeConfiguration 
    {
        Type Type { get; }
        Type ProxyType { get; set; }
        Type ProxyBuilder { get; set; }
        Dictionary<MethodInfo,IMethodInterceptionConfiguration> MethodInterceptionConfiguration { get; }
    }
}