// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TopKMapCommand.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the TopKMapCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Commands
{
    using System;
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    /// <summary>
    /// Defines the <code>topkmap</code> command.
    /// </summary>
    public class TopKMapCommand : ICommand
    {
        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "topkmap";
            }
        }

        /// <inheritdoc/>
        public string Args { get; set; }

        /// <inheritdoc/>
        public string Help
        {
            get
            {
                return Properties.Resources.Command_TopKMap_Help;
            }
        }
    }

    /// <summary>
    /// The <code>topkmap</code> command handler.
    /// </summary>
    public class TopKMapCommandHandler : ICommandHandler<DefinekeyCommand>
    {
        /// <inheritdoc/>
        public Task<bool> Execute(DefinekeyCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            throw new System.NotImplementedException();
        }
    }
}
