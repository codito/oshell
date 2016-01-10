// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinekeyCommand.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Defines the DefinekeyCommand type.
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
    /// Defines the <code>definekey</code> command.
    /// </summary>
    public class DefinekeyCommand : ICommand
    {
        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "definekey";
            }
        }

        /// <inheritdoc/>
        public string Args { get; set; }

        /// <inheritdoc/>
        public string Help
        {
            get
            {
                return Properties.Resources.Command_Definekey_Help;
            }
        }
    }

    /// <summary>
    /// The <code>definekey</code> command handler.
    /// </summary>
    public class DefinekeyCommandHandler : ICommandHandler<DefinekeyCommand>
    {
        private readonly IKeyMapService keyMapService;

        private readonly ICommandService commandService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinekeyCommandHandler"/> class.
        /// </summary>
        /// <param name="keyMapService">The <see cref="IKeyMapService"/> implementation.</param>
        /// <param name="commandService">The <see cref="ICommandService"/> implementation</param>
        public DefinekeyCommandHandler(IKeyMapService keyMapService, ICommandService commandService)
        {
            if (keyMapService == null)
            {
                throw new ArgumentNullException("keyMapService");
            }

            if (commandService == null)
            {
                throw new ArgumentNullException("commandService");
            }

            this.keyMapService = keyMapService;
            this.commandService = commandService;
        }

        /// <inheritdoc/>
        public Task<bool> Execute(DefinekeyCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            var args = string.IsNullOrEmpty(command.Args) ? null : command.Args.Split(' ');
            if (args == null || args.Length < 3)
            {
                throw new ArgumentException("Incorrect definekey command arguments: " + command.Args, "command.Args");
            }

            return Task.Run(() =>
                {
                    var keymap = this.keyMapService.GetKeyMapByName(args[0]);
                    Keys keySequence = Keys.None;
                    Exception innerException = null;

                    try
                    {
                        keySequence = (Keys)new KeysConverter().ConvertFrom(null, CultureInfo.CurrentCulture, args[1]);
                    }
                    catch (ArgumentException ex)
                    {
                        innerException = ex;
                    }
                    catch (FormatException ex)
                    {
                        innerException = ex;
                    }

                    if (keySequence == Keys.None)
                    {
                        throw new ArgumentException(
                            "Invalid key sequence provided to definekey command. Keys: " + args[1],
                            "command",
                            innerException);
                    }

                    var actionArguments = string.Join(" ", args, 2, args.Length - 2);
                    keymap.RegisterAction(keySequence, (s) => this.commandService.Run(actionArguments));
                    return true;
                });
        }
    }
}