using System.Data;
using QuanLyThuVien.DataAccess;

namespace QuanLyThuVien.BusinessLogic
{
    public class ThongKeBLL
    {
        private readonly ThongKeDAL _dal = new ThongKeDAL();

        public DataTable GetTongQuan() => _dal.GetTongQuan();
        public Models.ThongKeMuon? GetThongKeThang(int nam, int thang) => _dal.GetThongKeThang(nam, thang);
        public List<Models.ThongKeMuon> GetThongKeCacThang(int nam) => _dal.GetThongKeCacThang(nam);
        public List<Models.SachMuonNhieu> GetSachMuonNhieu(int nam, int thang, int top = 10) => _dal.GetSachMuonNhieu(nam, thang, top);
    }
}
