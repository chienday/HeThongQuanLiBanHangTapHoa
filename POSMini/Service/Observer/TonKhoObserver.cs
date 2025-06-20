using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using POSMini.DataAccess;
using POSMini.Service.Observer;
using POSMini.Service.Singleton;

namespace SPOSMini.Service.Observer
{
    public class TonKhoObserver : IObserver
    {
        private readonly DataGridView _dgvGioHang;

        public TonKhoObserver(DataGridView dgvGioHang)
        {
            _dgvGioHang = dgvGioHang;
        }

        public void Update()
        {
            var conn = DatabaseConnection.Instance.GetConnection();

            foreach (DataGridViewRow row in _dgvGioHang.Rows)
            {
                if (row.IsNewRow) continue;

                string maSP = row.Cells["MaSP"].Value?.ToString();
                int soLuongBan = Convert.ToInt32(row.Cells["SoLuong"].Value);

                var cmd = new SqlCommand("UPDATE SanPham SET SoLuong = SoLuong - @sl WHERE MaSP = @maSP", conn);
                cmd.Parameters.AddWithValue("@sl", soLuongBan);
                cmd.Parameters.AddWithValue("@maSP", maSP);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("🗃️ Tồn kho đã được cập nhật sau thanh toán.", "Thông báo");
        }

    }
}