using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Models;

public class Role
{
    [Key]
    public int RoleId { get; set; }

    [Required]
    [StringLength(50)]
    public string RoleName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
