using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyFactoryCore.Impl.Factory;
using ProxyFactoryCore.Tests.TestClasses.Interceptors;
using ProxyFactoryCore.Tests.TestClasses.Controllers;
using ProxyFactoryCore.Tests.TestClasses.ExceptionHandlers;

namespace ProxyFactoryCore.Tests.InterceptionTests
{
    [TestClass]
    public class InterceptionTests
    {
        [TestMethod("Should intercept a method which has multiple primitive input parameters and returns true")]
        public void InterceptMethodWithMultiplePrimitiveInputParameters()
        {
            var proxyFactory = new ProxyFactoryDefaultImpl();
            proxyFactory
                .Configure<EmployeeController>(conf =>
                    conf.ConfigureMethod(x => x.Register(default, default, default), mConf =>
                        mConf.AddInterceptor<Interceptor1>()
                            .AddInterceptor<Interceptor2>()
                            .SetExceptionHandler<ExceptionHandler>())

                    //.ConfigureMethod(x => x.Register(default, default, default), mConf =>
                    //    mConf.AddInterceptor<Interceptor1>())

                ).Configure<EmployeeController>(conf =>
                    conf.ConfigureMethod(x => x.Register(default, default, default), mConf =>
                        mConf.AddInterceptor<Interceptor1>()
                            .AddInterceptor<Interceptor2>()
                            .SetExceptionHandler<ExceptionHandler>())

                    .ConfigureMethod(x => x.Register(default, default, default), mConf =>
                        mConf.AddInterceptor<Interceptor1>())

                ).Configure<EmployeeController2>(conf =>
                    conf.ConfigureMethod(x => x.ToString(), mConf =>
                        mConf.AddInterceptor<Interceptor1>()
                ));

            
            var employeeController = proxyFactory.Create<EmployeeController>();
            var result = employeeController.Register("Robert Halford", 60, 10000);
            Assert.IsTrue(result);
        }
    }
}
