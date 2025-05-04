namespace LibraryReporter.Services
{
    public class ActionsHelper
    {
        public static string GetActionDescription(Enums.Action.Actions action)
        {
            switch (action)
            {
                case Enums.Action.Actions.Delete:
                    return "Удалил(а)";
                case Enums.Action.Actions.Create:
                    return "Создал(а)";
                case Enums.Action.Actions.Edit:
                    return "Изменил(а)";
                case Enums.Action.Actions.Issue:
                    return "Выдал(а)";
                case Enums.Action.Actions.Acceptance:
                    return "Принял(а)";
                default:
                    return "Неизвестный статус";
            }
        }

        public static Enums.Action.Actions GetActionEnum(string description)
        {
            switch (description)
            {
                case "Удалил(а)":
                    return Enums.Action.Actions.Delete;
                case "Создал(а)":
                    return Enums.Action.Actions.Create;
                case "Изменил(а)":
                    return Enums.Action.Actions.Edit;
                case "Выдал(а)":
                    return Enums.Action.Actions.Issue;
                case "Принял(а)":
                    return Enums.Action.Actions.Acceptance;
                default:
                    throw new ArgumentException("Неизвестное описание статуса");
            }
        }
    }
}
