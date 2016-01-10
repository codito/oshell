// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWindowManagerService.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System;

    /// <summary>
    /// The IWindowManagerService interface.
    /// </summary>
    public interface IWindowManagerService
    {
        /// <summary>
        /// Adds a window to currently managed windows list.
        /// </summary>
        /// <param name="handle">Native window handle</param>
        void AddWindow(IntPtr handle);

        /// <summary>
        /// Removes a window from the currently managed list.
        /// </summary>
        /// <param name="handle">Native window handle</param>
        void RemoveWindow(IntPtr handle);
    }
}
