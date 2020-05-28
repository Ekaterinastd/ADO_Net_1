using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Cache;

namespace ADO_Net
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var sqlExpression = "select ContactName, Country from dbo.Customers where Country in ('USA', 'Canada') order by ContactName";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connection is open");
                var command = new SqlCommand(sqlExpression, connection);

                var result1 = command.ExecuteNonQuery();

                var result = command.ExecuteReader();
                var i = 1;
                Console.WriteLine("Request: select ContactName, Country from dbo.Customers where Country in ('USA', 'Canada') order by ContactName\n");
                Console.WriteLine($"\tContactName \tCountry\n");
                
                while (result.Read())
                {
                    Console.WriteLine(string.Format("{0,-3} {1,-20} {2}", i, result[0], result[1]));
                    i++;
                }
                Console.WriteLine();
            }
            Console.WriteLine("Connection is close");
            Console.Read();
        }
    }
}
