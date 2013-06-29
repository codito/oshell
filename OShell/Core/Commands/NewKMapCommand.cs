// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewKMapCommand.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// <summary>
//   Defines the NewKMapCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core.Commands
{
    using System;
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    /// <summary>
    /// The <code>newkmap</code> command.
    /// </summary>
    public class NewKMapCommand : ICommand
    {
        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "newkmap";
            }
        }

        /// <inheritdoc/>
        public string Args { get; set; }

        /// <inheritdoc/>
        public string Help
        {
            get
            {
                return Properties.Resources.Command_Readkey_Help;
            }
        }
    }

    /// <summary>
    /// The <code>newkmap</code> command handler.
    /// </summary>
    public class NewKMapCommandHandler : ICommandHandler<NewKMapCommand>
    {
        private readonly IKeyMapService keyMapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewKMapCommandHandler"/> class.
        /// </summary>
        /// <param name="keyMapService">
        /// The key map service.
        /// </param>
        public NewKMapCommandHandler(IKeyMapService keyMapService)
        {
            if (keyMapService == null)
            {
                throw new ArgumentNullException("keyMapService");
            }

            this.keyMapService = keyMapService;
        }

        /// <inheritdoc/>
        public async Task<bool> Execute(NewKMapCommand command)
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

                        this.keyMapService.AddKeyMap(command.Args);
                        return true;
                    });
        }
    }
}