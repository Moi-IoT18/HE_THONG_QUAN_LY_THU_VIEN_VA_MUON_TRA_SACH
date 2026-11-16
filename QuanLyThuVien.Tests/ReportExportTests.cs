using System.IO.Compression;
using System.Text;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Services.ThongKe;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien.Tests;

public sealed class ReportExportTests
{
    [Fact]
    public void TaoExcel_CreatesWorkbookWithAllReportSheets()
    {
        var service = new ThongKeExportService();
        var bytes = service.TaoExcel(TaoThongKe());
        using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
        var entryNames = archive.Entries.Select(entry => entry.FullName).ToHashSet(StringComparer.Ordinal);

        Assert.Contains("xl/workbook.xml", entryNames);
        Assert.Contains("xl/worksheets/sheet1.xml", entryNames);
        Assert.Contains("xl/worksheets/sheet5.xml", entryNames);
        using var sharedStrings = archive.GetEntry("xl/sharedStrings.xml")!.Open();
        using var reader = new StreamReader(sharedStrings);
        var summaryXml = reader.ReadToEnd();
        Assert.Contains("Tiền phạt đã thu", summaryXml, StringComparison.Ordinal);
        Assert.Contains("Tiền phạt còn phải thu", summaryXml, StringComparison.Ordinal);
    }

    [Fact]
    public void TaoPdf_CreatesValidMultiPagePdfStructure()
    {
        var model = new ThongKeViewModel
        {
            SachTheoTheLoai = Enumerable.Range(1, 100)
                .Select(index => new SachTheoTheLoaiViewModel
                {
                    TenTheLoai = $"Thể loại {index}",
                    SoDauSach = index,
                    TongSoBan = index * 2,
                })
                .ToArray(),
        };
        var bytes = new ThongKeExportService().TaoPdf(model);
        var pdf = Encoding.ASCII.GetString(bytes);

        Assert.StartsWith("%PDF-1.4", pdf, StringComparison.Ordinal);
        Assert.Contains("/Count 9", pdf, StringComparison.Ordinal);
        Assert.EndsWith("%%EOF", pdf, StringComparison.Ordinal);
        Assert.Contains("xref", pdf, StringComparison.Ordinal);
        Assert.Contains("Tien phat phat sinh", pdf, StringComparison.Ordinal);
        Assert.Contains("Con phai thu", pdf, StringComparison.Ordinal);
    }

    private static ThongKeViewModel TaoThongKe() => new()
    {
        SoPhieuQuaHan = 2,
        TongTienPhat = 50000,
        TongTienPhatDaNop = 20000,
        TongTienPhatChuaNop = 30000,
        TongPhieuDaTra = 8,
        PhieuTraDungHan = 6,
        TyLeTraDungHan = 75,
        SachTheoTheLoai = [new SachTheoTheLoaiViewModel { TenTheLoai = "Văn học", SoDauSach = 1, TongSoBan = 3 }],
        LuotMuonTheoThang = [new LuotMuonTheoThangViewModel { Thang = 11, Nam = 2026, SoLuotMuon = 4 }],
        SachMuonNhieu = [new SachMuonNhieuViewModel { MaSach = 1, TenSach = "Tác phẩm", SoLuongMuon = 2 }],
        BanDocMuonNhieu = [new BanDocMuonNhieuViewModel { HoTen = "Bạn đọc mẫu", SoLuotMuon = 2 }],
    };
}
