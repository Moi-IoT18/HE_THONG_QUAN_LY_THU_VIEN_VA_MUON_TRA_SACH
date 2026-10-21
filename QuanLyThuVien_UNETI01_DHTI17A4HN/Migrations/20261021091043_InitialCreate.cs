using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NhaXuatBan",
                columns: table => new
                {
                    MaNhaXuatBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhaXuatBan = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaXuatBan", x => x.MaNhaXuatBan);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    VaiTro = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "TheLoai",
                columns: table => new
                {
                    MaTheLoai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTheLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    ThuTuHienThi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheLoai", x => x.MaTheLoai);
                });

            migrationBuilder.CreateTable(
                name: "BanDoc",
                columns: table => new
                {
                    MaBanDoc = table.Column<int>(type: "int", nullable: false),
                    SoThe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanDoc", x => x.MaBanDoc);
                    table.ForeignKey(
                        name: "FK_BanDoc_TaiKhoan_MaBanDoc",
                        column: x => x.MaBanDoc,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sach",
                columns: table => new
                {
                    MaSach = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSach = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TacGia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NamXuatBan = table.Column<int>(type: "int", nullable: false),
                    GiaTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    MaTheLoai = table.Column<int>(type: "int", nullable: false),
                    MaNhaXuatBan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sach", x => x.MaSach);
                    table.ForeignKey(
                        name: "FK_Sach_NhaXuatBan_MaNhaXuatBan",
                        column: x => x.MaNhaXuatBan,
                        principalTable: "NhaXuatBan",
                        principalColumn: "MaNhaXuatBan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sach_TheLoai_MaTheLoai",
                        column: x => x.MaTheLoai,
                        principalTable: "TheLoai",
                        principalColumn: "MaTheLoai",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuMuon",
                columns: table => new
                {
                    MaPhieuMuon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBanDoc = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    NgayMuon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanTra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuMuon", x => x.MaPhieuMuon);
                    table.ForeignKey(
                        name: "FK_PhieuMuon_BanDoc_MaBanDoc",
                        column: x => x.MaBanDoc,
                        principalTable: "BanDoc",
                        principalColumn: "MaBanDoc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuMuon_TaiKhoan_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietMuon",
                columns: table => new
                {
                    MaChiTietMuon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieuMuon = table.Column<int>(type: "int", nullable: false),
                    MaSach = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DaTra = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietMuon", x => x.MaChiTietMuon);
                    table.ForeignKey(
                        name: "FK_ChiTietMuon_PhieuMuon_MaPhieuMuon",
                        column: x => x.MaPhieuMuon,
                        principalTable: "PhieuMuon",
                        principalColumn: "MaPhieuMuon",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietMuon_Sach_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Sach",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TaiKhoan",
                columns: new[] { "MaTaiKhoan", "Email", "HoTen", "MatKhau", "NgayTao", "TenDangNhap", "TrangThai", "VaiTro" },
                values: new object[,]
                {
                    { 1, "admin@uneti.edu.vn", "Qu?n Tr? Viên", "123456", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin", true, 1 },
                    { 2, "thuthu@uneti.edu.vn", "Th? Thu Viên", "123456", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "thuthu01", true, 2 }
                });

            migrationBuilder.InsertData(
                table: "TheLoai",
                columns: new[] { "MaTheLoai", "MoTa", "TenTheLoai", "ThuTuHienThi", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Sách chuyên ngành CNTT và Ph?n m?m", "Công ngh? thông tin", 1, true },
                    { 2, "Sách qu?n tr? kinh doanh và tài chính", "Kinh t? - Qu?n tr?", 2, true },
                    { 3, "Sách h?c Ti?ng Anh, Ti?ng Nh?t, Ti?ng Trung", "Ngo?i ng?", 3, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietMuon_MaPhieuMuon",
                table: "ChiTietMuon",
                column: "MaPhieuMuon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietMuon_MaSach",
                table: "ChiTietMuon",
                column: "MaSach");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuMuon_MaBanDoc",
                table: "PhieuMuon",
                column: "MaBanDoc");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuMuon_MaNhanVien",
                table: "PhieuMuon",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_Sach_MaNhaXuatBan",
                table: "Sach",
                column: "MaNhaXuatBan");

            migrationBuilder.CreateIndex(
                name: "IX_Sach_MaTheLoai",
                table: "Sach",
                column: "MaTheLoai");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_Email",
                table: "TaiKhoan",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietMuon");

            migrationBuilder.DropTable(
                name: "PhieuMuon");

            migrationBuilder.DropTable(
                name: "Sach");

            migrationBuilder.DropTable(
                name: "BanDoc");

            migrationBuilder.DropTable(
                name: "NhaXuatBan");

            migrationBuilder.DropTable(
                name: "TheLoai");

            migrationBuilder.DropTable(
                name: "TaiKhoan");
        }
    }
}
