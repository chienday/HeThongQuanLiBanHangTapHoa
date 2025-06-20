using POSMini.Models;
using POSMini.Services;
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

namespace POSMini
{
    public partial class FormQuanLySanPham : Form
    {
        private string _selectedMaSP = null;
        public FormQuanLySanPham()
        {
            InitializeComponent();
        }

        private void FormQuanLySanPham_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadProductTypesComboBox();
        }
        private void LoadProductTypesComboBox()
        {
            cmbTenLoai.Items.Clear();
            cmbTenLoai.Items.AddRange(new object[] { "Nước giải khát", "Đồ ăn vặt", "Đồ gia dụng", "Sản phẩm khác" });
            cmbTenLoai.SelectedIndex = -1;
        }
        private void LoadData()
        {
            var allSP = SanPhamService.Instance.GetAllSP();
            SetupDataGridView();
            dgvSanPham.DataSource = allSP;
            ClearInputs();
        }
        private void SetupDataGridView()
        {
            dgvSanPham.AutoGenerateColumns = false;
            dgvSanPham.Columns.Clear();
            var checkColumn = new DataGridViewCheckBoxColumn
            {
                Name = "colChon",
                HeaderText = "Chọn",
                Width = 50,

            };
            dgvSanPham.Columns.Add(checkColumn);

            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSP", HeaderText = "Mã SP" });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenSP", HeaderText = "Tên Sản Phẩm" });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GiaBan", HeaderText = "Giá Bán" });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenLoai", HeaderText = "Loại" });

            dgvSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ClearInputs()
        {
            _selectedMaSP = null;
            txtMaSP.Clear();
            txtTenSP.Clear();
            cmbTenLoai.Text = "";
            txtGiaBan.Clear();
            txtSoLuong.Clear();
            txtHinhAnhUrl.Clear();
            picHinhAnh.ImageLocation = null;
            //picHinhAnh.Tag = null;
            txtMaSP.Enabled = true;
        }

        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvSanPham.Rows[e.RowIndex];
            var sanPham = row.DataBoundItem as SanPham;
            if (sanPham == null)
            {
                ClearInputs();
                return;
            }

            _selectedMaSP = sanPham.MaSP;
            txtMaSP.Text = sanPham.MaSP;
            txtTenSP.Text = sanPham.TenSP;
            cmbTenLoai.Text = sanPham.TenLoai;
            txtGiaBan.Text = sanPham.GiaBan.ToString();
            txtSoLuong.Text = sanPham.SoLuong.ToString();

            txtHinhAnhUrl.Text = sanPham.HinhAnh; // Hiển thị URL lên TextBox
            if (!string.IsNullOrEmpty(sanPham.HinhAnh))
            {
                try
                {
                    // PictureBox có thể tải ảnh trực tiếp từ URL
                    picHinhAnh.ImageLocation = sanPham.HinhAnh;
                }
                catch (Exception)
                {
                    // Nếu URL lỗi hoặc không có mạng, không hiển thị ảnh
                    picHinhAnh.Image = null;
                }
            }
            else
            {
                picHinhAnh.Image = null;
            }
            txtMaSP.Enabled = false;
        }

        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "Chọn một ảnh sản phẩm"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                picHinhAnh.ImageLocation = dialog.FileName;
                picHinhAnh.Tag = dialog.FileName;
            }
        }



        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) || string.IsNullOrWhiteSpace(txtTenSP.Text)
                || string.IsNullOrWhiteSpace(txtSoLuong.Text) || string.IsNullOrWhiteSpace(txtGiaBan.Text))
            {
                MessageBox.Show("Không được để trống trường thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //string imagePathInApp = SaveImageAndGetPath();

            var sanPham = new SanPham
            {
                MaSP = txtMaSP.Text,
                TenSP = txtTenSP.Text,
                TenLoai = cmbTenLoai.Text,
                GiaBan = decimal.TryParse(txtGiaBan.Text, out decimal gia) ? gia : 0,
                SoLuong = int.TryParse(txtSoLuong.Text, out int sl) ? sl : 0,
                HinhAnh = txtHinhAnhUrl.Text.Trim()
            };

            if (SanPhamService.Instance.ThemSP(sanPham))
            {
                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show("Thêm sản phẩm thất bại. Mã sản phẩm có thể đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaSP))
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ danh sách để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //string imagePathInApp = SaveImageAndGetPath();

            var sanPham = new SanPham
            {
                MaSP = _selectedMaSP,
                TenSP = txtTenSP.Text,
                TenLoai = cmbTenLoai.Text,
                GiaBan = decimal.TryParse(txtGiaBan.Text, out decimal gia) ? gia : 0,
                SoLuong = int.TryParse(txtSoLuong.Text, out int sl) ? sl : 0,
                HinhAnh = txtHinhAnhUrl.Text.Trim()
            };

            if (SanPhamService.Instance.SuaSP(sanPham))
            {
                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show("Cập nhật sản phẩm thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var maSPToDelete = new List<string>();

            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                bool isSelected = Convert.ToBoolean(row.Cells["colChon"].Value);

                if (isSelected)
                {
                    var sanPham = row.DataBoundItem as SanPham;
                    if (sanPham != null)
                    {
                        maSPToDelete.Add(sanPham.MaSP);
                    }
                }
            }

            if (maSPToDelete.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sản phẩm để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string message = $"Bạn có chắc chắn muốn xóa {maSPToDelete.Count} sản phẩm đã chọn?";
            var confirmResult = MessageBox.Show(message, "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {

                if (SanPhamService.Instance.XoaNhieuSP(maSPToDelete))
                {
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTimKiem.Text.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.");
                return;
            }

            var searchResults = SanPhamService.Instance.TimKiemSP(searchTerm);

            dgvSanPham.DataSource = searchResults;

            if (searchResults.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sản phẩm nào khớp.");
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            LoadData();
        }



        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            this.dgvSanPham.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSanPham_CellValueChanged);

            try
            {
                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    if (row.IsNewRow) continue;

                    var chkCell = row.Cells["colChon"] as DataGridViewCheckBoxCell;
                    if (chkCell != null)
                    {
                        chkCell.Value = chkSelectAll.Checked;
                    }
                }
            }
            finally
            {
                this.dgvSanPham.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSanPham_CellValueChanged);


                UpdateSelectAllCheckBoxState();
            }
        }
        private void UpdateSelectAllCheckBoxState()
        {
            //Tắt sự kiện
            this.chkSelectAll.CheckedChanged -= new System.EventHandler(this.chkSelectAll_CheckedChanged);

            bool allChecked = true;
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (row.IsNewRow) continue;
                if (Convert.ToBoolean(row.Cells["colChon"].Value) == false)
                {
                    allChecked = false;
                    break;
                }
            }
            chkSelectAll.Checked = allChecked;

            // Bật lại sự kiện
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
        }

        private void dgvSanPham_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                UpdateSelectAllCheckBoxState();
            }
        }

        private void btnXemAnh_Click(object sender, EventArgs e)
        {
            string imageUrl = txtHinhAnhUrl.Text.Trim();
            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    // Yêu cầu PictureBox tải ảnh từ URL trong TextBox
                    picHinhAnh.ImageLocation = imageUrl;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể tải ảnh từ URL này. Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    picHinhAnh.Image = null;
                }
            }
            else
            {
                picHinhAnh.Image = null;
            }
        }
    }
}
