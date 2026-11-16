// Họ và tên: Phan Vũ Thanh Thư
// Mã sinh viên: 23103100316
// Nội dung thực hiện: Kiểm thử luồng trạng thái, giới hạn kết quả và validation entity.

using System.ComponentModel.DataAnnotations;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Models.Enums;

namespace QuanLyThuVien.Tests;

public sealed class WorkflowAndValidationTests
{
    [Theory]
    [InlineData(TrangThaiPhieu.ChoDuyet, TrangThaiPhieu.DangMuon, true)]
    [InlineData(TrangThaiPhieu.ChoDuyet, TrangThaiPhieu.DaHuy, true)]
    [InlineData(TrangThaiPhieu.DangMuon, TrangThaiPhieu.QuaHan, true)]
    [InlineData(TrangThaiPhieu.QuaHan, TrangThaiPhieu.DaTra, true)]
    [InlineData(TrangThaiPhieu.DaTra, TrangThaiPhieu.DaHuy, false)]
    [InlineData(TrangThaiPhieu.DaHuy, TrangThaiPhieu.DangMuon, false)]
    public void CoTheChuyen_EnforcesAllowedStateTransitions(
        TrangThaiPhieu current, TrangThaiPhieu target, bool expected)
    {
        Assert.Equal(expected, TrangThaiPhieuService.CoTheChuyen(current, target));
    }

    [Theory]
    [InlineData(1, 0, true)]
    [InlineData(-1, 0, false)]
    [InlineData(0, 0, false)]
    [InlineData(1, -1, false)]
    [InlineData(1, 1_000_000_000_001, false)]
    public void ChiTietMuon_ValidatesQuantityAndPenaltyRanges(int quantity, decimal penalty, bool expectedValid)
    {
        var detail = new ChiTietMuon { MaPhieuMuon = 1, MaSach = 1, SoLuong = quantity, TienPhat = penalty };
        var errors = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(
            detail, new ValidationContext(detail), errors, validateAllProperties: true);

        Assert.Equal(expectedValid, valid);
    }

    [Fact]
    public void TaiKhoan_RequiresUsernameAndPassword()
    {
        var account = new TaiKhoan { TenDangNhap = "", MatKhau = "", HoTen = "Test" };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(
            account, new ValidationContext(account), errors, validateAllProperties: true);

        Assert.False(valid);
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(TaiKhoan.TenDangNhap)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(TaiKhoan.MatKhau)));
    }
}
