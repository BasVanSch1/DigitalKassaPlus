using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal abstract class Product : IEquatable<Product>
    {

        public int Code {  get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public TaxCode Tax { get; private set; }

        public Product(int _productCode, string _productDescription, decimal _price, TaxCode _tax) // full constructor
        {
            Code = _productCode;
            Description = _productDescription;
            Price = _price;
            Tax = _tax;
        }

        public override string ToString()
        {
            return $"{Code} \t {Description} \t {Price} \t {Tax.Code}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Product);
        }

        public bool Equals(Product other)
        {
            return !(other is null) &&
                   Code == other.Code;
        }

        public override int GetHashCode()
        {
            return -434485196 + Code.GetHashCode();
        }
    }
}
