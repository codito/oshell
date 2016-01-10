//-----------------------------------------------------------------------
// <copyright file="ICommandHandler.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System.Threading.Tasks;

    /// <summary>
    /// The CommandHandler interface.
    /// </summary>
    /// <typeparam name="TCommand">Command definition type</typeparam>
    public interface ICommandHandler<in TCommand>
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="command">Command definition.</param>
        /// <returns>A <see cref="Task"/> representing the command execution state.</returns>
        Task<bool> Execute(TCommand command);
    }
}