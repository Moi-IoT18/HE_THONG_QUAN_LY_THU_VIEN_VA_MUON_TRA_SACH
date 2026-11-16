# Mô tả Pipeline Hệ thống - Quản lý Thư viện UNETI

Hệ thống Quản lý Thư viện và Mượn trả sách (Đề tài 14 - Nhóm 01) được xây dựng trên nền tảng **ASP.NET Core 10 MVC**, **Entity Framework Core 10** và **SQL Server**. Tài liệu này mô tả chi tiết các pipeline (đường ống xử lý) chính trong toàn bộ vòng đời của hệ thống.

---

## 1. Tổng quan Kiến trúc Pipeline

```mermaid
flowchart TD
    subgraph Client["Client (Trình duyệt)"]
        UI["Giao diện Razor View / Bootstrap / AJAX"]
    end

    subgraph CI_CD["Pipeline CI/CD"]
        GitPush["Git Push / Pull Request"] --> GH_Actions["GitHub Actions (build.yml)"]
        GH_Actions --> DotnetRestore["dotnet restore"]
        DotnetRestore --> DotnetBuild["dotnet build"]
    end

    subgraph WebPipeline["Pipeline Xử lý Request (Middleware & Filters)"]
        Req["HTTP Request"] --> Https["UseHttpsRedirection & UseHsts"]
        Https --> Static["MapStaticAssets (CSS, JS, Uploads)"]
        Static --> Routing["UseRouting"]
        Routing --> SessionMid["UseSession (Cookie HttpOnly, SameSite)"]
        SessionMid --> AuthFilter["SessionAuthorizeAttribute (Re-validate DB)"]
        AuthFilter --> AntiForgery["[ValidateAntiForgeryToken]"]
        AntiForgery --> ControllerAction["Controller Action Invocation"]
    end

    subgraph BusinessPipeline["Pipeline Xử lý Nghiệp vụ Mượn - Trả"]
        Gio["Giỏ mượn sách"] --> TaoPhieu["Tạo phiếu Chờ duyệt (Serializable Tx)"]
        TaoPhieu --> DuyetPhieu["Thủ thư Duyệt phiếu (Cập nhật Hạn trả & Trừ kho)"]
        TaoPhieu --> TuChoi["Thủ thư Từ chối / Độc giả Hủy"]
        DuyetPhieu --> QuetQuaHan["Dịch vụ Quét Quá hạn"]
        QuetQuaHan --> TraSach["Nhận trả sách (Tính phạt quá hạn, Nhập kho)"]
        TraSach --> ThuPhat["Thu tiền phạt (Tiền mặt / Chuyển khoản)"]
    end

    subgraph DataPipeline["Pipeline Dữ liệu & Lưu trữ"]
        ControllerAction --> ServiceLayer["Service Layer (Nghiệp vụ)"]
        ServiceLayer --> RepoLayer["Repository Layer (EF Core LINQ)"]
        RepoLayer --> DbContext["AppDbContext (Unit of Work)"]
        DbContext --> SqlServer[("Cơ sở dữ liệu SQL Server")]
    end

    Client --> WebPipeline
    WebPipeline --> BusinessPipeline
    BusinessPipeline --> DataPipeline
```

---

## 2. Pipeline Tích hợp và Triển khai liên tục (CI/CD Pipeline)

Pipeline tự động hóa được thiết lập thông qua GitHub Actions tại tệp [`.github/workflows/build.yml`](file:///d:/TH_LTNET/HE_THONG_QUAN_LY_THU_VIEN_VA_MUON_TRA_SACH/.github/workflows/build.yml):

```mermaid
flowchart LR
    A["Push / PR vào main/develop"] --> B["Khởi tạo Ubuntu runner"]
    B --> C["Cài đặt .NET 10 SDK"]
    C --> D["Restore NuGet Dependencies"]
    D --> E["Build Project Web & Tests"]
    E --> F["Chạy Unit Tests tự động"]
    F -->|Pass| G["Cho phép Merge PR"]
    F -->|Fail| H["Chặn Merge & Báo lỗi"]
```

### Các bước thực thi:
1. **Trigger**: Kích hoạt tự động khi có commit hoặc Pull Request vào nhánh `main` hoặc `develop`.
2. **Environment Setup**: Thiết lập môi trường Linux runner với .NET 10.
3. **Dependency Restoration**: Phục hồi các package NuGet (`Microsoft.EntityFrameworkCore`, `ClosedXML`...).
4. **Build Verification**: Biên dịch toàn bộ dự án (`dotnet build`) đảm bảo không có lỗi cú pháp hay thiếu thư viện.

---

## 3. Pipeline Xử lý HTTP Request (ASP.NET Core Middleware & Filter Pipeline)

Mỗi yêu cầu từ trình duyệt được chuyển tiếp qua chuỗi Middleware được cấu hình trong [`Program.cs`](file:///d:/TH_LTNET/HE_THONG_QUAN_LY_THU_VIEN_VA_MUON_TRA_SACH/QuanLyThuVien_UNETI01_DHTI17A4HN/Program.cs):

```mermaid
sequenceDiagram
    autonumber
    actor User as Người dùng / Trình duyệt
    participant MW as Middleware Pipeline
    participant Filter as SessionAuthorize Filter
    participant Ctrl as Controller
    participant DB as SQL Server

    User->>MW: Gửi HTTP Request (kèm Cookie .AspNetCore.Session)
    MW->>MW: UseHttpsRedirection (Bắt buộc HTTPS)
    MW->>MW: UseSession (Đọc/giải mã Session từ MemoryCache)
    MW->>Filter: Kiểm tra quyền truy cập Endpoint
    alt Chưa đăng nhập
        Filter-->>User: 302 Redirect -> /Auth/Login
    else Đã đăng nhập
        Filter->>DB: Re-validate tài khoản (TrangThai == true)
        alt Tài khoản bị khóa / không tồn tại
            Filter->>MW: Xóa Session
            Filter-->>User: 302 Redirect -> /Auth/Login
        else Vai trò không khớp quyền endpoint
            Filter-->>User: 302 Redirect -> /Auth/TuChoiTruyCap
        else Hợp lệ
            Filter->>Ctrl: Cho phép gọi Action Method
            Ctrl->>DB: Thực thi nghiệp vụ & Đọc/Ghi dữ liệu
            Ctrl-->>User: Trả về HTML View / JSON / File kết quả
        end
    end
```

### Chi tiết các lớp bảo vệ:
1. **Session Cookie Hardening**:
   - `Cookie.HttpOnly = true`: Ngăn chặn mã JavaScript độc hại đánh cắp Cookie phiên thông qua tấn công XSS.
   - `Cookie.SameSite = SameSiteMode.Lax`: Ngăn ngừa tấn công Cross-Site Request Forgery (CSRF) đối với session.
   - `IdleTimeout = TimeSpan.FromMinutes(30)`: Tự động kết thúc phiên làm việc sau 30 phút không hoạt động.
2. **Dynamic DB Re-validation (`SessionAuthorizeAttribute`)**:
   - Khi có request gửi đến action được bảo vệ, bộ lọc không chỉ tin cậy vào Session trong RAM mà còn chủ động truy vấn cơ sở dữ liệu để kiểm tra trạng thái kích hoạt của tài khoản.
   - Ngăn chặn triệt để lỗ hổng: tài khoản đã bị Quản trị viên vô hiệu hóa nhưng vẫn giữ phiên làm việc cũ.

---

## 4. Pipeline Nghiệp vụ Mượn - Trả sách (Business Workflow Pipeline)

Quy trình quản lý phiếu mượn tuân theo sơ đồ máy trạng thái nghiêm ngặt:

```mermaid
stateDiagram-v2
    [*] --> ChoDuyet: Độc giả tạo phiếu từ Giỏ mượn (TaoPhieuAsync)
    ChoDuyet --> DaHuy: Độc giả tự hủy phiếu (HuyAsync)
    ChoDuyet --> DaHuy: Thủ thư Từ chối kèm lý do (TuChoiAsync)
    ChoDuyet --> DangMuon: Thủ thư Duyệt phiếu (DuyetAsync)
    
    note right of DangMuon
        Tại thời điểm DuyetAsync:
        - Cập nhật NgayMuon = DateTime.Now
        - Tính HanTra = NgayMuon + 14 ngày
        - Trừ tồn kho SoLuongCon trong Serializable Tx
    end note

    DangMuon --> QuaHan: Quét tự động mỗi khi HanTra < Hôm nay (QuetQuaHanAsync)
    DangMuon --> DaTra: Nhận trả sách đúng hạn (TraSachAsync)
    QuaHan --> DaTra: Nhận trả sách quá hạn (TraSachAsync)

    note right of DaTra
        Tại thời điểm TraSachAsync:
        - TinhTrangTra: Bình thường -> Cộng lại SoLuongCon
        - TinhTrangTra: Hỏng/Mất -> Giảm tổng SoLuong kho
        - Tính tiền phạt = Số ngày trễ × 5.000đ × Số lượng
    end note

    DaTra --> DaNopPhat: Xác nhận nộp tiền phạt nếu có phạt (XacNhanNopTienPhatAsync)
    DaTra --> [*]: Không phát sinh phạt
    DaNopPhat --> [*]
    DaHuy --> [*]
```

### Các bước bảo đảm an toàn trong giao dịch mượn trả:
1. **Chống Over-borrowing & Race condition (`TaoPhieuAsync`)**:
   - Quá trình kiểm tra điều kiện mượn (độc giả không quá 5 cuốn, không có sách quá hạn, sách còn phục vụ) và lưu phiếu được bọc trong giao dịch `IsolationLevel.Serializable`.
   - Ngăn ngừa hành vi gửi request song song để mượn vượt số lượng cho phép.
2. **Khắc phục lỗi tính hạn trả (`DuyetAsync`)**:
   - Hạn trả 14 ngày được tính lại chính xác từ **ngày thủ thư bàn giao sách thực tế**, không tính từ ngày độc giả gửi yêu cầu online.
   - Ngăn chặn phạt oan cho độc giả do độ trễ duyệt phiếu của thư viện.
3. **Toàn vẹn tồn kho (`TraSachAsync`)**:
   - Sách bình thường: Cộng hoàn trả vào `SoLuongCon`.
   - Sách hỏng/mất: Giảm tổng số lượng `SoLuong` của đầu sách, giữ nguyên `SoLuongCon`.

---

## 5. Pipeline Xử lý Dữ liệu và Concurrency (Data & Concurrency Pipeline)

Hệ thống áp dụng mô hình **Repository - Service** phân tách độc lập:

```text
[HTTP Request]
       │
       ▼
[Controllers]
       │
       ▼
[Services] ──────────► [Business Validation & Calculation]
       │
       ▼
[Repositories] ──────► [LINQ Query Definition & Projection]
       │
       ▼
[AppDbContext] ──────► [Change Tracker & Unit of Work]
       │
       ▼
[SQL Server Engine] ─► [Foreign Keys, Unique Indexes, Cascades]
```

### Các cơ chế an toàn dữ liệu:
- **Ngăn chặn Cascade Delete nguy hiểm**: `AppDbContext.cs` cấu hình `DeleteBehavior.Restrict` cho toàn bộ các quan hệ `BanDoc -> PhieuMuon`, `Sach -> ChiTietMuon`, `TaiKhoan -> BanDoc`, đảm bảo không bao giờ bị xóa dây chuyền mất dữ liệu lịch sử mượn và tài sản thư viện.
- **Bắt ngoại lệ Unique Constraint (`DbUpdateException`)**: Tại luồng đăng ký tài khoản, việc xung đột trùng tên đăng nhập song song được bắt ngoại lệ và trả thông báo lỗi hợp lệ cho giao diện thay vì lỗi máy chủ 500.

---

## 6. Pipeline Thông báo Email (Notification Pipeline)

Hệ thống tích hợp [`SmtpEmailNotificationService`](file:///d:/TH_LTNET/HE_THONG_QUAN_LY_THU_VIEN_VA_MUON_TRA_SACH/QuanLyThuVien_UNETI01_DHTI17A4HN/Services/Notifications/SmtpEmailNotificationService.cs) để tự động gửi thông báo đến độc giả:

1. **Trigger sự kiện**:
   - Thủ thư phê duyệt phiếu mượn.
   - Thủ thư từ chối phiếu mượn kèm lý do.
   - Thủ thư nhận trả sách và chốt tiền phạt.
2. **Kiểm tra cấu hình**:
   - Nếu SMTP chưa cấu hình, dịch vụ trả về trạng thái `Disabled` an toàn mà không làm gián đoạn hoặc rollback giao dịch mượn trả đã thành công.
   - Giao diện Razor View nhận `TempData["CanhBao"]` để thông báo rõ ràng cho nhân viên biết email chưa được phát đi.
