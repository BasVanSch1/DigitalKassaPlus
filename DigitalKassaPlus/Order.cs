using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    enum PayResult
    {
        SUCCESS, CANCELLED, PARTIALLYPAYED, PAYEDTOOMUCH, GENERICFAILED
    }

    internal class Order
    {
        public int Id { get; private set; }
        public decimal Total { get => CalculateTotal(); }
        public decimal Subtotal { get => CalculateSubtotal(); }
        public decimal PriceToPay { get; private set; }
        public Dictionary<Product, int> Products { get; private set; }
        public bool IsPayed { get; private set; }
        public Customer Customer { get; private set; }
        public User Employee { get; private set; }

        public Order(int _id, Dictionary<Product, int> _products, Customer _customer, User _employee, bool _ispayed) // used when getting all orders from the database
        {
            Id = _id;
            Products = _products;
            Customer = _customer;
            Employee = _employee;
            IsPayed = _ispayed;
        }

        public Order(User _employee) // used when creating a new order.
        {
            Id = 0;
            Products = new Dictionary<Product, int>();
            Customer = null;
            Employee = _employee;
            IsPayed = false;
        }

        /// <summary>
        /// Pay for order with the given PaymentMethod
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public PayResult Pay(PaymentMethod method)
        {
            if (method is CashPayment)
            {
                Console.Write("Amount payed by cash: ");
            } else if (method is CardPayment)
            {
                Console.Write("Amount payed by card: ");
            } else
            {
                Console.WriteLine($"{typeof(Order)}; Error during processing of PaymentMethod: invalid PaymentMethod.");
                return PayResult.GENERICFAILED;
            }

            if (Decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                if (amount == PriceToPay)
                {
                    IsPayed = true;
                    PriceToPay = 0;
                    return PayResult.SUCCESS;
                } 
                else if (amount < PriceToPay)
                {
                    PriceToPay -= amount;
                    return PayResult.PARTIALLYPAYED;
                } 
                else if (amount > PriceToPay)
                {
                    IsPayed = true;
                    PriceToPay -= amount;
                    return PayResult.PAYEDTOOMUCH;
                }
            }

            return PayResult.GENERICFAILED;
        }
        
        /// <summary>
        /// Adds a product to this order.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>boolean value of true if it succeeds, otherwise false.</returns>
        public bool AddProduct(Product product)
        {
            try
            {
                if (!Products.ContainsKey(product))
                {
                    Products.Add(product, 1);
                } else
                {
                    Products[product]++; // aantal van product met 1 verhogen.
                }
            } catch (Exception e)
            {
                Console.WriteLine($"Error occurred while trying to add a Product to Order: {Id}, product:( {product})\n" +
                    $"{e.Message}");
                return false;
            }

            PriceToPay += product.Price + (product.Price * product.Tax.Percentage);
            return true;
        }

        /// <summary>
        /// Removes a product from this order.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>boolean value of true if it succeeds, otherwise false.</returns>
        public bool RemoveProduct(Product product)
        {
            if (Products.ContainsKey(product)) // if Products contains this product
            {
                if (Products[product] > 1) // if more than 1 product
                {
                    Products[product]--; // remove 1
                    PriceToPay -= product.Price + (product.Price * product.Tax.Percentage); // subtract price from PriceToPay
                    return true;
                } else
                {
                    Products.Remove(product); // remove product from Products
                    PriceToPay -= product.Price + (product.Price * product.Tax.Percentage); // subtract price from PriceToPay
                    return true;
                }
            }

            return false; // if product does not exist in this order, return false
        }

        /// <summary>
        /// Get the amount of a specified <see cref="Product"/> in this order.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>false if the order does not contain any of the specified <see cref="Product"/></returns>
        public int GetCountOfProduct(Product product)
        {
            if (Products.TryGetValue(product, out int count))
            {
                return count;
            }

            return 0;
        }

        public void SetCustomer(Customer customer)
        {
            Customer = customer;
        }

        private decimal CalculateSubtotal()
        {
            decimal subtotal = 0;

            foreach (KeyValuePair<Product, int> pair in Products)
            {
                subtotal += pair.Key.Price;
            }

            return Decimal.Round(subtotal, 2);
        }

        private decimal CalculateTotal()
        {
            decimal total = 0;

            foreach (KeyValuePair<Product, int> pair in Products)
            {
                decimal tax = pair.Key.Price * pair.Key.Tax.Percentage ;
                total += (pair.Key.Price + tax) * pair.Value; // add the tax to the product and multiply by amount of this product is in the order.
            }

            return Decimal.Round(total, 2);
        }

    }
}
