namespace OShell.Test.Doubles
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using OShell.Core.Contracts;

    /// <summary>
    /// ICommandHandler stub for testing CommandService.
    /// </summary>
    public class ICommandHandlerStub : ICommandHandler<ICommandStub>
    {
        public string ExpectedCommandName { get; set; }

        public string ExpectedCommandArgs { get; set; }

        public string ExpectedCommandHelp { get; set; }

        public Func<bool> ExpectedExecuteResult { get; set; }

        public Task<bool> Execute(ICommandStub command)
        {
            command.Name.Should().Be(this.ExpectedCommandName);
            command.Args.Should().Be(this.ExpectedCommandArgs);
            command.Help.Should().Be(this.ExpectedCommandHelp);
            return Task.Run(this.ExpectedExecuteResult);
        }
    }

    /// <summary>
    /// Yet another CommandHandler stub.
    /// </summary>
    public class ICommandHandlerStub2 : ICommandHandler<ICommandStub2>
    {
        public string ExpectedCommandName { get; set; }

        public string ExpectedCommandArgs { get; set; }

        public string ExpectedCommandHelp { get; set; }

        public Func<bool> ExpectedExecuteResult { get; set; }

        public Task<bool> Execute(ICommandStub2 command)
        {
            command.Name.Should().Be(this.ExpectedCommandName);
            command.Args.Should().Be(this.ExpectedCommandArgs);
            command.Help.Should().Be(this.ExpectedCommandHelp);
            return Task.Run(this.ExpectedExecuteResult);
        }
    }
}