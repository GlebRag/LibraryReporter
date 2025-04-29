using Enums.Status;

namespace LibraryReporter.Models.Author
{
    public class AuthorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }


        public DateOnly AddedDate { get; set; }
    }
}