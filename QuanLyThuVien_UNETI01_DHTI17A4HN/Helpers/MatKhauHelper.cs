using Microsoft.AspNetCore.Identity;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class MatKhauHelper
{
    private static readonly PasswordHasher<User> Hasher = new();

    public static string BamMatKhau(string matKhau)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(matKhau);
        return Hasher.HashPassword(new User(), matKhau);
    }

    public static bool KiemTra(string matKhau, string hash)
    {
        ArgumentNullException.ThrowIfNull(hash);
        return Hasher.VerifyHashedPassword(new User(), hash, matKhau)
            is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
