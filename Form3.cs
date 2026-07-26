using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.VisualBasic;

namespace Projet_PE_HanineSmaily
{
    public partial class Form3 : Form
    {
        int clientId;
        SqlConnection con = new SqlConnection("Data Source=.;Initial Catalog=OnlineStoreDB;Integrated Security=True");

        public Form3(int cid)
        {
            InitializeComponent();
            clientId = cid;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            LoadCartData();
        }

        private void LoadCartData()
        {
            try
            {
                if (con.State == ConnectionState.Open) con.Close();

                con.Open();
                string query = @"SELECT o.product_id AS [Product ID], p.name AS [Product Name], 
                                        o.quantity AS [Quantity], p.price AS [Unit Price], 
                                        (o.quantity * p.price) AS [Total]
                                 FROM orders o
                                 JOIN Products p ON o.product_id = p.id
                                 WHERE o.client_id = @cid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@cid", clientId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvCart.DataSource = dt;

                if (dgvCart.Columns.Contains("Product ID"))
                    dgvCart.Columns["Product ID"].Visible = false;

                if (!dgvCart.Columns.Contains("DeleteBtn"))
                {
                    DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
                    btnDelete.Name = "DeleteBtn";
                    btnDelete.HeaderText = "Action";
                    btnDelete.Text = "❌ Remove";
                    btnDelete.UseColumnTextForButtonValue = true;
                    btnDelete.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvCart.Columns.Add(btnDelete);
                }

                CalculateTotal(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cart: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void CalculateTotal(DataTable dt)
        {
            decimal totalSum = 0;
            foreach (DataRow row in dt.Rows)
            {
                totalSum += Convert.ToDecimal(row["Total"]);
            }
            lblTotal.Text = "Total: " + totalSum.ToString() + " $";
        }

        private void dgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCart.Columns[e.ColumnIndex].Name == "DeleteBtn")
            {
                int prodId = Convert.ToInt32(dgvCart.Rows[e.RowIndex].Cells["Product ID"].Value);
                string prodName = dgvCart.Rows[e.RowIndex].Cells["Product Name"].Value.ToString();

                DialogResult result = MessageBox.Show($"Are you sure you want to remove {prodName} from cart?",
                                                      "Remove Item", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                        con.Open();
                        string query = "DELETE FROM orders WHERE client_id = @cid AND product_id = @pid";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@cid", clientId);
                        cmd.Parameters.AddWithValue("@pid", prodId);
                        cmd.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("Item removed successfully! 👍", "Done");
                        LoadCartData();
                  
                }
            }
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            string totalText = lblTotal.Text.Replace("Total: ", "").Replace(" $", "").Trim();
            decimal totalAmount = 0;
            decimal.TryParse(totalText, out totalAmount);

            if (dgvCart.Rows.Count == 0 || totalAmount <= 0)
            {
                MessageBox.Show("Your cart is empty! 🛒", "Info");
                return;
            }

            string address = Interaction.InputBox("Please enter your shipping address:", "Shipping Address", "");

            if (string.IsNullOrEmpty(address.Trim()))
            {
                MessageBox.Show("Address is required to complete the order!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           
                con.Open();

                string insertCheckoutQuery = "INSERT INTO Checkouts (client_id, address, order_date, total_amount) OUTPUT INSERTED.id VALUES (@cid, @addr, @dt, @total)";
                SqlCommand cmdCheckout = new SqlCommand(insertCheckoutQuery, con);
                cmdCheckout.Parameters.AddWithValue("@cid", clientId);
                cmdCheckout.Parameters.AddWithValue("@addr", address);
                cmdCheckout.Parameters.AddWithValue("@dt", DateTime.Now);
                cmdCheckout.Parameters.AddWithValue("@total", totalAmount);

                int newCheckoutId = (int)cmdCheckout.ExecuteScalar();

                string transferQuery = @"INSERT INTO Checkout_Details (checkout_id, product_id, quantity) 
                                 SELECT @checkoutId, product_id, quantity 
                                 FROM orders 
                                 WHERE client_id = @cid";

                SqlCommand cmdTransfer = new SqlCommand(transferQuery, con);
                cmdTransfer.Parameters.AddWithValue("@checkoutId", newCheckoutId);
                cmdTransfer.Parameters.AddWithValue("@cid", clientId);
                cmdTransfer.ExecuteNonQuery();

                string deleteQuery = "DELETE FROM orders WHERE client_id = @cid";
                SqlCommand cmdDelete = new SqlCommand(deleteQuery, con);
                cmdDelete.Parameters.AddWithValue("@cid", clientId);
                cmdDelete.ExecuteNonQuery();

                con.Close();

                MessageBox.Show("Thank you for your purchase from Haneen Store! Your order is on the way. 🎉🛒", "Success");
                this.Close();
            
           
        }
    
    }
}