//-----------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    /// <summary>
    /// Common interface for all Service implementations.
    /// </summary>
    public interface IServiceBase
    {
        void Start();
        void Stop();
    }

    /// <summary>
    /// Base class for all services.
    /// </summary>
    public abstract class ServiceBase : IServiceBase
    {
        #region IServiceBase implementation
        public abstract void Start();

        public abstract void Stop();
        #endregion
    }
}
