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
                    Console.WriteLine($"Welcome {register.Employee.Name}");

                    bool choosing = true;

                    while (choosing)
                    {
                        Console.WriteLine("\nSelect an action:\n" +
                            "Option 1: Scan CustomerCard\n" +
                            "Option 2: Add Product to this Order\n" +
                            "Option 3: Remove Product from this Order\n" +
                            "Option 4: See Products in this Order\n" +
                            "Option 5: Pay for this Order\n" +
                            "Option 6: See list of known products\n" +
                            "Option 7: See list of known customers\n" +
                            "Option 8: Logout");
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
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.Clear();
                                        Console.Write($"Enter productcode: ");

                                        if (Int32.TryParse(Console.ReadLine(), out int code))
                                        {
                                            register.AddProduct(code);
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

                                        break;
                                    }
                                case 4:
                                    {
                                        Console.Clear();
                                        Console.WriteLine(register.GetProductsInOrder());
                                        break;
                                    }
                                case 5:
                                    {
                                        Console.Clear();
                                        register.PayOrder();
                                        break;
                                    }
                                case 6:
                                    {
                                        Console.Clear();
                                        Console.WriteLine(register.GetProductsList());

                                        break;
                                    }
                                case 7:
                                    {
                                        Console.Clear();
                                        Console.WriteLine(register.GetCustomersList());
                                        break;
                                    }
                                case 8:
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
                                    break;
                            }
                        } else
                        {
                            Console.WriteLine($"\nERROR: please enter a valid number.\n");
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
                                    if (register.Login() == LoginResult.SUCCESS)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Login Successful\n");
                                        choosing = false;
                                        break;
                                    } else if (register.Login() == LoginResult.BLOCKED)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Your account is blocked, please login with a different account\n");
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
