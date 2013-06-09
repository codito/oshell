//-----------------------------------------------------------------------
// <copyright file="ICommandHandler.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
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
        /// Execute the <see cref="TCommand"/>
        /// </summary>
        /// <param name="command">Command definition <see cref="TCommand"/>.</param>
        /// <returns>A <see cref="Task"/> representing the command execution state.</returns>
        Task<bool> Execute(TCommand command);
    }
}