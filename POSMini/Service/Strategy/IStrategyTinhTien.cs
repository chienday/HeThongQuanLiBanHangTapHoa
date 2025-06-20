using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSMini.Service.Strategy
{
    public interface IStrategyTinhTien
    {
        decimal TinhTongTien(decimal tongTienGoc);
    }
}