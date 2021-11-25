
using ProxyFactoryCore.Impl;

namespace ProxyFactoryCore.Abstract
{
    public interface IInterceptor
    {
        void BeforeExecution(InvocationInfo invocationInfo);
        void AfterExecution(InvocationInfo invocationInfo);

    }
}
