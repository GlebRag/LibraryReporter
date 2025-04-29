using Enums.Status;

namespace LibraryReporter.Models.Book
{
    public class ReaderViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        public DateOnly Birtday { get; set; }

        public DateOnly AddedDate { get; set; }
    }
}