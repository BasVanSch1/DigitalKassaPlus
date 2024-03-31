using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus.DAL
{
    internal sealed class SingletonDAL
    {
        private static readonly SingletonDAL instance = new SingletonDAL();
        private readonly string connectionString = @"Data Source=localhost;Initial Catalog=Lj1_Blok3_KassaProgramma;User id=kassaUser;Password=Kaching!Betalen maar!";

        public static SingletonDAL Instance { get { return instance; } }

        static SingletonDAL() { }
        private SingletonDAL()
        {
        }

        public User GetUser(int id)
        {
            User user;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd;
                    SqlDataReader reader;

                    string sqlStatement = $"SELECT code, name, phone, email, ismanager, isactive " +
                                            $"FROM USERS WHERE code = {id}";

                    sqlConnection.Open();
                    cmd = new SqlCommand(sqlStatement, sqlConnection);
                    reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    reader.Read(); // only advance once, since there shouldn't be any other records.

                    string name = (string) reader["name"];

                    string phone;
                    if (reader["phone"] is DBNull) // can be null in db
                    {
                        phone = "";
                    } else
                    {
                        phone = (string)reader["phone"];
                    }

                    string email;
                    if (reader["email"] is DBNull) // can be null in db
                    {
                        email = "";
                    } else
                    {
                        email = (string)reader["email"];
                    }

                    bool ismanager = (bool) reader["ismanager"];
                    bool isactive = (bool) reader["isactive"];

                    if (ismanager)
                    {
                        user = new Manager(id, name, phone, email, isactive);
                    }
                    else
                    {
                        user = new Employee(id, name, phone, email, isactive);
                    }

                    return user;
                }
            } catch (SqlException ex) { throw ex; }
        }
        public int? GetPinFromUser(int id)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd;
                    SqlDataReader reader;

                    string sqlStatement = $"SELECT pincode FROM USERS WHERE code = {id}";

                    sqlConnection.Open();
                    cmd = new SqlCommand(sqlStatement , sqlConnection);
                    reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    reader.Read(); // only advance once, since there shouldn't be any other records.

                    int pincode = (int)reader["pincode"];

                    return pincode;
                }
            } catch (SqlException ex) { throw ex; }
        }
        public Dictionary<int, Product> GetProducts()
        {
            Dictionary<int, Product> products = new Dictionary<int, Product>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd;
                    SqlDataReader reader;

                    string sqlStatement = "SELECT p.code, p.description, p.price, p.taxcode, p.type, t.percentage, t.description as taxdescription, p.stock " +
                                            "FROM PRODUCT p " +
                                            "INNER JOIN TAXCODE t on t.code = p.taxcode";

                    sqlConnection.Open();
                    cmd = new SqlCommand(sqlStatement, sqlConnection);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int productcode = (int) reader["code"];
                        string description = (string) reader["description"];
                        decimal price = (decimal) reader["price"];
                        int taxid = (int) reader["taxcode"];
                        string type = (string) reader["type"];
                        decimal taxpercentage = (decimal) reader["percentage"];
                        string taxdescription = (string) reader["taxdescription"];
                        int stock = (int) reader["stock"];

                        switch (type.ToLower())
                        {
                            case "article":
                                {
                                    products.Add(productcode, new Article(productcode, description, price, new TaxCode(taxid, taxpercentage, taxdescription), stock));
                                    break;
                                }
                            case "service":
                                {
                                    products.Add(productcode, new Service(productcode, description, price, new TaxCode(taxid, taxpercentage, taxdescription), stock));
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            } catch (SqlException ex) { throw ex; }

            return products;
        }

        /// <summary>
        /// Gets a dictionary of customers from the database
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Customer> GetCustomers()
        {
            Dictionary<int, Customer> customers = new Dictionary<int, Customer>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd;
                    SqlDataReader reader;

                    string sqlStatement = "SELECT cus.code, cus.name, cus.birthdate, cus.phone, cus.email, cus.cardcode, cus.points, car.isactive " +
                                            "FROM CUSTOMER cus " +
                                            "INNER JOIN CUSTOMERCARD car ON cus.cardcode = car.code";

                    sqlConnection.Open();
                    cmd = new SqlCommand(sqlStatement, sqlConnection);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int customerCode = (int) reader["code"];
                        string name = (string)reader["name"];
                        DateTime birthdate = (DateTime)reader["birthdate"];
                        string phone = (string)reader["phone"];
                        string email = (string)reader["email"];
                        int cardCode = (int) reader["cardcode"];
                        int points = (int) reader["points"];
                        bool isActive = (bool) reader["isactive"];

                        customers.Add(customerCode, new Customer(customerCode, name, birthdate, phone, email, points, new CustomerCard(cardCode, isActive)));
                    }
                }
            } catch (SqlException ex) { throw ex; }

            return customers;

        }

        public Customer GetCustomerFromCard(CustomerCard card)
        {
            Customer customer = null;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd;
                    SqlDataReader reader;

                    string sqlStatement = $"SELECT c.code, c.name, c.birthdate, c.phone, c.email, c.cardcode, c.points "
                                            + $"FROM CUSTOMER c "
                                            + $"INNER JOIN CUSTOMERCARD cc ON c.cardcode = cc.code "
                                            + $"WHERE c.cardcode = {card.Id}";

                    sqlConnection.Open();
                    cmd = new SqlCommand(sqlStatement, sqlConnection);

                    reader = cmd.ExecuteReader();

                    reader.Read(); // Only read the first result.

                    int customerCode = (int)reader["code"];
                    string customerName = (string)reader["name"];
                    DateTime customerBirthdate = (DateTime)reader["birthdate"];
                    string customerPhone = (string)reader["phone"];
                    string customerEmail = (string)reader["email"];
                    int customerPoints = (int)reader["points"];

                    customer = new Customer(customerCode, customerName, customerBirthdate, customerPhone, customerEmail, customerPoints, card);
                }
            } catch (SqlException ex) { throw ex; }

            return customer;
        }

        /// <summary>
        /// Insert an order into the database
        /// </summary>
        /// <param name="order">The <see cref="Order"> to insert.</param>
        public bool InsertOrder(Order order)
        {
            try
            {
                SqlCommand cmd;
                SqlCommand cmd2;
                SqlCommand cmd3;
                string orderStatement = @"INSERT INTO ORDERS (employeecode, ispayed) VALUES(@employeecode, @ispayed); SELECT CAST(scope_identity() AS int);";
                string orderProductStatement = @"INSERT INTO ORDERPRODUCT (ordercode, productcode, amount) VALUES(@ordercode, @productcode, @amount);";
                string orderCustomerStatement = @"INSERT INTO CUSTOMERORDER (ordercode, customercode) VALUES(@ordercode_, @customercode);";
                
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
                    {
                        cmd = new SqlCommand(orderStatement, sqlConnection, sqlTransaction);
                        cmd.Parameters.AddWithValue("@employeecode", order.Employee.Id);
                        cmd.Parameters.AddWithValue("@ispayed", order.IsPayed);

                        int orderId = (int) cmd.ExecuteScalar();

                        cmd2 = new SqlCommand(orderProductStatement, sqlConnection, sqlTransaction);

                        foreach (KeyValuePair<Product, int> pair in order.Products)
                        {
                            cmd2.Parameters.Clear();
                            cmd2.Parameters.AddWithValue("@ordercode", orderId);
                            cmd2.Parameters.AddWithValue("@productcode", pair.Key.Code);
                            cmd2.Parameters.AddWithValue("@amount", pair.Value);

                            cmd2.ExecuteNonQuery();
                        }
                        
                        cmd3 = new SqlCommand(orderCustomerStatement, sqlConnection, sqlTransaction);
                        cmd3.Parameters.AddWithValue("@ordercode_", orderId);
                        cmd3.Parameters.AddWithValue("@customercode", order.Customer.Code);
                        cmd3.ExecuteNonQuery();

                        sqlTransaction.Commit();
                    }

                }

            } catch (SqlException) { return false; }

            return true;
        }

        /// <summary>
        /// Insert a new <see cref="Customer"/> into the database
        /// </summary>
        /// <param name="customer"></param>
        public bool InsertCustomer(Customer customer)
        {
            try
            {
                SqlCommand cmd;
                SqlCommand cmd2;
                string customerStatement = @"INSERT INTO CUSTOMER (name, birthdate, phone, email, cardcode, points) VALUES (@customername, @birthdate, @phone, @email, @cardcode, @points)";
                string customerCardStatement = @"INSERT INTO CUSTOMERCARD (isactive) VALUES (@isactive); SELECT CAST(scope_identity() AS int);";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
                    {
                        using (cmd = new SqlCommand(customerCardStatement, sqlConnection, sqlTransaction))
                        {
                            cmd.Parameters.AddWithValue("@isactive", customer.Card.IsActive);
                            int cardId = (int) cmd.ExecuteScalar();

                            using (cmd2 = new SqlCommand(customerStatement, sqlConnection, sqlTransaction))
                            {
                                cmd2.Parameters.AddWithValue("@customername", customer.Name);
                                cmd2.Parameters.AddWithValue("@birthdate", customer.BirthDate);
                                cmd2.Parameters.AddWithValue("@phone", customer.Phone);
                                cmd2.Parameters.AddWithValue("@email", customer.Email);
                                cmd2.Parameters.AddWithValue("@cardcode", cardId);
                                cmd2.Parameters.AddWithValue("@points", customer.Points);

                                cmd2.ExecuteNonQuery();

                                sqlTransaction.Commit();
                            }
                        }
                    }
                }
            } catch (SqlException ex) { throw ex;}

            return true;
        }

        public bool UpdateStock(Product product)
        {
            try
            {
                SqlCommand cmd;
                string sqlStatement = @"UPDATE PRODUCT SET stock = @stock WHERE code = @productcode;";
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
                    {
                        using (cmd = new SqlCommand(sqlStatement, sqlConnection, sqlTransaction))
                        {
                            cmd.Parameters.AddWithValue("@stock", product.Stock);
                            cmd.Parameters.AddWithValue("@productcode", product.Code);
                            cmd.ExecuteNonQuery();

                            sqlTransaction.Commit();
                        }
                    }
                }
            } catch (SqlException ex) { throw ex;}

            return true;
        }
    }
}
