using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal class Service : Product
    {
        public Service(int _code, string _description, decimal _price, TaxCode _taxid) :
            base(_code, _description, _price, _taxid)
        { }        
    }
}
