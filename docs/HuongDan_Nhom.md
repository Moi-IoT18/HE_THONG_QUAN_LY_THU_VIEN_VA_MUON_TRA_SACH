# Dữ liệu mẫu thư viện

## Khởi tạo

Seeder demo là thao tác chủ động, không chạy khi ứng dụng khởi động bình thường. Chỉ chạy trên database phát triển/test đã migrate; lệnh từ chối thực hiện nếu bất kỳ bảng thư viện nào đã có dữ liệu. Các bản ghi được thêm trong một transaction và không xóa dữ liệu.

Trước khi chạy, dùng PowerShell tại thư mục gốc để nhập mật khẩu demo cho từng vai trò (ẩn khi nhập, 8–128 ký tự), chạy seeder, rồi xóa các biến môi trường:

```powershell
$secureAdmin = Read-Host "Admin demo password" -AsSecureString
$env:LIBRARY_DEMO_ADMIN_PASSWORD = [System.Net.NetworkCredential]::new("", $secureAdmin).Password
$secureStaff = Read-Host "Staff demo password" -AsSecureString
$env:LIBRARY_DEMO_STAFF_PASSWORD = [System.Net.NetworkCredential]::new("", $secureStaff).Password
$secureReader = Read-Host "Reader demo password" -AsSecureString
$env:LIBRARY_DEMO_READER_PASSWORD = [System.Net.NetworkCredential]::new("", $secureReader).Password
dotnet run --project .\QuanLyThuVien_UNETI01_DHTI17A4HN\QuanLyThuVien_UNETI01_DHTI17A4HN.csproj -- --seed-demo-data
Remove-Item Env:\LIBRARY_DEMO_ADMIN_PASSWORD, Env:\LIBRARY_DEMO_STAFF_PASSWORD, Env:\LIBRARY_DEMO_READER_PASSWORD
```

Mật khẩu không được ghi vào mã nguồn hoặc in ra console; mỗi tài khoản được lưu PBKDF2-SHA256 với salt ngẫu nhiên riêng. Mỗi nhóm dùng mật khẩu demo đã nhập cho vai trò đó:

- Admin: `admin01`, `admin02`.
- Nhân viên: `nhanvien01`, `nhanvien02`.
- Bạn đọc: `bandoc01` đến `bandoc30`.

Chỉ sử dụng mật khẩu tạm thời trên môi trường phát triển riêng; không dùng lại mật khẩu tài khoản thật. Seeder tạo 5 thể loại, 3 nhà xuất bản, 15 sách ở nhiều trạng thái, 30 bạn đọc với trạng thái thẻ/tài khoản đa dạng, 50 phiếu ở các trạng thái Chờ duyệt/Đang mượn/Quá hạn/Đã trả/Đã hủy và các dòng chi tiết tương ứng. Mỗi phiếu nghiệp vụ phải có ít nhất một dòng sách; do đó 50 phiếu cần ít nhất 50 dòng chi tiết, dù yêu cầu mẫu riêng ghi 15–20 dòng chi tiết. Seeder tạo 62 dòng để một số phiếu có nhiều sách và bảo đảm toàn vẹn quan hệ. Tồn kho và tiền phạt mẫu được tính nhất quán với các giao dịch sinh ra.

## Phạm vi truy vấn và phân trang

Danh sách sách thực hiện tìm kiếm tên sách/thể loại/NXB, lọc kết hợp theo thể loại/năm/trạng thái/tình trạng tồn kho/khoảng giá, sắp xếp và phân trang trực tiếp trên `IQueryable` EF Core. Thanh phân trang dùng lại toàn bộ query string hiện tại, chỉ thay số trang; dữ liệu không được tải toàn bộ về trình duyệt để phân trang.

## Chạy kiểm thử tự động

Project kiểm thử `QuanLyThuVien.Tests` bao gồm đăng nhập (đúng/sai/không tồn tại/bị khóa), quyền Session tại Controller, luồng trạng thái, Data Annotation, kiểm tra upload ảnh, email chưa cấu hình, cấu trúc file Excel/PDF và SQL được EF Core sinh ra cho tìm kiếm/lọc kết hợp/phân trang sách. Truy vấn SQL được kiểm tra mà không mở kết nối hay ghi dữ liệu vào database:

```powershell
dotnet test .\QuanLyThuVien.Tests\QuanLyThuVien.Tests.csproj
```

Kiểm thử giao dịch end-to-end trên SQL Server cần database riêng và các tài khoản/dữ liệu demo; không chạy trên database người dùng có dữ liệu. Bộ kiểm thử tự động không tự seed dữ liệu.

## Giao diện, ảnh bìa, email và báo cáo

Sau khi cập nhật mã, chạy migration để thêm đường dẫn ảnh bìa vào bảng `Sach`:

```powershell
dotnet ef database update --project .\QuanLyThuVien_UNETI01_DHTI17A4HN\QuanLyThuVien_UNETI01_DHTI17A4HN.csproj
```

- Tìm kiếm sách chạy nền bằng AJAX khi nhập từ khóa hoặc đổi bộ lọc; form GET và phân trang thông thường vẫn dùng được nếu JavaScript bị tắt hoặc tải thất bại.
- Nhân viên/Admin tải ảnh bìa trong form thêm/sửa sách. Chỉ nhận PNG/JPEG/WebP tối đa 5 MB; ảnh lưu dưới `wwwroot/uploads/books` với tên ngẫu nhiên.
- Dashboard và trang thống kê có biểu đồ Chart.js. Trang thống kê có nút xuất Excel (`.xlsx`) và PDF; các file chứa số liệu đúng như báo cáo LINQ trên trang.
- Khi nhân viên duyệt, từ chối hoặc xác nhận trả phiếu, ứng dụng gửi email trạng thái tới địa chỉ email của bạn đọc. Cần cấu hình SMTP qua biến môi trường (không đưa mật khẩu vào mã nguồn):

```powershell
$env:Email__Host = "smtp.example.com"
$env:Email__Port = "587"
$env:Email__Username = "your-account"
$secure = Read-Host "SMTP password" -AsSecureString
$env:Email__Password = [System.Net.NetworkCredential]::new("", $secure).Password
$env:Email__From = "library@example.com"
$env:Email__EnableSsl = "true"
dotnet run --project .\QuanLyThuVien_UNETI01_DHTI17A4HN\QuanLyThuVien_UNETI01_DHTI17A4HN.csproj
Remove-Item Env:\Email__Host, Env:\Email__Port, Env:\Email__Username, Env:\Email__Password, Env:\Email__From, Env:\Email__EnableSsl
```

Nếu chưa cấu hình SMTP hoặc gửi thư thất bại, cập nhật giao dịch vẫn được giữ nguyên và giao diện báo rõ email chưa gửi. Biểu đồ tải Chart.js từ jsDelivr nên cần mạng để hiển thị; bảng số liệu vẫn khả dụng khi không có mạng.

Mật khẩu tiếp tục được băm PBKDF2 bằng cơ chế tài khoản hiện có; ứng dụng chưa chuyển sang ASP.NET Core Identity.

## 25. Quy trình nghiệp vụ

### 25.1. Quy trình đối với bạn đọc

1. Đăng nhập bằng tài khoản bạn đọc.
2. Tra cứu sách, xem thông tin và chọn sách đưa vào giỏ mượn.
3. Gửi phiếu mượn. Hệ thống kiểm tra trạng thái tài khoản/thẻ, giới hạn số sách và các điều kiện mượn khác; phiếu mới ở trạng thái **Chờ duyệt**.
4. Nhân viên duyệt phiếu. Khi duyệt thành công, phiếu chuyển sang **Đang mượn** và số lượng sách còn được cập nhật. Bạn đọc nhận sách theo quy trình của thư viện.
5. Trả sách cho nhân viên để ghi nhận kết quả. Hệ thống tính phạt quá hạn tại thời điểm nhận trả, dựa trên số ngày trễ, mức phạt mỗi ngày và số lượng sách; đồng thời cập nhật tồn kho theo tình trạng sách.
6. Phiếu chuyển sang **Đã trả**. Sách bình thường được cộng lại vào số lượng còn; sách hỏng/mất được xử lý theo quy định tồn kho.

Tiền phạt hiển thị trước khi trả là khoản **tạm tính**; tiền phạt chính thức được chốt khi nhân viên ghi nhận trả sách.

Sau khi phiếu chuyển sang **Đã trả**, bạn đọc nộp tiền mặt tại quầy thư viện hoặc liên hệ thủ thư để lấy thông tin tài khoản chuyển khoản chính thức. Khi chuyển khoản, ghi mã phiếu trong nội dung; thủ thư xác nhận đã đối soát/nhận đủ tiền trên màn hình xử lý phiếu. Hệ thống lưu số tiền, hình thức, thời điểm và tài khoản nhân viên xác nhận; hiện chưa tích hợp cổng thanh toán trực tuyến.

### 25.2. Quy trình đối với nhân viên/Admin

1. Đăng nhập và kiểm tra quyền theo vai trò.
2. Admin quản lý thể loại; nhân viên và Admin quản lý sách, nhà xuất bản và bạn đọc theo quyền được cấp.
3. Tiếp nhận phiếu **Chờ duyệt**, kiểm tra bạn đọc, điều kiện mượn và tồn kho.
4. Duyệt phiếu hợp lệ để chuyển sang **Đang mượn** và cập nhật tồn kho; hoặc từ chối/hủy theo điều kiện nghiệp vụ, kèm lý do khi từ chối.
5. Nhận trả sách, cập nhật tình trạng từng đầu sách, tiền phạt và tồn kho.
6. Theo dõi Dashboard và đối chiếu báo cáo thống kê sau khi dữ liệu nghiệp vụ được cập nhật.

### 25.3. Quy trình xử lý giao dịch

```text
Chờ duyệt ── duyệt hợp lệ ──> Đang mượn ── trả sách ──> Đã trả
    │                            │
    ├── bạn đọc hủy ──> Đã hủy   └── quá hạn ──> Quá hạn ── trả sách ──> Đã trả
    └── nhân viên từ chối ──> Đã hủy (ghi lý do)
```

- Chỉ phiếu **Chờ duyệt** mới có thể được bạn đọc hủy hoặc nhân viên từ chối.
- Chỉ phiếu **Đang mượn** hoặc **Quá hạn** mới được ghi nhận trả.
- Phiếu đang mượn tự được đánh dấu **Quá hạn** khi đã qua hạn trả.
- Từ chối yêu cầu lý do; các chuyển trạng thái không hợp lệ phải bị từ chối. Duyệt/nhận trả cập nhật các bản ghi liên quan trong transaction.

### 25.4. Quy trình xử lý dữ liệu

```text
Entity → AppDbContext → Entity Framework Core 10
       → Migration → SQL Server
       → LINQ qua Repository/Service → Controller → Razor View
```

Migration cập nhật schema; EF Core và LINQ đọc/ghi dữ liệu. Controller tiếp nhận request, gọi Service/Repository để thực hiện nghiệp vụ và trả ViewModel/Entity phù hợp cho Razor View.

## 22. Kiểm thử và hoàn thiện

Các mục dưới đây là checklist kiểm thử hồi quy. Thực hiện trên database phát triển/test riêng; không dùng database có dữ liệu thật. Kiểm thử tự động hiện có chạy bằng lệnh ở mục [Chạy kiểm thử tự động](#chạy-kiểm-thử-tự-động); các tình huống cần SQL Server hoặc tương tác giao diện cần được kiểm tra tích hợp/thủ công và không mặc định đã được bao phủ bởi unit test.

### 22.1. Tài khoản và phân quyền

- [ ] Đăng nhập thành công với thông tin hợp lệ; đăng xuất kết thúc phiên.
- [ ] Báo lỗi phù hợp khi mật khẩu sai hoặc tài khoản không tồn tại.
- [ ] Tài khoản bị khóa không thể sử dụng chức năng yêu cầu đăng nhập.
- [ ] Bạn đọc truy cập URL quản trị bị từ chối; kiểm tra quyền trên Controller cho từng vai trò.

### 22.2. CRUD và validation

- [ ] Kiểm tra Create/Edit/Details/Delete theo quyền trên từng loại dữ liệu.
- [ ] Thử dữ liệu hợp lệ, dữ liệu sai định dạng, trường bắt buộc bị bỏ trống và giá trị ngoài giới hạn.
- [ ] Kiểm tra dữ liệu trùng, số lượng/giá trị không hợp lệ và ngày/thời gian không hợp lệ.
- [ ] Validation Message hiển thị đúng tại form và dữ liệu không hợp lệ không được lưu.

### 22.3. Tìm kiếm, lọc, sắp xếp và phân trang

- [ ] Tìm kiếm có kết quả và không có kết quả.
- [ ] Kiểm tra từng điều kiện lọc riêng và kết hợp nhiều điều kiện.
- [ ] Kiểm tra sắp xếp tăng/giảm; chuyển trang đầu, giữa và cuối.
- [ ] Điều kiện tìm kiếm/lọc/sắp xếp được giữ khi chuyển trang; trang ngoài phạm vi được xử lý an toàn.

### 22.4. Giao dịch nghiệp vụ

- [ ] Tạo phiếu mượn hợp lệ; thử tạo phiếu khi tài khoản/thẻ, giới hạn sách hoặc tồn kho không đạt điều kiện.
- [ ] Thử gửi trùng yêu cầu và xác minh không phát sinh phiếu hoặc trừ kho ngoài dự kiến.
- [ ] Hủy phiếu Chờ duyệt thành công; không cho hủy phiếu đã được xử lý.
- [ ] Duyệt phiếu hợp lệ; từ chối phiếu với lý do; kiểm tra trạng thái và tồn kho sau mỗi thao tác.

### 22.5. Xung đột và khả năng đáp ứng

- [ ] Không cho mượn vượt số lượng còn hoặc vượt giới hạn số sách.
- [ ] Thử hai yêu cầu đồng thời tranh cùng đầu sách; tồn kho không được âm và không duyệt vượt số lượng.
- [ ] Kiểm tra tính toàn vẹn dữ liệu liên quan khi cập nhật/xóa.
- [ ] Không cho tiếp tục xử lý nếu trạng thái phiếu hoặc điều kiện nghiệp vụ không hợp lệ.

### 22.6. Kết quả xử lý

- [ ] Chỉ ghi nhận trả với phiếu Đang mượn/Quá hạn và danh sách sách khớp với phiếu.
- [ ] Kiểm tra số ngày quá hạn và tiền phạt theo công thức: `số ngày trễ × mức phạt/ngày × số lượng`.
- [ ] Thử ngày trả đúng hạn, quá hạn và giới hạn giá trị; kiểm tra không tính phạt âm hoặc vượt giới hạn.
- [ ] Kiểm tra tình trạng trả, ghi chú, trạng thái cuối, ngày trả và tổng phạt của phiếu.
- [ ] Đối chiếu tồn kho: sách bình thường được nhập lại; sách hỏng/mất được xử lý theo nghiệp vụ.

### 22.7. Quyền truy cập dữ liệu

- [ ] Bạn đọc chỉ xem/sửa hồ sơ và phiếu của mình.
- [ ] Thay mã trên URL không cho phép xem, sửa, hủy hoặc nhận trả dữ liệu của người khác khi không đủ quyền.
- [ ] Người chưa đăng nhập hoặc sai vai trò không thể gọi trực tiếp endpoint được bảo vệ.

### 22.8. Dashboard và thống kê

- [ ] Đối chiếu số liệu Dashboard/báo cáo với dữ liệu truy vấn trên database.
- [ ] Thêm, cập nhật, hủy phiếu hoặc nhận trả làm thay đổi đúng các số liệu liên quan.
- [ ] Kiểm tra danh sách quá hạn, tiền phạt tạm tính và các báo cáo sau khi phiếu chuyển trạng thái.
- [ ] Kiểm tra trường hợp chưa có dữ liệu và khoảng thời gian không có giao dịch.