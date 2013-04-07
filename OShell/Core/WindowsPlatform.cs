//-----------------------------------------------------------------------
// <copyright file="WindowsPlatform.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core
{
    using System;
    using System.Windows.Forms;

    using OShell.Core.Contracts;
    using OShell.Core.Internal;

    /// <summary>
    /// Implementation of platform specific routines for the Windows OS.
    /// </summary>
    public class WindowsPlatform : IPlatformFacade
    {
        /// <inheritdoc/>
        public bool RegisterHotKey(IntPtr handle, Keys key, int keyId)
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
            return Interop.RegisterHotKey(handle, keyId, (uint)modifiers, (uint)k);
        }

        /// <inheritdoc/>
        public bool UnregisterHotKey(IntPtr handle, int keyId)
        {
            return Interop.UnregisterHotKey(handle, keyId);
        }
    }
}
