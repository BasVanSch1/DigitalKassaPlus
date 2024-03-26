using DigitalKassaPlus.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalKassaPlus
{
    enum LoginResult
    {
        SUCCESS, BLOCKED, INVALIDINPUT, GENERICFAILED
    }

    internal class Register
    {
        private readonly SingletonDAL dbManager;
        public bool LoggedIn {  get; private set; }
        public User Employee { get; private set; }
        public decimal StartingCash { get; private set; }
        public decimal Cash { get; private set; }
        public List<Order> Orders { get; private set; }
        public Order CurrentOrder {  get; private set; }

        public Dictionary<int, Product> ProductList { get; private set; }
        public Dictionary<int, Customer> CustomerList { get; private set; }

        public Register()
        {
            dbManager = SingletonDAL.Instance;
            LoggedIn = false;
            StartingCash = 0;
            Cash = 0;
            Orders = new List<Order>();
            ProductList = dbManager.GetProducts();
            CustomerList = dbManager.GetCustomers();
        }

        public LoginResult Login()
        {
            Console.WriteLine("No employee has been logged in.");
            Console.Write("Employee ID: ");

            if (Int32.TryParse(Console.ReadLine(), out int employeeId))
            {
                int attempt = 1;
                User user = dbManager.GetUser(employeeId);

                while (attempt <= 3 && user.IsActive)
                {
                    Console.Write("Pincode: ");
                    if (Int32.TryParse(Console.ReadLine(), out int pass))
                    {
                        int? correctPass = user.GetPin();

                        if (correctPass == null)
                        {
                            Console.WriteLine($"ERROR: failed to get pincode from database USERID: {employeeId}");
                            return LoginResult.GENERICFAILED;
                        }

                        if (pass == user.GetPin())
                        {
                            Employee = user;
                            LoggedIn = true;
                            return LoginResult.SUCCESS;
                        } else
                        {
                            Console.WriteLine("Invalid pincode.");
                        }

                        attempt++;
                    }
                    else
                    {
                        Console.WriteLine("Error: a Pincode can only contain numeric values.");
                        return LoginResult.INVALIDINPUT;
                    }
                }

                user.IsActive = false;
                return LoginResult.BLOCKED;
            }
            else
            {
                Console.WriteLine("Error: an Employee ID can only contain numeric values.");
                return LoginResult.INVALIDINPUT;
            }
        }

        public bool Logout()
        {
            if (LoggedIn && Employee != null && Employee is User)
            {
                Employee = null;
                LoggedIn = false;
                return true;
            } else
            {
                Console.WriteLine("ERROR: no user is currenly logged in!");
                return true;
            }
        }

        public bool ScanCustomerCard(int id)
        {
            if (!LoggedIn)
            {
                Console.WriteLine("Error: no employee has logged in.");
                return false;
            }

            if (CurrentOrder == null)
            {
                CurrentOrder = new Order(Employee);
            }

            Customer customer = dbManager.GetCustomerFromCard(new CustomerCard(id));
            if (customer != null)
            {
                CurrentOrder.SetCustomer(customer);
                return true;
            }

            return false;
        }

        public void AddProduct(int code)
        {
            if (!LoggedIn)
            {
                Console.WriteLine("Error: no employee has logged in.");
                return;
            }

            if (CurrentOrder == null)
            {
                CurrentOrder = new Order(Employee);
            }

            if (ProductList.TryGetValue(code, out Product product))
            {
                CurrentOrder.AddProduct(product);
                Console.WriteLine($"Added product: (Code: {product.Code}, Description: {product.Description}, Price: €{product.Price})");
            } else
            {
                Console.WriteLine("ERROR: product does not exist.");
            }
            
        }
        public void RemoveProduct(int code)
        {
            if (!LoggedIn)
            {
                Console.WriteLine("Error: no employee has logged in.");
                return;
            }

            if (CurrentOrder == null)
            {
                CurrentOrder = new Order(Employee);
            }

            if (!ProductList.ContainsKey(code))
            {
                Console.WriteLine($"Error: a product with productcode {code} does not exist.");
                return;
            }

            if (ProductList.TryGetValue(code, out Product product))
            {
                if (CurrentOrder.RemoveProduct(product))
                {
                    Console.WriteLine($"Product with productcode {code} has been removed from this order.");
                }
                else
                {
                    Console.WriteLine($"ERROR: failed to remove product with productcode {code} from this order.");
                }
            }
            else
            {
                Console.WriteLine("ERROR: product does not exist.");
            }
        }
        public void PayOrder() // TODO: handle choice handling in Program.cs (when making UI)
        {
            if (!LoggedIn)
            {
                Console.WriteLine("Error: no employee has logged in.");
                return;
            }

            if (CurrentOrder == null)
            {
                Console.WriteLine("Error: CurrentOrder does not exist.");
            }

            if (CurrentOrder.Products.Count == 0)
            {
                Console.WriteLine("Error: this order does not contain any products.");
                return;
            }

            bool choosing = true;
            PaymentMethod method = new CardPayment(); // default

            int productcount = 0;
            foreach (KeyValuePair<Product, int> pair in CurrentOrder.Products)
            {
                productcount += pair.Value;
            }

            while (choosing)
            {
                Console.WriteLine($"Products in order: {productcount}\n" +
                    $"Subtotal: €{CurrentOrder.Subtotal}, " +
                    $"Total: €{CurrentOrder.Total}\n" +
                    $"Amount left to pay: €{CurrentOrder.PriceToPay}\n\n" +
                    $"Option 1: Pay by Card\n" +
                    $"Option 2: Pay with Cash");
                Console.Write("Choice: ");

                if (Int32.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice == 1)
                    {
                        method = new CardPayment();
                        choosing = false;
                    } else if (choice == 2)
                    {
                        method = new CashPayment();
                        choosing = false;
                    } else
                    {
                        Console.WriteLine("ERROR: invalid choice.");
                    }
                } else
                {
                    Console.WriteLine("ERROR: the choice can only contain a numerical input.");
                }
            }

            Console.Clear();
            switch (CurrentOrder.Pay(method))
            {
                case PayResult.SUCCESS:
                    try
                    {
                        Orders.Add(CurrentOrder);

                        Console.WriteLine("You have payed!\nThank you!");

                        CurrentOrder = null;

                    } catch (Exception e)
                    {
                        Console.WriteLine($"Failed to add CurrentOrder to Orders\n" +
                            $"{e.Message}");
                    }
                    break;
                case PayResult.PAYEDTOOMUCH:
                    try
                    {
                        Orders.Add(CurrentOrder);

                        Console.WriteLine("You have payed!\nThank you!");
                        Console.WriteLine($"Return: €{decimal.Negate(CurrentOrder.PriceToPay)}"); // negate to turn the negative value into a positive.
                        CurrentOrder = null;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to add CurrentOrder to Orders\n" +
                            $"{e.Message}");
                    }
                    break;
                case PayResult.CANCELLED:
                    Console.WriteLine("Order payment: cancelled");
                    break;
                case PayResult.PARTIALLYPAYED:
                    Console.WriteLine($"Order payment: partially payed, still €{CurrentOrder.PriceToPay} left to pay.");
                    break;
                case PayResult.GENERICFAILED:
                    break;
            }
        }

        public string GetProductsList()
        {
            if (!LoggedIn)
            {
                return "Error: no employee has logged in.";
            }

            if (ProductList.Count == 0)
            {
                return "Error: there are no products defined.";
            }

            string output = $"Product Code \t| Description \t| Price \t| Tax\n";

            foreach (KeyValuePair<int, Product> pair in ProductList)
            {
                output += $"{pair.Key} \t| {pair.Value.Description} \t| €{pair.Value.Price} \t| {pair.Value.Tax.Description}\n";
            }

            return output;
        }

        public string GetProductsInOrder()
        {
            if (!LoggedIn)
            {
                return "Error: no employee has logged in.";
            }

            if (CurrentOrder == null)
            {
                CurrentOrder = new Order(Employee);
                return "Error: this order does not contain any products.";
            }

            if (CurrentOrder.Products.Count == 0)
            {
                return "Error: this order does not contain any products.";
            }

            string output = $"Product Code \t| Description \t| Price \t| Tax \t| Amount \t| Total Price without Tax\n";

            foreach (KeyValuePair<Product, int> pair in CurrentOrder.Products)
            {
                output += $"{pair.Key.Code} \t| {pair.Key.Description} \t| €{pair.Key.Price} \t| {pair.Key.Tax.Description} \t| {pair.Value} \t| €{pair.Value * pair.Key.Price}\n";
            }

            int productcount = 0;
            foreach (KeyValuePair <Product, int> pair in CurrentOrder.Products)
            {
                productcount += pair.Value;
            }

            output += $"Products in order: {productcount}\nSubtotal cost: €{CurrentOrder.Subtotal}, Total cost: €{CurrentOrder.Total}";

            return output;
        }

        /// <summary>
        /// Get all known customers.
        /// </summary>
        /// <returns>A <see cref="string"/> with all <see cref="Customer"/>s and their data</returns>
        public string GetCustomersList()
        {
            if (!LoggedIn)
            {
                return "Error: no employee has logged in.";
            }

            if (CustomerList.Count() == 0)
            {
                return "There are no customers defined.";
            }

            string output = $"Code \t| Name \t\t| Birthdate \t| Phone \t| E-mail adres \t\t| Points \t| CardId\n"; // please ignore the bad formatting

            foreach (KeyValuePair<int, Customer> pair in CustomerList)
            {
                output += $"{pair.Key} \t| {pair.Value.Name} \t| {pair.Value.BirthDate.ToShortDateString()} \t| {pair.Value.Phone} \t|" +
                    $" {pair.Value.Email} \t| {pair.Value.Points} \t\t| {pair.Value.Card.Id}\n";
            }

            return output;
        }

    }
}
