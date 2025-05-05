namespace LibraryReporter.Models.LibraryReport
{
    public class ActionsReportViewModel
    {
        public string ModerSurname { get; set; }
        public string Action { get; set; }

        public string Description { get; set; }

        public DateOnly ExecutionDate { get; set; }
    }
}
