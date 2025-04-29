namespace LibraryReporter.Models.Book
{
    public class BookCreationViewModel
    {
        public string BookName { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }

        public List<AuthorSurnameAndIdViewModel>? Authors { get; set; }

        public List<PublisherNameAndIdViewModel>? Publishers { get; set; }
    }
}
