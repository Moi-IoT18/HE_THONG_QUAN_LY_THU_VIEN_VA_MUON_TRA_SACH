// Họ và tên: Đặng Thị Phượng
// Mã sinh viên: 23103100182
// Nội dung thực hiện: Chặn cascade delete trên các FK dữ liệu nền và lịch sử mượn.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class RestrictLibraryDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BanDoc_TaiKhoan_MaTaiKhoan",
                table: "BanDoc");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuon_BanDoc_MaBanDoc",
                table: "PhieuMuon");

            migrationBuilder.DropForeignKey(
                name: "FK_Sach_NhaXuatBan_MaNhaXuatBan",
                table: "Sach");

            migrationBuilder.DropForeignKey(
                name: "FK_Sach_TheLoai_MaTheLoai",
                table: "Sach");

            migrationBuilder.AddForeignKey(
                name: "FK_BanDoc_TaiKhoan_MaTaiKhoan",
                table: "BanDoc",
                column: "MaTaiKhoan",
                principalTable: "TaiKhoan",
                principalColumn: "MaTaiKhoan",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuon_BanDoc_MaBanDoc",
                table: "PhieuMuon",
                column: "MaBanDoc",
                principalTable: "BanDoc",
                principalColumn: "MaBanDoc",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sach_NhaXuatBan_MaNhaXuatBan",
                table: "Sach",
                column: "MaNhaXuatBan",
                principalTable: "NhaXuatBan",
                principalColumn: "MaNhaXuatBan",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sach_TheLoai_MaTheLoai",
                table: "Sach",
                column: "MaTheLoai",
                principalTable: "TheLoai",
                principalColumn: "MaTheLoai",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BanDoc_TaiKhoan_MaTaiKhoan",
                table: "BanDoc");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuon_BanDoc_MaBanDoc",
                table: "PhieuMuon");

            migrationBuilder.DropForeignKey(
                name: "FK_Sach_NhaXuatBan_MaNhaXuatBan",
                table: "Sach");

            migrationBuilder.DropForeignKey(
                name: "FK_Sach_TheLoai_MaTheLoai",
                table: "Sach");

            migrationBuilder.AddForeignKey(
                name: "FK_BanDoc_TaiKhoan_MaTaiKhoan",
                table: "BanDoc",
                column: "MaTaiKhoan",
                principalTable: "TaiKhoan",
                principalColumn: "MaTaiKhoan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuon_BanDoc_MaBanDoc",
                table: "PhieuMuon",
                column: "MaBanDoc",
                principalTable: "BanDoc",
                principalColumn: "MaBanDoc",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sach_NhaXuatBan_MaNhaXuatBan",
                table: "Sach",
                column: "MaNhaXuatBan",
                principalTable: "NhaXuatBan",
                principalColumn: "MaNhaXuatBan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sach_TheLoai_MaTheLoai",
                table: "Sach",
                column: "MaTheLoai",
                principalTable: "TheLoai",
                principalColumn: "MaTheLoai",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
