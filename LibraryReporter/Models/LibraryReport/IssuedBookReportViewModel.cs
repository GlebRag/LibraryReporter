using Enums.Status;
using LibraryReporter.Models.Book;

namespace LibraryReporter.Models.LibraryReport
{
    public class IssuedBookReportViewModel
    {
        public string ModerSurname { get; set; }

        public string ReaderSurname { get; set; }
        public string BookName { get; set; }

        public string Author { get; set; }
        public string Publisher { get; set; }
        //public string? UrlCover { get; set; }
        public string Barcode { get; set; }

        public DateOnly IssueDate { get; set; } // Дата выдачи книги
        public DateOnly AcceptanceLastDate { get; set; } // Последний день принятия книги

        public List<AuthorSurnameAndIdViewModel>? Authors { get; set; }

        public List<PublisherNameAndIdViewModel>? Publishers { get; set; }
    }
}