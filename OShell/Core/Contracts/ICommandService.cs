//-----------------------------------------------------------------------
// <copyright file="ICommand.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System.Threading.Tasks;

    public interface  ICommandService
    {
        Task<bool> Run(string commandSpec);
    }
}