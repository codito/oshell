﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DuplicateKeyBindingException.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OShell.Core
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Represents error when a binding already exists for the given key sequence.
    /// </summary>
    public sealed class DuplicateKeyBindingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyBindingException"/> class.
        /// </summary>
        /// <param name="keyData">Key sequence.</param>
        public DuplicateKeyBindingException(Keys keyData)
            : base(String.Format("definekey: binding already exists for '{0}' key", keyData))
        {
            this.Data.Add("KeyData", keyData);
        }
    }
}