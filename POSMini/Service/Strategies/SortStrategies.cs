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
        public List<SanPhamViewModel> Sort(List<SanPhamViewModel> viewModels)
        {
            return viewModels.OrderBy(p => p.TenSP).ToList();
        }
    }


    // Sắp xếp theo Loại sản phẩm
    public class SapXepLoai : ISortStrategy
    {
        public List<SanPhamViewModel> Sort(List<SanPhamViewModel> viewModels)
        {
            return viewModels.OrderBy(p => p.TenLoai).ThenBy(p => p.TenSP).ToList();
        }
    }

    //Sắp xếp tồn kho từ bé đến lớn và từ lớn đến bé
    public class SapxepTangdan : ISortStrategy
    {
        public List<SanPhamViewModel> Sort(List<SanPhamViewModel> viewModels)
        {
            return viewModels.OrderBy(p => p.SoLuongTon).ToList();
        }
    }

    public class SapxepGiamDan : ISortStrategy
    {
        public List<SanPhamViewModel> Sort(List<SanPhamViewModel> viewModels)
        {
            return viewModels.OrderByDescending(p => p.SoLuongTon).ToList();
        }
    }

}