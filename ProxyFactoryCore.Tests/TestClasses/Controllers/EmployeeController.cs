using ProxyFactoryCore.Tests.TestClasses;
using System;

namespace ProxyFactoryCore.Tests.InterceptionTests
{
    public class EmployeeController
    {
        public virtual bool Register(Employee employee)
        {
            return true;
        }
        public virtual bool Register(string name,int age,decimal salary)
        {
            throw new Exception();
            return true;
        }
    }
}
