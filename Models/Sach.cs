namespace QuanLyThuVien.Models
{
    public class Sach
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public string TacGia { get; set; } = string.Empty;
        public int MaTheLoai { get; set; }
        public string TenTheLoai { get; set; } = string.Empty;
        public string? NhaXuatBan { get; set; }
        public int? NamXuatBan { get; set; }
        public int SoLuongTong { get; set; } = 1;
        public int SoLuongCon { get; set; } = 1;
        public string? MoTa { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public bool TrangThai { get; set; } = true;

        // Thuoc tinh tinh toan
        public int SoLuongDangMuon => SoLuongTong - SoLuongCon;
        public bool ConSach => SoLuongCon > 0;
    }
}
