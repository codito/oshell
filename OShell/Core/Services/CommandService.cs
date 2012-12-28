//-----------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    public class CommandService : ServiceBase, ICommandService
    {
        private Dictionary<string, Func<object>> commandsMap;

        #region ServiceBase implementation
        public CommandService(MainWindow mainWindow, Dictionary<string, Func<object>> commandsMap)
            : base(mainWindow)
        {
            this.commandsMap = commandsMap;
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
            this.commandsMap.Clear();
            this.commandsMap = null;
        }
        #endregion

        #region ICommandService implementation
        public Task<bool> Run(string commandSpec)
        {
            // Sanitize command spec
            var commandName = commandSpec.Trim();
            if (string.IsNullOrEmpty(commandName))
            {
                throw new ArgumentException("Command specification cannot be null.", "commandSpec");
            }

            var commandHandler = this.commandsMap[commandName]();
            return Task.Run(() => true);
        }
        #endregion
    }
}
