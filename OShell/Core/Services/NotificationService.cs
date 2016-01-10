//-----------------------------------------------------------------------
// <copyright file="NotificationService.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        public NotificationService()
        {
            this.logger = Logger.Instance;
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

        /// <inheritdoc/>
        public void NotifyDebug(string format, params object[] args)
        {
            this.NotifyInternal(Verbosity.Debug, format, args);
        }

        /// <inheritdoc/>
        public void NotifyError(string format, params object[] args)
        {
            this.NotifyInternal(Verbosity.Error, format, args);
        }

        /// <inheritdoc/>
        public void NotifyInfo(string format, params object[] args)
        {
            this.NotifyInternal(Verbosity.Info, format, args);
        }

        private void NotifyInternal(Verbosity level, string format, params object[] args)
        {
            ////if (this.HasMessageBar)
            ////    ;
            this.logger.Write(level, format, args);
        }
    }
}
