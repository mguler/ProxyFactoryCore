using ProxyFactoryCore.Abstract.Factory;
using System;

namespace ProxyFactoryCore.Impl.Factory
{
    public class ExceptionHandlerDefaultImpl : IExceptionHandler
    {
        public void OnException(Exception exception, IInvocationInfo invocationInfo)
        {
            throw exception;
        }
    }
}
