using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POSMini.Services.Repository;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Printing;
using System.Xml.Linq;

namespace POSMini
{
    public partial class FormQuanLyHoaDon : Form
    {
        private TextBox txtMaHD, txtNguoiTao;
        private DateTimePicker dtpNgay;
        private Button btnTim, btnTaiLai, btnXuatPDF, btnXoa;
        private DataGridView dgvHoaDon, dgvChiTiet;
        private HoaDonRepository _repo;

        public FormQuanLyHoaDon()
        {
            InitializeComponent();
            LoadUI();
            _repo = new HoaDonRepository();
            LoadHoaDon();
            dgvHoaDon.CellClick += DgvHoaDon_CellClick;
            btnTaiLai.Click += (s, e) => LoadHoaDon();
            btnTim.Click += BtnTim_Click;
            btnXuatPDF.Click += BtnXuatPDF_Click;
            btnXoa.Click += BtnXoaHoaDon_Click;
        }

        private void LoadUI()
        {
            this.Text = "Quản Lý Hóa Đơn";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label lblMaHD = new Label { Text = "Mã HD:", Location = new Point(20, 20), Width = 50 };
            txtMaHD = new TextBox { Location = new Point(110, 18), Width = 100 };

            Label lblNgay = new Label { Text = "Ngày:", Location = new Point(20, 60), Width = 50 };
            dtpNgay = new DateTimePicker { Location = new Point(110, 58), Width = 100, Format = DateTimePickerFormat.Short };

            Label lblNguoiTao = new Label { Text = "Người tạo:", Location = new Point(20, 100), Width = 70 };
            txtNguoiTao = new TextBox { Location = new Point(110, 98), Width = 100 };

            btnTim = new Button { Text = "🔎 Tìm", Location = new Point(20, 140), Size = new Size(100, 35), BackColor = Color.LightGreen };
            btnTaiLai = new Button { Text = "🔄 Tải lại", Location = new Point(130, 140), Size = new Size(100, 35), BackColor = Color.LightSkyBlue };
            btnXuatPDF = new Button { Text = "📄 Xuất PDF", Location = new Point(20, 190), Size = new Size(210, 35), BackColor = Color.LightSalmon };
            btnXoa = new Button { Text = "🗑️ Xóa hóa đơn", Location = new Point(20, 240), Size = new Size(210, 35), BackColor = Color.IndianRed };

            dgvHoaDon = new DataGridView
            {
                Location = new Point(230, 20),
                Size = new Size(500, 220),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvHoaDon.Columns.Add("MaHD", "Mã HD");
            dgvHoaDon.Columns.Add("NgayLap", "Ngày lập");
            dgvHoaDon.Columns.Add("NguoiTao", "Người tạo");
            dgvHoaDon.Columns.Add("TongTien", "Tổng tiền");

            dgvChiTiet = new DataGridView
            {
                Location = new Point(230, 250),
                Size = new Size(500, 280),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvChiTiet.Columns.Add("MaSP", "Mã SP");
            dgvChiTiet.Columns.Add("TenSP", "Tên SP");
            dgvChiTiet.Columns.Add("SoLuong", "Số lượng");
            dgvChiTiet.Columns.Add("DonGia", "Đơn giá");
            dgvChiTiet.Columns.Add("ThanhTien", "Thành tiền");

            this.Controls.AddRange(new Control[]
            {
                lblMaHD, txtMaHD, lblNgay, dtpNgay, lblNguoiTao, txtNguoiTao,
                btnTim, btnTaiLai, btnXuatPDF, btnXoa, dgvHoaDon, dgvChiTiet
            });
        }

        private void LoadHoaDon()
        {
            dgvHoaDon.Rows.Clear();
            var list = _repo.GetAll();
            foreach (var hd in list)
            {
                dgvHoaDon.Rows.Add(hd.MaHD, hd.NgayLap.ToShortDateString(), hd.NguoiTao, hd.TongTien.ToString("N0"));
            }
        }

        private void DgvHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string maHD = dgvHoaDon.Rows[e.RowIndex].Cells["MaHD"].Value.ToString();
                LoadChiTiet(maHD);
            }
        }

        private void BtnTim_Click(object sender, EventArgs e)
        {
            string maHD = txtMaHD.Text.Trim();
            string nguoiTao = txtNguoiTao.Text.Trim();
            DateTime ngay = dtpNgay.Value.Date;

            dgvHoaDon.Rows.Clear();

            var list = _repo.GetAll();
            var filtered = list.Where(hd =>
                (string.IsNullOrEmpty(maHD) || hd.MaHD.Contains(maHD)) &&
                (string.IsNullOrEmpty(nguoiTao) || hd.NguoiTao.Contains(nguoiTao)) &&
                (hd.NgayLap.Date == ngay)
            ).ToList();

            foreach (var hd in filtered)
            {
                dgvHoaDon.Rows.Add(hd.MaHD, hd.NgayLap.ToShortDateString(), hd.NguoiTao, hd.TongTien.ToString("N0"));
            }

            if (filtered.Count == 0)
                MessageBox.Show("Không tìm thấy hóa đơn nào phù hợp.", "Kết quả tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadChiTiet(string maHD)
        {
            dgvChiTiet.Rows.Clear();
            var chiTietList = _repo.GetChiTiet(maHD);
            foreach (var ct in chiTietList)
            {
                dgvChiTiet.Rows.Add(ct.MaSP, ct.TenSP, ct.SoLuong, ct.DonGia.ToString("N0"), ct.ThanhTien.ToString("N0"));
            }
        }
        private void BtnXoaHoaDon_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn để xóa.", "Thông báo");
                return;
            }

            string maHD = dgvHoaDon.SelectedRows[0].Cells["MaHD"].Value.ToString();

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa hóa đơn {maHD}?",
                                                   "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                _repo.DeleteHoaDon(maHD);
                LoadHoaDon();
                dgvChiTiet.Rows.Clear();
                MessageBox.Show("✅ Hóa đơn đã được xóa.", "Thông báo");
            }
        }
        private void BtnXuatPDF_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn 1 hóa đơn để xuất PDF.");
                return;
            }

            string maHD = dgvHoaDon.SelectedRows[0].Cells["MaHD"].Value.ToString();
            var chiTietList = _repo.GetChiTiet(maHD);

            if (chiTietList == null || chiTietList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu chi tiết để in.");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = $"HoaDon_{maHD}.pdf"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;

            Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
            try
            {
                PdfWriter.GetInstance(doc, new FileStream(saveDialog.FileName, FileMode.Create));
                doc.Open();

                var title = new iTextSharp.text.Paragraph("HÓA ĐƠN BÁN HÀNG",
                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD));

                doc.Add(title);
                doc.Add(new Paragraph(" "));

                var hd = _repo.GetAll().FirstOrDefault(h => h.MaHD == maHD);
                if (hd != null)
                {
                    doc.Add(new Paragraph($"Mã HĐ: {hd.MaHD}"));
                    doc.Add(new Paragraph($"Ngày lập: {hd.NgayLap:dd/MM/yyyy HH:mm}"));
                    doc.Add(new Paragraph($"Người tạo: {hd.NguoiTao}"));
                    doc.Add(new Paragraph(" "));
                }

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 5, 2, 3, 3 });

                string[] headers = { "Mã SP", "Tên SP", "SL", "Đơn giá", "Thành tiền" };
                foreach (var h in headers)
                {
                    var cell = new PdfPCell(new Phrase(h)) { BackgroundColor = BaseColor.LIGHT_GRAY };
                    table.AddCell(cell);
                }

                decimal tong = 0;
                foreach (var ct in chiTietList)
                {
                    table.AddCell(ct.MaSP);
                    table.AddCell(ct.TenSP);
                    table.AddCell(ct.SoLuong.ToString());
                    table.AddCell(ct.DonGia.ToString("N0"));
                    table.AddCell(ct.ThanhTien.ToString("N0"));
                    tong += ct.ThanhTien;
                }

                doc.Add(table);
                doc.Add(new Paragraph(" "));
                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD);



                doc.Close();
                MessageBox.Show("✅ Đã xuất hóa đơn PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi xuất PDF: " + ex.Message);
            }
        }
    }
}