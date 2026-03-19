using QuanLyThuVien.Models;

namespace QuanLyThuVien.Reports
{
    public static class CsvExporter
    {
        public static void ExportBaoCaoMuonTra(
            string filePath,
            int nam, int thang,
            ThongKeMuon? thongKeThang,
            List<ThongKeMuon> cacThang,
            List<SachMuonNhieu> sachMuonNhieu)
        {
            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // BOM cho Excel doc UTF8
                writer.Write('\uFEFF');

                // === Phan 1: Tong quan thang ===
                writer.WriteLine("BAO CAO MUON/TRA SACH");
                writer.WriteLine($"Thang {thang} / {nam}");
                writer.WriteLine($"Ngay xuat:,{DateTime.Now:dd/MM/yyyy HH:mm}");
                writer.WriteLine();

                writer.WriteLine("CHI TIEU,GIA TRI");

                if (thongKeThang != null)
                {
                    writer.WriteLine($"Tong phieu muon,{thongKeThang.TongPhieuMuon}");
                    writer.WriteLine($"So phieu da tra,{thongKeThang.SoPhieuDaTra}");
                    writer.WriteLine($"So phieu dang muon,{thongKeThang.SoPhieuDangMuon}");
                    writer.WriteLine($"So phieu qua han,{thongKeThang.SoPhieuQuaHan}");
                    writer.WriteLine($"Tong so sach muon,{thongKeThang.TongSoSachMuon}");
                }
                else
                {
                    writer.WriteLine("Khong co du lieu,0");
                }

                writer.WriteLine();

                // === Phan 2: Thong ke cac thang ===
                writer.WriteLine($"THONG KE CAC THANG TRONG NAM {nam}");
                writer.WriteLine("Thang,Tong phieu,Da tra,Dang muon,Qua han");

                foreach (var t in cacThang)
                {
                    writer.WriteLine($"Thang {t.Thang},{t.TongPhieuMuon},{t.SoPhieuDaTra},{t.SoPhieuDangMuon},{t.SoPhieuQuaHan}");
                }

                writer.WriteLine();

                // === Phan 3: Sach muon nhieu ===
                writer.WriteLine($"SACH MUON NHIEU NHAT - THANG {thang}/{nam}");
                writer.WriteLine("STT,Ten sach,Tac gia,The loai,So lan muon");

                int stt = 1;
                foreach (var s in sachMuonNhieu)
                {
                    // Escape ten sach co dau phay
                    string tenSach = s.TenSach.Contains(',') ? $"\"{s.TenSach}\"" : s.TenSach;
                    string tacGia = s.TacGia.Contains(',') ? $"\"{s.TacGia}\"" : s.TacGia;
                    writer.WriteLine($"{stt++},{tenSach},{tacGia},{s.TenTheLoai},{s.SoLanMuon}");
                }
            }
        }
    }
}
