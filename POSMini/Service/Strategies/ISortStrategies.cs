using POSMini.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.Service.Strategies
{
    public interface ISortStrategy
    {
        List<SanPhamViewModel> Sort(List<SanPhamViewModel> viewModels);
    }
}
