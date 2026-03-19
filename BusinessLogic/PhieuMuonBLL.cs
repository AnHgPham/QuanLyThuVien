using QuanLyThuVien.DataAccess;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.BusinessLogic
{
    public class PhieuMuonBLL
    {
        private readonly PhieuMuonDAL _dal = new PhieuMuonDAL();
        private readonly BanDocDAL _banDocDAL = new BanDocDAL();
        private readonly SachDAL _sachDAL = new SachDAL();

        // So sach toi da 1 ban doc duoc muon cung luc
        public const int MAX_SACH_MUON = 5;

        public List<PhieuMuon> GetAll() => _dal.GetAll();
        public List<PhieuMuon> Search(string? keyword, string? trangThai) => _dal.Search(keyword, trangThai);
        public PhieuMuon? GetById(int maPhieuMuon) => _dal.GetById(maPhieuMuon);
        public List<ChiTietMuon> GetChiTiet(int maPhieuMuon) => _dal.GetChiTiet(maPhieuMuon);

        public (bool Success, string Message) TaoPhieuMuon(PhieuMuon pm, List<ChiTietMuon> chiTietList)
        {
            // Validate
            if (pm.MaBanDoc <= 0)
                return (false, "Vui long chon ban doc!");

            if (chiTietList.Count == 0)
                return (false, "Vui long chon it nhat 1 cuon sach!");

            // Kiem tra so sach dang muon
            int sachDangMuon = _banDocDAL.DemSachDangMuon(pm.MaBanDoc);
            if (sachDangMuon + chiTietList.Count > MAX_SACH_MUON)
                return (false, $"Ban doc dang muon {sachDangMuon} sach. Toi da {MAX_SACH_MUON} sach. Chi co the muon them {MAX_SACH_MUON - sachDangMuon} sach!");

            // Kiem tra so luong sach con
            foreach (var ct in chiTietList)
            {
                var sach = _sachDAL.GetById(ct.MaSach);
                if (sach == null)
                    return (false, $"Sach '{ct.TenSach}' khong ton tai!");
                if (sach.SoLuongCon <= 0)
                    return (false, $"Sach '{sach.TenSach}' da het ban con lai!");
            }

            // Tao phieu muon
            int maPhieuMuon = _dal.Insert(pm);
            if (maPhieuMuon <= 0)
                return (false, "Tao phieu muon that bai!");

            // Them chi tiet va giam so luong sach
            foreach (var ct in chiTietList)
            {
                ct.MaPhieuMuon = maPhieuMuon;
                _dal.InsertChiTiet(ct);
                _sachDAL.UpdateSoLuong(ct.MaSach, -1); // Giam so luong con
            }

            return (true, $"Tao phieu muon thanh cong! Ma phieu: {maPhieuMuon}");
        }

        public (bool Success, string Message) TraSach(int maPhieuMuon)
        {
            var phieu = _dal.GetById(maPhieuMuon);
            if (phieu == null)
                return (false, "Khong tim thay phieu muon!");

            if (phieu.TrangThai == "Da tra")
                return (false, "Phieu muon nay da tra roi!");

            bool result = _dal.TraSach(maPhieuMuon);
            
            string quaHanMsg = "";
            if (phieu.NgayHenTra < DateTime.Now)
            {
                int soNgayTre = (int)(DateTime.Now - phieu.NgayHenTra).TotalDays;
                quaHanMsg = $" (Tra tre {soNgayTre} ngay)";
            }

            return result ? (true, $"Tra sach thanh cong!{quaHanMsg}") : (false, "Tra sach that bai!");
        }

        public (bool Success, string Message) TraSachChiTiet(int maChiTiet)
        {
            var chiTiet = _dal.GetChiTietById(maChiTiet);
            if (chiTiet == null)
                return (false, "Khong tim thay ban ghi muon sach!");

            var phieu = _dal.GetById(chiTiet.MaPhieuMuon);
            if (phieu == null)
                return (false, "Khong tim thay phieu muon!");

            if (chiTiet.DaTra)
                return (false, "Cuon sach nay da duoc tra truoc do!");

            if (phieu.TrangThai == "Da tra")
                return (false, "Phieu muon nay da tra xong!");

            bool result = _dal.TraSachChiTiet(maChiTiet);
            if (!result)
                return (false, "Tra sach that bai!");

            // Kiem tra lai trang thai phieu sau khi tra tung cuon
            phieu = _dal.GetById(chiTiet.MaPhieuMuon);
            string msg = "Tra sach thanh cong!";

            if (phieu != null && phieu.TrangThai == "Da tra" && phieu.NgayHenTra < DateTime.Now)
            {
                int soNgayTre = (int)(DateTime.Now - phieu.NgayHenTra).TotalDays;
                msg += $" (Tra tre {soNgayTre} ngay)";
            }

            return (true, msg);
        }

        public void CapNhatQuaHan() => _dal.CapNhatQuaHan();

        public (bool Success, string Message) Delete(int maPhieuMuon)
        {
            try
            {
                var phieu = _dal.GetById(maPhieuMuon);
                if (phieu == null)
                    return (false, "Khong tim thay phieu muon!");

                if (phieu.TrangThai == "Dang muon" || phieu.TrangThai == "Qua han")
                    return (false, "Khong the xoa phieu muon chua tra sach!");

                bool result = _dal.Delete(maPhieuMuon);
                return result ? (true, "Xoa phieu muon thanh cong!") : (false, "Xoa phieu muon that bai!");
            }
            catch (Exception ex)
            {
                return (false, $"Loi: {ex.Message}");
            }
        }
    }
}
