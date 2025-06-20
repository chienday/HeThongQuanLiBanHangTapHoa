using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.model
{
    public class HoaDon
    {
        public string MaHD { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public decimal ThanhToan { get; set; }
        public string TrangThai { get; set; }
        public string NguoiTao { get; set; }
    }

}