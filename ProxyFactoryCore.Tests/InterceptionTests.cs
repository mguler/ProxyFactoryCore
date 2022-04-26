using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyFactoryCore.Impl;
using ProxyFactoryCore.Tests.Interceptors;

namespace ProxyFactoryCore.Tests.InterceptionTests
{
    [TestClass]
    public class InterceptionTests
    {
        [TestMethod("Should intercept a method which has multiple primitive input parameters and returns true")]
        public void InterceptMethodWithMultiplePrimitiveInputParameters()
        {
            var proxyFactory = new DefaultProxyFactory();

            proxyFactory.Register<EmployeeController>()
                .Intercept(type => type.Update(default,default, default, default))
                .With<Interceptor2>();

            var employeeController = proxyFactory.Create<EmployeeController>();
            var result = employeeController.Update(1, "new", 11, (decimal)67768.5);
            Assert.IsTrue(result);
        }
        [TestMethod("Should intercept method with single primitive input parameter and returns true")]
        public void InterceptMethodWithSinglePrimitiveInputParameter()
        {
            var proxyFactory = new DefaultProxyFactory();

            proxyFactory.Register<EmployeeController>()
                .Intercept<EmployeeController>(type => type.DeleteEmployee(default))
                .With<Interceptor2>()
                .With<Interceptor3>();

            var employeeController = proxyFactory.Create<EmployeeController>();
            var result = employeeController.DeleteEmployee(1);
            Assert.IsTrue(true);
        }
        [TestMethod("Should intercept a parameterless method and returns true")]
        public void InterceptParameterlessMethod()
        {
            var proxyFactory = new DefaultProxyFactory();

            proxyFactory.Register<EmployeeController>()
                .Intercept(type => type.IsRegistered())
                .With<Interceptor2>()
                .With<Interceptor3>();

            var employeeController = proxyFactory.Create<EmployeeController>();
            var result = employeeController.IsRegistered();
            Assert.IsTrue(result);
        }
        [TestMethod("Should intercept specific version of an overidden method and returns true")]
        public void InterceptOveriddenMethod()
        {
            var proxyFactory = new DefaultProxyFactory();

            proxyFactory.Register<EmployeeController>()
                .Intercept(type => type.Register(default))
                .With<Interceptor2>()
                .With<Interceptor3>()
                .Intercept(type => type.Register(default, default, default))
                .With<Interceptor2>()
                .With<Interceptor3>();

            var employeeController = proxyFactory.Create<EmployeeController>();
            var result = employeeController.Register("new", 11, 33);
            //var result = employeeController.Register(new TestClasses.Employee());
            Assert.IsTrue(result);
        }
        [TestMethod("Should intercept a method which returns void")]
        public void InterceptMethodWhichReturnsVoid()
        {
            var proxyFactory = new DefaultProxyFactory();

            proxyFactory.Register<EmployeeController>()
                .Intercept<EmployeeController>(type => type.CheckHeath())
                .With<Interceptor2>()
                .With<Interceptor3>();

            var employeeController = proxyFactory.Create<EmployeeController>();
            employeeController.CheckHeath();
        }

    }
}
