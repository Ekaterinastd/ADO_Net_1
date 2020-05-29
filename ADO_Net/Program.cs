using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Resources;

namespace ADO_Net
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //SelectFromDB(connectionString);
            //InsertToDb(connectionString);
            //ExeStorageProc(connectionString);
            //ProcWithOutputParameter(connectionString, 3);
            //Transaction(connectionString);
            UseSqlCommandBuilder(connectionString);
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
                var command = new SqlCommand(sqlExpression, connection);
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
                Console.WriteLine(string.Format("{0,12} {1,12} {2,12} {3,-10} {4}\n", result2.GetName(0), result2.GetName(1), result2.GetName(2), result2.GetName(3), result2.GetName(4)));
                while (result2.HasRows && result2.Read())
                {
                    Console.WriteLine(string.Format("{0,-2} {1,4} {2,-10} {3,-10} {4,-7} {5}", i, result2[0], result2[1], result2[2], result2[3], result2[4]));
                    i++;
                }
                result2.Close();
            }
            Console.WriteLine();
        }

        private static void ProcWithOutputParameter(string connectionString, int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var sqlExpression = "MaxOrerDate";
                var command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                var idEmployeeParam = new SqlParameter { ParameterName = "@employeeID", Value = id };
                command.Parameters.Add(idEmployeeParam);
                var maxOrderDateParam = new SqlParameter { ParameterName = "@maxOrderDate", SqlDbType=SqlDbType.DateTime };
                maxOrderDateParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(maxOrderDateParam);
                command.ExecuteNonQuery();
                Console.WriteLine($"Max order date for employeeID{id}: {command.Parameters["@maxOrderDate"].Value}");
            }
        }
        private static void Transaction(string connectionString)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                try
                {
                    //выполняем отдельные команды
                    command.CommandText= "INSERT INTO Users (ID, Name, Age) VALUES(1,'Tim', 34)";
                    //command.CommandText = "DELETE FROM Users WHERE Name='Tim'";
                    //command.CommandText = "DELETE FROM Users WHERE Name='Kat'";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Users (ID, Name, Age) VALUES(2, 'Kat', 31)";
                    command.ExecuteNonQuery();
                    //подтверждение транзакции
                    transaction.Commit();
                    Console.WriteLine("Data were insert to DB");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                }
            }
        }

        private static void UseSqlCommandBuilder(string connectionString)
        {
            string request = "SELECT * FROM Users";
            using(var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var adapter = new SqlDataAdapter(request, connection);
                var ds = new DataSet();//создаём хранилище для данных  из бд
                adapter.Fill(ds);//заполняем хранилище данными из бд
                var dt = ds.Tables[0];

                var newRow = dt.NewRow();
                newRow["ID"] = 3;
                newRow["Name"] = "Alice";
                newRow["Age"] = 24;
                dt.Rows.Add(newRow);

                var commandBuilder = new SqlCommandBuilder(adapter);//автоматическое определение необходимой команды на основе изменений dt
                adapter.Update(ds);
                ds.Clear();
                adapter.Fill(ds);

                foreach (DataColumn column in dt.Columns)
                    Console.Write("\t{0}", column.ColumnName);
                Console.WriteLine();
                foreach(DataRow row in dt.Rows)
                {
                    var cells = row.ItemArray;
                    foreach (var cell in cells)
                        Console.Write("\t{0}", cell);
                    Console.WriteLine();
                }

            }
        }
    }
}
