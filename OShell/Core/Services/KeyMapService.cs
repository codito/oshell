//-----------------------------------------------------------------------
// <copyright file="KeyMapService.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using OShell.Core.Contracts;
    using OShell.Core.Internal;

    /// <summary>
    /// Implementation of IKeyMapService.
    /// </summary>
    public class KeyMapService : ServiceBase, IKeyMapService
    {
        /// <summary>
        /// An implementation for the underlying platform specific routines.
        /// </summary>
        private readonly IPlatformFacade platformFacade;

        /// <summary>
        /// Map of names and associated <see cref="KeyMap"/>.
        /// </summary>
        private Dictionary<string, KeyMap> nameToKeyMapMapping;

        /// <summary>
        /// Map of hot key sequences and associated <see cref="KeyMap"/>.
        /// </summary>
        private Dictionary<Keys, KeyMap> topKeyToKeyMapMapping;

        /// <summary>
        /// Creates an instance of <see cref="KeyMapService"/>.
        /// </summary>
        /// <param name="platformFacade">
        /// An <see cref="IPlatformFacade"/> implementation
        /// </param>
        public KeyMapService(IPlatformFacade platformFacade)
        {
            this.nameToKeyMapMapping = new Dictionary<string, KeyMap>();
            this.topKeyToKeyMapMapping = new Dictionary<Keys, KeyMap>();
            this.platformFacade = platformFacade;
        }

        #region ServiceBase implementation

        /// <summary>
        /// Implementation of <see cref="ServiceBase.Start"/>.
        /// </summary>
        public override void Start()
        {
        }

        /// <summary>
        /// Implementation of <see cref="ServiceBase.Stop"/>. Unregisters and removes all
        /// registered hot keys.
        /// </summary>
        public override void Stop()
        {
            if (this.nameToKeyMapMapping != null)
            {
                foreach (var keyMap in this.nameToKeyMapMapping.Values)
                {
                    this.platformFacade.UnregisterHotKey(keyMap.GetHashCode());
                }

                this.nameToKeyMapMapping.Clear();
                this.nameToKeyMapMapping = null;
            }

            if (this.topKeyToKeyMapMapping != null)
            {
                this.topKeyToKeyMapMapping.Clear();
                this.topKeyToKeyMapMapping = null;
            }
        }

        #endregion

        #region Public methods

        /// <inheritdoc/>
        public void AddKeyMap(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            var keyMap = new KeyMap("keymap");
            this.nameToKeyMapMapping.Add(name, keyMap);
        }

        /// <inheritdoc/>
        public void SetTopKey(string name, Keys topKey)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (topKey == Keys.None || topKey == Keys.NoName)
            {
                throw new ArgumentException("Top key must be a valid Key sequence.", "topKey");
            }

            var keyMap = this.GetKeyMapByName(name);
            if (!this.platformFacade.RegisterHotKey(topKey, keyMap.GetHashCode()))
            {
                Logger.GetLogger().Error("KeyMapService: Failed to register hot key. Keys = " + topKey);
                throw new Exception("Binding a hot key failed.");
            }

            this.topKeyToKeyMapMapping.Add(topKey, keyMap);
            keyMap.TopKey = topKey;
        }

        /// <inheritdoc/>
        public void RemoveKeyMap(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            var keyMap = this.GetKeyMapByName(name);
            this.nameToKeyMapMapping.Remove(name);
            if (keyMap.TopKey != Keys.None)
            {
                this.topKeyToKeyMapMapping.Remove(keyMap.TopKey);
            }

            if (!this.platformFacade.UnregisterHotKey(keyMap.GetHashCode()))
            {
                Logger.GetLogger().Error("KeyMapService: Failed to unregister hot key. Keys = " + name);
                throw new Exception("Unbinding a hot key failed.");
            }
        }

        /// <inheritdoc/>
        public KeyMap GetKeyMapByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return this.nameToKeyMapMapping[name];
        }

        /// <inheritdoc/>
        public KeyMap GetKeyMapByTopKey(Keys topKey)
        {
            if (topKey == Keys.None || topKey == Keys.NoName)
            {
                throw new ArgumentException("Top key must be a valid Key sequence.", "topKey");
            }

            return this.topKeyToKeyMapMapping[topKey];
        }

        #endregion
    }
}
