﻿//-----------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    /// <summary>
    /// Base class for all services.
    /// </summary>
    public abstract class ServiceBase
    {
        #region IServiceBase implementation

        /// <summary>
        /// Starts the service.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops the service and cleans up any resources.
        /// </summary>
        public abstract void Stop();
        #endregion
    }
}
