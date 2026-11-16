// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Băm mật khẩu PBKDF2 có salt riêng và tương thích kiểm tra hash SHA-256 cũ.

using System.Security.Cryptography;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class MatKhauHelper
{
    private const int Iterations = 210_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const string Prefix = "pbkdf2-sha256";

    public static string BamMatKhau(string matKhau)
    {
        ArgumentException.ThrowIfNullOrEmpty(matKhau);
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Prefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool KiemTra(string matKhau, string hash)
    {
        if (string.IsNullOrEmpty(matKhau) || string.IsNullOrEmpty(hash))
            return false;

        var parts = hash.Split('$');
        if (parts.Length == 4 && parts[0] == Prefix
            && int.TryParse(parts[1], out var iterations) && iterations is >= 100_000 and <= 2_000_000)
        {
            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var expected = Convert.FromBase64String(parts[3]);
                if (salt.Length != SaltSize || expected.Length != HashSize)
                    return false;

                var actual = Rfc2898DeriveBytes.Pbkdf2(
                    matKhau, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // Verify password hashes created by the previous SHA-256 implementation during migration.
        if (parts.Length == 1)
        {
            try
            {
                var legacyHash = Convert.FromBase64String(hash);
                var legacyActual = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(matKhau));
                if (legacyHash.Length == legacyActual.Length
                    && CryptographicOperations.FixedTimeEquals(legacyActual, legacyHash))
                    return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        return false;
    }
}
