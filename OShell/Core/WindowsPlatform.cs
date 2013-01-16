//-----------------------------------------------------------------------
// <copyright file="PlatformFacade.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Internal
{
    using System;
    using System.Windows.Forms;

    using OShell.Core.Contracts;

    public class WindowsPlatform : IPlatformFacade
    {
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

        public bool UnregisterHotKey(IntPtr handle, int keyId)
        {
            return Interop.UnregisterHotKey(handle, keyId);
        }
    }
}
