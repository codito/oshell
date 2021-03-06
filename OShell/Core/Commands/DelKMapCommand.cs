﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelKMapCommand.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Defines the DelKMapCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Commands
{
    using System;
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    /// <summary>
    /// The <code>delkmap</code> command.
    /// </summary>
    public class DelKMapCommand : ICommand
    {
        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "delkmap";
            }
        }

        /// <inheritdoc/>
        public string Args { get; set; }

        /// <inheritdoc/>
        public string Help
        {
            get
            {
                return Properties.Resources.Command_DelKMap_Help;
            }
        }
    }

    /// <summary>
    /// The <code>delkmap</code> command handler.
    /// </summary>
    public class DelKMapCommandHandler : ICommandHandler<DelKMapCommand>
    {
        private readonly IKeyMapService keyMapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelKMapCommandHandler"/> class.
        /// </summary>
        /// <param name="keyMapService">
        /// The key map service.
        /// </param>
        public DelKMapCommandHandler(IKeyMapService keyMapService)
        {
            if (keyMapService == null)
            {
                throw new ArgumentNullException("keyMapService");
            }

            this.keyMapService = keyMapService;
        }

        /// <inheritdoc/>
        public async Task<bool> Execute(DelKMapCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            return await Task.Run(
                () =>
                    {
                        if (string.IsNullOrEmpty(command.Args))
                        {
                            return false;
                        }

                        this.keyMapService.RemoveKeyMap(command.Args);
                        return true;
                    });
        }
    }
}