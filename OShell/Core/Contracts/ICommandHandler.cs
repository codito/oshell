//-----------------------------------------------------------------------
// <copyright file="ICommandHandler.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System.Threading.Tasks;

    public interface ICommandHandler<in TCommand>
    {
        Task<bool> Execute(TCommand command);
    }
}