using System.Data;
using System.Data.SqlClient;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.DataAccess
{
    public class TheLoaiDAL
    {
        public List<TheLoai> GetAll()
        {
            var list = new List<TheLoai>();
            string query = "SELECT * FROM TheLoai ORDER BY TenTheLoai";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapFromDataRow(row));
            }
            return list;
        }

        public TheLoai? GetById(int maTheLoai)
        {
            string query = "SELECT * FROM TheLoai WHERE MaTheLoai = @MaTheLoai";
            DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@MaTheLoai", maTheLoai));
            if (dt.Rows.Count > 0)
                return MapFromDataRow(dt.Rows[0]);
            return null;
        }

        public int Insert(TheLoai tl)
        {
            string query = @"INSERT INTO TheLoai (TenTheLoai, MoTa) VALUES (@TenTheLoai, @MoTa);
                             SELECT SCOPE_IDENTITY();";
            var result = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@TenTheLoai", tl.TenTheLoai),
                new SqlParameter("@MoTa", (object?)tl.MoTa ?? DBNull.Value));
            return Convert.ToInt32(result);
        }

        public bool Update(TheLoai tl)
        {
            string query = "UPDATE TheLoai SET TenTheLoai = @TenTheLoai, MoTa = @MoTa WHERE MaTheLoai = @MaTheLoai";
            int rows = DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@MaTheLoai", tl.MaTheLoai),
                new SqlParameter("@TenTheLoai", tl.TenTheLoai),
                new SqlParameter("@MoTa", (object?)tl.MoTa ?? DBNull.Value));
            return rows > 0;
        }

        public bool Delete(int maTheLoai)
        {
            string query = "DELETE FROM TheLoai WHERE MaTheLoai = @MaTheLoai";
            int rows = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@MaTheLoai", maTheLoai));
            return rows > 0;
        }

        private TheLoai MapFromDataRow(DataRow row)
        {
            return new TheLoai
            {
                MaTheLoai = Convert.ToInt32(row["MaTheLoai"]),
                TenTheLoai = row["TenTheLoai"].ToString()!,
                MoTa = row["MoTa"]?.ToString(),
                NgayTao = Convert.ToDateTime(row["NgayTao"])
            };
        }
    }
}
