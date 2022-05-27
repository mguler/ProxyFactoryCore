using ProxyFactory.Core.Abstract;
using ProxyFactoryCore.Abstract;
using ProxyFactoryCore.Tests.TestClasses.ExceptionHandlers;
using System;
using System.Diagnostics;

namespace ProxyFactoryCore.Tests.TestClasses
{
    public class TestExceptionHandler : IExceptionHandler
    {
        public void OnException(Exception ex, IInvocationInfo invocationInfo)
        {
            Debug.Print(ex.ToString());
        }
        public void OnException(CustomException ex, IInvocationInfo invocationInfo)
        {
            Debug.Print(ex.ToString());

            if (invocationInfo.Method.ReturnType != typeof(void))
            {
                invocationInfo.Result = new
                {
                    Message = "Error Occurred"
                };
            }
        }
    }
}
