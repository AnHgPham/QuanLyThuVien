namespace QuanLyThuVien.Models
{
    public class PhieuMuon
    {
        public int MaPhieuMuon { get; set; }
        public int MaBanDoc { get; set; }
        public string TenBanDoc { get; set; } = string.Empty;
        public DateTime NgayMuon { get; set; }
        public DateTime NgayHenTra { get; set; }
        public DateTime? NgayTraThucTe { get; set; }
        public string TrangThai { get; set; } = "Dang muon";
        public string? GhiChu { get; set; }

        // Thuoc tinh tinh toan
        public bool IsQuaHan => TrangThai == "Dang muon" && DateTime.Now > NgayHenTra;
        public int SoNgayMuon => (int)(NgayHenTra - NgayMuon).TotalDays;
    }

    public class ChiTietMuon
    {
        public int MaChiTiet { get; set; }
        public int MaPhieuMuon { get; set; }
        public int MaSach { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public bool DaTra { get; set; }
    }

    // Model cho thong ke
    public class ThongKeMuon
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public int TongPhieuMuon { get; set; }
        public int SoPhieuDaTra { get; set; }
        public int SoPhieuDangMuon { get; set; }
        public int SoPhieuQuaHan { get; set; }
        public int TongSoSachMuon { get; set; }
    }

    public class SachMuonNhieu
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public string TacGia { get; set; } = string.Empty;
        public string TenTheLoai { get; set; } = string.Empty;
        public int SoLanMuon { get; set; }
    }
}
