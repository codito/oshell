// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinekeyCommand.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
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
                return "definekey keymap key command\nAdd a new key binding in keymap for key to execute command."
                       + " Default keymaps are top normally only containing C-t, which reads a key from root, containing"
                       + " all the normal commands.\n\nNote that you have to describe ':' by 'colon', '!' by 'exclam' and"
                       + " so on. If you cannot guess a name of a key, try either C-t key and look at the error message, "
                       + "or try :describekey root and pressing the key.";
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
                throw new ArgumentException("Incorrect definekey command arguments: " + command.Args);
            }

            return Task.Run(() =>
                {
                    var keymap = this.keyMapService.GetKeyMapByName(args[0]);

                    var keySequence = new KeysConverter().ConvertFrom(null, CultureInfo.CurrentCulture, args[1]);
                    if (keySequence == null)
                    {
                        throw new ArgumentException("Invalid key sequence provided to definekey command. Keys: " + args[1]);
                    }

                    var actionArguments = string.Join(" ", args, 2, args.Length - 2);
                    keymap.RegisterAction((Keys)keySequence, (s) => this.commandService.Run(actionArguments));
                    return true;
                });
        }
    }
}