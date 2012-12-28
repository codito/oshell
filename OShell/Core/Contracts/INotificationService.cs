namespace OShell.Core.Contracts
{
    public interface INotificationService
    {
        void NotifyError(string message);
        void NotifyInfo(string message);
        void NotifyDebug(string message);
    }
}
