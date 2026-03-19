using QuanLyThuVien.DataAccess;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.BusinessLogic
{
    public class TheLoaiBLL
    {
        private readonly TheLoaiDAL _dal = new TheLoaiDAL();

        public List<TheLoai> GetAll() => _dal.GetAll();
        public TheLoai? GetById(int maTheLoai) => _dal.GetById(maTheLoai);

        public (bool Success, string Message) Insert(TheLoai tl)
        {
            if (string.IsNullOrWhiteSpace(tl.TenTheLoai))
                return (false, "Ten the loai khong duoc de trong!");

            int id = _dal.Insert(tl);
            return id > 0 ? (true, $"Them the loai thanh cong! Ma: {id}") : (false, "Them the loai that bai!");
        }

        public (bool Success, string Message) Update(TheLoai tl)
        {
            if (string.IsNullOrWhiteSpace(tl.TenTheLoai))
                return (false, "Ten the loai khong duoc de trong!");

            bool result = _dal.Update(tl);
            return result ? (true, "Cap nhat the loai thanh cong!") : (false, "Cap nhat the loai that bai!");
        }

        public (bool Success, string Message) Delete(int maTheLoai)
        {
            try
            {
                bool result = _dal.Delete(maTheLoai);
                return result ? (true, "Xoa the loai thanh cong!") : (false, "Xoa the loai that bai!");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE"))
                    return (false, "Khong the xoa the loai da co sach!");
                return (false, $"Loi: {ex.Message}");
            }
        }
    }
}
