using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public int RoleId { get; set; }

    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }

    [NotMapped]
    public string TenDangNhap { get => Username; set => Username = value; }

    [NotMapped]
    public string MatKhau { get => PasswordHash; set => PasswordHash = value; }

    [NotMapped]
    public string HoTen { get => FullName; set => FullName = value; }

    [NotMapped]
    public bool TrangThai { get => IsActive; set => IsActive = value; }

    [NotMapped]
    public VaiTro VaiTro
    {
        get => RoleId switch
        {
            1 => VaiTro.Admin,
            2 => VaiTro.ThuThu,
            _ => VaiTro.BanDoc,
        };
        set => RoleId = value switch
        {
            VaiTro.Admin => 1,
            VaiTro.ThuThu => 2,
            _ => 3,
        };
    }

    [NotMapped]
    public DateTime NgayTao { get; set; }
}
