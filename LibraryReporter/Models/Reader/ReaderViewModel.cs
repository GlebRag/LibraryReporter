using Enums.Status;

namespace LibraryReporter.Models.Reader
{
    public class ReaderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        public DateOnly Birtday { get; set; }

        public DateOnly AddedDate { get; set; }
    }
}