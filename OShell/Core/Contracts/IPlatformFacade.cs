//-----------------------------------------------------------------------
// <copyright file="IPlatformFacade.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System;
    using System.Windows.Forms;

    public interface IPlatformFacade
    {
        bool RegisterHotKey(IntPtr handle, Keys key, int keyId);

        bool UnregisterHotKey(IntPtr handle, int keyId);
    }
}