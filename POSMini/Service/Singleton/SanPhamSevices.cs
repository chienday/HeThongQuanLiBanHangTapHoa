using POSMini.Models;
using POSMini.Service.Singleton;
using POSMini.Service.Strategies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace POSMini.Service.Singleton
{
    public sealed class SanPhamService
    {
        private static SanPhamService instance = null;
        private static readonly object locks = new object();

        private SanPhamService() { }

        public static SanPhamService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locks)
                    {
                        if (instance == null)
                        {
                            instance = new SanPhamService();
                        }
                    }
                }
                return instance;
            }
        }


        // Lấy danh sách sản phẩm từ database

        private List<SanPhamView> ExecuteViewModelQuery(string query, SqlParameter[] parameters = null)
        {
            var sp = new List<SanPhamView>();
            using (var conn = DatabaseConnection.Instance.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sp.Add(new SanPhamView
                        {
                            MaSP = reader["MaSP"].ToString(),
                            TenSP = reader["TenSP"].ToString(),
                            MaLoai = reader["MaLoai"].ToString(), // Đọc MaLoai từ kết quả query
                            TenLoai = reader["TenLoai"].ToString(),
                            GiaNhap = Convert.ToDecimal(reader["GiaNhap"]),
                            GiaBan = Convert.ToDecimal(reader["GiaBan"]),
                            SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                            HinhAnh = reader["HinhAnh"] == DBNull.Value ? null : reader["HinhAnh"].ToString(),
                            MoTa = reader["MoTa"] == DBNull.Value ? string.Empty : reader["MoTa"].ToString()
                        });
                    }
                }
            }
            return sp;
        }
        public List<SanPhamView> GetAllSP()
        {
            string query = @"
        SELECT 
            sp.MaSP, sp.TenSP, sp.MaLoai, lsp.TenLoai, sp.GiaNhap, 
            sp.GiaBan, sp.SoLuongTon, sp.HinhAnh, sp.MoTa 
        FROM SanPham sp
        LEFT JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai
        WHERE sp.TrangThai = 1
        ORDER BY sp.TenSP ASC";
            return ExecuteViewModelQuery(query);
        }

        //Tìm kiếm sản phẩm
        public List<SanPhamView> SearchProductsForDisplay(string searchTerm)
        {
            string query = @"
        SELECT 
            sp.MaSP, sp.TenSP, sp.MaLoai, lsp.TenLoai, sp.GiaNhap, sp.GiaBan, 
            sp.SoLuongTon, sp.HinhAnh, sp.MoTa 
        FROM SanPham sp
        LEFT JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai
        WHERE sp.TrangThai = 1 AND (sp.TenSP LIKE @searchTerm OR sp.MaSP LIKE @searchTerm OR lsp.TenLoai LIKE @searchTerm)
        ORDER BY sp.TenSP ASC";
            var parameters = new SqlParameter[] { new SqlParameter("@searchTerm", $"%{searchTerm}%") };
            return ExecuteViewModelQuery(query, parameters);
        }

        //Thêm sản phẩm
        public bool ThemSP(SanPham sanPham)
        {
            string query = "INSERT INTO SanPham (MaSP, TenSP, MaLoai, GiaNhap, GiaBan, SoLuongTon, SoLuongToiThieu, DonViTinh, MoTa, TrangThai, HinhAnh, NgayTao) VALUES (@MaSP, @TenSP, @MaLoai, @GiaNhap, @GiaBan, @SoLuongTon, @SoLuongToiThieu, @DonViTinh, @MoTa, @TrangThai, @HinhAnh, GETDATE())";
            using (var conn = DatabaseConnection.Instance.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaSP", sanPham.MaSP);
                cmd.Parameters.AddWithValue("@TenSP", sanPham.TenSP);
                cmd.Parameters.AddWithValue("@MaLoai", sanPham.MaLoai);
                cmd.Parameters.AddWithValue("@GiaNhap", sanPham.GiaNhap);
                cmd.Parameters.AddWithValue("@GiaBan", sanPham.GiaBan);
                cmd.Parameters.AddWithValue("@SoLuongTon", sanPham.SoLuongTon);
                cmd.Parameters.AddWithValue("@SoLuongToiThieu", sanPham.SoLuongToiThieu);
                cmd.Parameters.AddWithValue("@DonViTinh", sanPham.DonViTinh);
                cmd.Parameters.AddWithValue("@MoTa", (object)sanPham.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", sanPham.TrangThai);
                cmd.Parameters.AddWithValue("@HinhAnh", (object)sanPham.HinhAnh ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //Sửa sản phẩm
        public bool SuaSP(SanPham sanPham)
        {
            string query = @"
                UPDATE SanPham 
                SET TenSP = @TenSP, MaLoai = @MaLoai, GiaNhap = @GiaNhap, GiaBan = @GiaBan, 
                    SoLuongTon = @SoLuongTon, SoLuongToiThieu = @SoLuongToiThieu, 
                    DonViTinh = @DonViTinh, MoTa = @MoTa, TrangThai = @TrangThai, 
                    NgayCapNhat = GETDATE(), HinhAnh = @HinhAnh 
                WHERE MaSP = @MaSP";

            using (var conn = DatabaseConnection.Instance.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaSP", sanPham.MaSP);
                cmd.Parameters.AddWithValue("@TenSP", sanPham.TenSP);
                cmd.Parameters.AddWithValue("@MaLoai", sanPham.MaLoai);
                cmd.Parameters.AddWithValue("@GiaNhap", sanPham.GiaNhap);
                cmd.Parameters.AddWithValue("@GiaBan", sanPham.GiaBan);
                cmd.Parameters.AddWithValue("@SoLuongTon", sanPham.SoLuongTon);
                cmd.Parameters.AddWithValue("@SoLuongToiThieu", sanPham.SoLuongToiThieu);
                cmd.Parameters.AddWithValue("@DonViTinh", sanPham.DonViTinh);
                cmd.Parameters.AddWithValue("@MoTa", (object)sanPham.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", sanPham.TrangThai);
                cmd.Parameters.AddWithValue("@HinhAnh", (object)sanPham.HinhAnh ?? DBNull.Value);


                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //Xóa sản phẩm
        public bool XoaSP(string maSP)
        {
            // Thay vì DELETE, UPDATE cột TrangThai = 0
            string query = "UPDATE SanPham SET TrangThai = 0 WHERE MaSP = @MaSP";
            using (var conn = DatabaseConnection.Instance.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaSP", maSP);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaNhieuSP(List<string> maSPList)
        {

            if (maSPList == null || maSPList.Count == 0)
            {
                return true;
            }

            var parameters = new List<SqlParameter>();
            var parameterNames = new List<string>();
            for (int i = 0; i < maSPList.Count; i++)
            {
                string paramName = $"@p{i}";
                parameterNames.Add(paramName);
                parameters.Add(new SqlParameter(paramName, maSPList[i]));
            }

            string query = $"UPDATE SanPham SET TrangThai = 0 WHERE MaSP IN ({string.Join(", ", parameterNames)})";

            using (var conn = DatabaseConnection.Instance.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<LoaiSanPham> GetLoai()
        {
            var types = new List<LoaiSanPham>();
            string query = "SELECT MaLoai, TenLoai FROM LoaiSanPham WHERE TrangThai = 1 ORDER BY TenLoai";

            using (var conn = DatabaseConnection.Instance.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        types.Add(new LoaiSanPham
                        {
                            MaLoai = reader["MaLoai"].ToString(),
                            TenLoai = reader["TenLoai"].ToString()
                        });
                    }
                }
            }
            return types;
        }
    }
}

