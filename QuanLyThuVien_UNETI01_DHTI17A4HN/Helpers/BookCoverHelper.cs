namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Helpers;

public static class BookCoverHelper
{
    private static readonly string[] BookPhotos =
    [
        "bookshelf-colorful.jpg",
        "real-book-cover.jpg",
        "library-wall.jpg",
        "curved-library.jpg",
        "book-stack.jpg",
        "antique-library.jpg",
    ];

    public static string GetCoverPath(string? title)
    {
        uint hash = 2166136261;
        foreach (var character in title?.Trim() ?? string.Empty)
            hash = unchecked((hash ^ char.ToUpperInvariant(character)) * 16777619);

        var photo = BookPhotos[(int)(hash % (uint)BookPhotos.Length)];
        return $"/images/books/{photo}";
    }
}
