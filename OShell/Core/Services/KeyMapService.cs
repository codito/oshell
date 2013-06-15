﻿//-----------------------------------------------------------------------
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
        /// Map of top key sequences and associated <see cref="KeyMap"/>.
        /// </summary>
        private Dictionary<Keys, KeyMap> keyMaps;

        /// <summary>
        /// Creates an instance of <see cref="KeyMapService"/>.
        /// </summary>
        /// <param name="platformFacade">
        /// An <see cref="IPlatformFacade"/> implementation
        /// </param>
        public KeyMapService(IPlatformFacade platformFacade)
        {
            this.keyMaps = new Dictionary<Keys, KeyMap>();
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
            foreach (var keyMap in this.keyMaps.Values)
            {
                this.platformFacade.UnregisterHotKey(keyMap.GetHashCode());
            }

            this.keyMaps.Clear();
            this.keyMaps = null;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Registers the <paramref name="topKey"/> key sequence as a hot key. Associates
        /// a <see cref="KeyMap"/> instance to this hot key.
        /// </summary>
        /// <param name="topKey">Hot key sequence</param>
        public void AddKeyMap(Keys topKey)
        {
            var keyMap = new KeyMap("keymap") { TopKey = topKey };
            if (!this.platformFacade.RegisterHotKey(topKey, keyMap.GetHashCode()))
            {
                Logger.GetLogger().Error("KeyMapService: Failed to register hot key. Keys = " + topKey);
                throw new Exception("Binding a hot key failed.");
            }

            this.keyMaps.Add(topKey, keyMap);
        }

        /// <summary>
        /// Unregisters the hot key sequence binding and removes the associated <see cref="KeyMap"/>.
        /// </summary>
        /// <param name="topKey">Registered hot key sequence</param>
        public void RemoveKeyMap(Keys topKey)
        {
            var keyMapHash = this.GetKeyMap(topKey).GetHashCode();
            this.keyMaps.Remove(topKey);
            if (!this.platformFacade.UnregisterHotKey(keyMapHash))
            {
                Logger.GetLogger().Error("KeyMapService: Failed to unregister hot key. Keys = " + topKey);
                throw new Exception("Unbinding a hot key failed.");
            }
        }

        /// <summary>
        /// Gets the <see cref="KeyMap"/> associated with <paramref name="topKey"/> key sequence.
        /// </summary>
        /// <param name="topKey">Registered hot key sequence</param>
        /// <returns><see cref="KeyMap"/> instance associated with hot key</returns>
        public KeyMap GetKeyMap(Keys topKey)
        {
            return this.keyMaps[topKey];
        }

        #endregion
    }
}
