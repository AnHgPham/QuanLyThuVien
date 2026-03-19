# Ung dung Quan Ly Thu Vien (Library Manager)

## Mo ta
Ung dung quan ly thu vien offline, ho tro quan ly sach, ban doc, muon-tra sach tren may ca nhan.
Su dung C# Windows Forms + SQL Server theo kien truc MVC + DAO (3 lop).

## Chuc nang chinh

### 1. Quan ly Sach
- Them / Sua / Xoa / Tim kiem sach
- Loc theo the loai, tac gia
- Kiem tra so ban con lai

### 2. Quan ly Ban doc
- Them / Sua / Xoa ban doc
- Tim kiem theo ten, SDT, email

### 3. Muon / Tra sach
- Lap phieu muon sach (chon ban doc + chon sach)
- Rang buoc: 1 ban doc muon toi da 5 sach cung luc
- Qua han → tu dong danh dau
- Tra sach → cap nhat so luong tu dong

### 4. Bao cao & Thong ke
- Thong ke muon/tra theo thang
- Sach duoc muon nhieu nhat
- Xuat bao cao muon theo thang (CSV)

## Cong nghe su dung
- **Ngon ngu**: C# (.NET 6)
- **Giao dien**: Windows Forms
- **CSDL**: SQL Server (Express)
- **Kien truc**: MVC + DAO (3 lop: Models / DataAccess / BusinessLogic)
- **Bao cao**: CSV export

## Cau truc du an

```
QuanLyThuVien/
├── Database/
│   └── CreateDatabase.sql          # Script tao DB + du lieu mau
├── Models/
│   ├── TheLoai.cs                  # Model The loai
│   ├── Sach.cs                     # Model Sach
│   ├── BanDoc.cs                   # Model Ban doc
│   └── PhieuMuon.cs                # Model Phieu muon + Chi tiet + Thong ke
├── DataAccess/
│   ├── DatabaseHelper.cs           # Lop tien ich ket noi DB
│   ├── TheLoaiDAL.cs               # DAL The loai
│   ├── SachDAL.cs                  # DAL Sach
│   ├── BanDocDAL.cs                # DAL Ban doc
│   ├── PhieuMuonDAL.cs             # DAL Phieu muon (co transaction)
│   └── ThongKeDAL.cs               # DAL Thong ke
├── BusinessLogic/
│   ├── TheLoaiBLL.cs               # BLL The loai
│   ├── SachBLL.cs                  # BLL Sach (validate)
│   ├── BanDocBLL.cs                # BLL Ban doc
│   ├── PhieuMuonBLL.cs             # BLL Phieu muon (validate, rang buoc)
│   └── ThongKeBLL.cs               # BLL Thong ke
├── Forms/
│   ├── FormMain.cs                 # Giao dien chinh (Dashboard + Menu)
│   ├── FormSach.cs                 # Form quan ly sach
│   ├── FormBanDoc.cs               # Form quan ly ban doc
│   ├── FormPhieuMuon.cs            # Form muon/tra sach
│   └── FormThongKe.cs              # Form thong ke + xuat CSV
├── Reports/
│   └── CsvExporter.cs              # Xuat bao cao CSV
├── App.config                      # Cau hinh ket noi
├── Program.cs                      # Entry point
├── QuanLyThuVien.csproj            # Project file
├── QuanLyThuVien.sln               # Solution file
├── HUONG_DAN_CAI_DAT.txt           # Huong dan cai dat
└── README.md                       # Tai lieu du an
```

## Huong dan cai dat nhanh

1. Cai Visual Studio 2022 + SQL Server Express
2. Sua `App.config` (neu can)
3. Chay: `sqlcmd -S localhost\SQLEXPRESS01 -E -i "Database\CreateDatabase.sql"`
4. Chay: `dotnet build && dotnet run`

Xem chi tiet tai file `HUONG_DAN_CAI_DAT.txt`.
