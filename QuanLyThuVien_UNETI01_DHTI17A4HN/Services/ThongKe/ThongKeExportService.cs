using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.ThongKe;

public sealed class ThongKeExportService : IThongKeExportService
{
    private const int TableHeaderRow = 4;
    private const double PageWidth = 595;
    private const double PageHeight = 842;
    private const double TableLeft = 36;
    private const double TableWidth = 523;
    private static readonly Regex MultipleSpaces = new(@"\s+", RegexOptions.Compiled);

    public byte[] TaoExcel(ThongKeViewModel model)
    {
        using var workbook = new XLWorkbook();
        workbook.Style.Font.FontName = "Aptos";
        workbook.Style.Font.FontSize = 10;

        var summary = TaoTrangTinh(workbook, "Tổng quan", "Các chỉ số vận hành và công nợ tiền phạt.", 2);
        summary.Cell(TableHeaderRow, 1).Value = "CHỈ SỐ";
        summary.Cell(TableHeaderRow, 2).Value = "GIÁ TRỊ";
        var summaryRows = new (string Label, object Value, string? Format)[]
        {
            ("Phiếu quá hạn", model.SoPhieuQuaHan, null),
            ("Tổng tiền phạt đã phát sinh", model.TongTienPhat, "#,##0 \"đ\""),
            ("Tiền phạt đã thu", model.TongTienPhatDaNop, "#,##0 \"đ\""),
            ("Tiền phạt còn phải thu", model.TongTienPhatChuaNop, "#,##0 \"đ\""),
            ("Phiếu đã trả", model.TongPhieuDaTra, null),
            ("Phiếu trả đúng hạn", model.PhieuTraDungHan, null),
            ("Tỷ lệ trả đúng hạn", model.TyLeTraDungHan / 100m, "0.00%"),
        };
        for (var index = 0; index < summaryRows.Length; index++)
        {
            var row = TableHeaderRow + index + 1;
            summary.Cell(row, 1).Value = summaryRows[index].Label;
            summary.Cell(row, 2).Value = XLCellValue.FromObject(summaryRows[index].Value);
            if (summaryRows[index].Format is { } format)
                summary.Cell(row, 2).Style.NumberFormat.Format = format;
        }
        DinhDangBang(summary, TableHeaderRow, TableHeaderRow + summaryRows.Length, 2);
        summary.Column(1).Width = 36;
        summary.Column(2).Width = 22;

        var byCategory = TaoTrangTinh(workbook, "Sách theo thể loại", "Quy mô kho sách theo nhóm chủ đề.", 3);
        byCategory.Cell(TableHeaderRow, 1).Value = "Thể loại";
        byCategory.Cell(TableHeaderRow, 2).Value = "Đầu sách";
        byCategory.Cell(TableHeaderRow, 3).Value = "Bản sách";
        for (var index = 0; index < model.SachTheoTheLoai.Count; index++)
        {
            var row = TableHeaderRow + index + 1;
            var item = model.SachTheoTheLoai[index];
            byCategory.Cell(row, 1).Value = item.TenTheLoai;
            byCategory.Cell(row, 2).Value = item.SoDauSach;
            byCategory.Cell(row, 3).Value = item.TongSoBan;
        }
        DinhDangBang(byCategory, TableHeaderRow, TableHeaderRow + model.SachTheoTheLoai.Count, 3);

        var monthly = TaoTrangTinh(workbook, "Lượt mượn theo tháng", "Số phiếu mượn được duyệt trong 12 tháng gần nhất.", 2);
        monthly.Cell(TableHeaderRow, 1).Value = "Tháng";
        monthly.Cell(TableHeaderRow, 2).Value = "Lượt mượn";
        for (var index = 0; index < model.LuotMuonTheoThang.Count; index++)
        {
            var row = TableHeaderRow + index + 1;
            var item = model.LuotMuonTheoThang[index];
            monthly.Cell(row, 1).Value = $"{item.Thang:D2}/{item.Nam}";
            monthly.Cell(row, 2).Value = item.SoLuotMuon;
        }
        DinhDangBang(monthly, TableHeaderRow, TableHeaderRow + model.LuotMuonTheoThang.Count, 2);

        var popularBooks = TaoTrangTinh(workbook, "Sách được mượn nhiều", "Xếp hạng theo tổng số bản đã được mượn.", 3);
        popularBooks.Cell(TableHeaderRow, 1).Value = "Mã sách";
        popularBooks.Cell(TableHeaderRow, 2).Value = "Tên sách";
        popularBooks.Cell(TableHeaderRow, 3).Value = "Số bản mượn";
        for (var index = 0; index < model.SachMuonNhieu.Count; index++)
        {
            var row = TableHeaderRow + index + 1;
            var item = model.SachMuonNhieu[index];
            popularBooks.Cell(row, 1).Value = item.MaSach;
            popularBooks.Cell(row, 2).Value = item.TenSach;
            popularBooks.Cell(row, 3).Value = item.SoLuongMuon;
        }
        DinhDangBang(popularBooks, TableHeaderRow, TableHeaderRow + model.SachMuonNhieu.Count, 3);

        var popularReaders = TaoTrangTinh(workbook, "Bạn đọc mượn nhiều", "Xếp hạng theo số phiếu mượn.", 2);
        popularReaders.Cell(TableHeaderRow, 1).Value = "Bạn đọc";
        popularReaders.Cell(TableHeaderRow, 2).Value = "Lượt mượn";
        for (var index = 0; index < model.BanDocMuonNhieu.Count; index++)
        {
            var row = TableHeaderRow + index + 1;
            var item = model.BanDocMuonNhieu[index];
            popularReaders.Cell(row, 1).Value = item.HoTen;
            popularReaders.Cell(row, 2).Value = item.SoLuotMuon;
        }
        DinhDangBang(popularReaders, TableHeaderRow, TableHeaderRow + model.BanDocMuonNhieu.Count, 2);

        using var output = new MemoryStream();
        workbook.SaveAs(output);
        return output.ToArray();
    }

    public byte[] TaoPdf(ThongKeViewModel model)
    {
        var pages = new List<PdfPage>();
        var summary = TaoTrangPdf(pages);
        VeTongQuan(summary.Content, model);

        VeBangPdf(pages, "Sách theo thể loại",
            ["Thể loại", "Đầu sách", "Bản sách"],
            model.SachTheoTheLoai.Select(item => new[]
            {
                item.TenTheLoai, item.SoDauSach.ToString("N0"), item.TongSoBan.ToString("N0"),
            }).ToArray(),
            [335, 90, 98]);
        VeBangPdf(pages, "Lượt mượn theo tháng",
            ["Tháng", "Lượt mượn"],
            model.LuotMuonTheoThang.Select(item => new[]
            {
                $"{item.Thang:D2}/{item.Nam}", item.SoLuotMuon.ToString("N0"),
            }).ToArray(),
            [300, 223]);
        VeBangPdf(pages, "Sách được mượn nhiều",
            ["Mã sách", "Tên sách", "Số bản mượn"],
            model.SachMuonNhieu.Select(item => new[]
            {
                item.MaSach.ToString("N0"), item.TenSach, item.SoLuongMuon.ToString("N0"),
            }).ToArray(),
            [70, 350, 103]);
        VeBangPdf(pages, "Bạn đọc mượn nhiều",
            ["Bạn đọc", "Lượt mượn"],
            model.BanDocMuonNhieu.Select(item => new[]
            {
                item.HoTen, item.SoLuotMuon.ToString("N0"),
            }).ToArray(),
            [420, 103]);

        using var output = new MemoryStream();
        WriteAscii(output, "%PDF-1.4\n%UNETI library report\n");
        var objectCount = 4 + pages.Count * 2;
        var offsets = new long[objectCount + 1];
        WriteObject(output, 1, "<< /Type /Catalog /Pages 2 0 R >>", offsets);
        var pageIds = Enumerable.Range(0, pages.Count).Select(index => 5 + index * 2).ToArray();
        WriteObject(output, 2,
            $"<< /Type /Pages /Kids [{string.Join(" ", pageIds.Select(id => $"{id} 0 R"))}] /Count {pages.Count} >>",
            offsets);
        WriteObject(output, 3,
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>", offsets);
        WriteObject(output, 4,
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>", offsets);

        for (var index = 0; index < pages.Count; index++)
        {
            var pageId = pageIds[index];
            var contentId = pageId + 1;
            VeKhungTrang(pages[index].Content, index + 1, pages.Count);
            var content = Encoding.ASCII.GetBytes(pages[index].Content.ToString());
            WriteObject(output, pageId,
                $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth} {PageHeight}] /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents {contentId} 0 R >>",
                offsets);
            WriteStreamObject(output, contentId, content, offsets);
        }

        var xrefOffset = output.Position;
        WriteAscii(output, $"xref\n0 {objectCount + 1}\n0000000000 65535 f \n");
        for (var id = 1; id <= objectCount; id++)
            WriteAscii(output, $"{offsets[id]:D10} 00000 n \n");
        WriteAscii(output, $"trailer\n<< /Size {objectCount + 1} /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF");
        return output.ToArray();
    }

    private static IXLWorksheet TaoTrangTinh(XLWorkbook workbook, string title, string subtitle, int lastColumn)
    {
        var sheet = workbook.Worksheets.Add(title);
        sheet.Range(1, 1, 1, lastColumn).Merge();
        sheet.Cell(1, 1).Value = "THƯ VIỆN UNETI  |  BÁO CÁO THỐNG KÊ";
        sheet.Range(1, 1, 1, lastColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#183B2B");
        sheet.Range(1, 1, 1, lastColumn).Style.Font.FontColor = XLColor.White;
        sheet.Range(1, 1, 1, lastColumn).Style.Font.Bold = true;
        sheet.Range(1, 1, 1, lastColumn).Style.Font.FontSize = 15;
        sheet.Row(1).Height = 32;

        sheet.Range(2, 1, 2, lastColumn).Merge();
        sheet.Cell(2, 1).Value = $"{title}  ·  {subtitle}  ·  Xuất lúc {DateTime.Now:dd/MM/yyyy HH:mm}";
        sheet.Range(2, 1, 2, lastColumn).Style.Font.FontColor = XLColor.FromHtml("#64746A");
        sheet.Range(2, 1, 2, lastColumn).Style.Font.Italic = true;
        sheet.Row(2).Height = 25;

        sheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        sheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
        sheet.PageSetup.FitToPages(1, 0);
        return sheet;
    }

    private static void DinhDangBang(IXLWorksheet sheet, int headerRow, int lastRow, int lastColumn)
    {
        var header = sheet.Range(headerRow, 1, headerRow, lastColumn);
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#315B45");
        header.Style.Font.FontColor = XLColor.White;
        header.Style.Font.Bold = true;
        header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        sheet.Row(headerRow).Height = 25;

        for (var row = headerRow + 1; row <= lastRow; row++)
        {
            var dataRow = sheet.Range(row, 1, row, lastColumn);
            dataRow.Style.Fill.BackgroundColor = row % 2 == 0
                ? XLColor.FromHtml("#F1F5F0")
                : XLColor.White;
            dataRow.Style.Border.BottomBorder = XLBorderStyleValues.Hair;
            dataRow.Style.Border.BottomBorderColor = XLColor.FromHtml("#DCE5DD");
            sheet.Row(row).Height = 22;
        }

        if (lastRow > headerRow)
            sheet.Range(headerRow, 1, lastRow, lastColumn).SetAutoFilter();
        sheet.SheetView.FreezeRows(headerRow);
        sheet.Columns(1, lastColumn).AdjustToContents();
        for (var column = 1; column <= lastColumn; column++)
            sheet.Column(column).Width = Math.Min(Math.Max(sheet.Column(column).Width, 14), 46);
    }

    private static void VeTongQuan(StringBuilder content, ThongKeViewModel model)
    {
        DrawPdfText(content, "BAO CAO THONG KE THU VIEN", 36, 758, 22, "#183B2B", true);
        DrawPdfText(content, $"So lieu cap nhat: {DateTime.Now:dd/MM/yyyy HH:mm}", 36, 734, 10, "#66756B");

        var cards = new (string Label, string Value, string Color)[]
        {
            ("Phieu qua han", model.SoPhieuQuaHan.ToString("N0"), "#FFF4E4"),
            ("Tien phat phat sinh", $"{model.TongTienPhat:N0} d", "#F1F5F0"),
            ("Da thu", $"{model.TongTienPhatDaNop:N0} d", "#EAF4EA"),
            ("Con phai thu", $"{model.TongTienPhatChuaNop:N0} d", "#FFF0EB"),
            ("Phieu da tra", model.TongPhieuDaTra.ToString("N0"), "#F1F5F0"),
            ("Tra dung han", $"{model.TyLeTraDungHan:N2}%", "#EAF4EA"),
        };
        const double cardWidth = 165;
        const double cardHeight = 76;
        const double gap = 14;
        for (var index = 0; index < cards.Length; index++)
        {
            var column = index % 3;
            var row = index / 3;
            var x = 36 + column * (cardWidth + gap);
            var y = 625 - row * 96;
            DrawPdfRect(content, x, y, cardWidth, cardHeight, cards[index].Color);
            DrawPdfText(content, cards[index].Label, x + 12, y + 48, 9, "#65756A");
            DrawPdfText(content, cards[index].Value, x + 12, y + 22, 16, "#183B2B", true);
        }

        DrawPdfRect(content, 36, 380, 523, 78, "#F1F5F0");
        DrawPdfText(content, "DOI CHIEU TIEN PHAT", 50, 435, 10, "#183B2B", true);
        DrawPdfText(content, "Da thu + con phai thu = tong tien phat da phat sinh.", 50, 414, 10, "#34483A");
        DrawPdfText(content, "Tien phat chi duoc ghi nhan thanh toan sau khi nhan vien xac nhan thu du.", 50, 395, 9, "#66756B");
    }

    private static void VeBangPdf(
        List<PdfPage> pages, string title, IReadOnlyList<string> headers,
        IReadOnlyList<string[]> rows, IReadOnlyList<double> columnWidths)
    {
        var data = rows.Count == 0
            ? new[] { Enumerable.Range(0, headers.Count).Select(index => index == 0 ? "Chua co du lieu" : "").ToArray() }
            : rows;
        foreach (var group in data.Chunk(24))
        {
            var page = TaoTrangPdf(pages);
            DrawPdfText(page.Content, title.ToUpperInvariant(), 36, 754, 16, "#183B2B", true);
            DrawPdfText(page.Content, "THONG KE TONG HOP", 36, 736, 9, "#66756B");

            var headerY = 696.0;
            DrawPdfRect(page.Content, TableLeft, headerY, TableWidth, 27, "#315B45");
            var columnX = TableLeft;
            for (var column = 0; column < headers.Count; column++)
            {
                DrawPdfText(page.Content, headers[column], columnX + 9, headerY + 9, 9, "#FFFFFF", true);
                columnX += columnWidths[column];
            }

            for (var row = 0; row < group.Length; row++)
            {
                var top = headerY - row * 23;
                var bottom = top - 23;
                DrawPdfRect(page.Content, TableLeft, bottom, TableWidth, 23,
                    row % 2 == 0 ? "#F1F5F0" : "#FFFFFF");
                DrawPdfLine(page.Content, TableLeft, bottom, TableLeft + TableWidth, bottom, "#DCE5DD");

                columnX = TableLeft;
                for (var column = 0; column < headers.Count; column++)
                {
                    var value = column < group[row].Length ? group[row][column] : "";
                    var maxLength = Math.Max(5, (int)(columnWidths[column] / 5.2));
                    DrawPdfText(page.Content, FitText(value, maxLength), columnX + 9, bottom + 7, 9, "#26382D");
                    columnX += columnWidths[column];
                }
            }
        }
    }

    private static PdfPage TaoTrangPdf(List<PdfPage> pages)
    {
        var page = new PdfPage();
        pages.Add(page);
        return page;
    }

    private static void VeKhungTrang(StringBuilder content, int pageNumber, int pageCount)
    {
        DrawPdfRect(content, 0, 790, PageWidth, 52, "#183B2B");
        DrawPdfText(content, "THU VIEN UNETI", 36, 811, 13, "#FFFFFF", true);
        DrawPdfText(content, "BAO CAO THONG KE", 390, 812, 9, "#E4ECE4", true);
        DrawPdfLine(content, 36, 43, 559, 43, "#DCE5DD");
        DrawPdfText(content, "He thong quan ly thu vien va muon tra sach", 36, 25, 8, "#66756B");
        DrawPdfText(content, $"Trang {pageNumber}/{pageCount}", 505, 25, 8, "#66756B");
    }

    private static void DrawPdfRect(StringBuilder content, double x, double y, double width, double height, string color)
    {
        content.Append(ColorCommand(color, fill: true))
            .Append(FormattableString.Invariant($"{x:0.##} {y:0.##} {width:0.##} {height:0.##} re f\n"));
    }

    private static void DrawPdfLine(StringBuilder content, double x1, double y1, double x2, double y2, string color)
    {
        content.Append(ColorCommand(color, fill: false))
            .Append(FormattableString.Invariant($"{x1:0.##} {y1:0.##} m {x2:0.##} {y2:0.##} l S\n"));
    }

    private static void DrawPdfText(
        StringBuilder content, string value, double x, double y, int fontSize, string color, bool bold = false)
    {
        var normalized = ToAscii(value);
        content.Append("BT /").Append(bold ? "F2" : "F1").Append(' ')
            .Append(fontSize).Append(" Tf ").Append(ColorCommand(color, fill: true))
            .Append(FormattableString.Invariant($"{x:0.##} {y:0.##} Td "))
            .Append('(').Append(EscapePdfText(normalized)).Append(") Tj ET\n");
    }

    private static string ColorCommand(string hexColor, bool fill)
    {
        var red = Convert.ToInt32(hexColor[1..3], 16) / 255d;
        var green = Convert.ToInt32(hexColor[3..5], 16) / 255d;
        var blue = Convert.ToInt32(hexColor[5..7], 16) / 255d;
        return FormattableString.Invariant($"{red:0.###} {green:0.###} {blue:0.###} {(fill ? "rg" : "RG")} ");
    }

    private static string FitText(string value, int maxLength)
        => value.Length <= maxLength ? value : value[..Math.Max(1, maxLength - 1)] + "…";

    private static string ToAscii(string value)
    {
        var normalized = value.Replace('đ', 'd').Replace('Đ', 'D')
            .Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                continue;
            builder.Append(character is >= ' ' and <= '~' ? character : ' ');
        }
        return MultipleSpaces.Replace(builder.ToString(), " ").Trim();
    }

    private static string EscapePdfText(string value)
        => value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("(", "\\(", StringComparison.Ordinal)
            .Replace(")", "\\)", StringComparison.Ordinal);

    private static void WriteObject(MemoryStream output, int id, string body, long[] offsets)
    {
        offsets[id] = output.Position;
        WriteAscii(output, $"{id} 0 obj\n{body}\nendobj\n");
    }

    private static void WriteStreamObject(MemoryStream output, int id, byte[] content, long[] offsets)
    {
        offsets[id] = output.Position;
        WriteAscii(output, $"{id} 0 obj\n<< /Length {content.Length} >>\nstream\n");
        output.Write(content);
        WriteAscii(output, "\nendstream\nendobj\n");
    }

    private static void WriteAscii(Stream output, string value)
        => output.Write(Encoding.ASCII.GetBytes(value));

    private sealed class PdfPage
    {
        public StringBuilder Content { get; } = new();
    }
}
