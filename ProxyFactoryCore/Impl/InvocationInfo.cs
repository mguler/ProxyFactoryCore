using ProxyFactory.Core.Abstract;
using System.Collections.Generic;
using System.Reflection;

namespace ProxyFactoryCore.Impl
{
    public class InvocationInfo: IInvocationInfo
    {
        public object Result { get; set; }
        public object Proxy { get; }
        public object Source { get; }
        public MethodInfo Method { get; }
        public Dictionary<string, object> Parameters { get; set; }

        public bool _cancelExecution;
        public bool _bypassInterceptors;

        public InvocationInfo(object proxy, object source, MethodInfo methodInfo, Dictionary<string, object> parameters)
        {
            Proxy = proxy;
            Source = source;
            Method = methodInfo;
            Parameters = parameters;
        }

        public void CancelMethodExecution() => _cancelExecution = true;

        public void BypassInterceptors() => _bypassInterceptors = true;
    }
}
