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
namespace POSMini
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập Email!");
                txtEmail.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Vui lòng nhập Password!");
                txtPass.Focus();
                return;
            }
            if (AuthenticateUser(txtEmail.Text, txtPass.Text))
            {
                MessageBox.Show("Đăng nhập thành công!");
                FormMain mainForm = new FormMain();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Email hoặc mật khẩu không đúng!");
                txtEmail.Focus();
            }
        }
        private bool AuthenticateUser(string email, string password)
        {
            string connectionString = @"Server=LAPTOP-BAR8P7OI\SQLEXPRESS;Database=QuanLiBanHangTapHoa;Trusted_Connection=True;";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        MessageBox.Show("Không thể kết nối với SQL Server.");
                        return false;
                    }

                    string query = "SELECT COUNT(*) FROM Users WHERE Email=@Email AND Password=@Password";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối SQL: " + ex.Message);
                return false;
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPass.Text))
            {
                txtPass.UseSystemPasswordChar = !chkShowPassword.Checked;
            }
        }
    }
}
