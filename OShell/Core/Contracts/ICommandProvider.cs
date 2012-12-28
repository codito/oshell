namespace OShell.Core.Contracts
{
    using System;

    /// <summary>
    /// Exposes a factory pattern to obtain a ICommand and ICommandHandler.
    /// </summary>
    public interface ICommandProvider
    {
        ICommand GetCommand(string command);

        Object GetCommandHandler(string command);

        bool HasCommand(string command);
    }
}
