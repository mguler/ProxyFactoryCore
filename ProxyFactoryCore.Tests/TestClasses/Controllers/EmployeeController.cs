using ProxyFactoryCore.Tests.TestClasses;
using ProxyFactoryCore.Tests.TestClasses.ExceptionHandlers;
using System;

namespace ProxyFactoryCore.Tests.InterceptionTests
{
    public class EmployeeController
    {
        public virtual bool Register(Employee employee)
        {
            return true;
        }
        public virtual bool Register(string name, int age, decimal salary)
        {
            return true;
        }
        public virtual bool Update(int id, string name, int age, decimal salary)
        {
            return true;
        }
        public virtual bool IsRegistered() => true;
        public virtual void CheckHeath() { }
        public virtual bool DeleteEmployee(int id) => true;
        public virtual void ThrowsException() => throw new Exception();
        public virtual void ThrowsCustomException() => throw new CustomException();

        public virtual object ThrowsCustomExceptionThenReturnsValue()
        {
            throw new CustomException();
          
            return new
            {
                Message = "Successful"
            };
        }
    }
}
