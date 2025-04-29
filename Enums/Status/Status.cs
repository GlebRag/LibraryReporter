namespace Enums.Status
{
    [Flags]
    public enum Status
    {
        InArmas = 1,                // x 0000 0001
        InLibrary = 2,               // x 0000 0010
                                // x 0000 0100
                              // x 0000 1000 умножаем число на 2, т.е след роль будет числом 8
    }
}
