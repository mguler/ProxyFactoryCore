using ProxyFactoryCore.Tests.TestClasses.ComplexTypes;

namespace ProxyFactoryCore.Tests.TestClasses.Controllers
{
    public class EmployeeController2
    {
        public virtual bool Update(int id, string name, int age, decimal salary)
        {
            return true;
        }
        public virtual bool Update(Employee employee)
        {
            return true;
        }

    }
}
