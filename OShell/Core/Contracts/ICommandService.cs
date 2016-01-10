// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandService.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System.Threading.Tasks;

    using OShell.Core.Commands;

    /// <summary>
    /// The CommandService interface.
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Parse a <code>commandSpec</code> to a <see cref="ICommand"/> object and execute the
        /// <see cref="ICommandHandler{TCommand}"/> for the command.
        /// </summary>
        /// <param name="commandSpec">String representing command and arguments</param>
        /// <returns>True if command executes successfully</returns>
        /// <remarks>
        /// CommandService services the user specified commands by converting them from <code>string</code>
        /// to appropriate <see cref="ICommandHandler{TCommand}"/>. See <see cref="DefinekeyCommandHandler"/> for
        /// an example.
        /// </remarks>
        Task<bool> Run(string commandSpec);
    }
}