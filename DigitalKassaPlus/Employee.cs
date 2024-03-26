using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal class Employee : User
    {
        public List<Order> Orders { get; private set; }

        public Employee(int _id, string _name, string _phone, string _email, bool _isactive) :
            base(_id, _name, _phone, _email, _isactive)
        {
            Orders = new List<Order>();
        }
    }
}
