namespace OShell.Test.CommandTests
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OShell.Core.Commands;
    using OShell.Test.Doubles;

    [TestClass]
    public class SetCommandTests
    {
        private readonly TestableNotificationService notificationService;

        private readonly NameValueCollection configurationMap;

        public SetCommandTests()
        {
            this.notificationService = new TestableNotificationService();
            this.configurationMap = new NameValueCollection { { "var1", "value1" }, { "var2", "value2" } };
        }

        /**
         * TODO: Test to validate against specified config settings
         */
        [TestMethod]
        public void SetCommandHasNameAsSet()
        {
            var setCommand = new SetCommand();
            setCommand.Name.Should().Be("set");
        }

        [TestMethod]
        public void SetCommandHandlerThrowsForNullConfigurationMap()
        {
            var action = new Action(() => new SetCommandHandler(this.notificationService, null));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("configurationMap");
        }

        [TestMethod]
        public void SetCommandHandlerThrowsForNullNotificationService()
        {
            var action = new Action(() => new SetCommandHandler(null, new NameValueCollection()));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("notificationService");
        }

        [TestMethod]
        public void SetCommandHandlerExecuteThrowsForNullSetCommand()
        {
            var setCommandHandler = new SetCommandHandler(this.notificationService, this.configurationMap);
            Func<Task> action = async () => await setCommandHandler.Execute(null);

            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("command");
        }

        [TestMethod]
        public async Task SetCommandHandlerExecuteWithNullOrEmptyArgsPrintsValuesOfAllVariables()
        {
            var setCommand = new SetCommand();
            var setCommandHandler = new SetCommandHandler(this.notificationService, this.configurationMap);

            (await setCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.notificationService.InfoString.Should().Be("var1 = value1\r\nvar2 = value2\r\n");
        }

        [TestMethod]
        public async Task SetCommandHandlerExecutePrintsSettingIfNoValueIsSpecified()
        {
            var setCommand = new SetCommand { Args = "var1" };
            var setCommandHandler = new SetCommandHandler(this.notificationService, this.configurationMap);

            (await setCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.notificationService.InfoString.Should().Be("var1 = value1\r\n");
        }

        [TestMethod]
        public async Task SetCommandHandlerExecutePrintsNullForSettingIfNoValueIsSpecified()
        {
            var setCommand = new SetCommand { Args = "var1doesnotexist" };
            var setCommandHandler = new SetCommandHandler(this.notificationService, this.configurationMap);

            (await setCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.notificationService.InfoString.Should().Be("var1doesnotexist = <undefined>\r\n");
        }

        [TestMethod]
        public async Task SetCommandHandlerExecuteCreatesSettingIfNotExists()
        {
            var setCommand = new SetCommand { Args = "var2doesnotexist value2" };
            var setCommandHandler = new SetCommandHandler(this.notificationService, this.configurationMap);

            (await setCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.configurationMap["var2doesnotexist"].Should().Be("value2");
        }

        [TestMethod]
        public async Task SetCommandHandlerExecuteSetsValueToExistingSetting()
        {
            var setCommand = new SetCommand { Args = "var2 value2new" };
            var setCommandHandler = new SetCommandHandler(this.notificationService, this.configurationMap);

            (await setCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.configurationMap["var2"].Should().Be("value2new");
        }
    }
}
