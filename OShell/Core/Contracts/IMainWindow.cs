// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMainWindow.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Base for the Main Window of the application. Register process life time objects with
    /// this Window (if the object requires a Handle).
    /// </summary>
    public interface IMainWindow
    {
        /// <summary>
        /// Gets the underlying windows handle for this MainWindow.
        /// </summary>
        /// <returns>Window Handle</returns>
        IntPtr GetHandle();

        /// <summary>
        /// Activate and show the <see cref="IMainWindow"/>, wait for next key sequence.
        /// </summary>
        /// <param name="topKey">Top key for the key sequence.</param>
        void WaitForNextKeyAsync(Keys topKey);
    }
}
