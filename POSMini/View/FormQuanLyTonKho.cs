using POSMini.Models;
using POSMini.Service;
using POSMini.Service.Singleton;
using POSMini.Service.Strategies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace POSMini
{
    public partial class FormQuanLyTonKho : Form
    {

        private List<SanPhamViewModel> _allProductsDataSource;
        public FormQuanLyTonKho()
        {
            InitializeComponent();
        }

        private void FormQuanLyTonKho_Load(object sender, EventArgs e)
        {
            SetupSortComboBox();
            SetupChartModeComboBox();
            LoadData();
        }
        private void SetupGridView()
        {
            dgvTonKho.Columns["MaSP"].HeaderText = "Mã SP";
            dgvTonKho.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            dgvTonKho.Columns["TenLoai"].HeaderText = "Loại";
            dgvTonKho.Columns["SoLuong"].HeaderText = "Số Lượng Tồn";
            dgvTonKho.Columns["HinhAnh"].Visible = false;

            dgvTonKho.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void SetupSortComboBox()
        {
            cmbSapXep.Items.Clear();
            cmbSapXep.Items.Add("Theo loại sản phẩm");
            cmbSapXep.Items.Add("Theo tên A-Z");
            cmbSapXep.Items.Add("Tồn kho từ lớn đến bé");
            cmbSapXep.Items.Add("Tồn kho từ bé đến lớn");
            cmbSapXep.SelectedIndex = 0;
        }


        private void SetupChartModeComboBox()
        {
            cmbChartMode.Items.Clear();
            cmbChartMode.Items.Add("Theo nhóm loại sản phẩm");
            cmbChartMode.Items.Add("Theo từng sản phẩm");
            cmbChartMode.SelectedIndex = 0;
        }

        private void LoadData()
        {
            _allProductsDataSource = SanPhamService.Instance.GetAllSP();
            dgvTonKho.DataSource = _allProductsDataSource;
            SetupGridView();
            UpdateDisplay();
        }


        private void UpdateDisplay()
        {
            if (_allProductsDataSource == null) return;
            var listToDisplay = string.IsNullOrWhiteSpace(txtTimKiem.Text)
                ? new List<SanPhamViewModel>(_allProductsDataSource)
                : _allProductsDataSource.Where(p =>
                    p.TenSP.ToLower().Contains(txtTimKiem.Text.Trim().ToLower()) ||
                    p.MaSP.ToLower().Contains(txtTimKiem.Text.Trim().ToLower()))
                  .ToList();

            ISortStrategy strategy;
            // Sử dụng đúng tên lớp Strategy đã được chuẩn hóa
            switch (cmbSapXep.SelectedItem.ToString())
            {
                case "Theo tên A-Z": strategy = new SapXepAZ(); break;
                case "Tồn kho từ lớn đến bé": strategy = new SapxepGiamDan(); break;
                case "Tồn kho từ bé đến lớn": strategy = new SapxepTangdan(); break;
                default: strategy = new SapXepLoai(); break;
            }

            var sortedList = strategy.Sort(listToDisplay);
            dgvTonKho.DataSource = sortedList;

            // Vẽ lại biểu đồ dựa trên danh sách đã được lọc và sắp xếp
            DrawInventoryPieChart(sortedList);
        }

        private void DrawInventoryPieChart(List<SanPhamViewModel> sanPhamList)
        {
            chartTonKho.Series.Clear();
            chartTonKho.Titles.Clear();
            chartTonKho.Legends.Clear();

            var series = new Series("Tồn kho")
            {
                ChartType = SeriesChartType.Pie
            };

            string chartMode = cmbChartMode.SelectedItem.ToString();
            chartTonKho.Titles.Add($"Tỷ Lệ Tồn Kho {chartMode}");

            if (chartMode == "Theo nhóm loại sản phẩm")
            {
                var groupedData = sanPhamList
                    .GroupBy(p => p.TenLoai)
                    .Select(group => new
                    {
                        GroupName = string.IsNullOrEmpty(group.Key) ? "Không xác định" : group.Key,
                        TotalQuantity = group.Sum(p => p.SoLuongTon)
                    });

                foreach (var group in groupedData)
                {
                    if (group.TotalQuantity > 0)
                    {
                        series.Points.AddXY(group.GroupName, group.TotalQuantity);
                    }
                }
            }
            else // Theo từng sản phẩm
            {
                foreach (var product in sanPhamList)
                {
                    if (product.SoLuongTon > 0)
                    {
                        series.Points.AddXY(product.TenSP, product.SoLuongTon);
                    }
                }
            }

            series.IsValueShownAsLabel = true;
            series.Label = "#PERCENT{P0}";
            series.LegendText = "#VALX";

            chartTonKho.Series.Add(series);
            chartTonKho.Legends.Add("DefaultLegend");
            chartTonKho.Legends["DefaultLegend"].Docking = Docking.Bottom;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            LoadData();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void cmbSapXep_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void cmbChartMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
    }
}
