
using ProxyFactoryCore.Abstract;

namespace ProxyFactoryCore.Impl
{
    public class DefaultInterceptor:IInterceptor
    {
        public void BeforeExecution(InvocationInfo invocationInfo)
        {
            var result = invocationInfo;
        }
        public void AfterExecution(InvocationInfo invocationInfo)
        {
            var result = invocationInfo;
        }
    }
}
