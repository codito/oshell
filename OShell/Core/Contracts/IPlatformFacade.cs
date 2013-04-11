//-----------------------------------------------------------------------
// <copyright file="IPlatformFacade.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System.Windows.Forms;

    /// <summary>
    /// Abstracts platform related functionality.
    /// </summary>
    public interface IPlatformFacade
    {
        /// <summary>
        /// Gets or sets the main window.
        /// </summary>
        IMainWindow MainWindow { get; set; }

        /// <summary>
        /// Registers a hot key with the system.
        /// </summary>
        /// <param name="key">Key sequence</param>
        /// <param name="keyId">Unique id for the key</param>
        /// <returns>True if successful</returns>
        bool RegisterHotKey(Keys key, int keyId);

        /// <summary>
        /// Unregisters a hot key.
        /// </summary>
        /// <param name="keyId">Unique id of the key</param>
        /// <returns>True if successful</returns>
        bool UnregisterHotKey(int keyId);

        /// <summary>
        /// Registers the current <see cref="MainWindow"/> as a shell hook window.
        /// </summary>
        /// <returns>True if registration is successful.</returns>
        bool RegisterShellHookWindow();

        /// <summary>
        /// Unregister the current <see cref="MainWindow"/> as a shell hook window.
        /// </summary>
        /// <returns>True if deregistration is successful.</returns>
        bool DeregisterShellHookWindow();
    }
}