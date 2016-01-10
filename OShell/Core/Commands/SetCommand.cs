//-----------------------------------------------------------------------
// <copyright file="SetCommand.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    /// <summary>
    /// The <code>set</code> command.
    /// </summary>
    public class SetCommand : ICommand
    {
        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "set";
            }
        }

        /// <inheritdoc/>
        public string Args { get; set; }

        /// <inheritdoc/>
        public string Help
        {
            get
            {
                return OShell.Properties.Resources.Command_Set_Help;
            }
        }
    }

    /// <summary>
    /// The <code>set</code> command handler.
    /// </summary>
    public class SetCommandHandler : ICommandHandler<SetCommand>
    {
        private static readonly NameValueCollection DefaultConfigurationMap =
            new NameValueCollection
                    {
                        { "topkmap", "top" },
                        { "border", "1" }
                    };

        private readonly INotificationService notificationService;

        private readonly NameValueCollection configurationMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetCommandHandler"/> class.
        /// </summary>
        /// <param name="notificationService">Reference to NotificationService implementation</param>
        public SetCommandHandler(INotificationService notificationService)
            : this(notificationService, DefaultConfigurationMap)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetCommandHandler"/> class.
        /// </summary>
        /// <param name="notificationService">Reference to NotificationService implementation</param>
        /// <param name="configurationMap">Collection of configuration options</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurationMap"/> is null or empty.
        /// </exception>
        protected SetCommandHandler(INotificationService notificationService, NameValueCollection configurationMap)
        {
            if (notificationService == null)
            {
                throw new ArgumentNullException(nameof(notificationService));
            }

            if (configurationMap == null)
            {
                throw new ArgumentNullException(nameof(configurationMap));
            }

            this.notificationService = notificationService;
            this.configurationMap = configurationMap;
        }

        /// <inheritdoc/>
        public Task<bool> Execute(SetCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return Task.Run(() =>
                {
                    if (string.IsNullOrWhiteSpace(command.Args))
                    {
                        var configurationValues = this.GetConfigurationValuesAsString(this.configurationMap.AllKeys);
                        this.notificationService.NotifyInfo(configurationValues);
                    }
                    else
                    {
                        var args = command.Args.Split(new[] { ' ' }, 2);
                        if (args.Count() == 1)
                        {
                            var configValue = this.GetConfigurationValuesAsString(args);
                            this.notificationService.NotifyInfo(configValue);
                        }
                        else
                        {
                            this.configurationMap.Set(name: args[0], value: args[1]);
                        }
                    }

                    return true;
                });
        }

        private string GetConfigurationValuesAsString(IEnumerable<string> configKeys)
        {
            var configurationValues = new StringBuilder();
            foreach (var key in configKeys)
            {
                var value = this.configurationMap[key] ?? "<undefined>";
                configurationValues.AppendLine(string.Format(CultureInfo.CurrentUICulture, "{0} = {1}", key, value));
            }

            return configurationValues.ToString();
        }
    }
}