using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POSMini
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }

        private void OpenFormInPanel(Form form)
        {
            panel1.Controls.Clear(); // Xóa form cũ
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panel1.Controls.Add(form);
            form.Show();
        }

      
        private void btnlogout_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide(); // Ẩn FormMain
                Form1 loginForm = new Form1(); // Hiển thị Form đăng nhập
                loginForm.Show();
            }
        }

        private void btnQuanLyHoaDon_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new FormQuanLyHoaDon());
        }

        private void btnQuanLySanPham_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new FormQuanLySanPham());
        }

        private void btnQuanLyTonKho_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new FormQuanLyTonKho());
        }

        private void btnThongKe_Click_1(object sender, EventArgs e)
        {
            OpenFormInPanel(new FormThongKe());

        }

        private void btnlogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide(); // Ẩn FormMain
                Form1 loginForm = new Form1(); // Hiển thị Form đăng nhập
                loginForm.Show();
            }
        }

        private void btnBanHangNhanh_Click(object sender, EventArgs e)
        {
            OpenFormInPanel(new FormBanHangNhanh());
        }
    }
}
