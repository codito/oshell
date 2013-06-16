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
        private readonly Dictionary<Keys, Func<string, Task<bool>>> actionMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMap"/> class.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="KeyMap"/>.
        /// </param>
        public KeyMap(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.Name = name;
            this.TopKey = Keys.None;
            this.actionMap = new Dictionary<Keys, Func<string, Task<bool>>>();
        }

        /// <summary>
        /// Gets the name of <see cref="KeyMap"/>.
        /// </summary>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the root key which triggers this <see cref="KeyMap"/>.
        /// </summary>
        public Keys TopKey
        {
            get; set;
        }

        /// <summary>
        /// Register an action for a sub key trigger.
        /// </summary>
        /// <param name="keyData">Trigger key sequence</param>
        /// <param name="action">Action to execute on trigger</param>
        public void RegisterAction(Keys keyData, Func<string, Task<bool>> action)
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

            return await this.actionMap[keyData](args);
        }
    }
}