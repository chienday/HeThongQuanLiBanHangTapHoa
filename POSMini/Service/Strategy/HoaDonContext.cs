using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.Service.Strategy
{
    public class HoaDonContext
    {
        private IStrategyTinhTien _tinhTienStrategy;

        public HoaDonContext(IStrategyTinhTien strategy)
        {
            _tinhTienStrategy = strategy;
        }

        public void SetStrategy(IStrategyTinhTien strategy)
        {
            _tinhTienStrategy = strategy;
        }

        public decimal TinhTien(decimal tongTienGoc)
        {
            return _tinhTienStrategy.TinhTongTien(tongTienGoc);
        }
    }

}