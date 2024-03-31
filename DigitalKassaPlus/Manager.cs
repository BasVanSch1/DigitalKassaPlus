using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal class Manager : Employee
    {
        public Manager(int _id, string _name, string _phone, string _email, bool _isactive) :
            base(_id, _name, _phone, _email, _isactive)
        { }
    }
}
