namespace OShell.Test.CommandTests
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    using OShell.Core.Commands;
    using OShell.Core.Contracts;
    using OShell.Core.Services;

    [TestClass]
    public class TopKMapCommandTests
    {
        private readonly IPlatformFacade platformFacade;

        private KeyMapService keyMapService;

        public TopKMapCommandTests()
        {
            this.platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(this.platformFacade);
        }

        [TestMethod]
        public void TopKMapCommandHasNameAsTopKMap()
        {
            var command = new TopKMapCommand();
            command.Name.Should().Be("topkmap");
        }

        [TestMethod]
        public void TopKMapCommandHandlerExecuteThrowsForNullCommand()
        {
            var topkmapHandler = new TopKMapCommandHandler();
            Func<Task> action = async () => await topkmapHandler.Execute(null);
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("command");
        }
    }
}
