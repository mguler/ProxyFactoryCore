using System.Collections.Generic;
using System.Reflection;

namespace ProxyFactory.Core.Abstract
{
    public interface IInvocationInfo
    {
        public object Result { get; set; }
        public object Proxy { get; }
        public object Source { get; }
        public MethodInfo Method { get; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}