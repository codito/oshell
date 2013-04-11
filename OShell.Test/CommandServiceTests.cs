namespace OShell.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OShell.Core;
    using OShell.Core.Contracts;
    using OShell.Core.Services;
    using OShell.Test.Doubles;

    [TestClass]
    public class CommandServiceTests
    {
        private readonly ICommandStub commandStub;

        private readonly ICommandHandlerStub commandHandlerStubReturnsFalse;

        private readonly ICommandHandlerStub commandHandlerStubReturnsTrue;

        public CommandServiceTests()
        {
            this.commandStub = new ICommandStub
                                    {
                                        Args = "arg1 arg2 arg3",
                                        Name = "stubcmd",
                                        Help = "sample help!"
                                    };
            this.commandHandlerStubReturnsFalse = this.GetCommandHandler(this.commandStub, () => false);
            this.commandHandlerStubReturnsTrue = this.GetCommandHandler(this.commandStub, () => true);
        }

        [TestMethod, Priority(0)]
        public async Task RunASingleCommandWhichReturnsFalse()
        {
            var cmdsvc = new CommandService(new List<ICommand> { this.commandStub }, new List<object> { this.commandHandlerStubReturnsFalse });
            var result = await cmdsvc.Run(this.commandStub.Name + " " + this.commandStub.Args);

            result.Should().BeFalse();
        }

        [TestMethod, Priority(0)]
        public async Task RunASecondCommandWhileFirstCommandIsRunning()
        {
            // Create a command stub which sleeps for some time
            var commandStub2 = new ICommandStub2
                                    {
                                        Args = "arg1 arg2 arg3",
                                        Name = "stubcmd2",
                                        Help = "sample help!"
                                    };
            var commandHandler2 = new ICommandHandlerStub2
                                      {
                                          ExpectedCommandArgs = commandStub2.Args,
                                          ExpectedCommandName = commandStub2.Name,
                                          ExpectedCommandHelp = commandStub2.Help,
                                          ExpectedExecuteResult = () =>
                                              {
                                                  Thread.Sleep(5 * 1000);
                                                  return false;
                                              }
                                      };

            var cmdsvc = new CommandService(
                new List<ICommand> { this.commandStub, commandStub2 },
                new List<object> { this.commandHandlerStubReturnsTrue, commandHandler2 });
            var result2 = cmdsvc.Run(commandStub2.Name + " " + commandStub2.Args);

            // Trigger a second command meanwhile, validate that it shouldn't block
            var result = await cmdsvc.Run(this.commandStub.Name + " " + this.commandStub.Args);
            result.Should().BeTrue();

            result2.Wait();
            result2.Result.Should().BeFalse();
        }

        [TestMethod, Priority(1)]
        public void NullCommandSpecThrowsArgumentException()
        {
            var cmdsvc = new CommandService(new List<ICommand> { this.commandStub }, new List<object> { this.commandHandlerStubReturnsFalse });
            Func<Task> run = async () => await cmdsvc.Run(null);
            run.ShouldThrow<ArgumentException>();
        }

        [TestMethod, Priority(1)]
        public void EmptyCommandSpecThrowsArgumentException()
        {
            var cmdsvc = new CommandService(new List<ICommand> { this.commandStub }, new List<object> { this.commandHandlerStubReturnsFalse });
            Func<Task> run = async () => await cmdsvc.Run(string.Empty);
            run.ShouldThrow<ArgumentException>();
        }

        [TestMethod, Priority(1)]
        public void UnregisteredCommandInCommandSpecThrowsInvalidCommandException()
        {
            var cmdsvc = new CommandService(new List<ICommand> { this.commandStub }, new List<object> { this.commandHandlerStubReturnsFalse });
            const string InputCmd = "cmdnotregistered arg1 arg2";
            Func<Task> run = async () => await cmdsvc.Run(InputCmd);
            run.ShouldThrow<InvalidCommandException>().And.Data["InputCommand"].Should().Be(InputCmd);
        }

        [TestMethod, Priority(1)]
        public void MalformedCommandSpecThrowsInvalidCommandException()
        {
            var cmdsvc = new CommandService(new List<ICommand> { this.commandStub }, new List<object> { this.commandHandlerStubReturnsFalse });
            const string InputCmd = " ";
            Func<Task> run = async () => await cmdsvc.Run(InputCmd);
            run.ShouldThrow<InvalidCommandException>().And.Data["InputCommand"].Should().Be(InputCmd);
        }

        #region Helper methods
        private ICommandHandlerStub GetCommandHandler(ICommandStub cmdStub, Func<bool> result)
        {
            return new ICommandHandlerStub
                       {
                           ExpectedCommandArgs = cmdStub.Args,
                           ExpectedCommandName = cmdStub.Name,
                           ExpectedCommandHelp = cmdStub.Help,
                           ExpectedExecuteResult = result
                       };
        }
        #endregion
    }
}
