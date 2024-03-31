using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal class Article : Product
    {
        public string Manufacturer { get; private set; }

        public Article(int _productCode, string _description, decimal _price, string _manufacturer, TaxCode _tax, int _stock) : 
            base(_productCode, _description, _price, _tax, _stock)
        {
            Manufacturer = _manufacturer;
        }
        public Article(int _productCode, string _description, decimal _price, TaxCode _tax, int _stock) :
            this(_productCode, _description, _price, "", _tax, _stock) // zonder manufacturer
        { }



    }
}
