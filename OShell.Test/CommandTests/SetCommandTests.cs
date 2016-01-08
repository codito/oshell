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

        private readonly TestableSetCommandHandler testableSetCommandHandler;

        public SetCommandTests()
        {
            this.notificationService = new TestableNotificationService();
            this.configurationMap = new NameValueCollection { { "var1", "value1" }, { "var2", "value2" } };
            this.testableSetCommandHandler = new TestableSetCommandHandler(this.notificationService, this.configurationMap);
        }

        [TestMethod]
        public void SetCommandHasNameAsSet()
        {
            var setCommand = new SetCommand();
            setCommand.Name.Should().Be("set");
        }

        #region Constructor tests
        [TestMethod]
        public void SetCommandHandlerThrowsForNullConfigurationMap()
        {
            var action = new Action(() => new TestableSetCommandHandler(this.notificationService, null));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("configurationMap");
        }

        [TestMethod]
        public void SetCommandHandlerThrowsForNullNotificationService()
        {
            var action = new Action(() => new TestableSetCommandHandler(null, new NameValueCollection()));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("notificationService");
        }

        [TestMethod]
        public void SetCommandHandlerExecuteThrowsForNullSetCommand()
        {
            var setCommandHandler = new TestableSetCommandHandler(this.notificationService, this.configurationMap);
            Func<Task> action = async () => await setCommandHandler.Execute(null);

            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("command");
        }
        #endregion

        #region Default configuration variables tests
        [TestMethod]
        public async Task SetCommandHandlerDefaultConfigurationHasDefinedVariables()
        {
            var definedVariables = new[] { "border", "topkmap" };
            var setCommand = new SetCommand();
            var setCommandHandler = new SetCommandHandler(this.notificationService);

            (await setCommandHandler.Execute(setCommand)).Should().BeTrue();

            var output = this.notificationService.InfoString;
            foreach (var variable in definedVariables)
            {
                output.Should().Contain(variable);
            }

            // Set command appends a new line by default to end of output, hence + 1
            output.Split('\r').Length.Should().Be(definedVariables.Length + 1);
        }

        [TestMethod]
        public async Task SetCommandHandlerHasDefaultValueForTopKMapVariable()
        {
            var setCommand = new SetCommand { Args = "topkmap" };
            var setCommandHandler = new SetCommandHandler(this.notificationService);

            (await setCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.notificationService.InfoString.Should().Be("topkmap = top\r\n");
        }

        [TestMethod]
        public async Task SetCommandHandlerHasDefaultValueForBorderVariable()
        {
            var setCommand = new SetCommand { Args = "border" };
            var setCommandHandler = new SetCommandHandler(this.notificationService);

            (await setCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.notificationService.InfoString.Should().Be("border = 1\r\n");
        }
        #endregion

        [TestMethod]
        public async Task SetCommandHandlerExecuteWithNullOrEmptyArgsPrintsValuesOfAllVariables()
        {
            var setCommand = new SetCommand();

            (await this.testableSetCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.notificationService.InfoString.Should().Be("var1 = value1\r\nvar2 = value2\r\n");
        }

        [TestMethod]
        public async Task SetCommandHandlerExecutePrintsSettingIfNoValueIsSpecified()
        {
            var setCommand = new SetCommand { Args = "var1" };

            (await this.testableSetCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.notificationService.InfoString.Should().Be("var1 = value1\r\n");
        }

        [TestMethod]
        public async Task SetCommandHandlerExecutePrintsNullForSettingIfNoValueIsSpecified()
        {
            var setCommand = new SetCommand { Args = "var1doesnotexist" };

            (await this.testableSetCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.notificationService.InfoString.Should().Be("var1doesnotexist = <undefined>\r\n");
        }

        [TestMethod]
        public async Task SetCommandHandlerExecuteCreatesSettingIfNotExists()
        {
            var setCommand = new SetCommand { Args = "var2doesnotexist value2" };

            (await this.testableSetCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.configurationMap["var2doesnotexist"].Should().Be("value2");
        }

        [TestMethod]
        public async Task SetCommandHandlerExecuteSetsValueToExistingSetting()
        {
            var setCommand = new SetCommand { Args = "var2 value2new" };

            (await this.testableSetCommandHandler.Execute(setCommand)).Should().BeTrue();

            this.configurationMap["var2"].Should().Be("value2new");
        }
    }
}
