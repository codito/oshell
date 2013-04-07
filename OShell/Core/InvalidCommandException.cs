//-----------------------------------------------------------------------
// <copyright file="InvalidCommandException.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core
{
    using System;

    /// <summary>
    /// The invalid command exception.
    /// </summary>
    public sealed class InvalidCommandException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException"/> class.
        /// </summary>
        /// <param name="inputCommand">Command specification</param>
        public InvalidCommandException(string inputCommand)
            : base(String.Format("command: invalid command '{0}' specified.", inputCommand))
        {
            this.Data.Add("InputCommand", inputCommand);
        }
    }
}