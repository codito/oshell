namespace OShell.Test.CommandTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    using OShell.Core.Commands;
    using OShell.Core.Contracts;
    using OShell.Core.Services;
    using OShell.Test.Doubles;

    [TestClass]
    public class DefinekeyCommandTests
    {
        private readonly IPlatformFacade platformFacade;

        private readonly KeyMapService keyMapService;

        private readonly CommandService commandService;

        public DefinekeyCommandTests()
        {
            this.platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(this.platformFacade);

            var commandStub = new ICommandStub
                                    {
                                        Args = "arg1",
                                        Name = "testCommand",
                                        Help = "sample help!"
                                    };
            var commandHandlerStub = new ICommandHandlerStub
                       {
                           ExpectedCommandArgs = commandStub.Args,
                           ExpectedCommandName = commandStub.Name,
                           ExpectedCommandHelp = commandStub.Help,
                           ExpectedExecuteResult = () => true
                       };
            this.commandService = new CommandService(
                new List<ICommand> { commandStub }, new List<object> { commandHandlerStub });
        }

        [TestMethod]
        public void DefinekeyCommandHasNameAsDefinekey()
        {
            var defineKey = new DefinekeyCommand();
            defineKey.Name.Should().Be("definekey");
        }

        [TestMethod]
        public void DefinekeyCommandHandlerThrowsForNullKeymapService()
        {
            var definekeyHandler = new Action(() => new DefinekeyCommandHandler(null, this.commandService));
            definekeyHandler.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("keyMapService");
        }

        [TestMethod]
        public void DefinekeyCommandHandlerThrowsForNullCommandService()
        {
            var definekeyHandler = new Action(() => new DefinekeyCommandHandler(this.keyMapService, null));
            definekeyHandler.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("commandService");
        }

        [TestMethod]
        public void DefineKeyCommandHandlerExecuteThrowsForNullCommand()
        {
            var definekeyHandler = new DefinekeyCommandHandler(this.keyMapService, this.commandService);
            Func<Task> action = async () => await definekeyHandler.Execute(null);
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("command");
        }

        [TestMethod]
        public void DefineKeyCommandHandlerExecuteThrowsForInvalidCommandArguments()
        {
            this.keyMapService.AddKeyMap("testKeyMap");
            var invalidArguments = new[] { string.Empty, null, "a b", "testKeyMap invalid,,key c" };
            var definekeyHandler = new DefinekeyCommandHandler(this.keyMapService, this.commandService);
            foreach (var a in invalidArguments)
            {
                var invalidArgument = a;
                Func<Task> action = async () => await definekeyHandler.Execute(new DefinekeyCommand { Args = invalidArgument });
                action.ShouldThrow<ArgumentException>();
            }
        }

        [TestMethod]
        public async Task DefineKeyCommandHandlerExecuteRegistersActionForAKeyMap()
        {
            this.keyMapService.AddKeyMap("testKeyMap");

            var commandArgs = new[] { "testKeyMap T testCommand arg1", "testKeyMap T,Alt testCommand arg1" };
            var definekeyHandler = new DefinekeyCommandHandler(this.keyMapService, this.commandService);
            foreach (var arg in commandArgs)
            {
                var commandArg = arg;
                var keySequence = GetKeysFromString(commandArg.Split(' ')[1]);
                (await definekeyHandler.Execute(new DefinekeyCommand { Args = commandArg })).Should().BeTrue();

                var keymap = this.keyMapService.GetKeyMapByName("testKeyMap");
                (await keymap.Execute(keySequence, string.Empty)).Should().BeTrue();
            }
        }

        private static Keys GetKeysFromString(string keys)
        {
            var converter = new KeysConverter();
            var keySequence = converter.ConvertFrom(null, CultureInfo.CurrentCulture, keys);
            if (keySequence != null)
            {
                return (Keys)keySequence;
            }

            return Keys.None;
        }
    }
}