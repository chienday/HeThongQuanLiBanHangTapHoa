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
    public partial class FormBanHangNhanh : Form
    {
        public FormBanHangNhanh()
        {
            InitializeComponent();
        }

        private void FormBanHangNhanh_Load(object sender, EventArgs e)
        {

        }

        using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using POSMini.DataAccess;
using POSMini.Services.Observer;
using POSMini.Services.Strategy;
using SPOSMini.Services.Observer;


namespace POSMini
    {
        public partial class FormBanHangNhanh : Form
        {
            private FlowLayoutPanel flpSanPham;
            private TextBox tbTim;
            private DataGridView dgvGioHang;
            private Label lblTongTien;

            private List<SanPham> sanPhamList = new List<SanPham>();
            private int pageSize = 8;
            private int currentPage = 1;
            private HoaDonContext contextTinhTien = new HoaDonContext(new TinhTienMacDinh());



            public FormBanHangNhanh()
            {
                InitializeComponent();
                LoadSanPhamFromDatabase();
                SetupAutoComplete();
            }

            private void InitializeComponent()
            {
                this.Text = "POSMini - Bán hàng nhanh";
                this.Size = new Size(1200, 700);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.WhiteSmoke;

                // Khu vực chính
                Panel pnlMain = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10)
                };
                this.Controls.Add(pnlMain);

                // Ô tìm kiếm
                tbTim = new TextBox
                {
                    Text = "Nhập tên sản phẩm hoặc mã",
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 12),
                    Height = 35,
                    Width = 300,
                    Location = new Point(10, 10)
                };
                tbTim.GotFocus += (s, e) =>
                {
                    if (tbTim.Text == "Nhập tên sản phẩm hoặc mã")
                    {
                        tbTim.Text = "";
                        tbTim.ForeColor = Color.Black;
                    }
                };
                tbTim.LostFocus += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(tbTim.Text))
                    {
                        tbTim.Text = "Nhập tên sản phẩm hoặc mã";
                        tbTim.ForeColor = Color.Gray;
                    }
                };
                tbTim.KeyDown += TbTim_KeyDown;
                pnlMain.Controls.Add(tbTim);

                // Danh sách sản phẩm
                flpSanPham = new FlowLayoutPanel
                {
                    Location = new Point(10, 50),
                    Size = new Size(300, 350),
                    AutoScroll = true,
                    Padding = new Padding(10),
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = true
                };
                pnlMain.Controls.Add(flpSanPham);

                // Giỏ hàng
                dgvGioHang = new DataGridView
                {
                    Location = new Point(320, 10),
                    Size = new Size(400, 200),
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = true
                };
                dgvGioHang.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaSP", HeaderText = "Mã SP", ReadOnly = true });
                dgvGioHang.Columns.Add("TenSP", "Tên sản phẩm");
                dgvGioHang.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoLuong", HeaderText = "Số lượng" });
                dgvGioHang.Columns.Add(new DataGridViewTextBoxColumn { Name = "DonGia", HeaderText = "Đơn giá" });
                dgvGioHang.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThanhTien", HeaderText = "Thành tiền", ReadOnly = true });
                dgvGioHang.CellEndEdit += DgvGioHang_CellEndEdit;
                pnlMain.Controls.Add(dgvGioHang);
                var btnCol = new DataGridViewButtonColumn
                {
                    Name = "Xoa",
                    HeaderText = "Xóa",
                    Text = "X",
                    UseColumnTextForButtonValue = true,
                    Width = 40
                };
                dgvGioHang.Columns.Add(btnCol);


                lblTongTien = new Label
                {
                    Text = "Tạm tính: 0 VND\nCần thanh toán: 0 VND",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Size = new Size(400, 45),
                    Location = new Point(320, 220), // Ngay dưới DataGridView
                    AutoSize = false
                };
                pnlMain.Controls.Add(lblTongTien);
                Button btnThanhToan = new Button
                {
                    Text = "Thanh Toán",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    BackColor = Color.MediumSeaGreen,
                    ForeColor = Color.White,
                    Size = new Size(150, 40),
                    Location = new Point(320, 410)
                };
                btnThanhToan.Click += BtnThanhToan_Click;
                pnlMain.Controls.Add(btnThanhToan);
                Button btnHuy = new Button
                {
                    Text = "Hủy",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    BackColor = Color.Orange,
                    ForeColor = Color.White,
                    Size = new Size(150, 40),
                    Location = new Point(480, 410)
                };
                btnHuy.Click += BtnHuy_Click;
                pnlMain.Controls.Add(btnHuy);
                dgvGioHang.CellClick += DgvGioHang_CellClick;
                ComboBox cbChienLuoc = new ComboBox
                {
                    Location = new Point(320, 460),
                    Size = new Size(200, 30),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10)
                };
                cbChienLuoc.Items.AddRange(new string[] { "Không giảm giá", "Chiết khấu 10%", "Tính VAT 8%" });
                cbChienLuoc.SelectedIndexChanged += (s, e) =>
                {
                    switch (cbChienLuoc.SelectedIndex)
                    {
                        case 0:
                            contextTinhTien.SetStrategy(new TinhTienMacDinh());
                            break;
                        case 1:
                            contextTinhTien.SetStrategy(new ChietKhau10());
                            break;
                        case 2:
                            contextTinhTien.SetStrategy(new TinhTienVAT());
                            break;
                    }
                    CapNhatTongTien();
                };
                pnlMain.Controls.Add(cbChienLuoc);
                cbChienLuoc.SelectedIndex = 0;
            }

            private void LoadSanPhamFromDatabase()
            {
                var conn = DatabaseConnection.Instance.GetConnection();
                {
                    try
                    {
                        if (conn.State != System.Data.ConnectionState.Open)
                            conn.Open();

                        string query = "SELECT MaSP, TenSP, DonGia, HinhAnh, SoLuong FROM SanPham";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            sanPhamList.Clear();

                            // ✅ Đặt trước vòng lặp
                            string basePath = Path.Combine(Application.StartupPath, "Images");

                            while (reader.Read())
                            {
                                var sp = new SanPham
                                {
                                    MaSP = reader["MaSP"].ToString(),
                                    TenSP = reader["TenSP"].ToString(),
                                    DonGia = Convert.ToDecimal(reader["DonGia"]),
                                    SoLuong = Convert.ToInt32(reader["SoLuong"]),
                                    HinhAnhPath = Path.Combine(basePath, reader["HinhAnh"]?.ToString() ?? "")
                                };

                                sanPhamList.Add(sp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message);
                    }
                }

                LoadSanPhamPage(1);
            }


            private void LoadSanPhamPage(int page)
            {
                flpSanPham.Controls.Clear();
                var products = sanPhamList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                foreach (var sp in products)
                {
                    Panel card = new Panel
                    {
                        Width = 100,
                        Height = 160,
                        Margin = new Padding(5),
                        BorderStyle = BorderStyle.FixedSingle,
                        Cursor = Cursors.Hand,
                        Tag = sp
                    };

                    PictureBox pic = new PictureBox
                    {
                        Width = 100,
                        Height = 100,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = LoadImage(sp.HinhAnhPath) ?? SystemIcons.Question.ToBitmap(),
                        Top = 5,
                        Left = 15,
                        Cursor = Cursors.Hand,
                        Tag = sp
                    };

                    Label lblTen = new Label
                    {
                        Text = sp.TenSP,
                        AutoSize = false,
                        Width = 100,
                        Height = 30,
                        Top = 110,
                        Left = 0,
                        Font = new Font("Segoe UI", 9),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Cursor = Cursors.Hand,
                        Tag = sp
                    };

                    Label lblGia = new Label
                    {
                        Text = $"{sp.DonGia:N0} đ",
                        AutoSize = false,
                        Width = 100,
                        Height = 20,
                        Top = 140,
                        Left = 0,
                        ForeColor = Color.OrangeRed,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Cursor = Cursors.Hand,
                        Tag = sp
                    };

                    card.Controls.Add(pic);
                    card.Controls.Add(lblTen);
                    card.Controls.Add(lblGia);

                    card.Click += Card_Click;
                    pic.Click += Card_Click;
                    lblTen.Click += Card_Click;
                    lblGia.Click += Card_Click;

                    flpSanPham.Controls.Add(card);

                }
            }

            private Image LoadImage(string path)
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    try
                    {
                        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            MemoryStream ms = new MemoryStream();
                            stream.CopyTo(ms);
                            ms.Position = 0;
                            return Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi load ảnh: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Ảnh không tồn tại: " + path);
                }
                return null;
            }

            private void Card_Click(object sender, EventArgs e)
            {
                var sanPham = (SanPham)((Control)sender).Tag;
                if (sanPham.SoLuong < 1)
                {
                    MessageBox.Show($"Sản phẩm {sanPham.TenSP} đã hết hàng!", "Thông báo");
                    return;
                }

                dgvGioHang.Rows.Add(sanPham.MaSP, sanPham.TenSP, 1, sanPham.DonGia, sanPham.DonGia);
                CapNhatTongTien();
            }

            private void SetupAutoComplete()
            {
                AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
                foreach (var item in sanPhamList)
                {
                    autoComplete.Add(item.TenSP);
                    autoComplete.Add(item.MaSP);
                }

                tbTim.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbTim.AutoCompleteSource = AutoCompleteSource.CustomSource;
                tbTim.AutoCompleteCustomSource = autoComplete;
            }

            private void TbTim_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(tbTim.Text))
                {
                    string input = tbTim.Text.Trim();
                    var sp = sanPhamList.FirstOrDefault(p =>
                        p.TenSP.Equals(input, StringComparison.OrdinalIgnoreCase) ||
                        p.MaSP.Equals(input, StringComparison.OrdinalIgnoreCase));

                    if (sp != null)
                    {
                        if (sp.SoLuong < 1)
                        {
                            MessageBox.Show($"Sản phẩm {sp.TenSP} đã hết hàng!", "Thông báo");
                            return;
                        }

                        dgvGioHang.Rows.Add(sp.MaSP, sp.TenSP, 1, sp.DonGia, sp.DonGia);
                        CapNhatTongTien();
                        tbTim.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm!", "Thông báo");
                    }
                }
            }

            private void DgvGioHang_CellEndEdit(object sender, DataGridViewCellEventArgs e)
            {
                try
                {
                    if (e.RowIndex >= 0)
                    {
                        var row = dgvGioHang.Rows[e.RowIndex];
                        string maSP = row.Cells["MaSP"].Value?.ToString();
                        var sanPham = sanPhamList.FirstOrDefault(sp => sp.MaSP == maSP);

                        if (!int.TryParse(row.Cells["SoLuong"].Value?.ToString(), out int soLuong) || soLuong <= 0)
                        {
                            MessageBox.Show("Số lượng phải là số nguyên dương.");
                            row.Cells["SoLuong"].Value = 1;
                            return;
                        }

                        if (sanPham != null && soLuong > sanPham.SoLuong)
                        {
                            MessageBox.Show($"Số lượng tồn kho của {sanPham.TenSP} chỉ còn {sanPham.SoLuong}!");
                            row.Cells["SoLuong"].Value = sanPham.SoLuong;
                            soLuong = sanPham.SoLuong;
                        }

                        if (!decimal.TryParse(row.Cells["DonGia"].Value?.ToString(), out decimal donGia) || donGia <= 0)
                        {
                            MessageBox.Show("Đơn giá phải là số dương.");
                            row.Cells["DonGia"].Value = sanPham?.DonGia ?? 0;
                            return;
                        }
                        row.Cells["ThanhTien"].Value = soLuong * donGia;
                        CapNhatTongTien();
                    }
                }
                catch
                {
                    MessageBox.Show("Lỗi nhập dữ liệu.");
                }
            }

            private void CapNhatTongTien()
            {
                decimal tong = 0;
                foreach (DataGridViewRow row in dgvGioHang.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (row.Cells["ThanhTien"].Value != null &&
                        decimal.TryParse(row.Cells["ThanhTien"].Value.ToString(), out decimal tt))
                    {
                        tong += tt;
                    }
                }

                decimal thanhToanSauTinh = contextTinhTien.TinhTien(tong);

                lblTongTien.Text = $"Tạm tính: {tong:N0} VND\nCần thanh toán: {thanhToanSauTinh:N0} VND";
            }



            private void BtnThanhToan_Click(object sender, EventArgs e)
            {
                if (dgvGioHang.Rows.Count <= 1)
                {
                    MessageBox.Show("Giỏ hàng đang trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Xác nhận thanh toán?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Gọi Observer Pattern
                    var subject = new ThanhToanSubject();
                    subject.Attach(new HoaDonObserver(dgvGioHang, "admin", contextTinhTien));

                    subject.Attach(new TonKhoObserver(dgvGioHang));
                    subject.Notify();
                    // Xóa giỏ hàng
                    dgvGioHang.Rows.Clear();
                    CapNhatTongTien();
                    MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }


            private void DgvGioHang_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex >= 0 && dgvGioHang.Columns[e.ColumnIndex].Name == "Xoa")
                {
                    var row = dgvGioHang.Rows[e.RowIndex];
                    string tenSp = row.Cells["TenSP"].Value?.ToString();
                    DialogResult confirm = MessageBox.Show($"Bạn có muốn xóa sản phẩm '{tenSp}' khỏi giỏ hàng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes)
                    {
                        dgvGioHang.Rows.RemoveAt(e.RowIndex);
                        CapNhatTongTien();
                    }
                }
            }
            private void BtnHuy_Click(object sender, EventArgs e)
            {
                if (dgvGioHang.Rows.Count <= 1)
                {
                    MessageBox.Show("Giỏ hàng đang trống!", "Thông báo");
                    return;
                }
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa toàn bộ giỏ hàng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    dgvGioHang.Rows.Clear();
                    CapNhatTongTien();
                }
            }


        }
    }
}
}
