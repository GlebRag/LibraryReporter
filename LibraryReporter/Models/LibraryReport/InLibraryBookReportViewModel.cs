using Enums.Status;
using LibraryReporter.Models.Book;

namespace LibraryReporter.Models.LibraryReport
{
    public class InLibraryBookReportViewModel
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }

        public string Barcode { get; set; }

    }
}