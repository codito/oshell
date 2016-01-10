// <copyright file="UndefinekeyCommandTests.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace OShell.Test.CommandTests
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    using OShell.Core.Commands;
    using OShell.Core.Contracts;
    using OShell.Core.Services;

    [TestClass]
    public class UndefinekeyCommandTests
    {
        private const string TestKeyMap = "TestKeyMap";

        private readonly IKeyMapService keyMapService;

        public UndefinekeyCommandTests()
        {
            var platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(platformFacade);
            this.keyMapService.AddKeyMap(TestKeyMap);
        }

        [TestMethod]
        public void UndefinekeyCommandHasNameAsUndefinekey()
        {
            var undefinekey = new UndefinekeyCommand();
            undefinekey.Name.Should().Be("undefinekey");
        }

        [TestMethod]
        public void UndefinekeyCommandHanlderThrowsForNullKeyMapService()
        {
            var action = new Action(() => new UndefinekeyCommandHandler(null));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("keyMapService");
        }

        [TestMethod]
        public void UndefinekeyCommandHandlerExecuteThrowsForNullCommand()
        {
            var action = new Action(() => new UndefinekeyCommandHandler(this.keyMapService).Execute(null));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("command");
        }

        [TestMethod]
        public void UndefinekeyCommandHandlerExecuteThrowsForInvalidCommandArgs()
        {
            var invalidArgs = new[] { null, string.Empty, "arg1", "arg1 arg2 arg3", TestKeyMap + " NotAKey" };
            var undefinekeyCommandHandler = new UndefinekeyCommandHandler(this.keyMapService);
            foreach (var invalidArg in invalidArgs)
            {
                var undefinekeyCommand = new UndefinekeyCommand { Args = invalidArg };
                Func<Task> action = async () => await undefinekeyCommandHandler.Execute(undefinekeyCommand);
                action.ShouldThrow<ArgumentException>();
            }
        }

        [TestMethod]
        public async Task UndefinekeyCommandHandlerExecuteUnregistersAKeySequence()
        {
            var keySequences = new[] { "A", "Control+T", "Control+Shift+Alt+T" };
            var undefinekeyCommandHandler = new UndefinekeyCommandHandler(this.keyMapService);
            var keymap = this.keyMapService.GetKeyMapByName(TestKeyMap);
            foreach (var keySequence in keySequences)
            {
                var undefinekeyCommand = new UndefinekeyCommand { Args = TestKeyMap + " " + keySequence };
                (await undefinekeyCommandHandler.Execute(undefinekeyCommand)).Should().BeTrue();

                var registerKey = new Action(
                    () =>
                        {
                            keymap.RegisterAction((Keys)new KeysConverter().ConvertFrom(keySequence), args => Task.Run(() => true));
                        });
                registerKey.ShouldNotThrow();
            }
        }
    }
}