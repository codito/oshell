// <copyright file="TestableSetCommandHandler.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace OShell.Test.Doubles
{
    using System.Collections.Specialized;

    using OShell.Core.Commands;

    internal class TestableSetCommandHandler : SetCommandHandler
    {
        public TestableSetCommandHandler(TestableNotificationService notificationService, NameValueCollection configurationMap)
            : base(notificationService, configurationMap)
        {
        }
    }
}