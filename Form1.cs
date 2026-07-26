using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Projet_PE_HanineSmaily
{
    public partial class Form1 : Form
    {
        string conn = "Data Source=.;Initial Catalog=OnlineStoreDB;Integrated Security=True";
        
        public Form1()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=.;Initial Catalog=OnlinestoreDB;Integrated Security=True");

            string query = "SELECT * FROM users WHERE username=@us AND password=@pw";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@us", txtUsername.Text);
            cmd.Parameters.AddWithValue("@pw", txtPassword.Text);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int clientId = Convert.ToInt32(reader["id"]);

                Form2 f2 = new Form2(clientId); 
                f2.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Wrong login ❌");
            }

            con.Close();
        }

    }
}
