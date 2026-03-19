using System.Data;
using System.Data.SqlClient;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.DataAccess
{
    public class PhieuMuonDAL
    {
        public List<PhieuMuon> GetAll()
        {
            var list = new List<PhieuMuon>();
            string query = @"SELECT pm.*, bd.HoTen AS TenBanDoc 
                             FROM PhieuMuon pm 
                             INNER JOIN BanDoc bd ON pm.MaBanDoc = bd.MaBanDoc 
                             ORDER BY pm.NgayMuon DESC";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapFromDataRow(row));
            }
            return list;
        }

        public List<PhieuMuon> Search(string? keyword, string? trangThai)
        {
            var list = new List<PhieuMuon>();
            string query = @"SELECT pm.*, bd.HoTen AS TenBanDoc 
                             FROM PhieuMuon pm 
                             INNER JOIN BanDoc bd ON pm.MaBanDoc = bd.MaBanDoc 
                             WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query += " AND (bd.HoTen LIKE @Keyword OR CAST(pm.MaPhieuMuon AS NVARCHAR) LIKE @Keyword)";
                parameters.Add(new SqlParameter("@Keyword", $"%{keyword}%"));
            }

            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "Tat ca")
            {
                query += " AND pm.TrangThai = @TrangThai";
                parameters.Add(new SqlParameter("@TrangThai", trangThai));
            }

            query += " ORDER BY pm.NgayMuon DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapFromDataRow(row));
            }
            return list;
        }

        public PhieuMuon? GetById(int maPhieuMuon)
        {
            string query = @"SELECT pm.*, bd.HoTen AS TenBanDoc 
                             FROM PhieuMuon pm 
                             INNER JOIN BanDoc bd ON pm.MaBanDoc = bd.MaBanDoc 
                             WHERE pm.MaPhieuMuon = @MaPhieuMuon";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@MaPhieuMuon", maPhieuMuon));
            if (dt.Rows.Count > 0)
                return MapFromDataRow(dt.Rows[0]);
            return null;
        }

        public int Insert(PhieuMuon pm)
        {
            string query = @"INSERT INTO PhieuMuon (MaBanDoc, NgayMuon, NgayHenTra, TrangThai, GhiChu) 
                             VALUES (@MaBanDoc, @NgayMuon, @NgayHenTra, @TrangThai, @GhiChu);
                             SELECT SCOPE_IDENTITY();";
            var result = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@MaBanDoc", pm.MaBanDoc),
                new SqlParameter("@NgayMuon", pm.NgayMuon),
                new SqlParameter("@NgayHenTra", pm.NgayHenTra),
                new SqlParameter("@TrangThai", pm.TrangThai),
                new SqlParameter("@GhiChu", (object?)pm.GhiChu ?? DBNull.Value));
            return Convert.ToInt32(result);
        }

        public void InsertChiTiet(ChiTietMuon ct)
        {
            string query = @"INSERT INTO ChiTietMuon (MaPhieuMuon, MaSach, TenSach, DaTra) 
                             VALUES (@MaPhieuMuon, @MaSach, @TenSach, @DaTra)";
            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@MaPhieuMuon", ct.MaPhieuMuon),
                new SqlParameter("@MaSach", ct.MaSach),
                new SqlParameter("@TenSach", ct.TenSach),
                new SqlParameter("@DaTra", ct.DaTra));
        }

        public List<ChiTietMuon> GetChiTiet(int maPhieuMuon)
        {
            var list = new List<ChiTietMuon>();
            string query = "SELECT * FROM ChiTietMuon WHERE MaPhieuMuon = @MaPhieuMuon";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@MaPhieuMuon", maPhieuMuon));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ChiTietMuon
                {
                    MaChiTiet = Convert.ToInt32(row["MaChiTiet"]),
                    MaPhieuMuon = Convert.ToInt32(row["MaPhieuMuon"]),
                    MaSach = Convert.ToInt32(row["MaSach"]),
                    TenSach = row["TenSach"].ToString()!,
                    DaTra = Convert.ToBoolean(row["DaTra"])
                });
            }
            return list;
        }

        public ChiTietMuon? GetChiTietById(int maChiTiet)
        {
            string query = "SELECT * FROM ChiTietMuon WHERE MaChiTiet = @MaChiTiet";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@MaChiTiet", maChiTiet));
            if (dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];
            return new ChiTietMuon
            {
                MaChiTiet = Convert.ToInt32(row["MaChiTiet"]),
                MaPhieuMuon = Convert.ToInt32(row["MaPhieuMuon"]),
                MaSach = Convert.ToInt32(row["MaSach"]),
                TenSach = row["TenSach"].ToString()!,
                DaTra = Convert.ToBoolean(row["DaTra"])
            };
        }

        // Tra toan bo sach trong phieu: cap nhat trang thai phieu + chi tiet + so luong sach
        public bool TraSach(int maPhieuMuon)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Cap nhat chi tiet muon
                        var cmdChiTiet = new SqlCommand(
                            "UPDATE ChiTietMuon SET DaTra = 1 WHERE MaPhieuMuon = @MaPhieuMuon AND DaTra = 0",
                            conn, transaction);
                        cmdChiTiet.Parameters.AddWithValue("@MaPhieuMuon", maPhieuMuon);
                        cmdChiTiet.ExecuteNonQuery();

                        // Tang so luong sach con lai
                        var cmdSach = new SqlCommand(
                            @"UPDATE Sach SET SoLuongCon = SoLuongCon + sub.SoLuong
                              FROM Sach s
                              INNER JOIN (
                                  SELECT MaSach, COUNT(*) AS SoLuong
                                  FROM ChiTietMuon 
                                  WHERE MaPhieuMuon = @MaPhieuMuon
                                  GROUP BY MaSach
                              ) sub ON s.MaSach = sub.MaSach",
                            conn, transaction);
                        cmdSach.Parameters.AddWithValue("@MaPhieuMuon", maPhieuMuon);
                        cmdSach.ExecuteNonQuery();

                        // Cap nhat trang thai phieu muon
                        var cmdPhieu = new SqlCommand(
                            "UPDATE PhieuMuon SET TrangThai = N'Da tra', NgayTraThucTe = GETDATE() WHERE MaPhieuMuon = @MaPhieuMuon",
                            conn, transaction);
                        cmdPhieu.Parameters.AddWithValue("@MaPhieuMuon", maPhieuMuon);
                        cmdPhieu.ExecuteNonQuery();

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        // Tra tung cuon sach theo MaChiTiet
        public bool TraSachChiTiet(int maChiTiet)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Lay thong tin chi tiet
                        var cmdSelect = new SqlCommand(
                            "SELECT MaPhieuMuon, MaSach, DaTra FROM ChiTietMuon WHERE MaChiTiet = @MaChiTiet",
                            conn, transaction);
                        cmdSelect.Parameters.AddWithValue("@MaChiTiet", maChiTiet);

                        int maPhieuMuon;
                        int maSach;
                        bool daTra;

                        using (var reader = cmdSelect.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                transaction.Rollback();
                                return false;
                            }

                            maPhieuMuon = reader.GetInt32(reader.GetOrdinal("MaPhieuMuon"));
                            maSach = reader.GetInt32(reader.GetOrdinal("MaSach"));
                            daTra = reader.GetBoolean(reader.GetOrdinal("DaTra"));
                        }

                        if (daTra)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        // Cap nhat chi tiet thanh da tra
                        var cmdUpdateChiTiet = new SqlCommand(
                            "UPDATE ChiTietMuon SET DaTra = 1 WHERE MaChiTiet = @MaChiTiet AND DaTra = 0",
                            conn, transaction);
                        cmdUpdateChiTiet.Parameters.AddWithValue("@MaChiTiet", maChiTiet);
                        int rows = cmdUpdateChiTiet.ExecuteNonQuery();
                        if (rows == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        // Tang so luong sach con lai cho cuon sach nay
                        var cmdUpdateSach = new SqlCommand(
                            "UPDATE Sach SET SoLuongCon = SoLuongCon + 1 WHERE MaSach = @MaSach",
                            conn, transaction);
                        cmdUpdateSach.Parameters.AddWithValue("@MaSach", maSach);
                        cmdUpdateSach.ExecuteNonQuery();

                        // Neu tat ca sach trong phieu da tra thi cap nhat trang thai phieu
                        var cmdCheckRemain = new SqlCommand(
                            "SELECT COUNT(*) FROM ChiTietMuon WHERE MaPhieuMuon = @MaPhieuMuon AND DaTra = 0",
                            conn, transaction);
                        cmdCheckRemain.Parameters.AddWithValue("@MaPhieuMuon", maPhieuMuon);
                        int conSachChuaTra = Convert.ToInt32(cmdCheckRemain.ExecuteScalar());

                        if (conSachChuaTra == 0)
                        {
                            var cmdUpdatePhieu = new SqlCommand(
                                "UPDATE PhieuMuon SET TrangThai = N'Da tra', NgayTraThucTe = GETDATE() WHERE MaPhieuMuon = @MaPhieuMuon",
                                conn, transaction);
                            cmdUpdatePhieu.Parameters.AddWithValue("@MaPhieuMuon", maPhieuMuon);
                            cmdUpdatePhieu.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool Delete(int maPhieuMuon)
        {
            string query = "DELETE FROM PhieuMuon WHERE MaPhieuMuon = @MaPhieuMuon";
            int rows = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@MaPhieuMuon", maPhieuMuon));
            return rows > 0;
        }

        // Cap nhat phieu qua han
        public void CapNhatQuaHan()
        {
            string query = @"UPDATE PhieuMuon SET TrangThai = N'Qua han' 
                             WHERE TrangThai = N'Dang muon' AND NgayHenTra < GETDATE()";
            DatabaseHelper.ExecuteNonQuery(query);
        }

        private PhieuMuon MapFromDataRow(DataRow row)
        {
            return new PhieuMuon
            {
                MaPhieuMuon = Convert.ToInt32(row["MaPhieuMuon"]),
                MaBanDoc = Convert.ToInt32(row["MaBanDoc"]),
                TenBanDoc = row["TenBanDoc"].ToString()!,
                NgayMuon = Convert.ToDateTime(row["NgayMuon"]),
                NgayHenTra = Convert.ToDateTime(row["NgayHenTra"]),
                NgayTraThucTe = row["NgayTraThucTe"] != DBNull.Value ? Convert.ToDateTime(row["NgayTraThucTe"]) : null,
                TrangThai = row["TrangThai"].ToString()!,
                GhiChu = row["GhiChu"]?.ToString()
            };
        }
    }
}
