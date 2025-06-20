using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.Service.Strategy
{
    public class ChietKhau10 : IStrategyTinhTien
    {
        public decimal TinhTongTien(decimal tongTienGoc)
        {
            return tongTienGoc * 0.9m;
        }
    }

}