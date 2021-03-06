﻿//-----------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Views
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using OShell.Core;
    using OShell.Core.Contracts;
    using OShell.Core.Internal;

    #pragma warning disable SA1306 // Field names must begin with lower-case letter
    #pragma warning disable SA1310 // Field names must not contain underscore

    /// <summary>
    /// The main window.
    /// </summary>
    internal sealed class MainWindow : Form, IMainWindow
    {
        private static uint WM_SHELLHOOK;
        private readonly IKeyMapService keyMapService;

        private readonly IWindowManagerService windowManagerService;

        private Keys activeHotKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="platformFacade">Platform implementation.</param>
        /// <param name="keyMapService">
        /// The key map service.
        /// </param>
        /// <param name="windowManagerService">
        /// The window manager service.
        /// </param>
        public MainWindow(IPlatformFacade platformFacade, IKeyMapService keyMapService, IWindowManagerService windowManagerService)
        {
            platformFacade.MainWindow = this;
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

        #region IMainWindow Implmentation

        /// <inheritdoc/>
        public IntPtr GetHandle()
        {
            return this.Handle;
        }

        /// <inheritdoc/>
        public void WaitForNextKeyAsync(Keys topKey)
        {
            this.ExecuteOnUIThread(() =>
            {
                this.activeHotKey = topKey;
                this.Activate();
                this.Show();
            });
        }
        #endregion

        #region Form Overrides

        /// <inheritdoc/>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            this.Hide();

            if (this.activeHotKey == Keys.None)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            var activeKeyMap = this.keyMapService.GetKeyMapByTopKey(this.activeHotKey);
            Task.Run(() => activeKeyMap.Execute(keyData, string.Empty))
                .ContinueWith(
                (task) =>
                    {
                        Logger.Instance
                              .Info(string.Format(
                                      "Fault on key press action. Key sequence: {0}. Exception: {1}",
                                      keyData,
                                      task.Exception));
                    },
                TaskContinuationOptions.OnlyOnFaulted);

            this.activeHotKey = Keys.None;

            return true;
        }

        /// <inheritdoc/>
        protected override void WndProc(ref Message m)
        {
            const int WM_DISPLAYCHANGE = 0x007e;
            const int WM_SETTINGCHANGE = 0x001A;
            const int WM_HOTKEY = 0x312;
            if (m.Msg == WM_SHELLHOOK)
            {
                switch (m.WParam.ToInt64())
                {
                    case (long)Interop.ShellHookMessages.HSHELL_RUDEAPPACTIVATED:
                    case (long)Interop.ShellHookMessages.HSHELL_WINDOWACTIVATED:
                        Logger.Instance.Debug("MainWindow: Shell hook: Window activated. HWnd = {0}", m.LParam);
                        break;
                    case (long)Interop.ShellHookMessages.HSHELL_WINDOWCREATED:
                        Logger.Instance.Debug("MainWindow: Shell hook: Window created. HWnd = {0}", m.LParam);
                        this.windowManagerService.AddWindow(m.LParam);
                        break;
                    case (long)Interop.ShellHookMessages.HSHELL_WINDOWDESTROYED:
                        Logger.Instance.Debug("MainWindow: Shell hook: Window destroyed. HWnd = {0}", m.LParam);
                        this.windowManagerService.RemoveWindow(m.LParam);
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
                    ////var width = (uint)(m.LParam.ToInt32() & 0xffff);
                    ////var height = (uint)(m.LParam.ToInt32() >> 16);
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

                    this.WaitForNextKeyAsync(keyData);
                    break;
                case WM_SETTINGCHANGE:
                    // TODO Handle add/remove of monitors to system
                    break;
            }

            base.WndProc(ref m);
        }
        #endregion

        private void MainWindowActivated(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            ////Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2,
            ////                Screen.PrimaryScreen.Bounds.Height / 2);
        }

        private void ExecuteOnUIThread(Action action)
        {
            var result = this.BeginInvoke(action);
            ////result.AsyncWaitHandle.WaitOne();
        }
    }
}
