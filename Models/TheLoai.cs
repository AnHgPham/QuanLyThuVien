namespace QuanLyThuVien.Models
{
    public class TheLoai
    {
        public int MaTheLoai { get; set; }
        public string TenTheLoai { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
