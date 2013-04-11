//-----------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Views
{
    using System;

    using OShell.Core;
    using OShell.Core.Contracts;

    using System.Drawing;
    using System.Windows.Forms;

    using OShell.Core.Internal;

    internal sealed class MainWindowImpl : Form, IMainWindow 
    {
// ReSharper disable InconsistentNaming
        private static uint WM_SHELLHOOK;
// ReSharper restore InconsistentNaming

        private Keys activeHotKey;

        private readonly IKeyMapService keyMapService;

        private readonly IWindowManagerService windowManagerService;

        public MainWindowImpl(IKeyMapService keyMapService, IWindowManagerService windowManagerService)
        {
            // FIXME Ugly, needs refactor
            //this.keyMapService = Program.GetInstance<IKeyMapService>();
            //this.windowManagerService = Program.GetInstance<IWindowManagerService>();
            this.keyMapService = keyMapService;
            this.windowManagerService = windowManagerService;

            WM_SHELLHOOK = Interop.RegisterWindowMessage("SHELLHOOK");

            this.AllowTransparency = true;
            this.BackColor = Color.DarkGray;
            this.Cursor = Cursors.NoMove2D;
            this.FormBorderStyle = FormBorderStyle.None;
            this.KeyPreview = true;
            this.Opacity = 0.3;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;

            this.Activated += this.MainWindowActivated;
        }

        public IntPtr GetHandle()
        {
            return this.Handle;
        }

        private void MainWindowActivated(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            //Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2,
            //                Screen.PrimaryScreen.Bounds.Height / 2);
        }

        #region Form Overrides
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            this.Hide();

            if (this.activeHotKey == Keys.None)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            // FIXME
            var activeKeyMap = this.keyMapService.GetKeyMap(keyData);
            ////var activeKeyMap = Program.GetInstance<IKeyMapService>().GetKeyMap(keyData);

            activeKeyMap.Execute(keyData, string.Empty);

            this.activeHotKey = Keys.None;

            return true;
        }

        protected override void WndProc(ref Message m)
        {
// ReSharper disable InconsistentNaming
            const int WM_DISPLAYCHANGE = 0x007e;
            const int WM_SETTINGCHANGE = 0x001A;
            const int WM_HOTKEY = 0x312;
// ReSharper restore InconsistentNaming

            if (m.Msg == WM_SHELLHOOK)
            {
                //Logger.GetLogger().Debug(String.Format("MainWindow: Shell hook: Message. Type = {0}, HWnd = {1}, App = {2}", m.WParam, m.LParam, OShell.Core.Window.GetApplicationName(m.LParam)));
                switch (m.WParam.ToInt64())
                {
                    case (long)Interop.ShellHookMessages.HSHELL_RUDEAPPACTIVATED:
                    case (long)Interop.ShellHookMessages.HSHELL_WINDOWACTIVATED:
                        Logger.GetLogger().Debug("MainWindow: Shell hook: Window activated. HWnd = " + m.LParam);
                        break;
                    case (long)Interop.ShellHookMessages.HSHELL_WINDOWCREATED:
                        Logger.GetLogger().Debug("MainWindow: Shell hook: Window created. HWnd = " + m.LParam);
                        this.windowManagerService.AddWindow(m.LParam);
                        ////Program.GetInstance<IWindowManagerService>().AddWindow(m.LParam);
                        break;
                    case (long)Interop.ShellHookMessages.HSHELL_WINDOWDESTROYED:
                        Logger.GetLogger().Debug("MainWindow: Shell hook: Window destroyed. HWnd = " + m.LParam);
                        this.windowManagerService.RemoveWindow(m.LParam);
                        ////Program.GetInstance<IWindowManagerService>().RemoveWindow(m.LParam);
                        break;
                }
            }

            switch (m.Msg)
            {
                case WM_DISPLAYCHANGE:
                    // Handle resolution change for the system
                    // WParam = bit depth
                    // LParam = Width and Height
                    // TODO how does width/height come for secondary monitor
                    uint width = (uint)(m.LParam.ToInt32() & 0xffff);
                    uint height = (uint)(m.LParam.ToInt32() >> 16);
                    break;
                case WM_HOTKEY:
                    // WParam = key id of the hotkey which generated this message
                    // LParam = low word is modifier, high word is hot key
                    var keyData = (Keys)(m.LParam.ToInt32() >> 16);
                    int modifier = m.LParam.ToInt32() & 0xffff;
                    if ((modifier & (int)ModifierKey.Alt) == (int)ModifierKey.Alt)
                    {
                        keyData = keyData | Keys.Alt;
                    }
                    
                    if ((modifier & (int)ModifierKey.Control) == (int)ModifierKey.Control)
                    {
                        keyData = keyData | Keys.Control;
                    }
                    
                    if ((modifier & (int)ModifierKey.Shift) == (int)ModifierKey.Shift)
                    {
                        keyData = keyData | Keys.Shift;
                    }

                    this.activeHotKey = keyData;
                    this.Activate();
                    this.Show();
                    break;
                case WM_SETTINGCHANGE:
                    // TODO Handle add/remove of monitors to system
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion
    }
}
