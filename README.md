# HE THONG QUAN LY THU VIEN VA MUON TRA SACH (UNETI)

## 1. THONG TIN DU AN
- Ten du an: He thong Quan ly Thu vien va Muon tra Sach
- Framework: ASP.NET Core 10.0 MVC
- ORM: Entity Framework Core 10.0.9 (SQL Server)
- Kien truc: 3-Layer Architecture (Controller - Service - Repository)
- Lop hoc: DHTI17A4HN
- Truong: Dai hoc Kinh te - Ky thuat Cong nghiep (UNETI)

## 2. DANH SACH THANH VIEN VA PHAN CONG MODULE

| STT | Ho va ten | Ma sinh vien | Vai tro / Vi tri | Module phu trach | Nhanh Git (Branch) |
|---|---|---|---|---|---|
| 1 | Nguyen The Dong | 23103100187 | Nhom truong (SV1) | Module 1: Tai khoan, phan quyen, the loai | feature/m1-taikhoan |
| 2 | Tran Gia Nghia | 23103100228 | Thanh vien (SV2) | Module 2: Sach, nha xuat ban, tim kiem | feature/m2-sach |
| 3 | Dang Thi Phuong | 23103100182 | Thanh vien (SV3) | Module 3: Ban doc, ho so ca nhan | feature/m3-bandoc |
| 4 | Trinh Thi Thanh Tam | 23103100193 | Thanh vien (SV4) | Module 4: Duyet phieu, tra sach, tinh phat | feature/m4-xulyphieu |
| 5 | Phan Vu Thanh Thu | 23103100316 | Thanh vien (SV5) | Module 5: Chi tiet muon, qua han, Dashboard | feature/m5-thongke |

## 3. CAU TRUC KIEN TRUC HE THONG
Du an duoc to chuc theo kien truc 3 lop phan chia ro rang:
- Presentation Layer (Controllers / Views): Tiep nhan yeu cau tu nguoi dung va hien thi giao dien.
- Business Logic Layer (Services): Xu ly logic nghiep vu trung gian.
- Data Access Layer (Repositories / Data): Thao tac voi co so du lieu thong qua EF Core.

## 4. HUONG DAN CHAY PROJECT
1. Yeu cau moi truong: .NET 10.0 SDK, SQL Server Express / LocalDB, Visual Studio 2022 hoac VS Code.
2. Cap nhat chuoi ket noi CSDL trong file `QuanLyThuVien_UNETI01_DHTI17A4HN/appsettings.json`.
3. Chay Migration cap nhat CSDL:
   dotnet ef database update --project QuanLyThuVien_UNETI01_DHTI17A4HN
4. Khoi chay ung dung:
   dotnet run --project QuanLyThuVien_UNETI01_DHTI17A4HN
5. Chay va tu dong mo Microsoft Edge (PowerShell tai thu muc goc repo):
   .\run-edge.ps1