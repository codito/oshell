// <copyright file="ICommandStub.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace OShell.Test.Doubles
{
    using OShell.Core.Contracts;

    /// <summary>
    /// A Stub to help test CommandService.
    /// </summary>
    public class ICommandStub : ICommand
    {
        public string Name { get; set; }

        public string Args { get; set; }

        public string Help { get; set; }
    }

    /// <summary>
    /// Yet another command stub.
    /// </summary>
    public class ICommandStub2 : ICommand
    {
        public string Name { get; set; }

        public string Args { get; set; }

        public string Help { get; set; }
    }
}