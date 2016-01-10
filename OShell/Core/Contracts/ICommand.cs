//-----------------------------------------------------------------------
// <copyright file="ICommand.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    /// <summary>
    /// Interface for commands.
    /// </summary>
    /// <remarks>
    /// Every command implements <code>ICommand</code>. Commands are passed to CommandHandlers with
    /// <code>Args</code> property set for execution.
    /// </remarks>
    public interface ICommand
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the arguments passed with the command.
        /// </summary>
        string Args { get; set; }

        /// <summary>
        /// Gets the help text for the command.
        /// </summary>
        string Help { get; }
    }
}