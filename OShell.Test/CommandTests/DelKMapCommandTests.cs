namespace OShell.Test.CommandTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OShell.Core.Commands;
    using OShell.Core.Services;
    using OShell.Test.Doubles;

    [TestClass]
    public class DelKMapCommandTests
    {
        private readonly KeyMapService keyMapService = new KeyMapService(new TestablePlatform());

        [TestMethod]
        public void DelKMapCommandHasNameAsDelKMap()
        {
            var delkmap = new DelKMapCommand();
            delkmap.Name.Should().Be("delkmap");
        }

        [TestMethod]
        public void DelKMapCommandHandlerThrowsForNullKeymapService()
        {
            var delkmapHandler = new Action(() => new DelKMapCommandHandler(null));
            delkmapHandler.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("keyMapService");
        }

        [TestMethod]
        public void DelKMapCommandHandlerExecuteThrowsForNullCommand()
        {
            Func<Task> action = async () => await new DelKMapCommandHandler(this.keyMapService).Execute(null);
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("command");
        }

        [TestMethod]
        public async Task DelKMapCommandHandlerExecuteShouldDeleteAKeyMapWithGivenName()
        {
            var delkmapHandler = new DelKMapCommandHandler(this.keyMapService);
            var newkmapHandler = new NewKMapCommandHandler(this.keyMapService);
            (await newkmapHandler.Execute(new NewKMapCommand { Args = "dummyKeyMap" })).Should().BeTrue();

            var result = await delkmapHandler.Execute(new DelKMapCommand { Args = "dummyKeyMap" });
            result.Should().BeTrue();

            var getKeyMap = new Action(() => this.keyMapService.GetKeyMapByName("dummyKeyMap"));
            getKeyMap.ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod]
        public async Task DelKMapCommandHandlerExecuteReturnsFalseForACommandWithNullArguments()
        {
            var delkmapHandler = new DelKMapCommandHandler(this.keyMapService);
            (await delkmapHandler.Execute(new DelKMapCommand())).Should().BeFalse();
        }

        [TestMethod]
        public void DelKMapCommandHandlerExecuteShouldThrowForNonExistentKeyMap()
        {
            var delkmapHandler = new DelKMapCommandHandler(this.keyMapService);
            Func<Task> action = async () => await delkmapHandler.Execute(new DelKMapCommand { Args = "dummyKeyMap" });
            action.ShouldThrow<KeyNotFoundException>();
        }
    }
}
