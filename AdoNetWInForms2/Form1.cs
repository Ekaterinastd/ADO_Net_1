using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdoNetWInForms2
{
    public partial class Form1 : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        DataSet ds;
        SqlDataAdapter adapter;
        string request = "SELECT * FROM Employees";
        SqlCommandBuilder commandBuilder;
        public Form1()
        {
            InitializeComponent();
            dataGridView1.SelectionMode= DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(request, connection);

                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns["EmployeeID"].ReadOnly = true;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            using(var connection = new SqlConnection(connectionString))
            {
            //    connection.Open();
            //    var adapter = new SqlDataAdapter(request, connection);
            //    var commandBuilder = new SqlCommandBuilder(adapter);
            //    adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
            //    adapter.InsertCommand.Parameters.Add(new SqlParameter("@lastName", SqlDbType.NVarChar, 50, "LastName"));
            //    adapter.InsertCommand.Parameters.Add(new SqlParameter("@firstName", SqlDbType.NVarChar, 50, "FirstName"));
            //    var outputParam = adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
            //    outputParam.Direction = ParameterDirection.Output;

            //    ds = new DataSet();
            //    adapter.Fill(ds);
            //    var dt = ds.Tables[0];
            //    var newRow = dt.NewRow();
            //    newRow["LastName"] = "LastName1";
            //    newRow["FirstName"] = "FirstName";
            //    newRow["BirthDate"] = 30 / 05 / 2020;
            //    dt.Rows.Add(newRow);

            //    adapter.Update(ds);
            //    ds.AcceptChanges();

                var row = ds.Tables[0].NewRow();
                ds.Tables[0].Rows.Add(row);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(request, connection);
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.InsertCommand = new SqlCommand("CreateEmployee", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@lastName", SqlDbType.NVarChar, 50, "LastName"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@firstName", SqlDbType.NVarChar, 50, "FirstName"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@birthDate", SqlDbType.DateTime,10, "BirthDate"));
                var outputParam = adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "EmployeeID");
                outputParam.Direction = ParameterDirection.Output;
                adapter.Update(ds);
            }
        }
    }
}
