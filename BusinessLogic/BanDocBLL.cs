using QuanLyThuVien.DataAccess;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.BusinessLogic
{
    public class BanDocBLL
    {
        private readonly BanDocDAL _dal = new BanDocDAL();

        public List<BanDoc> GetAll() => _dal.GetAll();
        public List<BanDoc> Search(string? keyword) => _dal.Search(keyword);
        public BanDoc? GetById(int maBanDoc) => _dal.GetById(maBanDoc);
        public int DemSachDangMuon(int maBanDoc) => _dal.DemSachDangMuon(maBanDoc);

        public (bool Success, string Message) Insert(BanDoc bd)
        {
            if (string.IsNullOrWhiteSpace(bd.HoTen))
                return (false, "Ho ten ban doc khong duoc de trong!");

            int id = _dal.Insert(bd);
            return id > 0 ? (true, $"Them ban doc thanh cong! Ma: {id}") : (false, "Them ban doc that bai!");
        }

        public (bool Success, string Message) Update(BanDoc bd)
        {
            if (string.IsNullOrWhiteSpace(bd.HoTen))
                return (false, "Ho ten ban doc khong duoc de trong!");

            bool result = _dal.Update(bd);
            return result ? (true, "Cap nhat ban doc thanh cong!") : (false, "Cap nhat ban doc that bai!");
        }

        public (bool Success, string Message) Delete(int maBanDoc)
        {
            try
            {
                // Kiem tra ban doc con phieu muon chua tra
                int sachDangMuon = _dal.DemSachDangMuon(maBanDoc);
                if (sachDangMuon > 0)
                    return (false, $"Ban doc dang muon {sachDangMuon} sach. Khong the xoa!");

                bool result = _dal.Delete(maBanDoc);
                return result ? (true, "Xoa ban doc thanh cong!") : (false, "Xoa ban doc that bai!");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE"))
                    return (false, "Khong the xoa ban doc da co phieu muon!");
                return (false, $"Loi: {ex.Message}");
            }
        }
    }
}
