namespace LibraryReporter.Models.Book
{
    public class BookEditViewModel
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }

        public string Barcode { get; set; }

        public string Status { get; set; }

        public List<AuthorSurnameAndIdViewModel>? Authors { get; set; }

        public List<PublisherNameAndIdViewModel>? Publishers { get; set; }
    }
}
