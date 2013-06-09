// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMainWindow.cs" company="OShell Development Team">
//   Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System;

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
    }
}
