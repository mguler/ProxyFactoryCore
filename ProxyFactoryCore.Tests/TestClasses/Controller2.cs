using System;

namespace ProxyFactoryCore.Tests
{
    public class Controller2
    {
        public virtual int AddtoCart(string productName, int amount)
        {
            return new Int32();
        }
        public virtual int AddtoCart(string productName, decimal amount)
        {
            return new Int32();
        }
        public virtual int Pay(float amount) 
        {
            return 1;
        }
    }
}
