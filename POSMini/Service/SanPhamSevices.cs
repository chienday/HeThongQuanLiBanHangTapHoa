using POSMini.Models;
using POSMini.Service.Singleton;
using POSMini.Service.Strategies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace POSMini.Service
{
    public sealed class SanPhamService
    {
        private static readonly Lazy<SanPhamService> _lazyInstance = new Lazy<SanPhamService>(() => new SanPhamService());
        private SanPhamService() { }
        public static SanPhamService Instance => _lazyInstance.Value;


        private List<SanPham> ExecuteProductQuery(string query, SqlParameter[] parameters = null)
        {
            var sp = new List<SanPham>();
            using (var conn = DatabaseManager.Instance.GetConnection())
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
                        sp.Add(new SanPham
                        {
                            MaSP = reader["MaSP"].ToString(),
                            TenSP = reader["TenSP"].ToString(),
                            GiaBan = Convert.ToDecimal(reader["GiaBan"]),
                            SoLuong = Convert.ToInt32(reader["SoLuong"]),
                            TenLoai = reader["TenLoai"].ToString(),
                            HinhAnh = reader["HinhAnh"] == DBNull.Value ? null : reader["HinhAnh"].ToString()
                        });
                    }
                }
            }
            return sp;
        }
        // Lấy danh sách sản phẩm từ database
        public List<SanPham> GetAllSP()
        {
            string query = "SELECT * FROM SanPham ORDER BY TenSP ASC";
            return ExecuteProductQuery(query);
        }

        //Tìm kiếm sản phẩm
        public List<SanPham> TimKiemSP(string searchTerm)
        {
            string query = "SELECT * FROM SanPham WHERE TenSP LIKE @searchTerm OR MaSP LIKE @searchTerm OR TenLoai LIKE @searchTerm ORDER BY TenSP ASC";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@searchTerm", $"%{searchTerm}%")
            };
            return ExecuteProductQuery(query, parameters);
        }

        //Thêm sản phẩm
        public bool ThemSP(SanPham sanPham)
        {
            using (var conn = DatabaseManager.Instance.GetConnection())
            {

                string query = "INSERT INTO SanPham (MaSP, TenSP, GiaBan, SoLuong, TenLoai, HinhAnh) VALUES (@MaSP, @TenSP, @GiaBan, @SoLuong, @TenLoai, @HinhAnh)";
                // using (var conn = DatabaseManager.Instance.GetConnection())
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSP", sanPham.MaSP);
                    cmd.Parameters.AddWithValue("@TenSP", sanPham.TenSP);
                    cmd.Parameters.AddWithValue("@GiaBan", sanPham.GiaBan);
                    cmd.Parameters.AddWithValue("@SoLuong", sanPham.SoLuong);
                    cmd.Parameters.AddWithValue("@TenLoai", (object)sanPham.TenLoai ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HinhAnh", (object)sanPham.HinhAnh ?? DBNull.Value);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        //Sửa sản phẩm
        public bool SuaSP(SanPham sanPham)
        {
            string query = "UPDATE SanPham SET TenSP = @TenSP, GiaBan = @GiaBan, SoLuong = @SoLuong, TenLoai = @TenLoai, HinhAnh = @HinhAnh WHERE MaSP = @MaSP";
            using (var conn = DatabaseManager.Instance.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaSP", sanPham.MaSP);
                cmd.Parameters.AddWithValue("@TenSP", sanPham.TenSP);
                cmd.Parameters.AddWithValue("@GiaBan", sanPham.GiaBan);
                cmd.Parameters.AddWithValue("@SoLuong", sanPham.SoLuong);
                cmd.Parameters.AddWithValue("@TenLoai", (object)sanPham.TenLoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HinhAnh", (object)sanPham.HinhAnh ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //Xóa sản phẩm
        public bool XoaSP(string maSP)
        {
            string query = "DELETE FROM SanPham WHERE MaSP = @MaSP";
            using (var conn = DatabaseManager.Instance.GetConnection())
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

            string inClause = string.Join(", ", parameterNames);
            string query = $"DELETE FROM SanPham WHERE MaSP IN ({inClause})";

            using (var conn = DatabaseManager.Instance.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                conn.Open();

                return cmd.ExecuteNonQuery() > 0;
            }
        }


    }
}
