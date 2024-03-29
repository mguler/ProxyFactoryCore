﻿using ProxyFactoryCore.Tests.TestClasses.ComplexTypes;

namespace ProxyFactoryCore.Tests.TestClasses.Controllers
{
    public class EmployeeController
    {
        public virtual bool Register(string name, int age, decimal salary)
        {
            return true;
        }
        public virtual bool Register(Employee employee)
        {
            return true;
        }
    }
}
