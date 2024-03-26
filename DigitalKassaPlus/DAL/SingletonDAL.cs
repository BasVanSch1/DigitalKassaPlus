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
                    string phone = (string) reader["phone"];
                    string email = (string) reader["email"];
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

                    string sqlStatement = "SELECT p.code, p.description, p.price, p.taxcode, p.type, t.percentage, t.description as taxdescription " +
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

                        switch (type.ToLower())
                        {
                            case "article":
                                {
                                    products.Add(productcode, new Article(productcode, description, price, new TaxCode(taxid, taxpercentage, taxdescription)));
                                    break;
                                }
                            case "service":
                                {
                                    products.Add(productcode, new Service(productcode, description, price, new TaxCode(taxid, taxpercentage, taxdescription)));
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
        public void InsertOrder(Order order)
        {
            try
            {
                SqlCommand cmd;
                string sqlStatement = @"INSERT INTO ORDERS (employeecode, ispayed) VALUES(@employeecode, @ispayed); SELECT CAST(scope_identity() AS int);";
                string orderProductStatement = @"INSERT INTO ORDERPRODUCT (ordercode, productcode, amount) VALUES(@ordercode, @productcode, @amount)";
                
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
                    {
                        cmd = new SqlCommand(sqlStatement, sqlConnection);
                        cmd.Parameters.Add("@employeecode", System.Data.SqlDbType.Int, order.Employee.Id);
                        cmd.Parameters.Add("@ispayed", System.Data.SqlDbType.Bit, order.IsPayed ? 1 : 0); // if IsPayed == True: '1' else '0'

                        int orderId = (int) cmd.ExecuteScalar();

                        cmd.CommandText = orderProductStatement;

                        foreach (KeyValuePair<Product, int> pair in order.Products)
                        {
                            cmd.Parameters.Clear();

                            cmd.Parameters.Add("@ordercode", System.Data.SqlDbType.Int, orderId);
                            cmd.Parameters.Add("@productcode", System.Data.SqlDbType.Int, pair.Key.Code);
                            cmd.Parameters.Add("@amount", System.Data.SqlDbType.Int, pair.Value);

                            cmd.ExecuteNonQuery();

                        }
                        
                        sqlTransaction.Commit();
                    }

                }

            } catch (SqlException ex) { throw ex;}

            
        }

    }
}
