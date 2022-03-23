using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyFactoryCore.Impl;
using ProxyFactoryCore.Tests.Interceptors;

namespace ProxyFactoryCore.Tests.InterceptionTests
{
    [TestClass]
    public class InterceptionTests
    {
        [TestMethod("Should be intercepted and returns null")]
        public void Interception()
        {
            var proxyFactory = new DefaultProxyFactory();

            proxyFactory.Register<EmployeeController>()
                .Intercept(type => type.Register(default))
                .With<Interceptor1>()
                .With<Interceptor2>()
                .With<Interceptor3>()
                .Intercept(type => type.Register(default, default, default))
                .With<Interceptor4>();

            var employeeController = proxyFactory.Create<EmployeeController>();
            var result = employeeController.Register("new", 11, 33);
        }

    }
}
