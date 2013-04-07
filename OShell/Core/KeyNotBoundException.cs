// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyNotBoundException.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Represents error when a key sequence is not bound a command.
    /// </summary>
    public sealed class KeyNotBoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyNotBoundException"/> class.
        /// </summary>
        /// <param name="topKey">Root key</param>
        /// <param name="keyData">Auxiliary key sequence</param>
        public KeyNotBoundException(Keys topKey, Keys keyData)
            : base(String.Format("readkey: undefined '{0}' key", keyData))
        {
            this.Data.Add("TopKey", topKey);
            this.Data.Add("KeyData", keyData);
        }
    }
}