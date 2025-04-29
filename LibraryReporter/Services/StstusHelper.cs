namespace LibraryReporter.Services
{
    public class StatusHelper
    {
        public static string GetStatusDescription(Enums.Status.Status status)
        {
            switch (status)
            {
                case Enums.Status.Status.InArmas:
                    return "На руках";
                case Enums.Status.Status.InLibrary:
                    return "В библиотеке";
                default:
                    return "Неизвестный статус";
            }
        }
    }
}
