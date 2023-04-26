using ProxyFactoryCore.Abstract.Factory;
using System.Collections.Generic;
using System.Reflection;

namespace ProxyFactoryCore.Impl.Factory
{
    public class InvocationInfo : IInvocationInfo
    {
        public bool IsCancelled { get; private set; }

        public object Instance { get; private set; }
        public MethodInfo Method { get; private set; }

        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public object Result { get; set; }

        public void CancelExecution() => IsCancelled = true;
        internal InvocationInfo(object instance, MethodInfo methodInfo)
        {
            Instance = instance;
            Method = methodInfo;
        }
    }
}
