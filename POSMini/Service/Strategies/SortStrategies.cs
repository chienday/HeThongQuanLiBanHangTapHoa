using POSMini.Models;
using POSMini.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.Service.Strategies
{
    // Sắp xếp theo Tên sản phẩm A-Z
    public class SapXepAZ : ISortStrategy
    {
        public List<SanPham> Sort(List<SanPham> sp)
        {
            return sp.OrderBy(p => p.TenSP).ToList();
        }
    }


    // Sắp xếp theo Loại sản phẩm
    public class SapXepTheoLoai : ISortStrategy
    {
        public List<SanPham> Sort(List<SanPham> sp)
        {
            return sp.OrderBy(p => p.TenLoai).ThenBy(p => p.TenSP).ToList();
        }
    }

    //Sắp xếp tồn kho từ bé đến lớn và từ lớn đến bé
    public class BedenLon : ISortStrategy
    {
        public List<SanPham> Sort(List<SanPham> sp)
        {
            return sp.OrderBy(p => p.SoLuong).ToList();
        }
    }

    public class LondenBe : ISortStrategy
    {
        public List<SanPham> Sort(List<SanPham> sp)
        {
            return sp.OrderByDescending(x => x.SoLuong).ToList();
        }
    }

}