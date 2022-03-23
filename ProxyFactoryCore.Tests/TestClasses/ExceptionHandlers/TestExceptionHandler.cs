using ProxyFactory.Core.Abstract;
using ProxyFactoryCore.Abstract;
using System;
using System.Diagnostics;

namespace ProxyFactoryCore.Tests.TestClasses
{
    public class TestExceptionHandler : IExceptionHandler
    {

        public void OnException(Exception ex, IInvocationInfo invocationInfo)
        {
            Debug.Assert(true);
        }
    }
}
