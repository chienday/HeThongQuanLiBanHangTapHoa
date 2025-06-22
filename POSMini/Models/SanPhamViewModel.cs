using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.Models
{
    public class SanPhamViewModel
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string TenLoai { get; set; }
        public decimal GiaBan { get; set; }
        public decimal GiaNhap { get; set; }
        public int SoLuongTon { get; set; }
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
    }
}