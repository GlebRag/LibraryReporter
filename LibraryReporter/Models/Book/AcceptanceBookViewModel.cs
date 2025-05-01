using LibraryReporter.Models.Reader;

namespace LibraryReporter.Models.Book
{
    public class AcceptanceBookViewModel
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }
        //public string? UrlCover { get; set; }
        public string Barcode { get; set; }

        public DateOnly AcceptanceDate { get; set; }

        public string Reader { get; set; }
    }
}
