//-----------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    public class CommandService : ServiceBase, ICommandService
    {
        private Dictionary<string, ICommand> commandInstances;

        private readonly IEnumerable<object> commandHandlers;

        #region ServiceBase implementation
        public CommandService(IMainWindow mainWindow, IEnumerable<ICommand> commandInstances, IEnumerable<object> commandHandlers)
            : base(mainWindow)
        {
            this.commandHandlers = commandHandlers;
            this.commandInstances = new Dictionary<string, ICommand>();
            foreach (var command in commandInstances)
            {
                this.commandInstances.Add(command.Name, command);
            }
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
            this.commandInstances.Clear();
            this.commandInstances = null;
        }
        #endregion

        #region ICommandService implementation
        public async Task<bool> Run(string commandSpec)
        {
            // Sanitize command spec
            var commandParts = commandSpec.Trim().Split(new[] { ' ' });
            var commandName = commandParts[0];
            if (string.IsNullOrEmpty(commandName))
            {
                throw new ArgumentException("CommandService: Command specification cannot be null.", "commandSpec");
            }

            // Dynamic magic ensures the 'command' object is of TCommand
            dynamic command = this.commandInstances[commandName];

            // Get the ICommandHandler<TCommand> instance
            var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic commandHandler = (from handler in this.commandHandlers
                                      where handler.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                                      select handler).FirstOrDefault();
            if (commandHandler == null)
            {
                var ex = new Exception(String.Format("CommandService: Unable to create an instance of {0} for {1}.",
                    commandHandlerType, commandName));
                ex.Data.Add("commandName", commandName);
                ex.Data.Add("commandHandlerType", commandHandlerType);
                throw ex;
            }
            command.Args = string.Join(" ", commandParts, 1, commandParts.Length - 1);
            // Invoke the command handler with the TCommand implementation
            return await commandHandler.Execute(command);
        }
        #endregion
    }
}
