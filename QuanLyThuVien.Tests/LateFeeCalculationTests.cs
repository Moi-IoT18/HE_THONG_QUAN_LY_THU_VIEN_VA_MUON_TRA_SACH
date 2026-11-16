using QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

namespace QuanLyThuVien.Tests;

public sealed class LateFeeCalculationTests
{
    [Theory]
    [InlineData(0, 3, 0)]
    [InlineData(1, 1, 5000)]
    [InlineData(2, 3, 30000)]
    public void TinhTienPhatTamTinh_CalculatesDaysRateAndQuantity(
        int daysLate, int quantity, decimal expected)
    {
        var dueDate = new DateTime(2026, 9, 15);
        var today = dueDate.AddDays(daysLate);

        var fee = QuaHanService.TinhTienPhatTamTinh(dueDate, quantity, today);

        Assert.Equal(expected, fee);
    }

    [Fact]
    public void TinhTienPhatTamTinh_RejectsNegativeQuantity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            QuaHanService.TinhTienPhatTamTinh(
                new DateTime(2026, 9, 15), -1, new DateTime(2026, 9, 16)));
    }
}
