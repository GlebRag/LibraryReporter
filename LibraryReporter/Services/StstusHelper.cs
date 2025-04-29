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

        public static Enums.Status.Status GetStatusEnum(string description)
        {
            switch (description)
            {
                case "На руках":
                    return Enums.Status.Status.InArmas;
                case "В библиотеке":
                    return Enums.Status.Status.InLibrary;
                default:
                    throw new ArgumentException("Неизвестное описание статуса");
            }
        }
    }
}
