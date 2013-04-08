//-----------------------------------------------------------------------
// <copyright file="CommandService.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    /// <summary>
    /// The command service.
    /// </summary>
    public class CommandService : ServiceBase, ICommandService
    {
        /// <summary>
        /// The command handlers.
        /// </summary>
        private readonly IEnumerable<object> commandHandlers;

        /// <summary>
        /// The command instances.
        /// </summary>
        private Dictionary<string, ICommand> commandInstances;

        #region ServiceBase implementation

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandService"/> class.
        /// </summary>
        /// <param name="mainWindow">
        /// The main window.
        /// </param>
        /// <param name="commandInstances">
        /// The command instances.
        /// </param>
        /// <param name="commandHandlers">
        /// The command handlers.
        /// </param>
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
        /// <inheritdoc/>
        public async Task<bool> Run(string commandSpec)
        {
            // Sanitize command spec
            if (string.IsNullOrEmpty(commandSpec))
            {
                throw new ArgumentException("CommandService: Command specification cannot be null.", "commandSpec");
            }

            var commandParts = commandSpec.Trim().Split(new[] { ' ' });
            var commandName = commandParts[0];
            if (string.IsNullOrEmpty(commandName) || !this.commandInstances.ContainsKey(commandName))
            {
                throw new InvalidCommandException(commandSpec);
            }

            // Dynamic magic ensures the 'command' object is of TCommand
            dynamic command = this.commandInstances[commandName];

            // Get the ICommandHandler<TCommand> instance
            var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic commandHandler = (from handler in this.commandHandlers
                                      where handler.GetType().GetInterfaces().Any(i => i.UnderlyingSystemType.Equals(commandHandlerType))
                                      select handler).FirstOrDefault();
            if (commandHandler == null)
            {
                var ex =
                    new Exception(
                        String.Format(
                            "CommandService: Unable to create an instance of {0} for {1}.",
                            commandHandlerType,
                            commandName));
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
