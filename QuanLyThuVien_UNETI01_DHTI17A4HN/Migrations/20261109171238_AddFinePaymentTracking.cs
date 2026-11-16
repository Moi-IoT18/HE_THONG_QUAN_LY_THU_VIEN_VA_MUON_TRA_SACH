using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class AddFinePaymentTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaNopTienPhat",
                table: "PhieuMuon",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HinhThucNopPhat",
                table: "PhieuMuon",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayNopTienPhat",
                table: "PhieuMuon",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiThuTienPhat",
                table: "PhieuMuon",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuMuon_NguoiThuTienPhat",
                table: "PhieuMuon",
                column: "NguoiThuTienPhat");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuon_TaiKhoan_NguoiThuTienPhat",
                table: "PhieuMuon",
                column: "NguoiThuTienPhat",
                principalTable: "TaiKhoan",
                principalColumn: "MaTaiKhoan",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuon_TaiKhoan_NguoiThuTienPhat",
                table: "PhieuMuon");

            migrationBuilder.DropIndex(
                name: "IX_PhieuMuon_NguoiThuTienPhat",
                table: "PhieuMuon");

            migrationBuilder.DropColumn(
                name: "DaNopTienPhat",
                table: "PhieuMuon");

            migrationBuilder.DropColumn(
                name: "HinhThucNopPhat",
                table: "PhieuMuon");

            migrationBuilder.DropColumn(
                name: "NgayNopTienPhat",
                table: "PhieuMuon");

            migrationBuilder.DropColumn(
                name: "NguoiThuTienPhat",
                table: "PhieuMuon");
        }
    }
}
