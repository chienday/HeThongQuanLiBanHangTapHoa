using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using POSMini.DataAccess;
using POSMini.model;
using POSMini.Service.Observer;
using POSMini.Services.Strategy;
using POSMini.Service.Singleton;

namespace SPOSMini.Service.Observer
{
    public class HoaDonObserver : IObserver
    {
        private readonly DataGridView _dgvGioHang;
        private readonly string _nguoiTao;
        decimal tongTien = 0;
        private readonly HoaDonContext _contextTinhTien;

        public HoaDonObserver(DataGridView dgvGioHang, string nguoiTao, HoaDonContext context)
        {
            _dgvGioHang = dgvGioHang;
            _nguoiTao = nguoiTao;
            _contextTinhTien = context;
        }


        public void Update()
        {
            var conn = DatabaseConnection.Instance.GetConnection();

            string maHD = $"HD{DateTime.Now:yyyyMMddHHmmss}";
            DateTime now = DateTime.Now;
            decimal tongTienTruocChienLuoc = 0;

            var sanPhamGop = new Dictionary<string, ChiTietHoaDon>();

            foreach (DataGridViewRow row in _dgvGioHang.Rows)
            {
                if (row.IsNewRow) continue;

                string maSP = row.Cells["MaSP"].Value?.ToString();
                string tenSP = row.Cells["TenSP"].Value?.ToString();
                int soLuong = Convert.ToInt32(row.Cells["SoLuong"].Value);
                decimal donGia = Convert.ToDecimal(row.Cells["DonGia"].Value);

                tongTienTruocChienLuoc += soLuong * donGia;

                if (sanPhamGop.ContainsKey(maSP))
                {
                    sanPhamGop[maSP].SoLuong += soLuong;
                }
                else
                {
                    sanPhamGop[maSP] = new ChiTietHoaDon
                    {
                        MaHD = maHD,
                        MaSP = maSP,
                        TenSP = tenSP,
                        SoLuong = soLuong,
                        DonGia = donGia
                    };
                }
            }


            decimal tongTienSauChienLuoc = _contextTinhTien.TinhTien(tongTienTruocChienLuoc);
            var hoaDonCmd = new SqlCommand(@"
    INSERT INTO HoaDon (MaHD, NgayLap, TongTien, TrangThai, NguoiTao)
    VALUES (@maHD, @ngay, @tong, @trangThai, @nguoiTao)", conn);

            hoaDonCmd.Parameters.AddWithValue("@maHD", maHD);
            hoaDonCmd.Parameters.AddWithValue("@ngay", now);
            hoaDonCmd.Parameters.AddWithValue("@tong", tongTienSauChienLuoc);
            hoaDonCmd.Parameters.AddWithValue("@trangThai", "Đã thanh toán");
            hoaDonCmd.Parameters.AddWithValue("@nguoiTao", _nguoiTao);
            hoaDonCmd.ExecuteNonQuery();
            foreach (var ct in sanPhamGop.Values)
            {
                var ctCmd = new SqlCommand(@"
            INSERT INTO ChiTietHoaDon (MaHD, MaSP, TenSP, SoLuong, DonGia)
            VALUES (@maHD, @maSP, @tenSP, @sl, @gia)", conn);
                ctCmd.Parameters.AddWithValue("@maHD", ct.MaHD);
                ctCmd.Parameters.AddWithValue("@maSP", ct.MaSP);
                ctCmd.Parameters.AddWithValue("@tenSP", ct.TenSP);
                ctCmd.Parameters.AddWithValue("@sl", ct.SoLuong);
                ctCmd.Parameters.AddWithValue("@gia", ct.DonGia);
                ctCmd.ExecuteNonQuery();
            }

            MessageBox.Show("🧾 Đã lưu hóa đơn vào hệ thống.", "Thông báo");
        }
    }
}