//-----------------------------------------------------------------------
// <copyright file="IPlatformFacade.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Abstracts platform related functionality.
    /// </summary>
    public interface IPlatformFacade
    {
        /// <summary>
        /// Registers a hot key with the system.
        /// </summary>
        /// <param name="handle">Handle of the Window which owns the key</param>
        /// <param name="key">Key sequence</param>
        /// <param name="keyId">Unique id for the key</param>
        /// <returns>True if successful</returns>
        bool RegisterHotKey(IntPtr handle, Keys key, int keyId);

        /// <summary>
        /// Unregisters a hot key.
        /// </summary>
        /// <param name="handle">Handle of the Window which owns the key</param>
        /// <param name="keyId">Unique id of the key</param>
        /// <returns>True if successful</returns>
        bool UnregisterHotKey(IntPtr handle, int keyId);
    }
}