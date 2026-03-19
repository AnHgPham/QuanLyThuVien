-- ============================================
-- Ung dung Quan ly Thu vien
-- Script tao Database SQL Server
-- ============================================

-- Tao Database
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyThuVienDB')
BEGIN
    ALTER DATABASE QuanLyThuVienDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyThuVienDB;
END
GO

CREATE DATABASE QuanLyThuVienDB;
GO

USE QuanLyThuVienDB;
GO

-- ============================================
-- Bang The loai sach (TheLoai)
-- ============================================
CREATE TABLE TheLoai (
    MaTheLoai   INT IDENTITY(1,1) PRIMARY KEY,
    TenTheLoai  NVARCHAR(100) NOT NULL,
    MoTa        NVARCHAR(255) NULL,
    NgayTao     DATETIME DEFAULT GETDATE()
);
GO

-- ============================================
-- Bang Sach (Sach)
-- ============================================
CREATE TABLE Sach (
    MaSach          INT IDENTITY(1,1) PRIMARY KEY,
    TenSach         NVARCHAR(300) NOT NULL,
    TacGia          NVARCHAR(200) NOT NULL,
    MaTheLoai       INT NOT NULL,
    NhaXuatBan      NVARCHAR(200) NULL,
    NamXuatBan      INT NULL,
    SoLuongTong     INT NOT NULL DEFAULT 1,         -- Tong so ban
    SoLuongCon      INT NOT NULL DEFAULT 1,         -- So ban con lai (chua cho muon)
    MoTa            NVARCHAR(1000) NULL,
    NgayTao         DATETIME DEFAULT GETDATE(),
    NgayCapNhat     DATETIME DEFAULT GETDATE(),
    TrangThai       BIT DEFAULT 1,                  -- 1: Con hoat dong, 0: Ngung
    CONSTRAINT FK_Sach_TheLoai FOREIGN KEY (MaTheLoai) REFERENCES TheLoai(MaTheLoai)
);
GO

-- ============================================
-- Bang Ban doc (BanDoc)
-- ============================================
CREATE TABLE BanDoc (
    MaBanDoc        INT IDENTITY(1,1) PRIMARY KEY,
    HoTen           NVARCHAR(200) NOT NULL,
    NgaySinh        DATE NULL,
    GioiTinh        NVARCHAR(10) NULL,              -- Nam / Nu
    SoDienThoai     NVARCHAR(20) NULL,
    Email           NVARCHAR(200) NULL,
    DiaChi          NVARCHAR(500) NULL,
    NgayLapThe      DATETIME DEFAULT GETDATE(),
    TrangThai       BIT DEFAULT 1                   -- 1: Hoat dong, 0: Khoa
);
GO

-- ============================================
-- Bang Phieu muon (PhieuMuon)
-- ============================================
CREATE TABLE PhieuMuon (
    MaPhieuMuon     INT IDENTITY(1,1) PRIMARY KEY,
    MaBanDoc        INT NOT NULL,
    NgayMuon        DATETIME NOT NULL DEFAULT GETDATE(),
    NgayHenTra      DATETIME NOT NULL,              -- Hen tra (VD: 14 ngay sau)
    NgayTraThucTe   DATETIME NULL,                  -- NULL = chua tra
    TrangThai       NVARCHAR(20) DEFAULT N'Dang muon', -- Dang muon / Da tra / Qua han
    GhiChu          NVARCHAR(500) NULL,
    CONSTRAINT FK_PhieuMuon_BanDoc FOREIGN KEY (MaBanDoc) REFERENCES BanDoc(MaBanDoc)
);
GO

-- ============================================
-- Bang Chi tiet muon (ChiTietMuon)
-- ============================================
CREATE TABLE ChiTietMuon (
    MaChiTiet       INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuMuon     INT NOT NULL,
    MaSach          INT NOT NULL,
    TenSach         NVARCHAR(300) NOT NULL,         -- Luu ten sach tai thoi diem muon
    DaTra           BIT DEFAULT 0,                  -- 0: chua tra, 1: da tra
    CONSTRAINT FK_ChiTiet_PhieuMuon FOREIGN KEY (MaPhieuMuon) REFERENCES PhieuMuon(MaPhieuMuon) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTiet_Sach FOREIGN KEY (MaSach) REFERENCES Sach(MaSach)
);
GO

-- ============================================
-- Stored Procedures
-- ============================================

-- SP: Thong ke muon/tra theo thang
CREATE PROCEDURE sp_ThongKe_MuonTra_Thang
    @Nam INT,
    @Thang INT
AS
BEGIN
    SELECT 
        @Nam AS Nam,
        @Thang AS Thang,
        COUNT(DISTINCT pm.MaPhieuMuon) AS TongPhieuMuon,
        ISNULL(SUM(CASE WHEN pm.TrangThai = N'Da tra' THEN 1 ELSE 0 END), 0) AS SoPhieuDaTra,
        ISNULL(SUM(CASE WHEN pm.TrangThai = N'Dang muon' THEN 1 ELSE 0 END), 0) AS SoPhieuDangMuon,
        ISNULL(SUM(CASE WHEN pm.TrangThai = N'Qua han' THEN 1 ELSE 0 END), 0) AS SoPhieuQuaHan,
        (SELECT COUNT(*) FROM ChiTietMuon ct WHERE ct.MaPhieuMuon IN 
            (SELECT MaPhieuMuon FROM PhieuMuon WHERE YEAR(NgayMuon) = @Nam AND MONTH(NgayMuon) = @Thang)
        ) AS TongSoSachMuon
    FROM PhieuMuon pm
    WHERE YEAR(pm.NgayMuon) = @Nam AND MONTH(pm.NgayMuon) = @Thang
END
GO

-- SP: Thong ke cac thang trong nam
CREATE PROCEDURE sp_ThongKe_MuonTra_CacThang
    @Nam INT
AS
BEGIN
    SELECT 
        MONTH(pm.NgayMuon) AS Thang,
        COUNT(DISTINCT pm.MaPhieuMuon) AS TongPhieuMuon,
        ISNULL(SUM(CASE WHEN pm.TrangThai = N'Da tra' THEN 1 ELSE 0 END), 0) AS SoPhieuDaTra,
        ISNULL(SUM(CASE WHEN pm.TrangThai = N'Dang muon' THEN 1 ELSE 0 END), 0) AS SoPhieuDangMuon,
        ISNULL(SUM(CASE WHEN pm.TrangThai = N'Qua han' THEN 1 ELSE 0 END), 0) AS SoPhieuQuaHan
    FROM PhieuMuon pm
    WHERE YEAR(pm.NgayMuon) = @Nam
    GROUP BY MONTH(pm.NgayMuon)
    ORDER BY Thang
END
GO

-- SP: Sach duoc muon nhieu nhat
CREATE PROCEDURE sp_ThongKe_SachMuonNhieu
    @Nam INT,
    @Thang INT,
    @Top INT = 10
AS
BEGIN
    SELECT TOP(@Top)
        s.MaSach,
        s.TenSach,
        s.TacGia,
        tl.TenTheLoai,
        COUNT(ct.MaChiTiet) AS SoLanMuon
    FROM ChiTietMuon ct
    INNER JOIN Sach s ON ct.MaSach = s.MaSach
    INNER JOIN TheLoai tl ON s.MaTheLoai = tl.MaTheLoai
    INNER JOIN PhieuMuon pm ON ct.MaPhieuMuon = pm.MaPhieuMuon
    WHERE YEAR(pm.NgayMuon) = @Nam AND MONTH(pm.NgayMuon) = @Thang
    GROUP BY s.MaSach, s.TenSach, s.TacGia, tl.TenTheLoai
    ORDER BY SoLanMuon DESC
END
GO

-- SP: Tong quan he thong (cho Dashboard)
CREATE PROCEDURE sp_TongQuan
AS
BEGIN
    SELECT
        (SELECT COUNT(*) FROM Sach WHERE TrangThai = 1) AS TongSach,
        (SELECT COUNT(*) FROM BanDoc WHERE TrangThai = 1) AS TongBanDoc,
        (SELECT COUNT(*) FROM PhieuMuon WHERE TrangThai = N'Dang muon') AS DangMuon,
        (SELECT COUNT(*) FROM PhieuMuon WHERE TrangThai = N'Qua han') AS QuaHan,
        (SELECT COUNT(*) FROM PhieuMuon WHERE YEAR(NgayMuon) = YEAR(GETDATE()) AND MONTH(NgayMuon) = MONTH(GETDATE())) AS PhieuThangNay,
        (SELECT COUNT(*) FROM PhieuMuon WHERE TrangThai = N'Da tra' AND YEAR(NgayTraThucTe) = YEAR(GETDATE()) AND MONTH(NgayTraThucTe) = MONTH(GETDATE())) AS TraThangNay
END
GO

-- ============================================
-- Du lieu mau
-- ============================================

-- The loai
INSERT INTO TheLoai (TenTheLoai, MoTa) VALUES 
(N'Van hoc', N'Tieu thuyet, truyen ngan, tho'),
(N'Khoa hoc', N'Sach khoa hoc tu nhien, cong nghe'),
(N'Lich su', N'Sach lich su Viet Nam va the gioi'),
(N'Kinh te', N'Sach kinh te, tai chinh, quan tri'),
(N'Tin hoc', N'Sach lap trinh, cong nghe thong tin'),
(N'Thieu nhi', N'Sach danh cho thieu nhi'),
(N'Tam ly', N'Sach tam ly, ky nang song'),
(N'Ngoai ngu', N'Sach hoc tieng Anh, Nhat, Han...');
GO

-- Sach
INSERT INTO Sach (TenSach, TacGia, MaTheLoai, NhaXuatBan, NamXuatBan, SoLuongTong, SoLuongCon, MoTa) VALUES
(N'Truyen Kieu', N'Nguyen Du', 1, N'NXB Van hoc', 2020, 5, 3, N'Kiet tac van hoc Viet Nam'),
(N'Tat den', N'Ngo Tat To', 1, N'NXB Giao duc', 2019, 4, 2, N'Tieu thuyet hien thuc phe phan'),
(N'Chi Pheo', N'Nam Cao', 1, N'NXB Van hoc', 2021, 3, 2, N'Truyen ngan kinh dien'),
(N'Vat ly dai cuong', N'Luong Duyen Binh', 2, N'NXB Giao duc', 2018, 3, 2, N'Giao trinh vat ly co ban'),
(N'Hoa hoc vo co', N'Hoang Nham', 2, N'NXB Giao duc', 2017, 2, 1, N'Giao trinh hoa hoc'),
(N'Lich su Viet Nam', N'Tran Trong Kim', 3, N'NXB Tre', 2020, 4, 3, N'Viet Nam su luoc'),
(N'Dai Viet su ky toan thu', N'Ngo Si Lien', 3, N'NXB Khoa hoc', 2019, 2, 1, N'Su ky chinh thong'),
(N'Kinh te hoc vi mo', N'N. Gregory Mankiw', 4, N'NXB Lao dong', 2021, 3, 2, N'Giao trinh kinh te'),
(N'Dac nhan tam', N'Dale Carnegie', 4, N'NXB Tre', 2022, 5, 3, N'Sach ky nang giao tiep'),
(N'Lap trinh C#', N'Pham Huu Khang', 5, N'NXB Lao dong', 2021, 4, 3, N'Giao trinh C# co ban den nang cao'),
(N'SQL Server tu co ban', N'Duong Thien An', 5, N'NXB Tre', 2020, 3, 2, N'Hoc SQL Server'),
(N'De men phieu luu ky', N'To Hoai', 6, N'NXB Kim Dong', 2022, 5, 4, N'Truyen thieu nhi kinh dien'),
(N'Cho toi xin mot ve di tuoi tho', N'Nguyen Nhat Anh', 6, N'NXB Tre', 2023, 4, 3, N'Truyen thieu nhi hay'),
(N'Dac nhan tam', N'Dale Carnegie', 7, N'NXB Tong hop', 2022, 3, 2, N'Nghe thuat doi nhan xu the'),
(N'7 thoi quen hieu qua', N'Stephen Covey', 7, N'NXB Tre', 2021, 3, 2, N'Sach phat trien ban than'),
(N'English Grammar in Use', N'Raymond Murphy', 8, N'Cambridge', 2020, 4, 3, N'Ngu phap tieng Anh'),
(N'TOEIC 990', N'Nhieu tac gia', 8, N'NXB Tong hop', 2022, 3, 2, N'Luyen thi TOEIC'),
(N'Nhat ky trong tu', N'Ho Chi Minh', 1, N'NXB Chinh tri', 2020, 3, 2, N'Tho Ho Chi Minh');
GO

-- Ban doc
INSERT INTO BanDoc (HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi) VALUES
(N'Nguyen Van An', '2000-05-15', N'Nam', '0901234567', 'an.nv@email.com', N'123 Le Loi, Q1, TP.HCM'),
(N'Tran Thi Binh', '2001-08-20', N'Nu', '0912345678', 'binh.tt@email.com', N'456 Nguyen Trai, Q5, TP.HCM'),
(N'Le Van Cuong', '1999-12-01', N'Nam', '0923456789', 'cuong.lv@email.com', N'789 Vo Van Tan, Q3, TP.HCM'),
(N'Pham Thi Dung', '2002-03-25', N'Nu', '0934567890', 'dung.pt@email.com', N'012 Hai Ba Trung, Q1, TP.HCM'),
(N'Hoang Van Em', '2000-11-10', N'Nam', '0945678901', 'em.hv@email.com', N'345 Ly Tu Trong, Q1, TP.HCM'),
(N'Vo Thi Phuong', '2001-07-08', N'Nu', '0956789012', 'phuong.vt@email.com', N'678 CMT8, Q3, TP.HCM'),
(N'Bui Quang Gia', '1998-09-30', N'Nam', '0967890123', 'gia.bq@email.com', N'901 Dien Bien Phu, Q10, TP.HCM'),
(N'Do Thi Huong', '2003-01-14', N'Nu', '0978901234', 'huong.dt@email.com', N'234 Nguyen Dinh Chieu, Q3, TP.HCM');
GO

-- Phieu muon mau
INSERT INTO PhieuMuon (MaBanDoc, NgayMuon, NgayHenTra, NgayTraThucTe, TrangThai, GhiChu) VALUES
(1, '2026-01-05', '2026-01-19', '2026-01-18', N'Da tra', N'Tra dung han'),
(2, '2026-01-10', '2026-01-24', '2026-01-25', N'Da tra', N'Tra tre 1 ngay'),
(3, '2026-01-15', '2026-01-29', '2026-01-28', N'Da tra', N''),
(1, '2026-02-01', '2026-02-15', '2026-02-14', N'Da tra', N''),
(4, '2026-02-10', '2026-02-24', '2026-02-20', N'Da tra', N'Tra som'),
(5, '2026-02-14', '2026-02-28', NULL, N'Qua han', N'Chua tra - qua han'),
(2, '2026-03-01', '2026-03-15', NULL, N'Dang muon', N''),
(6, '2026-03-02', '2026-03-16', NULL, N'Dang muon', N''),
(3, '2026-03-03', '2026-03-17', NULL, N'Dang muon', N''),
(7, '2026-03-04', '2026-03-18', NULL, N'Dang muon', N'');
GO

-- Chi tiet muon
INSERT INTO ChiTietMuon (MaPhieuMuon, MaSach, TenSach, DaTra) VALUES
-- Phieu 1 (Da tra)
(1, 1, N'Truyen Kieu', 1),
(1, 9, N'Dac nhan tam', 1),
-- Phieu 2 (Da tra)
(2, 10, N'Lap trinh C#', 1),
(2, 11, N'SQL Server tu co ban', 1),
(2, 3, N'Chi Pheo', 1),
-- Phieu 3 (Da tra)
(3, 6, N'Lich su Viet Nam', 1),
(3, 12, N'De men phieu luu ky', 1),
-- Phieu 4 (Da tra)
(4, 8, N'Kinh te hoc vi mo', 1),
(4, 15, N'7 thoi quen hieu qua', 1),
-- Phieu 5 (Da tra)
(5, 16, N'English Grammar in Use', 1),
(5, 13, N'Cho toi xin mot ve di tuoi tho', 1),
-- Phieu 6 (Qua han - chua tra)
(6, 4, N'Vat ly dai cuong', 0),
(6, 5, N'Hoa hoc vo co', 0),
-- Phieu 7 (Dang muon)
(7, 1, N'Truyen Kieu', 0),
(7, 2, N'Tat den', 0),
-- Phieu 8 (Dang muon)
(8, 14, N'Dac nhan tam', 0),
(8, 17, N'TOEIC 990', 0),
-- Phieu 9 (Dang muon)
(9, 7, N'Dai Viet su ky toan thu', 0),
-- Phieu 10 (Dang muon)
(10, 18, N'Nhat ky trong tu', 0),
(10, 9, N'Dac nhan tam', 0);
GO

-- Cap nhat so luong con lai cho sach dang duoc muon (chua tra)
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 4;   -- Vat ly dai cuong (Phieu 6)
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 5;   -- Hoa hoc vo co (Phieu 6)
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 1;   -- Truyen Kieu (Phieu 7)
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 2;   -- Tat den (Phieu 7)
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 14;  -- Dac nhan tam (Phieu 8)
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 17;  -- TOEIC 990 (Phieu 8)
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 7;   -- Dai Viet su ky (Phieu 9)
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 18;  -- Nhat ky trong tu (Phieu 10)
UPDATE Sach SET SoLuongCon = SoLuongCon - 1 WHERE MaSach = 9;   -- Dac nhan tam (Phieu 10)
GO

PRINT N'=== Tao Database QuanLyThuVienDB thanh cong! ===';
GO
