using LibraryReporter.Models.Reader;

namespace LibraryReporter.Models.Book
{
    public class IssueBookViewModel
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }

        public int Reader { get; set; }
        /// <summary>
        /// Последний день - дата выдачи = срок на котороый выдается книга, по умолчанию это будет 2 недели
        /// </summary>
        public DateOnly IssueDate { get; set; } // Дата выдачи книги
        public DateOnly AcceptanceLastDate { get; set; } // Последний день принятия книги

        public List<ReaderSurnameAndIdViewModel>? Readers { get; set; }

    }
}
