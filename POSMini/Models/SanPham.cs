using System;
namespace POSMini.Models
{
    public class SanPham
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string MaLoai { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public string HinhAnh { get; set; }
        public int SoLuongToiThieu { get; set; }
        public string DonViTinh { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        // Các trường NgayTao, NgayCapNhat để CSDL tự xử lý
    }
}