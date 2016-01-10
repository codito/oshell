// <copyright file="ReadkeyCommandTests.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace OShell.Test.CommandTests
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    using OShell.Core.Commands;
    using OShell.Core.Contracts;
    using OShell.Core.Services;

    [TestClass]
    public class ReadkeyCommandTests
    {
        private readonly KeyMapService keyMapService;

        private readonly IMainWindow mainWindow;

        private readonly IPlatformFacade platformFacade;

        public ReadkeyCommandTests()
        {
            this.platformFacade = Substitute.For<IPlatformFacade>();
            this.platformFacade.RegisterHotKey(Keys.None, 0).ReturnsForAnyArgs(true);
            this.keyMapService = new KeyMapService(this.platformFacade);
            this.mainWindow = Substitute.For<IMainWindow>();
        }

        [TestMethod]
        public void ReadkeyCommandHasReadkeyAsName()
        {
            var readkey = new ReadkeyCommand();
            readkey.Name.Should().Be("readkey");
        }

        [TestMethod]
        public void ReadkeyCommandHandlerShouldThrowForNullKeyMapService()
        {
            var action = new Action(() => new ReadkeyCommandHandler(null, this.mainWindow));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("keyMapService");
        }

        [TestMethod]
        public void ReadkeyCommandHandlerShouldThrowForNullMainWindow()
        {
            var action = new Action(() => new ReadkeyCommandHandler(this.keyMapService, null));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("mainWindow");
        }

        [TestMethod]
        public void ReadkeyCommandHandlerExecuteShouldThrowForNullCommand()
        {
            var readkeyHandler = new ReadkeyCommandHandler(this.keyMapService, this.mainWindow);
            readkeyHandler.Invoking(t => t.Execute(null))
                          .ShouldThrow<ArgumentNullException>()
                          .And.ParamName.Should()
                          .Be("command");
        }

        [TestMethod]
        public void ReadkeyCommandHandlerExecuteShouldThrowForNullKeyMapName()
        {
            var readkeyHandler = new ReadkeyCommandHandler(this.keyMapService, this.mainWindow);
            readkeyHandler.Invoking(t => t.Execute(new ReadkeyCommand())).ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public async Task ReadkeyCommandHandlerExecuteWaitsForNextKeySequenceOfTopKey()
        {
            this.keyMapService.AddKeyMap("dummyKeyMap");
            this.keyMapService.SetTopKey("dummyKeyMap", Keys.T);

            var readkeyHandler = new ReadkeyCommandHandler(this.keyMapService, this.mainWindow);
            await readkeyHandler.Execute(new ReadkeyCommand { Args = "dummyKeyMap" });

            this.mainWindow.Received(1).WaitForNextKeyAsync(Keys.T);
        }
    }
}
