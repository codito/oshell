namespace OShell.Test.Doubles
{
    using System;
    using System.Windows.Forms;

    using OShell.Core.Contracts;

    internal class TestablePlatform : IPlatformFacade
    {
        public IMainWindow MainWindow { get; set; }

        public bool RegisterHotKey(Keys key, int keyId)
        {
            return true;
        }

        public bool UnregisterHotKey(int keyId)
        {
            return true;
        }

        public bool RegisterShellHookWindow()
        {
            throw new NotImplementedException();
        }

        public bool DeregisterShellHookWindow()
        {
            throw new NotImplementedException();
        }
    }
}