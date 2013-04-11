//-----------------------------------------------------------------------
// <copyright file="NotificationService.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Services
{
    using OShell.Core.Contracts;
    using OShell.Core.Internal;

    /// <summary>
    /// Implements the notification service. Used to send messages to the message bar.
    /// </summary>
    public class NotificationService : ServiceBase, INotificationService
    {
        private readonly Logger logger;

        /// <summary>
        /// Initializes an instance of the <see cref="NotificationService"/> class.
        /// </summary>
        public NotificationService()
        {
            this.logger = Logger.GetLogger();
            this.logger.UseConsole = true;
            this.HasMessageBar = false;
        }

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether we have a message bar available. If false,
        /// messages are shown on standard output only.
        /// </summary>
        public bool HasMessageBar { get; set; }
        #endregion

        #region ServiceBase overrides
        /// <summary>
        /// Starts the service.
        /// </summary>
        public override void Start()
        {
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public override void Stop()
        {
        }
        #endregion

        /// <summary>
        /// Sends a debug notification.
        /// </summary>
        /// <param name="message">Message string</param>
        public void NotifyDebug(string message)
        {
            this.NotifyInternal(Verbosity.Debug, message);
        }

        /// <summary>
        /// Sends an error notification.
        /// </summary>
        /// <param name="message">Message string</param>
        public void NotifyError(string message)
        {
            this.NotifyInternal(Verbosity.Error, message);
        }

        /// <summary>
        /// Sends an information notification.
        /// </summary>
        /// <param name="message">Message string</param>
        public void NotifyInfo(string message)
        {
            this.NotifyInternal(Verbosity.Info, message);
        }

        private void NotifyInternal(Verbosity level, string message)
        {
            ////if (this.HasMessageBar)
            ////    ;
            this.logger.Write(level, message);
        }
    }
}
