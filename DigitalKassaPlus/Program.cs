using System;
using System.Text;

namespace DigitalKassaPlus
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Welcome to DigitalKassaPlus!\n");

            Register register = new Register();
            bool registerOn = true;
            while (registerOn)
            {
                if (register.LoggedIn)
                {
                    const string menu = "\nSelect an action:\n" +
                            "Option 1: Scan CustomerCard\n" +
                            "Option 2: Add Product to this Order\n" +
                            "Option 3: Remove Product from this Order\n" +
                            "Option 4: See Products in this Order\n" +
                            "Option 5: Pay for this Order\n" +
                            "Option 6: See list of known products\n" +
                            "Option 7: See list of known customers\n" +
                            "Option 99: Logout";

                    const string managerMenu = "\nSelect an action:\n" +
                            "Option 1: Scan CustomerCard\n" +
                            "Option 2: Add Product to this Order\n" +
                            "Option 3: Remove Product from this Order\n" +
                            "Option 4: See Products in this Order\n" +
                            "Option 5: Pay for this Order\n" +
                            "Option 6: See list of known products\n" +
                            "Option 7: See list of known customers\n" +
                            "---------------------------------------\n" +
                            "Option 8: Add new Customer\n" +
                            "Option 9: Manage Stock\n" +
                            "Option 99: Logout";

                    bool choosing = true;

                    while (choosing)
                    {
                        Console.Clear();
                        Console.WriteLine($"Welcome {register.Employee.Name}");
                        Console.WriteLine(register.Employee is Manager ? managerMenu : menu); // check if employee is of type Manager.
                        Console.Write("Choice: ");

                        if (Int32.TryParse(Console.ReadLine(), out int decision))
                        {
                            switch (decision)

                            {
                                case 1:
                                    {
                                        Console.Clear();
                                        Console.Write("Enter cardcode: ");

                                        if (Int32.TryParse(Console.ReadLine(), out int code))
                                        {
                                            if (register.ScanCustomerCard(code))
                                            {
                                                Console.WriteLine($"Added customer {register.CurrentOrder.Customer.Name} to the order");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Failed to add customer to order.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("ERROR: a cardcode can only contain numeric values.");
                                        }

                                        Console.Write("\nPress any key to continue...");
                                        Console.ReadKey();
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.Clear();
                                        Console.WriteLine(register.GetProductsList());
                                        Console.Write($"Enter productcode: ");

                                        if (Int32.TryParse(Console.ReadLine(), out int code))
                                        {
                                            register.AddProduct(code);
                                            Console.Write("\nPress any key to continue...");
                                            Console.ReadKey();
                                        }
                                        else
                                        {
                                            Console.WriteLine("ERROR: a productcode can only contain numeric values.");
                                        }
                                        break;
                                    }
                                case 3:
                                    { 
                                        Console.Clear();
                                        Console.WriteLine(register.GetProductsInOrder());

                                        Console.Write("Productcode of the product to remove: ");
                                        if (Int32.TryParse(Console.ReadLine(), out int code))
                                        {
                                            register.RemoveProduct(code);
                                        } else
                                        {
                                            Console.WriteLine("ERROR: a productcode can only contain numeric values.");
                                        }

                                        Console.Write("\nPress any key to continue...");
                                        Console.ReadKey();
                                        break;
                                    }
                                case 4:
                                    {
                                        Console.Clear();
                                        Console.WriteLine(register.GetProductsInOrder());
                                        Console.Write("\nPress any key to continue...");
                                        Console.ReadKey();
                                        break;
                                    }
                                case 5:
                                    {
                                        Console.Clear();
                                        register.PayOrder();
                                        Console.Write("\nPress any key to continue...");
                                        Console.ReadKey();
                                        break;
                                    }
                                case 6:
                                    {
                                        Console.Clear();
                                        Console.WriteLine("NOTICE: stock changes while making orders is not implemented yet.\n");
                                        Console.WriteLine(register.GetProductsList());
                                        Console.Write("\nPress any key to continue...");
                                        Console.ReadKey();

                                        break;
                                    }
                                case 7:
                                    {
                                        Console.Clear();
                                        Console.WriteLine(register.GetCustomersList());
                                        Console.Write("\nPress any key to continue...");
                                        Console.ReadKey();
                                        break;
                                    }
                                case 8:
                                    if (register.Employee is Manager)
                                    {
                                        Console.Clear();
                                        string _name;
                                        string _phone;
                                        string _email;
                                        DateTime _birthdate;

                                        Console.Write("Enter the name of the new Customer: ");
                                        _name = Console.ReadLine();

                                        Console.Write("Enter the phonenumer of the new Customer: ");
                                        _phone = Console.ReadLine();

                                        Console.Write("Enter the emailadress of the new Customer: ");
                                        _email = Console.ReadLine();

                                        Console.Write("Enter the birthdate of the new Customer: ");
                                        if (!DateTime.TryParse(Console.ReadLine(), out _birthdate)) // if failed
                                        {
                                            _birthdate = DateTime.Now;
                                            Console.WriteLine("ERROR: Invalid format, setting birthdate to today.");
                                        }

                                        if (register.CreateCustomer(_name, _birthdate, _phone, _email))
                                        {
                                            Console.WriteLine($"Succesfully created customer: {_name}");
                                        } else
                                        {
                                            Console.WriteLine("ERROR: failed to create new customer.");
                                        }
                                        Console.Write("\nPress any key to continue...");
                                        Console.ReadKey();

                                    } else
                                    {
                                        Console.WriteLine("ERROR: that option does not exist!\nPress any key to continue...");
                                        Console.ReadKey();
                                    }
                                    break;
                                case 9: // manage stock
                                    if (register.Employee is Manager)
                                    {
                                        Console.Clear();

                                        Console.WriteLine(register.GetProductsList());

                                        Console.Write("Enter a productID: ");
                                        if (Int32.TryParse(Console.ReadLine(), out int _productId)) {

                                            if (register.ProductList.ContainsKey(_productId))
                                            {
                                                Console.Write("Set stock to: ");
                                                if (Int32.TryParse(Console.ReadLine(), out int _stock))
                                                {
                                                    register.ManageStock(_productId, _stock);
                                                } else
                                                {
                                                    Console.WriteLine("ERROR: please enter a valid number.\n");
                                                }
                                            } else
                                            {
                                                Console.WriteLine("ERROR: that product does not exist.\n");
                                            }
                                            

                                        } else
                                        {
                                            Console.WriteLine("ERROR: please enter a valid number.\n");
                                        }



                                        Console.Write("\nPress any key to continue...");
                                        Console.ReadKey();
                                    } else
                                    {
                                        Console.WriteLine("ERROR: that option does not exist!\nPress any key to continue...");
                                        Console.ReadKey();
                                    }
                                    break;
                                case 99:
                                    {
                                        if (register.Logout()) // if logout successful
                                        {
                                            Console.Clear();
                                            Console.WriteLine("You have been logged out!");
                                            choosing = false;
                                        }
                                        break;
                                    }
                                default:
                                    Console.WriteLine("ERROR: that option does not exist!\nPress any key to continue...");
                                    Console.ReadKey();
                                    break;
                            }
                        } else
                        {
                            Console.WriteLine($"\nERROR: please enter a valid number.\n");
                            Console.Write("\nPress any key to continue...");
                            Console.ReadKey();
                        }
                    }
                } 
                else
                {
                    bool choosing = true;

                    while (choosing)
                    {
                        Console.WriteLine("Select an action:\n" +
                            "Option 1: Login\n" +
                            "Option 2: Turn off the register");
                        Console.Write("Choice: ");

                        if (Int32.TryParse(Console.ReadLine(), out int decision))
                        {
                            switch (decision)
                            {
                                case 1:
                                    switch (register.Login())
                                    {
                                        case LoginResult.SUCCESS:
                                            Console.Clear();
                                            Console.WriteLine("Login Successful\n");
                                            choosing = false;
                                            break;
                                        case LoginResult.BLOCKED:
                                            Console.Clear();
                                            Console.WriteLine("Your account is blocked, please login with a different account\n");
                                            break;
                                        case LoginResult.INVALIDUSER:
                                            Console.Clear();
                                            Console.WriteLine("ERROR: That userid does not exist.\n");
                                            break;
                                    }
                                    break;
                                    
                                case 2:
                                    choosing = false;
                                    registerOn = false;
                                    break;
                                default:
                                    Console.WriteLine("Error: that option does not exist!");
                                    break;
                            }
                        } else
                        {
                            Console.WriteLine("Error: choice can only contain numeric values.");
                        }
                    }
                }
            }

            Console.Clear();
            Console.WriteLine("Register is offline.\nPress any key to exit the program.");
            Console.ReadKey();
        }
    }
}
