namespace LibraryReporter.Models.Reader
{
    public class ReaderCreationViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        public DateOnly Birtday { get; set; }

        public bool Success { get; set; }


    }
}
