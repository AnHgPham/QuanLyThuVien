using System.Data;
using System.Data.SqlClient;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.DataAccess
{
    public class SachDAL
    {
        public List<Sach> GetAll()
        {
            var list = new List<Sach>();
            string query = @"SELECT s.*, tl.TenTheLoai 
                             FROM Sach s 
                             INNER JOIN TheLoai tl ON s.MaTheLoai = tl.MaTheLoai 
                             ORDER BY s.TenSach";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapFromDataRow(row));
            }
            return list;
        }

        public List<Sach> Search(string? keyword, int? maTheLoai, string? tacGia)
        {
            var list = new List<Sach>();
            string query = @"SELECT s.*, tl.TenTheLoai 
                             FROM Sach s 
                             INNER JOIN TheLoai tl ON s.MaTheLoai = tl.MaTheLoai 
                             WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query += " AND (s.TenSach LIKE @Keyword OR s.MoTa LIKE @Keyword)";
                parameters.Add(new SqlParameter("@Keyword", $"%{keyword}%"));
            }

            if (maTheLoai.HasValue && maTheLoai.Value > 0)
            {
                query += " AND s.MaTheLoai = @MaTheLoai";
                parameters.Add(new SqlParameter("@MaTheLoai", maTheLoai.Value));
            }

            if (!string.IsNullOrWhiteSpace(tacGia))
            {
                query += " AND s.TacGia LIKE @TacGia";
                parameters.Add(new SqlParameter("@TacGia", $"%{tacGia}%"));
            }

            query += " ORDER BY s.TenSach";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapFromDataRow(row));
            }
            return list;
        }

        public Sach? GetById(int maSach)
        {
            string query = @"SELECT s.*, tl.TenTheLoai 
                             FROM Sach s 
                             INNER JOIN TheLoai tl ON s.MaTheLoai = tl.MaTheLoai 
                             WHERE s.MaSach = @MaSach";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@MaSach", maSach));
            if (dt.Rows.Count > 0)
                return MapFromDataRow(dt.Rows[0]);
            return null;
        }

        public int Insert(Sach s)
        {
            string query = @"INSERT INTO Sach (TenSach, TacGia, MaTheLoai, NhaXuatBan, NamXuatBan, SoLuongTong, SoLuongCon, MoTa, TrangThai) 
                             VALUES (@TenSach, @TacGia, @MaTheLoai, @NhaXuatBan, @NamXuatBan, @SoLuongTong, @SoLuongCon, @MoTa, @TrangThai);
                             SELECT SCOPE_IDENTITY();";
            var result = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@TenSach", s.TenSach),
                new SqlParameter("@TacGia", s.TacGia),
                new SqlParameter("@MaTheLoai", s.MaTheLoai),
                new SqlParameter("@NhaXuatBan", (object?)s.NhaXuatBan ?? DBNull.Value),
                new SqlParameter("@NamXuatBan", (object?)s.NamXuatBan ?? DBNull.Value),
                new SqlParameter("@SoLuongTong", s.SoLuongTong),
                new SqlParameter("@SoLuongCon", s.SoLuongCon),
                new SqlParameter("@MoTa", (object?)s.MoTa ?? DBNull.Value),
                new SqlParameter("@TrangThai", s.TrangThai));
            return Convert.ToInt32(result);
        }

        public bool Update(Sach s)
        {
            string query = @"UPDATE Sach SET 
                             TenSach = @TenSach, TacGia = @TacGia, MaTheLoai = @MaTheLoai,
                             NhaXuatBan = @NhaXuatBan, NamXuatBan = @NamXuatBan,
                             SoLuongTong = @SoLuongTong, SoLuongCon = @SoLuongCon,
                             MoTa = @MoTa, TrangThai = @TrangThai, NgayCapNhat = GETDATE()
                             WHERE MaSach = @MaSach";
            int rows = DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@MaSach", s.MaSach),
                new SqlParameter("@TenSach", s.TenSach),
                new SqlParameter("@TacGia", s.TacGia),
                new SqlParameter("@MaTheLoai", s.MaTheLoai),
                new SqlParameter("@NhaXuatBan", (object?)s.NhaXuatBan ?? DBNull.Value),
                new SqlParameter("@NamXuatBan", (object?)s.NamXuatBan ?? DBNull.Value),
                new SqlParameter("@SoLuongTong", s.SoLuongTong),
                new SqlParameter("@SoLuongCon", s.SoLuongCon),
                new SqlParameter("@MoTa", (object?)s.MoTa ?? DBNull.Value),
                new SqlParameter("@TrangThai", s.TrangThai));
            return rows > 0;
        }

        public bool Delete(int maSach)
        {
            string query = "DELETE FROM Sach WHERE MaSach = @MaSach";
            int rows = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@MaSach", maSach));
            return rows > 0;
        }

        public bool UpdateSoLuong(int maSach, int soLuongThayDoi)
        {
            string query = "UPDATE Sach SET SoLuongCon = SoLuongCon + @SoLuong, NgayCapNhat = GETDATE() WHERE MaSach = @MaSach";
            int rows = DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@MaSach", maSach),
                new SqlParameter("@SoLuong", soLuongThayDoi));
            return rows > 0;
        }

        private Sach MapFromDataRow(DataRow row)
        {
            return new Sach
            {
                MaSach = Convert.ToInt32(row["MaSach"]),
                TenSach = row["TenSach"].ToString()!,
                TacGia = row["TacGia"].ToString()!,
                MaTheLoai = Convert.ToInt32(row["MaTheLoai"]),
                TenTheLoai = row["TenTheLoai"].ToString()!,
                NhaXuatBan = row["NhaXuatBan"]?.ToString(),
                NamXuatBan = row["NamXuatBan"] != DBNull.Value ? Convert.ToInt32(row["NamXuatBan"]) : null,
                SoLuongTong = Convert.ToInt32(row["SoLuongTong"]),
                SoLuongCon = Convert.ToInt32(row["SoLuongCon"]),
                MoTa = row["MoTa"]?.ToString(),
                NgayTao = Convert.ToDateTime(row["NgayTao"]),
                NgayCapNhat = Convert.ToDateTime(row["NgayCapNhat"]),
                TrangThai = Convert.ToBoolean(row["TrangThai"])
            };
        }
    }
}
