using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POSMini.Models;
using POSMini.Service.Singleton;

namespace POSMini.Service.FactoryMethod
{
    // Factory
    public abstract class ThongKeDoanhThuFactory
    {
        public abstract IThongKeDoanhThu TaoThongKe();

        // Template method để thực hiện thống kê
        public ThongKe ThucHienThongKe(DateTime tuNgay, DateTime denNgay, string maTK)
        {
            var thongKe = TaoThongKe();
            return thongKe.TinhThongKe(tuNgay, denNgay, maTK);
        }
    }

    
    public interface IThongKeDoanhThu
    {
        ThongKe TinhThongKe(DateTime tuNgay, DateTime denNgay, string maTK);
        string LayLoaiThongKe();
        DataTable LayChiTietHoaDon(DateTime tuNgay, DateTime denNgay);
    }

    
    public class ThongKeNgayFactory : ThongKeDoanhThuFactory
    {
        public override IThongKeDoanhThu TaoThongKe()
        {
            return new ThongKeTheoNgay();
        }
    }

    
    public class ThongKeTuanFactory : ThongKeDoanhThuFactory
    {
        public override IThongKeDoanhThu TaoThongKe()
        {
            return new ThongKeTheoTuan();
        }
    }

    
    public class ThongKeThangFactory : ThongKeDoanhThuFactory
    {
        public override IThongKeDoanhThu TaoThongKe()
        {
            return new ThongKeTheoThang();
        }
    }

    
    public class ThongKeNamFactory : ThongKeDoanhThuFactory
    {
        public override IThongKeDoanhThu TaoThongKe()
        {
            return new ThongKeTheoNam();
        }
    }

    // Concrete Products
    public class ThongKeTheoNgay : IThongKeDoanhThu
    {
        public ThongKe TinhThongKe(DateTime tuNgay, DateTime denNgay, string maTK)
        {
            var thongKe = new ThongKe
            {
                LoaiThongKe = LayLoaiThongKe(),
                TuNgay = tuNgay.Date,
                DenNgay = denNgay.Date,
                MaTK = maTK,
                GhiChu = $"Thống kê doanh thu ngày {tuNgay:dd/MM/yyyy}"
            };

            TinhDoanhThu(thongKe);
            return thongKe;
        }

        public string LayLoaiThongKe() => "Doanh thu theo ngày";

        private void TinhDoanhThu(ThongKe thongKe)
        {
            string query = @"
                SELECT 
                    ISNULL(SUM(ct.ThanhTien), 0) as TongDoanhThu,
                    ISNULL(SUM(ct.SoLuong * sp.GiaNhap), 0) as TongVonHangBan,
                    COUNT(DISTINCT hd.MaHD) as SoLuongGiaoDich
                FROM HoaDon hd
                INNER JOIN ChiTietHoaDon ct ON hd.MaHD = ct.MaHD
                INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                WHERE hd.TrangThai = N'Đã thanh toán'
                AND CAST(hd.NgayLap AS DATE) BETWEEN @TuNgay AND @DenNgay";

            using (var connection = DatabaseConnection.Instance.GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TuNgay", thongKe.TuNgay);
                command.Parameters.AddWithValue("@DenNgay", thongKe.DenNgay);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        thongKe.TongDoanhThu = reader.GetDecimal(reader.GetOrdinal("TongDoanhThu"));
                        thongKe.TongVonHangBan = reader.GetDecimal(reader.GetOrdinal("TongVonHangBan"));
                        thongKe.SoLuongGiaoDich = reader.GetInt32(reader.GetOrdinal("SoLuongGiaoDich"));
                    }
                }
            }
        }

        public DataTable LayChiTietHoaDon(DateTime tuNgay, DateTime denNgay)
        {
            string query = @"
                SELECT hd.MaHD, hd.NgayLap, sp.TenSP, ct.SoLuong, ct.ThanhTien
                FROM HoaDon hd
                INNER JOIN ChiTietHoaDon ct ON hd.MaHD = ct.MaHD
                INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                WHERE hd.TrangThai = N'Đã thanh toán'
                AND CAST(hd.NgayLap AS DATE) BETWEEN @TuNgay AND @DenNgay
                ORDER BY hd.NgayLap DESC";

            var dt = new DataTable();
            using (var connection = DatabaseConnection.Instance.GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TuNgay", tuNgay);
                command.Parameters.AddWithValue("@DenNgay", denNgay);
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
    public class ThongKeTheoTuan : IThongKeDoanhThu
    {
        public ThongKe TinhThongKe(DateTime tuNgay, DateTime denNgay, string maTK)
        {
            var startOfWeek = tuNgay.AddDays(-(int)tuNgay.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            var thongKe = new ThongKe
            {
                LoaiThongKe = LayLoaiThongKe(),
                TuNgay = startOfWeek.Date,
                DenNgay = endOfWeek.Date,
                MaTK = maTK,
                GhiChu = $"Thống kê doanh thu tuần từ {startOfWeek:dd/MM/yyyy} đến {endOfWeek:dd/MM/yyyy}"
            };

            TinhDoanhThu(thongKe);
            return thongKe;
        }

        public string LayLoaiThongKe() => "Doanh thu theo tuần";

        private void TinhDoanhThu(ThongKe thongKe)
        {
            string query = @"
                SELECT 
                    ISNULL(SUM(ct.ThanhTien), 0) as TongDoanhThu,
                    ISNULL(SUM(ct.SoLuong * sp.GiaNhap), 0) as TongVonHangBan,
                    COUNT(DISTINCT hd.MaHD) as SoLuongGiaoDich
                FROM HoaDon hd
                INNER JOIN ChiTietHoaDon ct ON hd.MaHD = ct.MaHD
                INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                WHERE hd.TrangThai = N'Đã thanh toán'
                AND CAST(hd.NgayLap AS DATE) BETWEEN @TuNgay AND @DenNgay";

            using (var connection = DatabaseConnection.Instance.GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TuNgay", thongKe.TuNgay);
                command.Parameters.AddWithValue("@DenNgay", thongKe.DenNgay);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        thongKe.TongDoanhThu = reader.GetDecimal(reader.GetOrdinal("TongDoanhThu"));
                        thongKe.TongVonHangBan = reader.GetDecimal(reader.GetOrdinal("TongVonHangBan"));
                        thongKe.SoLuongGiaoDich = reader.GetInt32(reader.GetOrdinal("SoLuongGiaoDich"));
                    }
                }
            }
        }

        public DataTable LayChiTietHoaDon(DateTime tuNgay, DateTime denNgay)
        {
            var startOfWeek = tuNgay.AddDays(-(int)tuNgay.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            string query = @"
                SELECT hd.MaHD, hd.NgayLap, sp.TenSP, ct.SoLuong, ct.ThanhTien
                FROM HoaDon hd
                INNER JOIN ChiTietHoaDon ct ON hd.MaHD = ct.MaHD
                INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                WHERE hd.TrangThai = N'Đã thanh toán'
                AND CAST(hd.NgayLap AS DATE) BETWEEN @TuNgay AND @DenNgay
                ORDER BY hd.NgayLap DESC";

            var dt = new DataTable();
            using (var connection = DatabaseConnection.Instance.GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TuNgay", startOfWeek);
                command.Parameters.AddWithValue("@DenNgay", endOfWeek);
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }

    public class ThongKeTheoThang : IThongKeDoanhThu
    {
        private DateTime startOfMonth;
        private DateTime endOfMonth;

        public ThongKe TinhThongKe(DateTime tuNgay, DateTime denNgay, string maTK)
        {
            startOfMonth = new DateTime(tuNgay.Year, tuNgay.Month, 1);
            endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var thongKe = new ThongKe
            {
                LoaiThongKe = LayLoaiThongKe(),
                TuNgay = startOfMonth.Date,
                DenNgay = endOfMonth.Date,
                MaTK = maTK,
                GhiChu = $"Thống kê doanh thu tháng {tuNgay.Month}/{tuNgay.Year}"
            };

            TinhDoanhThu(thongKe);
            return thongKe;
        }

        public string LayLoaiThongKe() => "Doanh thu theo tháng";

        private void TinhDoanhThu(ThongKe thongKe)
        {
            string query = @"
            SELECT 
                ISNULL(SUM(ct.ThanhTien), 0) as TongDoanhThu,
                ISNULL(SUM(ct.SoLuong * sp.GiaNhap), 0) as TongVonHangBan,
                COUNT(DISTINCT hd.MaHD) as SoLuongGiaoDich
            FROM HoaDon hd
            INNER JOIN ChiTietHoaDon ct ON hd.MaHD = ct.MaHD
            INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
            WHERE hd.TrangThai = N'Đã thanh toán'
            AND YEAR(hd.NgayLap) = @Nam AND MONTH(hd.NgayLap) = @Thang";

            using (var connection = DatabaseConnection.Instance.GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Nam", thongKe.TuNgay.Year);
                command.Parameters.AddWithValue("@Thang", thongKe.TuNgay.Month);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        thongKe.TongDoanhThu = reader.GetDecimal(reader.GetOrdinal("TongDoanhThu"));
                        thongKe.TongVonHangBan = reader.GetDecimal(reader.GetOrdinal("TongVonHangBan"));
                        thongKe.SoLuongGiaoDich = reader.GetInt32(reader.GetOrdinal("SoLuongGiaoDich"));
                    }
                }
            }
        }

        public DataTable LayChiTietHoaDon(DateTime tuNgay, DateTime denNgay)
        {
            var dt = new DataTable();
            string query = @"
            SELECT hd.MaHD, hd.NgayLap, sp.TenSP, ct.SoLuong, ct.ThanhTien
            FROM HoaDon hd
            INNER JOIN ChiTietHoaDon ct ON hd.MaHD = ct.MaHD
            INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
            WHERE hd.TrangThai = N'Đã thanh toán'
            AND CAST(hd.NgayLap AS DATE) BETWEEN @TuNgay AND @DenNgay
            ORDER BY hd.NgayLap DESC";

            using (var connection = DatabaseConnection.Instance.GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                

                command.Parameters.AddWithValue("@TuNgay", tuNgay);
                command.Parameters.AddWithValue("@DenNgay", denNgay);
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
    public class ThongKeTheoNam : IThongKeDoanhThu
    {
        private DateTime startOfYear;
        private DateTime endOfYear;

        public ThongKe TinhThongKe(DateTime tuNgay, DateTime denNgay, string maTK)
        {
            startOfYear = new DateTime(tuNgay.Year, 1, 1);
            endOfYear = new DateTime(tuNgay.Year, 12, 31);

            var thongKe = new ThongKe
            {
                LoaiThongKe = LayLoaiThongKe(),
                TuNgay = startOfYear.Date,
                DenNgay = endOfYear.Date,
                MaTK = maTK,
                GhiChu = $"Thống kê doanh thu năm {tuNgay.Year}"
            };

            TinhDoanhThu(thongKe);
            return thongKe;
        }

        public string LayLoaiThongKe() => "Doanh thu theo năm";

        private void TinhDoanhThu(ThongKe thongKe)
        {
            string query = @"
            SELECT 
                ISNULL(SUM(ct.ThanhTien), 0) as TongDoanhThu,
                ISNULL(SUM(ct.SoLuong * sp.GiaNhap), 0) as TongVonHangBan,
                COUNT(DISTINCT hd.MaHD) as SoLuongGiaoDich
            FROM HoaDon hd
            INNER JOIN ChiTietHoaDon ct ON hd.MaHD = ct.MaHD
            INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
            WHERE hd.TrangThai = N'Đã thanh toán'
            AND YEAR(hd.NgayLap) = @Nam";

            using (var connection = DatabaseConnection.Instance.GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Nam", thongKe.TuNgay.Year);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        thongKe.TongDoanhThu = reader.GetDecimal(reader.GetOrdinal("TongDoanhThu"));
                        thongKe.TongVonHangBan = reader.GetDecimal(reader.GetOrdinal("TongVonHangBan"));
                        thongKe.SoLuongGiaoDich = reader.GetInt32(reader.GetOrdinal("SoLuongGiaoDich"));
                    }
                }
            }
        }

        public DataTable LayChiTietHoaDon(DateTime tuNgay, DateTime denNgay)
        {
            var dt = new DataTable();
            string query = @"
            SELECT hd.MaHD, hd.NgayLap, sp.TenSP, ct.SoLuong, ct.ThanhTien
            FROM HoaDon hd
            INNER JOIN ChiTietHoaDon ct ON hd.MaHD = ct.MaHD
            INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
            WHERE hd.TrangThai = N'Đã thanh toán'
            AND CAST(hd.NgayLap AS DATE) BETWEEN @TuNgay AND @DenNgay
            ORDER BY hd.NgayLap DESC";

            using (var connection = DatabaseConnection.Instance.GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TuNgay", tuNgay);
                command.Parameters.AddWithValue("@DenNgay",denNgay);
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
}
