//-----------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
        IntPtr GetHandle();
    }
}
