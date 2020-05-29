using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System;

namespace AdoNetWinForms
{
    public partial class Form1 : Form
    {
        int pageSize = 20; // размер страницы
        int pageNumber = 0; // текущая страница
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlDataAdapter adapter;
        DataSet ds;
        public Form1()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(GetSql(), connection);
                ds = new DataSet();
                adapter.Fill(ds,"Orders");
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns["OrderID"].ReadOnly = true;
            }
        }

        private string GetSql()
        {
            return $"SELECT * FROM dbo.Orders ORDER BY OrderID OFFSET {pageNumber}*{pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {

        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (ds.Tables["Orders"].Rows.Count < pageSize) return;
            pageNumber++;
            using(var connection = new SqlConnection(connectionString))
            {
                adapter = new SqlDataAdapter(GetSql(), connection);
                ds.Tables["Orders"].Rows.Clear();
                adapter.Fill(ds, "Orders");
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (pageNumber == 0) return;
            pageNumber--;
            using(var connection = new SqlConnection(connectionString))
            {
                adapter = new SqlDataAdapter(GetSql(), connection);
                ds.Tables["Orders"].Rows.Clear();
                adapter.Fill(ds, "Orders");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
