// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandService.cs" company="OShell Development Team">
//   Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System.Threading.Tasks;

    /// <summary>
    /// The CommandService interface.
    /// </summary>
    public interface ICommandService
    {
        Task<bool> Run(string commandSpec);
    }
}