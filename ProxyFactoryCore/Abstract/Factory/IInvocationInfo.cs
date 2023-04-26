using System.Collections.Generic;
using System.Reflection;

namespace ProxyFactoryCore.Abstract.Factory
{
    public interface IInvocationInfo
    {
        bool IsCancelled { get; }
        MethodInfo Method { get; }
        Dictionary<string, object> Parameters { get; }
        object Result { get; set; }
        void CancelExecution();
    }
}
