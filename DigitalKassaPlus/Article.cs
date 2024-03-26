using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal class Article : Product
    {
        public string Manufacturer { get; private set; } // wordt uiteindelijk van de class Manufacturer

        public Article(int _productCode, string _description, decimal _price, string _manufacturer, TaxCode _tax) : 
            base(_productCode, _description, _price, _tax)
        {
            Manufacturer = _manufacturer;
        }
        public Article(int _productCode, string _description, decimal _price, TaxCode _tax) :
            this(_productCode, _description, _price, "", _tax) // zonder manufacturer
        { }



    }
}
