// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Module 3 – Nghiệp vụ Bạn đọc: tạo kèm tài khoản, kiểm tra ngày sinh/ngày cấp/hạn thẻ,
//                     khóa thẻ, gia hạn thẻ 1 năm, xóa an toàn; hồ sơ cá nhân và đổi mật khẩu.
 
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;
 
namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services;
 
public class BanDocService : IBanDocService
{
    private readonly IBanDocRepository _repo;
 
    public BanDocService(IBanDocRepository repo)
    {
        _repo = repo;
    }
 
    // =====================================================================
    // QUẢN LÝ BẠN ĐỌC
    // =====================================================================
 
    public async Task<BanDocIndexViewModel> LayDanhSachAsync(string? tuKhoa, TrangThaiBanDoc? trangThai,
        string? tinhTrangThe, string sapXep, int trang) => new()
    {
        TuKhoa = tuKhoa,
        TrangThai = trangThai,
        TinhTrangThe = tinhTrangThe,
        SapXep = sapXep,
        DanhSach = await PhanTrang<BanDocDong>.TaoAsync(
            _repo.TruyVanDanhSach(tuKhoa, trangThai, tinhTrangThe, sapXep), trang, QuyDinh.KichThuocTrang),
    };
 
    public async Task<BanDocChiTietViewModel?> LayChiTietAsync(int id)
    {
        var bd = await _repo.LayKemTaiKhoanAsync(id);
        if (bd is null) return null;
 
        var phieus = await _repo.LayLichSuPhieuAsync(id);
        return new BanDocChiTietViewModel
        {
            BanDoc = bd,
            PhieuMuons = phieus,
            SoSachDangGiu = phieus.Where(p => TrangThaiPhieuService.DangXuLy(p.TrangThai))
                .SelectMany(p => p.ChiTietMuons).Sum(ct => ct.SoLuong),
            TongTienPhat = phieus.Sum(p => p.TienPhat),
        };
    }
 
    public Task<BanDoc?> LayKemTaiKhoanAsync(int id) => _repo.LayKemTaiKhoanAsync(id);
 
    public Task<int> DemPhieuAsync(int id) => _repo.DemPhieuAsync(id);
 
    public async Task<BanDocFormViewModel?> LayFormSuaAsync(int id)
    {
        var b = await _repo.LayKemTaiKhoanAsync(id);
        if (b is null) return null;
        return new BanDocFormViewModel
        {
            MaBanDoc = b.MaBanDoc,
            TenDangNhap = b.TaiKhoan?.TenDangNhap,
            HoTen = b.HoTen,
            NgaySinh = b.NgaySinh,
            GioiTinh = b.GioiTinh,
            SoDienThoai = b.SoDienThoai,
            Email = b.Email ?? b.TaiKhoan?.Email ?? "",
            DiaChi = b.DiaChi,
            NgayCapThe = b.NgayCapThe,
            HanThe = b.HanThe,
            TrangThai = b.TrangThai,
            GhiChu = b.GhiChu,
        };
    }
 
    public async Task<ServiceResult<int>> TaoAsync(BanDocFormViewModel vm)
    {
        var kq = new ServiceResult<int>();
        if (string.IsNullOrWhiteSpace(vm.TenDangNhap))
            kq.ThemLoi("Vui lòng nhập tên đăng nhập", nameof(vm.TenDangNhap));
        else if (await _repo.TrungTenDangNhapAsync(vm.TenDangNhap.Trim()))
            kq.ThemLoi("Tên đăng nhập đã tồn tại", nameof(vm.TenDangNhap));
        if (string.IsNullOrWhiteSpace(vm.MatKhau))
            kq.ThemLoi("Vui lòng nhập mật khẩu", nameof(vm.MatKhau));
        KiemTraNgay(kq, vm.NgaySinh, vm.NgayCapThe, vm.HanThe);
        if (!kq.ThanhCong) return kq;
 
        var banDoc = new BanDoc
        {
            TaiKhoan = new TaiKhoan
            {
                TenDangNhap = vm.TenDangNhap!.Trim(),
                MatKhau = MatKhauHelper.BamMatKhau(vm.MatKhau!),
                HoTen = vm.HoTen.Trim(),
                Email = vm.Email.Trim(),
                VaiTro = VaiTro.BanDoc,
                TrangThai = true,
                NgayTao = DateTime.Now,
            },
        };
        GanThongTin(banDoc, vm);
        await _repo.AddAsync(banDoc);
        await _repo.SaveChangesAsync();
 
        return ServiceResult<int>.Ok(banDoc.MaBanDoc, $"Đã thêm bạn đọc \"{banDoc.HoTen}\" (tài khoản: {vm.TenDangNhap}).");
    }
 
    public async Task<ServiceResult> CapNhatAsync(int id, BanDocFormViewModel vm)
    {
        var banDoc = await _repo.LayKemTaiKhoanAsync(id, theoDoi: true);
        if (banDoc is null) return ServiceResult.KhongTonTai();
 
        var kq = new ServiceResult();
        KiemTraNgay(kq, vm.NgaySinh, vm.NgayCapThe, vm.HanThe);
        if (!kq.ThanhCong) return kq;
 
        GanThongTin(banDoc, vm);
        if (banDoc.TaiKhoan is not null)
        {
            banDoc.TaiKhoan.HoTen = banDoc.HoTen;
            banDoc.TaiKhoan.Email = vm.Email.Trim();
            if (!string.IsNullOrWhiteSpace(vm.MatKhau))
                banDoc.TaiKhoan.MatKhau = MatKhauHelper.BamMatKhau(vm.MatKhau);
        }
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok($"Đã cập nhật bạn đọc \"{banDoc.HoTen}\".");
    }
 
    public async Task<ServiceResult> KhoaMoAsync(int id)
    {
        var bd = await _repo.GetByIdAsync(id);
        if (bd is null) return ServiceResult.KhongTonTai();
 
        bd.TrangThai = bd.TrangThai == TrangThaiBanDoc.HoatDong ? TrangThaiBanDoc.TamKhoa : TrangThaiBanDoc.HoatDong;
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok($"Thẻ của \"{bd.HoTen}\" chuyển sang: {bd.TrangThai.TenHienThi()}.");
    }
 
    /// <summary>Gia hạn thẻ thêm 1 năm (tính từ hạn cũ hoặc từ hôm nay nếu đã hết hạn).</summary>
    public async Task<ServiceResult> GiaHanTheAsync(int id)
    {
        var bd = await _repo.GetByIdAsync(id);
        if (bd is null) return ServiceResult.KhongTonTai();
 
        var moc = bd.HanThe.Date < DateTime.Today ? DateTime.Today : bd.HanThe.Date;
        bd.HanThe = moc.AddYears(1);
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok($"Đã gia hạn thẻ của \"{bd.HoTen}\" đến {bd.HanThe:dd/MM/yyyy}.");
    }
 
    public async Task<ServiceResult> XoaAsync(int id)
    {
        var bd = await _repo.LayKemTaiKhoanAsync(id, theoDoi: true);
        if (bd is null) return ServiceResult.KhongTonTai();
 
        if (await _repo.DemPhieuAsync(id) > 0)
            return ServiceResult.Loi($"Bạn đọc \"{bd.HoTen}\" đã có lịch sử mượn nên không thể xóa. Hãy tạm khóa thẻ.");
 
        _repo.XoaKemTaiKhoan(bd);
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok($"Đã xóa bạn đọc \"{bd.HoTen}\".");
    }
 
    // =====================================================================
    // HỒ SƠ CÁ NHÂN
    // =====================================================================
 
    public async Task<HoSoViewModel?> LayHoSoAsync(int? maTaiKhoan)
    {
        var bd = await _repo.LayTheoTaiKhoanAsync(maTaiKhoan);
        if (bd is null) return null;
        return new HoSoViewModel
        {
            MaBanDoc = bd.MaBanDoc,
            TenDangNhap = bd.TaiKhoan?.TenDangNhap ?? "",
            HoTen = bd.HoTen,
            NgaySinh = bd.NgaySinh,
            GioiTinh = bd.GioiTinh,
            SoDienThoai = bd.SoDienThoai,
            Email = bd.Email ?? bd.TaiKhoan?.Email ?? "",
            DiaChi = bd.DiaChi,
            NgayCapThe = bd.NgayCapThe,
            HanThe = bd.HanThe,
            TrangThai = bd.TrangThai,
            SoSachDangGiu = await _repo.DemSachDangGiuAsync(bd.MaBanDoc),
        };
    }
 
    public async Task<ServiceResult<string>> CapNhatHoSoAsync(int? maTaiKhoan, HoSoViewModel vm)
    {
        var bd = await _repo.LayTheoTaiKhoanAsync(maTaiKhoan);
        if (bd is null) return ServiceResult<string>.KhongTonTai();
 
        if (vm.NgaySinh.HasValue && vm.NgaySinh.Value.Date >= DateTime.Today)
            return ServiceResult<string>.Loi("Ngày sinh phải nhỏ hơn ngày hiện tại", nameof(vm.NgaySinh));
 
        // Chỉ cập nhật các trường bạn đọc được phép sửa
        bd.HoTen = vm.HoTen.Trim();
        bd.NgaySinh = vm.NgaySinh;
        bd.GioiTinh = vm.GioiTinh;
        bd.SoDienThoai = vm.SoDienThoai;
        bd.Email = vm.Email.Trim();
        bd.DiaChi = vm.DiaChi;
        if (bd.TaiKhoan is not null)
        {
            bd.TaiKhoan.HoTen = bd.HoTen;
            bd.TaiKhoan.Email = vm.Email;
        }
        await _repo.SaveChangesAsync();
        return ServiceResult<string>.Ok(bd.HoTen, "Đã cập nhật hồ sơ cá nhân.");
    }
 
    public async Task<ServiceResult> DoiMatKhauAsync(int? maTaiKhoan, DoiMatKhauViewModel vm)
    {
        var bd = await _repo.LayTheoTaiKhoanAsync(maTaiKhoan);
        if (bd?.TaiKhoan is null) return ServiceResult.KhongTonTai();
 
        if (!MatKhauHelper.KiemTra(vm.MatKhauCu, bd.TaiKhoan.MatKhau))
            return ServiceResult.Loi("Mật khẩu hiện tại không đúng.");
 
        bd.TaiKhoan.MatKhau = MatKhauHelper.BamMatKhau(vm.MatKhauMoi);
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok("Đã đổi mật khẩu.");
    }
 
    // =====================================================================
    // HÀM HỖ TRỢ
    // =====================================================================
 
    private static void KiemTraNgay(ServiceResult kq, DateTime? ngaySinh, DateTime ngayCap, DateTime hanThe)
    {
        if (ngaySinh.HasValue && ngaySinh.Value.Date >= DateTime.Today)
            kq.ThemLoi("Ngày sinh phải nhỏ hơn ngày hiện tại", nameof(BanDocFormViewModel.NgaySinh));
        if (ngayCap.Date > DateTime.Today)
            kq.ThemLoi("Ngày cấp thẻ không được lớn hơn hôm nay", nameof(BanDocFormViewModel.NgayCapThe));
        if (hanThe.Date <= ngayCap.Date)
            kq.ThemLoi("Hạn thẻ phải sau ngày cấp thẻ", nameof(BanDocFormViewModel.HanThe));
    }
 
    private static void GanThongTin(BanDoc bd, BanDocFormViewModel vm)
    {
        bd.HoTen = vm.HoTen.Trim();
        bd.NgaySinh = vm.NgaySinh;
        bd.GioiTinh = vm.GioiTinh;
        bd.SoDienThoai = vm.SoDienThoai;
        bd.Email = vm.Email.Trim();
        bd.DiaChi = vm.DiaChi;
        bd.NgayCapThe = vm.NgayCapThe.Date;
        bd.HanThe = vm.HanThe.Date;
        bd.TrangThai = vm.TrangThai;
        bd.GhiChu = vm.GhiChu;
    }
}
