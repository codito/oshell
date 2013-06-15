namespace OShell.Test.CommandTests
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OShell.Core.Commands;
    using OShell.Core.Services;
    using OShell.Test.Doubles;

    [TestClass]
    public class NewKMapCommandTests
    {
        private readonly KeyMapService keyMapService = new KeyMapService(new TestablePlatform());

        [TestMethod]
        public void NewKMapCommandHasNameAsNewKMap()
        {
            var newkmap = new NewKMapCommand();
            newkmap.Name.Should().Be("newkmap");
        }

        [TestMethod]
        public void NewKMapCommandHandlerThrowsForNullKeymapService()
        {
            var action = new Action(() => new NewKMapCommandHandler(null));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("keyMapService");
        }

        [TestMethod]
        public void NewKMapCommandHandlerExecuteShouldThrowForNullCommand()
        {
            Func<Task> action = async () => await new NewKMapCommandHandler(this.keyMapService).Execute(null);
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("command");
        }

        [TestMethod]
        public async Task NewKMapCommandHandlerExecuteShouldCreateANewKeyMapWithDefinedTopKey()
        {
            var command = new NewKMapCommand { Args = (Keys.Control | Keys.T).ToString() };
            var commandHandler = new NewKMapCommandHandler(this.keyMapService);
            var result = await commandHandler.Execute(command);
            result.Should().BeTrue();

            var keyMap = this.keyMapService.GetKeyMap(Keys.Control | Keys.T);
            keyMap.Should().NotBeNull();
            keyMap.TopKey.ShouldBeEquivalentTo(Keys.Control | Keys.T);
        }

        [TestMethod]
        public async Task NewKMapCommandHandlerExecuteShouldReturnFalseForCommandWithoutArguments()
        {
            var command = new NewKMapCommand { Args = null };
            var commandHandler = new NewKMapCommandHandler(this.keyMapService);
            (await commandHandler.Execute(command)).Should().BeFalse();
        }

        [TestMethod]
        public void NewKMapCommandHandlerExecuteShouldThrowForCommandWithInvalidTopKey()
        {
            var command = new NewKMapCommand { Args = "notakey" };
            var commandHandler = new NewKMapCommandHandler(this.keyMapService);
            Func<Task> action = async () => await commandHandler.Execute(command);
            action.ShouldThrow<ArgumentException>();
        }

    }
}