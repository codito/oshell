namespace OShell.Test
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    using OShell.Core.Contracts;
    using OShell.Core.Services;
    using OShell.Test.Assets;

    [TestClass]
    public class CommandServiceTests
    {
        [TestMethod, Priority(0)]
        public async Task RunSingleCommand()
        {
            string commandName = "stubcmd";
            string commandArgs = "arg1 arg2 arg3";
            string commandHelp = "sample help!";
            var command = new ICommandStub { Args = commandArgs, Name = commandName, Help = commandHelp };
            var commandHandler = new ICommandHandlerStub
                                     {
                                         ExpectedCommandArgs = commandArgs,
                                         ExpectedCommandName = commandName,
                                         ExpectedCommandHelp = commandHelp,
                                         ExpectedExecuteResult = false
                                     };
            var cmdsvc = new CommandService(Substitute.For<IMainWindow>(), new List<ICommand> { command }, new List<object> { commandHandler });
            var result = await cmdsvc.Run(commandName + " " + commandArgs);

            result.Should().BeFalse();
        }

        [TestMethod, Priority(0)]
        public void RunMultipleCommandWithDifferentRunTime()
        {
            Assert.Fail();
        }

        [TestMethod, Priority(1)]
        public void NullCommandSpecThrowsArgumentException()
        {
            Assert.Fail();
        }

        [TestMethod, Priority(1)]
        public void EmptyCommandSpecThrowsArgumentException()
        {
            Assert.Fail();
        }

        [TestMethod, Priority(1)]
        public void UnregisteredCommandInCommandSpecThrowsInvalidCommandException()
        {
            Assert.Fail();
        }

        [TestMethod, Priority(1)]
        public void MalformedCommandSpecThrowsInvalidCommandException()
        {
            Assert.Fail();
        }
    }
}
