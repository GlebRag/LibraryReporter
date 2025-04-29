using Enums.Status;

namespace LibraryReporter.Models.Book
{
    public class BookViewModel
    {
        public string BookName { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }
        //public string? UrlCover { get; set; }
        public string Barcode { get; set; }

        public string Status { get; set; }
    }
}