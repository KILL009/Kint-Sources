namespace OpenNos.Domain
{
    public enum LoginFailType
    {
        OldClient = 1,
        UnhandledError,
        Maintenance,
        AlreadyConnected,
        AccountOrPasswordWrong,
        CantConnect,
        Banned,
        WrongCountr,
        WrongCaps
    }
}