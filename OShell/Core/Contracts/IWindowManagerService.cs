namespace OShell.Core.Contracts
{
    using System;

    interface IWindowManagerService
    {
        void AddWindow(IntPtr intPtr);

        void RemoveWindow(IntPtr intPtr);
    }
}
