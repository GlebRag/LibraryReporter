using Enums.Status;

namespace LibraryReporter.Models.Book
{
    public class PublisherViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }

        public string Email { get; set; }
        public DateOnly AddedDate { get; set; }
    }
}