using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using POSMini.Service.Singleton;

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
            txtPass.UseSystemPasswordChar = true;
        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập tài khoản");
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                txtPass.Focus();
                return;
            }

            if (AuthenticateUser(txtEmail.Text, txtPass.Text))
            {
                

                FormMain mainForm = new FormMain();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không đúng!");
                txtEmail.Focus();
            }
        }

        private bool AuthenticateUser(string tenDangNhap, string password)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM TaiKhoan 
                    WHERE TenDangNhap = @TenDangNhap 
                      AND MatKhau = @MatKhau 
                      AND TrangThai = 1";

                
                using (SqlConnection conn = DatabaseConnection.Instance.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        cmd.Parameters.AddWithValue("@MatKhau", password);

                        conn.Open(); // mở kết nối

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
            txtPass.UseSystemPasswordChar = !chkShowPassword.Checked;
        }
    }
}
