namespace OShell.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OShell.Core.Contracts;
    using OShell.Core.Contracts.Fakes;
    using OShell.Core.Services;

    [TestClass]
    public class CommandServiceTests
    {
        [TestMethod, Priority(0)]
        public async Task RunSingleCommand()
        {
            string commandName = "stubcmd";
            string commandArgs = "arg1 arg2 arg3";
            var command = new StubICommand { NameGet = () => commandName };
            Func<StubICommand, bool> commandHandlerExecute = (cmd) =>
                {
                    Assert.AreEqual(commandName, cmd.NameGet());
                    Assert.AreEqual(commandArgs, cmd.ArgsGet());
                    return true;
                };
            var commandHandler = new DummyICommandHandler(commandHandlerExecute);
            var cmdsvc = new CommandService(new StubMainWindow(), new List<ICommand> { command }, new List<object> { commandHandler });
            var result = await cmdsvc.Run(commandName + " " + commandArgs);

            Assert.IsTrue(result);
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

    public class DummyICommandHandler : ICommandHandler<StubICommand>
    {
        private readonly Func<StubICommand, bool> execute;

        public DummyICommandHandler(Func<StubICommand, bool> execute)
        {
            this.execute = execute;
        }

        public Task<bool> Execute(StubICommand command)
        {
            return Task.Run(() => this.execute(command));
        }
    }
}
