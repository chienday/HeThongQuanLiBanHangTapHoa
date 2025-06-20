using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.Service.Strategy
{
    public class TinhTienVAT : IStrategyTinhTien
    {
        public decimal TinhTongTien(decimal tongTienGoc)
        {
            return tongTienGoc * 1.08m;
        }
    }
}