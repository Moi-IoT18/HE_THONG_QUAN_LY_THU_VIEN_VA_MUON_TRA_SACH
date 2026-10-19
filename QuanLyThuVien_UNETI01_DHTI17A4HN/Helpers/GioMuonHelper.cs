using System.Text.Json;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class GioMuonHelper
{
    private const string Khoa = "GioMuon";

    public static List<int> Lay(ISession session)
    {
        var json = session.GetString(Khoa);
        return string.IsNullOrEmpty(json)
            ? new List<int>()
            : JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
    }

    public static void Luu(ISession session, List<int> dsMaSach)
        => session.SetString(Khoa, JsonSerializer.Serialize(dsMaSach.Distinct().ToList()));

    public static void Xoa(ISession session) => session.Remove(Khoa);
}
