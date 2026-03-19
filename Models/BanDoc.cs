namespace QuanLyThuVien.Models
{
    public class BanDoc
    {
        public int MaBanDoc { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public DateTime NgayLapThe { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}
