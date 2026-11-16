using QuanLyThuVien_UNETI01_DHTI17A4HN.ViewModels;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Services.ThongKe;

public interface IThongKeExportService
{
    byte[] TaoExcel(ThongKeViewModel model);
    byte[] TaoPdf(ThongKeViewModel model);
}
