// <copyright file="TestableNotificationService.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace OShell.Test.Doubles
{
    using System.Globalization;

    using OShell.Core.Contracts;

    public class TestableNotificationService : INotificationService
    {
        public string ErrorString { get; private set; }

        public string InfoString { get; private set; }

        public string DebugString { get; private set; }

        #region INotificationService implementation
        public void NotifyError(string format, params object[] args)
        {
            this.ErrorString = string.Format(CultureInfo.CurrentUICulture, format, args);
        }

        public void NotifyInfo(string format, params object[] args)
        {
            this.InfoString = string.Format(CultureInfo.CurrentUICulture, format, args);
        }

        public void NotifyDebug(string format, params object[] args)
        {
            this.DebugString = string.Format(CultureInfo.CurrentUICulture, format, args);
        }
        #endregion
    }
}