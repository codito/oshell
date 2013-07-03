// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotificationService.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the INotificationService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    /// <summary>
    /// The NotificationService interface.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="format">Message format</param>
        /// <param name="args">Objects to show</param>
        void NotifyError(string format, params object[] args);

        /// <summary>
        /// Shows an Information message to the user.
        /// </summary>
        /// <param name="format">Message format</param>
        /// <param name="args">Objects to show</param>
        void NotifyInfo(string format, params object[] args);

        /// <summary>
        /// Shows a Debug message to the user.
        /// </summary>
        /// <param name="format">Message format</param>
        /// <param name="args">Objects to show</param>
        void NotifyDebug(string format, params object[] args);
    }
}
