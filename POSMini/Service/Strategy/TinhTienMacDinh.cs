using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.Service.Strategy
{
    public class TinhTienMacDinh : IStrategyTinhTien
    {
        public decimal TinhTongTien(decimal tongTienGoc) => tongTienGoc;
    }

}