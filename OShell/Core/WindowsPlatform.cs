//-----------------------------------------------------------------------
// <copyright file="WindowsPlatform.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core
{
    using System.Windows.Forms;

    using OShell.Core.Contracts;
    using OShell.Core.Internal;

    /// <summary>
    /// Implementation of platform specific routines for the Windows OS.
    /// </summary>
    public class WindowsPlatform : IPlatformFacade
    {
        /// <inheritdoc/>
        public IMainWindow MainWindow { get; set; }

        /// <inheritdoc/>
        public bool RegisterHotKey(Keys key, int keyId)
        {
            int modifiers = 0;

            if ((key & Keys.Alt) == Keys.Alt)
            {
                modifiers = modifiers | (int)ModifierKey.Alt;
            }

            if ((key & Keys.Control) == Keys.Control)
            {
                modifiers = modifiers | (int)ModifierKey.Control;
            }

            if ((key & Keys.Shift) == Keys.Shift)
            {
                modifiers = modifiers | (int)ModifierKey.Shift;
            }

            Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
            return Interop.RegisterHotKey(this.MainWindow.GetHandle(), keyId, (uint)modifiers, (uint)k);
        }

        /// <inheritdoc/>
        public bool UnregisterHotKey(int keyId)
        {
            return Interop.UnregisterHotKey(this.MainWindow.GetHandle(), keyId);
        }

        /// <inheritdoc/>
        public bool RegisterShellHookWindow()
        {
            return Interop.RegisterShellHookWindow(this.MainWindow.GetHandle());
        }

        /// <inheritdoc/>
        public bool DeregisterShellHookWindow()
        {
            return Interop.DeregisterShellHookWindow(this.MainWindow.GetHandle());
        }
    }
}
