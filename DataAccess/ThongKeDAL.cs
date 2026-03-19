using System.Data;
using System.Data.SqlClient;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.DataAccess
{
    public class ThongKeDAL
    {
        public DataTable GetTongQuan()
        {
            return DatabaseHelper.ExecuteStoredProcedure("sp_TongQuan");
        }

        public ThongKeMuon? GetThongKeThang(int nam, int thang)
        {
            DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_ThongKe_MuonTra_Thang",
                new SqlParameter("@Nam", nam),
                new SqlParameter("@Thang", thang));
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new ThongKeMuon
                {
                    Nam = Convert.ToInt32(row["Nam"]),
                    Thang = Convert.ToInt32(row["Thang"]),
                    TongPhieuMuon = Convert.ToInt32(row["TongPhieuMuon"]),
                    SoPhieuDaTra = Convert.ToInt32(row["SoPhieuDaTra"]),
                    SoPhieuDangMuon = Convert.ToInt32(row["SoPhieuDangMuon"]),
                    SoPhieuQuaHan = Convert.ToInt32(row["SoPhieuQuaHan"]),
                    TongSoSachMuon = Convert.ToInt32(row["TongSoSachMuon"])
                };
            }
            return null;
        }

        public List<ThongKeMuon> GetThongKeCacThang(int nam)
        {
            var list = new List<ThongKeMuon>();
            DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_ThongKe_MuonTra_CacThang",
                new SqlParameter("@Nam", nam));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ThongKeMuon
                {
                    Nam = nam,
                    Thang = Convert.ToInt32(row["Thang"]),
                    TongPhieuMuon = Convert.ToInt32(row["TongPhieuMuon"]),
                    SoPhieuDaTra = Convert.ToInt32(row["SoPhieuDaTra"]),
                    SoPhieuDangMuon = Convert.ToInt32(row["SoPhieuDangMuon"]),
                    SoPhieuQuaHan = Convert.ToInt32(row["SoPhieuQuaHan"])
                });
            }
            return list;
        }

        public List<SachMuonNhieu> GetSachMuonNhieu(int nam, int thang, int top = 10)
        {
            var list = new List<SachMuonNhieu>();
            DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_ThongKe_SachMuonNhieu",
                new SqlParameter("@Nam", nam),
                new SqlParameter("@Thang", thang),
                new SqlParameter("@Top", top));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SachMuonNhieu
                {
                    MaSach = Convert.ToInt32(row["MaSach"]),
                    TenSach = row["TenSach"].ToString()!,
                    TacGia = row["TacGia"].ToString()!,
                    TenTheLoai = row["TenTheLoai"].ToString()!,
                    SoLanMuon = Convert.ToInt32(row["SoLanMuon"])
                });
            }
            return list;
        }
    }
}
