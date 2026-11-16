// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Kiểm thử đăng nhập, băm mật khẩu và đăng ký bạn đọc.

using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories.TaiKhoan;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.Auth;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ReturnsAccount_WhenPasswordIsCorrectAndAccountIsActive()
    {
        var account = TaoTaiKhoan("reader", "correct-password");
        var service = new AuthService(new FakeTaiKhoanRepository(account));

        var result = await service.LoginAsync("reader", "correct-password");

        Assert.Same(account, result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenPasswordIsWrong()
    {
        var service = new AuthService(new FakeTaiKhoanRepository(TaoTaiKhoan("reader", "correct-password")));

        var result = await service.LoginAsync("reader", "wrong-password");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenAccountDoesNotExist()
    {
        var service = new AuthService(new FakeTaiKhoanRepository(null));

        var result = await service.LoginAsync("missing", "correct-password");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenAccountIsLocked()
    {
        var account = TaoTaiKhoan("locked", "correct-password");
        account.TrangThai = false;
        var service = new AuthService(new FakeTaiKhoanRepository(account));

        var result = await service.LoginAsync("locked", "correct-password");

        Assert.Null(result);
    }

    [Fact]
    public void PasswordHash_UsesUniqueSaltsAndVerifiesCorrectPassword()
    {
        var firstHash = MatKhauHelper.BamMatKhau("Demo-Strong-Password");
        var secondHash = MatKhauHelper.BamMatKhau("Demo-Strong-Password");

        Assert.NotEqual(firstHash, secondHash);
        Assert.StartsWith("pbkdf2-sha256$", firstHash, StringComparison.Ordinal);
        Assert.True(MatKhauHelper.KiemTra("Demo-Strong-Password", firstHash));
        Assert.False(MatKhauHelper.KiemTra("Wrong-Password", firstHash));
    }

    [Fact]
    public void PasswordVerification_SupportsPreviousSha256Hashes()
    {
        var legacyHash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("legacy-password")));

        Assert.True(MatKhauHelper.KiemTra("legacy-password", legacyHash));
        Assert.False(MatKhauHelper.KiemTra("other-password", legacyHash));
    }

    [Fact]
    public async Task RegisterReaderAsync_CreatesActiveReaderAndHashesPassword()
    {
        var repository = new FakeTaiKhoanRepository(null);
        var service = new AuthService(repository);

        var created = await service.RegisterReaderAsync(new DangKyViewModel
        {
            TenDangNhap = " new-reader ",
            HoTen = " New Reader ",
            Email = " reader@example.com ",
            MatKhau = "Long-Enough-Password",
            XacNhanMatKhau = "Long-Enough-Password",
        });

        Assert.True(created);
        var account = repository.AddedAccount ?? throw new InvalidOperationException("Registration did not add an account.");
        Assert.Equal("new-reader", account.TenDangNhap);
        Assert.Equal(VaiTro.BanDoc, account.VaiTro);
        Assert.True(account.TrangThai);
        Assert.NotEqual("Long-Enough-Password", account.MatKhau);
        Assert.True(MatKhauHelper.KiemTra("Long-Enough-Password", account.MatKhau));
        var reader = account.BanDoc ?? throw new InvalidOperationException("Registration did not create a reader profile.");
        Assert.Equal("New Reader", reader.HoTen);
        Assert.Equal("reader@example.com", reader.Email);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task RegisterReaderAsync_ReturnsFalse_WhenUsernameAlreadyExists()
    {
        var repository = new FakeTaiKhoanRepository(TaoTaiKhoan("taken", "existing-password"));
        var service = new AuthService(repository);

        var created = await service.RegisterReaderAsync(new DangKyViewModel
        {
            TenDangNhap = "taken",
            HoTen = "New Reader",
            Email = "reader@example.com",
            MatKhau = "Long-Enough-Password",
            XacNhanMatKhau = "Long-Enough-Password",
        });

        Assert.False(created);
        Assert.Null(repository.AddedAccount);
        Assert.Equal(0, repository.SaveCount);
    }

    private static TaiKhoan TaoTaiKhoan(string username, string password)
        => new()
        {
            TenDangNhap = username,
            MatKhau = MatKhauHelper.BamMatKhau(password),
            HoTen = "Test Reader",
            TrangThai = true,
        };

    private sealed class FakeTaiKhoanRepository(TaiKhoan? account) : ITaiKhoanRepository
    {
        private readonly TaiKhoan? _account = account;
        public TaiKhoan? AddedAccount { get; private set; }
        public int SaveCount { get; private set; }

        public Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap)
            => Task.FromResult(_account?.TenDangNhap == tenDangNhap ? _account : null);

        public Task<List<TaiKhoan>> GetAllAsync() => Task.FromResult(new List<TaiKhoan>());
        public Task<TaiKhoan?> GetByIdAsync(int id) => Task.FromResult<TaiKhoan?>(null);
        public Task<bool> ExistsAsync(string tenDangNhap, int? excludeId = null)
            => Task.FromResult(_account?.TenDangNhap == tenDangNhap);
        public Task AddAsync(TaiKhoan taiKhoan)
        {
            AddedAccount = taiKhoan;
            return Task.CompletedTask;
        }
        public Task UpdateAsync(TaiKhoan taiKhoan) => Task.CompletedTask;
        public Task SaveChangesAsync()
        {
            SaveCount++;
            return Task.CompletedTask;
        }
    }
}
