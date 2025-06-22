using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.Models
{
    public class TaiKhoan
    {
        public string MaTK { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public string VaiTro { get; set; } = "QuanLy";
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;
    }

    /*public class LoaiSanPham
    {
        public string MaLoai { get; set; }
        public string TenLoai { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
    */
    public class NhaCungCap
    {
        public string MaNCC { get; set; }
        public string TenNCC { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }

    /*public class SanPham
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string MaLoai { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; } = 0;
        public string HinhAnh { get; set; }
        public int SoLuongToiThieu { get; set; } = 5;
        public string DonViTinh { get; set; } = "Cái";
        public string MoTa { get; set; }
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;
    }*/

    public class PhieuNhapHang
    {
        public string MaPhieuNhap { get; set; }
        public string MaNCC { get; set; }
        public string MaTK { get; set; }
        public DateTime NgayNhap { get; set; } = DateTime.Now;
        public decimal TongTien { get; set; } = 0;
        public string GhiChu { get; set; }
        public string TrangThai { get; set; } = "Đã nhập";
    }

    public class ChiTietPhieuNhap
    {
        public string MaPhieuNhap { get; set; }
        public string MaSP { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal ThanhTien => SoLuongNhap * GiaNhap;
    }

    public class HoaDon
    {
        public string MaHD { get; set; }
        public string MaTK { get; set; }
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public decimal TongTien { get; set; } = 0;
        public decimal? ThanhToan { get; set; }
        public string TrangThai { get; set; } = "Đã thanh toán";
    }

    public class ChiTietHoaDon
    {
        public string MaHD { get; set; }
        public string MaSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class TonKho
    {
        public int MaTonKho { get; set; }
        public string MaSP { get; set; }
        public string LoaiGiaoDich { get; set; }
        public int SoLuongTruoc { get; set; }
        public int SoLuongThayDoi { get; set; }
        public int SoLuongSau { get; set; }
        public DateTime NgayGiaoDich { get; set; } = DateTime.Now;
        public string MaPhieu { get; set; }
        public string GhiChu { get; set; }
        public string MaTK { get; set; }
    }


    // Enum định nghĩa các kiểu thống kê
    public enum KieuThongKe
    {
        Ngay = 1,
        Tuan = 2,
        Thang = 3,
        Nam = 4
    }
    public class ThongKe
    {
        public int MaThongKe { get; set; }
        public string LoaiThongKe { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public decimal TongDoanhThu { get; set; } = 0;
        public decimal TongVonHangBan { get; set; } = 0;
        public decimal LoiNhuan => TongDoanhThu - TongVonHangBan;
        public int SoLuongGiaoDich { get; set; } = 0;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public string MaTK { get; set; }
        public string GhiChu { get; set; }

        // Chi tiết hóa đơn
        public DataTable ChiTietHoaDon { get; set; }
    }


}
    
    

   

