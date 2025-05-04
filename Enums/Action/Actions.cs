namespace Enums.Action
{
    [Flags]
    public enum Actions
    {
        Delete = 1,                // x 0000 0001
        Create = 2,               // x 0000 0010
        Edit = 4,
        Issue = 8,
        Acceptance = 16             // x 0000 0100
                                     // x 0000 1000 умножаем число на 2, т.е след роль будет числом 8
    }
}
