using QuanLyThuVien.DataAccess;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.BusinessLogic
{
    public class SachBLL
    {
        private readonly SachDAL _dal = new SachDAL();

        public List<Sach> GetAll() => _dal.GetAll();

        public List<Sach> Search(string? keyword, int? maTheLoai, string? tacGia)
            => _dal.Search(keyword, maTheLoai, tacGia);

        public Sach? GetById(int maSach) => _dal.GetById(maSach);

        public (bool Success, string Message) Insert(Sach s)
        {
            if (string.IsNullOrWhiteSpace(s.TenSach))
                return (false, "Ten sach khong duoc de trong!");

            if (string.IsNullOrWhiteSpace(s.TacGia))
                return (false, "Tac gia khong duoc de trong!");

            if (s.MaTheLoai <= 0)
                return (false, "Vui long chon the loai sach!");

            if (s.SoLuongTong <= 0)
                return (false, "So luong phai lon hon 0!");

            s.SoLuongCon = s.SoLuongTong; // Ban dau so luong con = tong

            int id = _dal.Insert(s);
            return id > 0 ? (true, $"Them sach thanh cong! Ma sach: {id}") : (false, "Them sach that bai!");
        }

        public (bool Success, string Message) Update(Sach s)
        {
            if (string.IsNullOrWhiteSpace(s.TenSach))
                return (false, "Ten sach khong duoc de trong!");

            if (string.IsNullOrWhiteSpace(s.TacGia))
                return (false, "Tac gia khong duoc de trong!");

            if (s.MaTheLoai <= 0)
                return (false, "Vui long chon the loai sach!");

            if (s.SoLuongTong <= 0)
                return (false, "So luong phai lon hon 0!");

            if (s.SoLuongCon < 0)
                return (false, "So luong con lai khong duoc am!");

            if (s.SoLuongCon > s.SoLuongTong)
                return (false, "So luong con lai khong duoc lon hon tong!");

            bool result = _dal.Update(s);
            return result ? (true, "Cap nhat sach thanh cong!") : (false, "Cap nhat sach that bai!");
        }

        public (bool Success, string Message) Delete(int maSach)
        {
            try
            {
                bool result = _dal.Delete(maSach);
                return result ? (true, "Xoa sach thanh cong!") : (false, "Xoa sach that bai!");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE"))
                    return (false, "Khong the xoa sach da co phieu muon!");
                return (false, $"Loi: {ex.Message}");
            }
        }
    }
}
