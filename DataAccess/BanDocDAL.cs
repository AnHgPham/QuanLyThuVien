using System.Data;
using System.Data.SqlClient;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.DataAccess
{
    public class BanDocDAL
    {
        public List<BanDoc> GetAll()
        {
            var list = new List<BanDoc>();
            string query = "SELECT * FROM BanDoc ORDER BY HoTen";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapFromDataRow(row));
            }
            return list;
        }

        public List<BanDoc> Search(string? keyword)
        {
            var list = new List<BanDoc>();
            string query = "SELECT * FROM BanDoc WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query += " AND (HoTen LIKE @Keyword OR SoDienThoai LIKE @Keyword OR Email LIKE @Keyword)";
                parameters.Add(new SqlParameter("@Keyword", $"%{keyword}%"));
            }

            query += " ORDER BY HoTen";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapFromDataRow(row));
            }
            return list;
        }

        public BanDoc? GetById(int maBanDoc)
        {
            string query = "SELECT * FROM BanDoc WHERE MaBanDoc = @MaBanDoc";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@MaBanDoc", maBanDoc));
            if (dt.Rows.Count > 0)
                return MapFromDataRow(dt.Rows[0]);
            return null;
        }

        public int Insert(BanDoc bd)
        {
            string query = @"INSERT INTO BanDoc (HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi, TrangThai) 
                             VALUES (@HoTen, @NgaySinh, @GioiTinh, @SoDienThoai, @Email, @DiaChi, @TrangThai);
                             SELECT SCOPE_IDENTITY();";
            var result = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@HoTen", bd.HoTen),
                new SqlParameter("@NgaySinh", (object?)bd.NgaySinh ?? DBNull.Value),
                new SqlParameter("@GioiTinh", (object?)bd.GioiTinh ?? DBNull.Value),
                new SqlParameter("@SoDienThoai", (object?)bd.SoDienThoai ?? DBNull.Value),
                new SqlParameter("@Email", (object?)bd.Email ?? DBNull.Value),
                new SqlParameter("@DiaChi", (object?)bd.DiaChi ?? DBNull.Value),
                new SqlParameter("@TrangThai", bd.TrangThai));
            return Convert.ToInt32(result);
        }

        public bool Update(BanDoc bd)
        {
            string query = @"UPDATE BanDoc SET 
                             HoTen = @HoTen, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh,
                             SoDienThoai = @SoDienThoai, Email = @Email, DiaChi = @DiaChi,
                             TrangThai = @TrangThai
                             WHERE MaBanDoc = @MaBanDoc";
            int rows = DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@MaBanDoc", bd.MaBanDoc),
                new SqlParameter("@HoTen", bd.HoTen),
                new SqlParameter("@NgaySinh", (object?)bd.NgaySinh ?? DBNull.Value),
                new SqlParameter("@GioiTinh", (object?)bd.GioiTinh ?? DBNull.Value),
                new SqlParameter("@SoDienThoai", (object?)bd.SoDienThoai ?? DBNull.Value),
                new SqlParameter("@Email", (object?)bd.Email ?? DBNull.Value),
                new SqlParameter("@DiaChi", (object?)bd.DiaChi ?? DBNull.Value),
                new SqlParameter("@TrangThai", bd.TrangThai));
            return rows > 0;
        }

        public bool Delete(int maBanDoc)
        {
            string query = "DELETE FROM BanDoc WHERE MaBanDoc = @MaBanDoc";
            int rows = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@MaBanDoc", maBanDoc));
            return rows > 0;
        }

        // Dem so sach dang muon cua ban doc
        public int DemSachDangMuon(int maBanDoc)
        {
            string query = @"SELECT COUNT(ct.MaChiTiet) 
                             FROM ChiTietMuon ct 
                             INNER JOIN PhieuMuon pm ON ct.MaPhieuMuon = pm.MaPhieuMuon
                             WHERE pm.MaBanDoc = @MaBanDoc AND ct.DaTra = 0";
            var result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@MaBanDoc", maBanDoc));
            return Convert.ToInt32(result);
        }

        private BanDoc MapFromDataRow(DataRow row)
        {
            return new BanDoc
            {
                MaBanDoc = Convert.ToInt32(row["MaBanDoc"]),
                HoTen = row["HoTen"].ToString()!,
                NgaySinh = row["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(row["NgaySinh"]) : null,
                GioiTinh = row["GioiTinh"]?.ToString(),
                SoDienThoai = row["SoDienThoai"]?.ToString(),
                Email = row["Email"]?.ToString(),
                DiaChi = row["DiaChi"]?.ToString(),
                NgayLapThe = Convert.ToDateTime(row["NgayLapThe"]),
                TrangThai = Convert.ToBoolean(row["TrangThai"])
            };
        }
    }
}
