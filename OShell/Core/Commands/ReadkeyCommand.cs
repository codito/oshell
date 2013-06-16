﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadkeyCommand.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadkeyCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Commands
{
    using System;
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    /// <summary>
    /// The <code>readkey</code> command.
    /// </summary>
    public class ReadkeyCommand : ICommand
    {
        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "readkey";
            }
        }

        /// <inheritdoc/>
        public string Args { get; set; }

        /// <inheritdoc/>
        public string Help
        {
            get
            {
                return "readkey keymap\nGrab the next key pressed, and execute the command associated to this key in keymap. To show it is waiting for a key, ratpoison will change the rat cursor to a square if waitcursor is set.\n"
                + "This command is perhaps best described with its usage in the default configuration: By pressing C-t, which is the only key in the keymap top, the command readkey root is executed. The next key then executes the "
                + "command in keymap root belonging to this command.";
            }
        }
    }

    /// <summary>
    /// The <code>readkey</code> command handler.
    /// </summary>
    public class ReadkeyCommandHandler : ICommandHandler<ReadkeyCommand>
    {
        private readonly IKeyMapService keyMapService;

        private readonly IMainWindow mainWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadkeyCommandHandler"/> class.
        /// </summary>
        /// <param name="keyMapService">Key map service.</param>
        /// <param name="mainWindow">Main window implementation.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="keyMapService"/> is null.</exception>
        public ReadkeyCommandHandler(IKeyMapService keyMapService, IMainWindow mainWindow)
        {
            if (keyMapService == null)
            {
                throw new ArgumentNullException("keyMapService");
            }

            if (mainWindow == null)
            {
                throw new ArgumentNullException("mainWindow");
            }

            this.keyMapService = keyMapService;
            this.mainWindow = mainWindow;
        }

        /// <inheritdoc/>
        public Task<bool> Execute(ReadkeyCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (string.IsNullOrEmpty(command.Args))
            {
                throw new ArgumentException("readkey must be provided with a valid keymap");
            }

            return Task.Run(
                () =>
                    {
                        // Activate the square cursor and wait for input
                        var keymap = this.keyMapService.GetKeyMapByName(command.Args);
                        this.mainWindow.WaitForNextKey(keymap.TopKey);
                        return true;
                    });
        }
    }
}