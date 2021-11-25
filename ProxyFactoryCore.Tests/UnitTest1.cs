using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyFactoryCore.Impl;
using ProxyFactoryCore.Tests.Interceptors;

namespace ProxyFactoryCore.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var proxyFactory = new DefaultProxyFactory();

            proxyFactory.Register<Controller2>()
                .Intercept(type => type.AddtoCart(default,default))
                .With<Interceptor1>()
                .With<Interceptor2>()
                .With<Interceptor3>()
                .Intercept(type => type.Pay(default))
                .With<Interceptor4>();

            var controllerTest2 = proxyFactory.Create<Controller2>();
            var result = controllerTest2.AddtoCart("Potato", 10);

        }
    }
}
