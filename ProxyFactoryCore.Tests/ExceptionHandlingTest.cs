using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyFactoryCore.Impl;
using ProxyFactoryCore.Tests.Interceptors;
using ProxyFactoryCore.Tests.TestClasses;
using ProxyFactoryCore.Tests.TestClasses.Interceptors;
using System.Runtime;

namespace ProxyFactoryCore.Tests
{
    [TestClass]
    public class ExceptionHandlingTest
    {
        [TestMethod]
        public void HandleException()
        {
            var proxyFactory = new DefaultProxyFactory();

            proxyFactory.Register<Controller2>().Intercept(type => type.AddtoCart(default, default))
                .With<InterceptorThrowsException>();

            var controllerTest2 = proxyFactory.Create<Controller2>();
            var result = controllerTest2.AddtoCart("Test", 10);
        }

        [TestMethod]
        public void HandleSpecificException()
        {
            var proxyFactory = new DefaultProxyFactory();
            proxyFactory.Register<ControllerWithConstructorOverload>();
            var proxyType = proxyFactory.Create<ControllerWithConstructorOverload>(new ComplexType1(), 99, new ComplexType2(), "");
        }
    }
}