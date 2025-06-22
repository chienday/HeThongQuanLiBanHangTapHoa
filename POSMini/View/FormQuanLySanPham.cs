using POSMini.Models;
using POSMini.Service;
using POSMini.Service.Singleton;
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
            dgvSanPham.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSanPham.MultiSelect = false;
            LoadData();
            LoadProductTypesComboBox();
        }
        private void LoadProductTypesComboBox()
        {
            var productTypes = SanPhamService.Instance.GetLoai();

            // 2. Gán danh sách này làm nguồn dữ liệu
            cmbTenLoai.DataSource = productTypes;

            // 3. Chỉ định cho ComboBox biết:
            // - Hiển thị thuộc tính 'TenLoai' cho người dùng thấy
            cmbTenLoai.DisplayMember = "TenLoai";
            // - Khi ta lấy giá trị, hãy lấy thuộc tính 'MaLoai'
            cmbTenLoai.ValueMember = "MaLoai";

            // 4. Bỏ chọn mục mặc định
            cmbTenLoai.SelectedIndex = -1;
        }
        private void LoadData()
        {
            dgvSanPham.DataSource = null; // Xóa dữ liệu cũ
            var allProducts = SanPhamService.Instance.GetAllSP();
            SetupDataGridView(); // Cài đặt cột trước
            dgvSanPham.DataSource = allProducts; // Gán dữ liệu mới
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
                Width = 50
            };
            dgvSanPham.Columns.Add(checkColumn);

            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSP", HeaderText = "Mã SP" });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenSP", HeaderText = "Tên Sản Phẩm" });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenLoai", HeaderText = "Loại" });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GiaNhap", HeaderText = "Giá Nhập" });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GiaBan", HeaderText = "Giá Bán" });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MoTa", HeaderText = "Mô Tả" });

            dgvSanPham.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ClearInputs()
        {
            _selectedMaSP = null;
            txtMaSP.Clear();
            txtTenSP.Clear();
            cmbTenLoai.SelectedIndex = -1;
            cmbTenLoai.Text = "";
            txtGiaBan.Clear();
            txtGiaNhap.Clear();
            txtSoLuongTon.Clear();
            txtMoTa.Clear();
            txtHinhAnhUrl.Clear();
            picHinhAnh.ImageLocation = null;
            //picHinhAnh.Tag = null;
            txtMaSP.Enabled = true;
            if (chkSelectAll != null) chkSelectAll.Checked = false;
        }

        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvSanPham.Rows[e.RowIndex];
            var viewModel = row.DataBoundItem as SanPhamView;
            if (viewModel == null) { ClearInputs(); return; }

            _selectedMaSP = viewModel.MaSP;
            txtMaSP.Text = viewModel.MaSP;
            txtTenSP.Text = viewModel.TenSP;
            cmbTenLoai.SelectedValue = viewModel.MaLoai;
            txtGiaNhap.Text = viewModel.GiaNhap.ToString();
            txtGiaBan.Text = viewModel.GiaBan.ToString();
            txtSoLuongTon.Text = viewModel.SoLuongTon.ToString();
            txtHinhAnhUrl.Text = viewModel.HinhAnh;
            txtMoTa.Text = viewModel.MoTa;

            if (!string.IsNullOrEmpty(viewModel.HinhAnh))
            {
                try { picHinhAnh.ImageLocation = viewModel.HinhAnh; }
                catch { picHinhAnh.Image = null; }
            }
            else { picHinhAnh.Image = null; }

            picHinhAnh.Tag = viewModel.HinhAnh;
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

        private void btnXemAnh_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtHinhAnhUrl.Text))
            {
                try { picHinhAnh.ImageLocation = txtHinhAnhUrl.Text.Trim(); }
                catch (Exception ex) { MessageBox.Show("Không thể tải ảnh: " + ex.Message); }
            }
        }

        private void chkSelectAll_CheckedChanged_1(object sender, EventArgs e)
        {
            if (dgvSanPham.Rows.Count == 0) return;
            dgvSanPham.EndEdit();
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (row.IsNewRow) continue;
                row.Cells["colChon"].Value = chkSelectAll.Checked;
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            LoadData();

        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTimKiem.Text.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                LoadData();
                return;
            }
            dgvSanPham.DataSource = SanPhamService.Instance.SearchProductsForDisplay(searchTerm);

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) || string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Mã và Tên sản phẩm không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sanPham = new SanPham
            {
                MaSP = txtMaSP.Text.Trim(),
                TenSP = txtTenSP.Text.Trim(),
                MaLoai = cmbTenLoai.SelectedValue?.ToString(),
                GiaNhap = decimal.TryParse(txtGiaNhap.Text, out var gn) ? gn : 0,
                GiaBan = decimal.TryParse(txtGiaBan.Text, out var gb) ? gb : 0,
                SoLuongTon = int.TryParse(txtSoLuongTon.Text, out var sl) ? sl : 0,
                MoTa = txtMoTa.Text.Trim(),
                HinhAnh = txtHinhAnhUrl.Text.Trim(),
                DonViTinh = "Cái",
                TrangThai = true
            };

            if (SanPhamService.Instance.ThemSP(sanPham))
            {
                MessageBox.Show("Thêm sản phẩm thành công!");
                LoadData();
            }
            else
            {
                MessageBox.Show("Thêm thất bại. Mã sản phẩm có thể đã tồn tại.");
            }

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaSP))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để sửa!");
                return;
            }

            var sanPham = new SanPham
            {
                MaSP = _selectedMaSP,
                TenSP = txtTenSP.Text.Trim(),
                MaLoai = cmbTenLoai.SelectedValue?.ToString(),
                GiaNhap = decimal.TryParse(txtGiaNhap.Text, out var gn) ? gn : 0,
                GiaBan = decimal.TryParse(txtGiaBan.Text, out var gb) ? gb : 0,
                SoLuongTon = int.TryParse(txtSoLuongTon.Text, out var sl) ? sl : 0,
                MoTa = txtMoTa.Text.Trim(),
                HinhAnh = txtHinhAnhUrl.Text.Trim(),
                DonViTinh = "Cái",
                TrangThai = true
            };

            if (SanPhamService.Instance.SuaSP(sanPham))
            {
                MessageBox.Show("Cập nhật thành công!");
                LoadData();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại.");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var maSPToDelete = new List<string>();
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (row.IsNewRow) continue;
                if (Convert.ToBoolean(row.Cells["colChon"].Value))
                {
                    var viewModel = row.DataBoundItem as SanPhamView;
                    if (viewModel != null) maSPToDelete.Add(viewModel.MaSP);
                }
            }

            if (maSPToDelete.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sản phẩm để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string message = $"Bạn có chắc muốn xóa (ngừng kinh doanh) {maSPToDelete.Count} sản phẩm đã chọn?";
            if (MessageBox.Show(message, "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (SanPhamService.Instance.XoaNhieuSP(maSPToDelete))
                {
                    MessageBox.Show("Xóa sản phẩm thành công!");
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại.");
                }
            }

        }
    }
}
