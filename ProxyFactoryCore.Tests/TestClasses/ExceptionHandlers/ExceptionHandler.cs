using ProxyFactoryCore.Abstract.Factory;
using System;

namespace ProxyFactoryCore.Tests.TestClasses.ExceptionHandlers
{
    public class ExceptionHandler : IExceptionHandler
    {
        public void OnException(Exception exception, IInvocationInfo invocationInfo)
        {

        }
    }
}
