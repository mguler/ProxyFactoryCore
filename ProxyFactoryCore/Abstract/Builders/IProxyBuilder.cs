using ProxyFactoryCore.Abstract.Configuration;
using System;

namespace ProxyFactoryCore.Abstract.Builders
{
    public interface IProxyBuilder
    {
        public Type Build(IProxyTypeConfiguration proxyTypeConfiguration);
    }
}
