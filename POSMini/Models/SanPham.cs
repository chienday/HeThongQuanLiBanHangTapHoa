using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// File: Models/SanPham.cs
namespace POSMini.Models
{
    public class SanPham
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public string TenLoai { get; set; }
        public string HinhAnh { get; set; }
    }
}