//-----------------------------------------------------------------------
// <copyright file="IKeyMapService.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System.Windows.Forms;

    /// <summary>
    /// The KeyMapService interface.
    /// </summary>
    public interface IKeyMapService
    {
        /// <summary>
        /// Creates a <see cref="KeyMap"/> with <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the key map</param>
        void AddKeyMap(string name);

        /// <summary>
        /// Registers the <paramref name="topKey"/> key sequence as a hot key. Associates
        /// a <see cref="KeyMap"/> instance to this hot key.
        /// </summary>
        /// <param name="name">Name of the <see cref="KeyMap"/></param>
        /// <param name="topKey">Hot key sequence</param>
        void SetTopKey(string name, Keys topKey);

        /// <summary>
        /// Unregisters the hot key sequence binding and removes the associated <see cref="KeyMap"/>.
        /// </summary>
        /// <param name="name">Name of the <see cref="KeyMap"/></param>
        void RemoveKeyMap(string name);

        /// <summary>
        /// Gets the <see cref="KeyMap"/> with <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name of the <see cref="KeyMap"/></param>
        /// <returns><see cref="KeyMap"/> instance associated with hot key</returns>
        KeyMap GetKeyMapByName(string name);

        /// <summary>
        /// Gets the <see cref="KeyMap"/> with top key sequence <paramref name="topKey"/>.
        /// </summary>
        /// <param name="topKey">Key sequence</param>
        /// <returns><see cref="KeyMap"/> instance associated with hot key</returns>
        KeyMap GetKeyMapByTopKey(Keys topKey);
    }
}
