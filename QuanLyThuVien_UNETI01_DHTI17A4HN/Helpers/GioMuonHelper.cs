using Microsoft.AspNetCore.Http;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class GioMuonHelper
{
    private const string Key = "GioMuon";

    public static List<int> Lay(ISession session)
    {
        var value = session.GetString(Key);
        if (string.IsNullOrWhiteSpace(value))
            return new List<int>();

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(v => int.TryParse(v, out var id) ? id : 0)
            .Where(id => id > 0)
            .Distinct()
            .ToList();
    }
}
