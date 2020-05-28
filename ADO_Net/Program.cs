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
            //SelectFromDB(connectionString);
            //InsertToDb(connectionString);
            ExeStorageProc(connectionString);           
            Console.Read();
        }
        private static void SelectFromDB(string connectionString)
        {
            var sqlExpression = "select ContactName, Country from dbo.Customers where Country in ('USA', 'Canada') order by ContactName";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(sqlExpression, connection);
                var result = command.ExecuteReader();
                var i = 1;
                Console.WriteLine($"Request: {sqlExpression}\n");
                Console.WriteLine($"\t{result.GetName(0)} \t{result.GetName(1)}\n");

                while (result.HasRows && result.Read())
                {
                    Console.WriteLine(string.Format("{0,-3} {1,-20} {2}", i, result[0], result[1]));
                    i++;
                }
                result.Close();
                Console.WriteLine();
            }
        }

        private static void InsertToDb(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sqlExpression = "INSERT INTO dbo.Region (RegionID, RegionDescription) VALUES (11, 'newRegion11')";
                var command = new SqlCommand(sqlExpression,connection);
                Console.WriteLine($"Request: {command.CommandText}\n");
                var result1 = command.ExecuteReader();
                Console.WriteLine($"Number of rows changed: {result1}");
                result1.Close();
                Console.WriteLine();
            }
        }

        private static void ExeStorageProc(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sqlExpression = "GreatestOrders";
                var command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var yearParam = new SqlParameter { ParameterName = "@Year", Value = 1997 };
                var countParam = new SqlParameter { ParameterName = "@Count", Value = 100 };
                command.Parameters.Add(yearParam);
                command.Parameters.Add(countParam);
                var result2 = command.ExecuteReader();
                var i = 1;
                Console.WriteLine(string.Format("{0,12} {1,12} {2,12} {3,-10} {4}\n",result2.GetName(0), result2.GetName(1), result2.GetName(2),result2.GetName(3),result2.GetName(4)));
                while (result2.HasRows && result2.Read())
                {
                    Console.WriteLine(string.Format("{0,-2} {1,4} {2,-10} {3,-10} {4,-7} {5}", i, result2[0], result2[1], result2[2], result2[3], result2[4]));
                    i++;
                }
                result2.Close();
            }
        }
    }
}
