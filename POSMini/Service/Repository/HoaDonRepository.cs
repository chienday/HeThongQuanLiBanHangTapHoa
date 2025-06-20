using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using POSMini.DataAccess;
using POSMini.Models;

namespace POSMini.Service.Repository
{
    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly SqlConnection _conn;

        public HoaDonRepository()
        {
            _conn = DatabaseConnection.Instance.GetConnection();
        }

        public List<HoaDon> GetAll()
        {
            List<HoaDon> list = new List<HoaDon>();

            string query = @"SELECT MaHD, NgayLap, TongTien, TrangThai, NguoiTao
                             FROM HoaDon
                             ORDER BY NgayLap DESC";

            using (SqlCommand cmd = new SqlCommand(query, _conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new HoaDon
                    {
                        MaHD = reader["MaHD"].ToString(),
                        NgayLap = Convert.ToDateTime(reader["NgayLap"]),
                        TongTien = Convert.ToDecimal(reader["TongTien"]),
                        TrangThai = reader["TrangThai"].ToString(),
                        NguoiTao = reader["NguoiTao"]?.ToString() ?? "admin"
                    });
                }
            }

            return list;
        }

        public List<ChiTietHoaDon> GetChiTiet(string maHD)
        {
            List<ChiTietHoaDon> list = new List<ChiTietHoaDon>();

            string query = @"SELECT MaSP, TenSP, SoLuong, DonGia
                             FROM ChiTietHoaDon
                             WHERE MaHD = @maHD";

            using (SqlCommand cmd = new SqlCommand(query, _conn))
            {
                cmd.Parameters.AddWithValue("@maHD", maHD);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ChiTietHoaDon
                        {
                            MaSP = reader["MaSP"].ToString(),
                            TenSP = reader["TenSP"].ToString(),
                            SoLuong = Convert.ToInt32(reader["SoLuong"]),
                            DonGia = Convert.ToDecimal(reader["DonGia"])
                        });
                    }
                }
            }

            return list;
        }

        public void AddHoaDon(HoaDon hoaDon)
        {
            string query = @"INSERT INTO HoaDon (MaHD, NgayLap, TongTien, TrangThai, NguoiTao)
                             VALUES (@MaHD, @NgayLap, @TongTien, @TrangThai, @NguoiTao)";

            using (SqlCommand cmd = new SqlCommand(query, _conn))
            {
                cmd.Parameters.AddWithValue("@MaHD", hoaDon.MaHD);
                cmd.Parameters.AddWithValue("@NgayLap", hoaDon.NgayLap);
                cmd.Parameters.AddWithValue("@TongTien", hoaDon.TongTien); // ĐÃ áp dụng chiến lược
                cmd.Parameters.AddWithValue("@TrangThai", hoaDon.TrangThai);
                cmd.Parameters.AddWithValue("@NguoiTao", hoaDon.NguoiTao ?? "admin");
                cmd.ExecuteNonQuery();
            }
        }

        public void AddChiTietHoaDon(ChiTietHoaDon ct)
        {
            string query = @"INSERT INTO ChiTietHoaDon (MaHD, MaSP, TenSP, SoLuong, DonGia)
                             VALUES (@MaHD, @MaSP, @TenSP, @SoLuong, @DonGia)";

            using (SqlCommand cmd = new SqlCommand(query, _conn))
            {
                cmd.Parameters.AddWithValue("@MaHD", ct.MaHD);
                cmd.Parameters.AddWithValue("@MaSP", ct.MaSP);
                cmd.Parameters.AddWithValue("@TenSP", ct.TenSP);
                cmd.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                cmd.Parameters.AddWithValue("@DonGia", ct.DonGia);
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteHoaDon(string maHD)
        {
            string deleteChiTiet = "DELETE FROM ChiTietHoaDon WHERE MaHD = @maHD";
            string deleteHoaDon = "DELETE FROM HoaDon WHERE MaHD = @maHD";

            using (SqlCommand cmd1 = new SqlCommand(deleteChiTiet, _conn))
            using (SqlCommand cmd2 = new SqlCommand(deleteHoaDon, _conn))
            {
                cmd1.Parameters.AddWithValue("@maHD", maHD);
                cmd2.Parameters.AddWithValue("@maHD", maHD);
                cmd1.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();
            }
        }

    }
}