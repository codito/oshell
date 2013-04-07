//-----------------------------------------------------------------------
// <copyright file="KeyMap.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using OShell.Core.Services;

    /// <summary>
    /// Available modifier keys.
    /// </summary>
    [Flags]
    public enum ModifierKey
    {
        /// <summary>
        /// ALT Key.
        /// </summary>
        Alt = 0x1,

        /// <summary>
        /// CTRL Key.
        /// </summary>
        Control = 0x2,

        /// <summary>
        /// Shift Key.
        /// </summary>
        Shift = 0x4,

        /// <summary>
        /// Windows Key.
        /// </summary>
        Win = 0x8
    }

    /// <summary>
    /// Represents a key map which can be triggered with a <see cref="TopKey"/>.
    /// </summary>
    public class KeyMap
    {
        /// <summary>
        /// Map of key sequence triggers and actions.
        /// </summary>
        private readonly Dictionary<Keys, Func<string, bool>> actionMap;

        /// <summary>
        /// Creates an instance of <see cref="KeyMap"/>.
        /// </summary>
        /// <param name="topKey">Root key sequence to trigger this key map</param>
        public KeyMap(Keys topKey)
        {
            this.actionMap = new Dictionary<Keys, Func<string, bool>>();
            this.TopKey = topKey;
        }

        /// <summary>
        /// Gets the root key which triggers this <see cref="KeyMap"/>.
        /// </summary>
        public Keys TopKey
        {
            get;
            private set;
        }

        /// <summary>
        /// Register an action for a sub key trigger.
        /// </summary>
        /// <param name="keyData">Trigger key sequence</param>
        /// <param name="action">Action to execute on trigger</param>
        public void RegisterAction(Keys keyData, Func<string, bool> action)
        {
            if (action == null)
            {
                throw new ArgumentException("Action cannot be null.", "action");
            }

            try
            {
                this.actionMap.Add(keyData, action);
            }
            catch (ArgumentException)
            {
                throw new DuplicateKeyBindingException(keyData);
            }
        }

        /// <summary>
        /// Removes a key sequence trigger.
        /// </summary>
        /// <param name="keyData">Key sequence to remove</param>
        public void UnregisterAction(Keys keyData)
        {
            this.actionMap.Remove(keyData);
        }

        /// <summary>
        /// Executes the action registered for a key sequence.
        /// </summary>
        /// <param name="keyData">Trigger key sequence</param>
        /// <param name="args">Arguments to the action</param>
        /// <returns>True if action executes successfully</returns>
        public async Task<bool> Execute(Keys keyData, string args)
        {
            if (!this.actionMap.ContainsKey(keyData))
            {
                throw new KeyNotBoundException(this.TopKey, keyData);
            }

            return await Task.Run(() => this.actionMap[keyData](args));
        }
    }
}