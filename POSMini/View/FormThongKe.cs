using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POSMini.Models;
using POSMini.Service.FactoryMethod;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;
using POSMini.Service.Builder;


namespace POSMini
{
    public partial class FormThongKe : Form
    {
        private ThongKe thongKeHienTai;
        private string maTKDangNhap = "TK001"; 

        public FormThongKe()
        {
            InitializeComponent();
        }

        private KieuThongKe loaiThongKe;
        private IThongKeDoanhThu thongKeChiTietHienTai;

        private void FormThongKe_Load(object sender, EventArgs e)
        {
            KhoiTaoComboBox();
            comboBox1.SelectedIndex = 0;

            // Thiết lập DateTimePicker mặc định
            if (dateTimePicker1 != null)
                dateTimePicker1.Value = DateTime.Today;
         

           
            
            XoaKetQua();
        }

        private void KhoiTaoComboBox()
        {
            
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[]
            {
                "Doanh thu theo ngày",
                "Doanh thu theo tuần",
                "Doanh thu theo tháng",
                "Doanh thu theo năm"
            });
        }

        // Button Thống kê - SỬ DỤNG FACTORY METHOD
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
               
                button1.Text = "Đang xử lý...";
                button1.Enabled = false;
                Application.DoEvents();


                DateTime ngayChon = dateTimePicker1.Value;
                DateTime tuNgay, denNgay;

                switch (loaiThongKe)
                {
                    case KieuThongKe.Ngay:
                        tuNgay = ngayChon.Date;
                        denNgay = ngayChon.Date;
                        break;

                    case KieuThongKe.Tuan:
                        tuNgay = dateTimePicker1.Value.Date;
                        denNgay = dateTimePicker2.Value.Date;
                        break;

                    case KieuThongKe.Thang:
                        tuNgay = new DateTime(ngayChon.Year, ngayChon.Month, 1);
                        denNgay = tuNgay.AddMonths(1).AddDays(-1);
                        break;

                    case KieuThongKe.Nam:
                        tuNgay = new DateTime(ngayChon.Year, 1, 1);
                        denNgay = new DateTime(ngayChon.Year, 12, 31);
                        break;

                    default:
                        throw new Exception("Loại thống kê không hợp lệ");
                }



                //  CHẶN giá trị ngày không hợp lệ SQL Server
                DateTime sqlMinDate = new DateTime(1753, 1, 1);
                if (tuNgay < sqlMinDate) tuNgay = sqlMinDate;
                if (denNgay < sqlMinDate) denNgay = sqlMinDate;
                // Kiểm tra ngày hợp lệ
                if (tuNgay > denNgay)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
               


                // SỬ DỤNG FACTORY METHOD để tạo thống kê
                ThongKeDoanhThuFactory factory = LayFactory(comboBox1.SelectedIndex);
                thongKeHienTai = factory.ThucHienThongKe(tuNgay, denNgay, maTKDangNhap);
                // Lưu lại thể hiện IThongKeDoanhThu để gọi LayChiTietHoaDon
                thongKeChiTietHienTai = factory.TaoThongKe();

                // Lấy bảng chi tiết hóa đơn và gán vào DataGridView
                DataTable dtChiTiet = thongKeChiTietHienTai.LayChiTietHoaDon(
                    thongKeHienTai.TuNgay, thongKeHienTai.DenNgay); // Dùng khoảng đã tính

                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = dtChiTiet;
                VeBieuDo(dtChiTiet);


                
                HienThiKetQua();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thực hiện thống kê: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                XoaKetQua();
            }
            finally
            {
               
                button1.Text = "Thống Kê";
                button1.Enabled = true;
            }
        }

        // Chọn Factory tương ứng với loại thống kê
        private ThongKeDoanhThuFactory LayFactory(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0: // Theo ngày
                    return new ThongKeNgayFactory();
                case 1: // Theo tuần
                    return new ThongKeTuanFactory();
                case 2: // Theo tháng
                    return new ThongKeThangFactory();
                case 3: // Theo năm
                    return new ThongKeNamFactory();
                default:
                    throw new ArgumentException("Loại thống kê không hợp lệ!");
            }
        }

        // Hiển thị kết quả thống kê vào các TextBox
        private void HienThiKetQua()
        {
            if (thongKeHienTai == null)
            {
                XoaKetQua();
                return;
            }
            
            textBox1.Text = $"{thongKeHienTai.TongDoanhThu:N0} VNĐ";
           

           
            textBox2.Text = $"{thongKeHienTai.TongVonHangBan:N0} VNĐ";
            

           
            decimal loiNhuan = thongKeHienTai.TongDoanhThu - thongKeHienTai.TongVonHangBan;
         
            textBox3.Text = $"{loiNhuan:N0} VNĐ";
            

           
        }

        private void XoaKetQua()
        {
            textBox1.Text = "0 VNĐ";
         

            textBox2.Text = "0 VNĐ";
           

            textBox3.Text = "0 VNĐ";
            

            this.Text = "FormThongKe";
            thongKeHienTai = null;
            dataGridView1.DataSource = null;
            chart1.Series.Clear();

        }

        // Tổng doanh thu
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        // Tổng vốn bán hàng
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        // Lợi nhuận
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }

        // ComboBox chọn kiểu doanh thu
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                // Xóa kết quả cũ khi thay đổi loại báo cáo
                XoaKetQua();

                // Cập nhật loại thống kê
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        loaiThongKe = KieuThongKe.Ngay;
                        break;
                    case 1:
                        loaiThongKe = KieuThongKe.Tuan;
                        DateTime today = DateTime.Now;
                        DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

                        // Lùi về đầu tuần (thứ Hai là đầu tuần)
                        int daysToMonday = ((int)firstDayOfMonth.DayOfWeek == 0) ? -6 : -(int)firstDayOfMonth.DayOfWeek + 1;
                        DateTime monday = firstDayOfMonth.AddDays(daysToMonday);
                        DateTime sunday = monday.AddDays(6);

                        // Gán vào 2 DateTimePicker
                        dateTimePicker1.Value = monday;
                        dateTimePicker2.Value = sunday;
                        break;
                    case 2:
                        loaiThongKe = KieuThongKe.Thang;
                        DateTime now = DateTime.Now;
                        DateTime firstDay = new DateTime(now.Year, now.Month, 1);
                        DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
                        dateTimePicker1.Value = firstDay;
                        dateTimePicker2.Value = lastDay;
                        break;
                    case 3:
                        loaiThongKe = KieuThongKe.Nam;
                        DateTime namChon = dateTimePicker1.Value;
                        dateTimePicker1.Value = new DateTime(namChon.Year, 1, 1);
                        dateTimePicker2.Value = new DateTime(namChon.Year, 12, 31);
                        break;
                }
                CapNhatHienThiDateTimePicker();
            }
        }
        private void CapNhatHienThiDateTimePicker()
        {
            bool hien1Picker = comboBox1.SelectedIndex == 0;

            dateTimePicker1.Visible = true;
            dateTimePicker2.Visible = !hien1Picker;

            // Nếu có label mô tả cho 2 DateTimePicker thì xử lý luôn:
            if (label1 != null) label1.Visible = true;
            if (label2 != null) label2.Visible = !hien1Picker;
        }


        // Ngày bắt đầu 
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Xóa kết quả cũ khi thay đổi ngày
            if (thongKeHienTai != null)
            {
                XoaKetQua();
            }
        }

        // Ngày kết thúc
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            // Xóa kết quả cũ khi thay đổi ngày
            if (thongKeHienTai != null)
            {
                XoaKetQua();
            }
        }

        private void VeBieuDo(DataTable dt)
        {
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.Title = "Ngày";
            chart1.ChartAreas[0].AxisY.Title = "Doanh thu";

            Series series = new Series("Doanh thu");
            series.ChartType = SeriesChartType.Column;
            series.XValueType = ChartValueType.String;

            // Nhóm theo ngày (hoặc thời gian tương ứng)
            var doanhThuTheoNgay = dt.AsEnumerable()
                .GroupBy(row => Convert.ToDateTime(row["NgayLap"]).ToString("dd/MM/yyyy"))
                .Select(g => new
                {
                    Ngay = g.Key,
                    TongTien = g.Sum(r => Convert.ToDecimal(r["ThanhTien"]))
                });

            foreach (var item in doanhThuTheoNgay)
            {
                series.Points.AddXY(item.Ngay, item.TongTien);
            }

            chart1.Series.Add(series);
        }


        private void chart1_Click(object sender, EventArgs e)
        {

        }

    
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //Xuất file

        private void button2_Click(object sender, EventArgs e)
        {
            if (thongKeHienTai == null || thongKeChiTietHienTai == null)
            {
                MessageBox.Show("Vui lòng thống kê trước khi xuất file.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog()
            {
                Filter = "Excel File (*.xlsx)|*.xlsx|Word File (*.docx)|*.docx",
                FileName = "BaoCaoThongKe"
            })
            {
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = saveDialog.FileName;
                        string extension = System.IO.Path.GetExtension(filePath).ToLower();

                        // Lấy chi tiết dữ liệu
                        DataTable chiTiet = thongKeChiTietHienTai.LayChiTietHoaDon(
                            thongKeHienTai.TuNgay, thongKeHienTai.DenNgay);

                        IThongKeExporterBuilder builder;

                        if (extension == ".xlsx")
                        {
                            builder = new POSMini.Service.Builder.ExcelExporterBuilder();
                        }
                        else if (extension == ".docx")
                        {
                            builder = new POSMini.Service.Builder.WordExporterBuilder();
                        }
                        else
                        {
                            MessageBox.Show("Định dạng không được hỗ trợ.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Gọi director xuất
                        var director = new POSMini.Service.Builder.ThongKeExporterDirector(builder);
                        director.Export(filePath, "BÁO CÁO THỐNG KÊ DOANH THU", thongKeHienTai, chiTiet);

                        MessageBox.Show("Xuất file thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }


        }
    }
}