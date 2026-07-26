using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projet_PE_HanineSmaily
{
    public partial class Form2 : Form
    {
        int selectedProductId;
        int clientId; 
        public Form2(int cid)
        {
            InitializeComponent();
            clientId = cid;
        }

        SqlConnection con = new SqlConnection("Data Source=.;Initial Catalog=OnlineStoreDB;Integrated Security=True");
        private void Form2_Load(object sender, EventArgs e)
        {
                con.Open();
               
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Products", con);
               
                DataTable dt = new DataTable();
                da.Fill(dt);
               
            listBox1.DisplayMember = "name";
            listBox1.DataSource = dt;

            con.Close();
            
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            DataRowView row = (DataRowView)listBox1.SelectedItem;
            selectedProductId = Convert.ToInt32(row["id"]);
            lblName.Text = row["name"].ToString();
            lblPrice.Text = row["price"].ToString() + " $";
            
            
            string imageName = row["image"].ToString().Trim();
            string path = Path.Combine(Application.StartupPath, "images", imageName);
            pictureBox1.Image = null;
            pictureBox1.Image = Image.FromFile(path);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            
          
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            int quantity = (int)numericUpDown1.Value;
            string query = "INSERT INTO orders (client_id, product_id, quantity) VALUES (@cid, @pid, @qty)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@cid", clientId);
            cmd.Parameters.AddWithValue("@pid", selectedProductId);
            cmd.Parameters.AddWithValue("@qty", quantity);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Added 😎🛒");
        }

        private void btncart_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3(clientId);
            f3.ShowDialog();
        }

  
    }     
}
