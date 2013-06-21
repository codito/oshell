// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UndefinekeyCommand.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the UndefinekeyCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Commands
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using OShell.Core.Contracts;

    /// <summary>
    /// The <code>undefinekey</code> command.
    /// </summary>
    public class UndefinekeyCommand : ICommand
    {
        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "undefinekey";
            }
        }

        /// <inheritdoc/>
        public string Args { get; set; }

        /// <inheritdoc/>
        public string Help
        {
            get
            {
                return "undefinekey keymap key\nRemove the binding for key from keymap.";
            }
        }
    }

    /// <summary>
    /// The <code>undefinekey</code> command handler.
    /// </summary>
    public class UndefinekeyCommandHandler : ICommandHandler<UndefinekeyCommand>
    {
        private readonly IKeyMapService keyMapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndefinekeyCommandHandler"/> class.
        /// </summary>
        /// <param name="keyMapService">The <code>keymap</code> service.</param>
        public UndefinekeyCommandHandler(IKeyMapService keyMapService)
        {
            if (keyMapService == null)
            {
                throw new ArgumentNullException("keyMapService");
            }

            this.keyMapService = keyMapService;
        }

        /// <inheritdoc/>
        public Task<bool> Execute(UndefinekeyCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            var commandArgs = string.IsNullOrEmpty(command.Args) ? null : command.Args.Split(' ');
            if (commandArgs == null || commandArgs.Length != 2)
            {
                throw new ArgumentException("Incorrect definekey command arguments: " + command.Args);
            }

            return Task.Run(
                () =>
                    {
                        var keymap = this.keyMapService.GetKeyMapByName(commandArgs[0]);
                        var keySequence = new KeysConverter().ConvertFrom(null, CultureInfo.CurrentCulture, commandArgs[1]);
                        if (keySequence == null)
                        {
                            throw new ArgumentException(
                                "Invalid key sequence provided to undefinekey command. Keys: " + commandArgs[1]);
                        }

                        keymap.UnregisterAction((Keys)keySequence);
                        return true;
                    });
        }
    }
}
