﻿namespace OShell.Test
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    using OShell.Core;
    using OShell.Core.Contracts;
    using OShell.Core.Services;

    [TestClass]
    public class KeyMapServiceTests
    {
        private IKeyMapService keyMapService;

        //[TestInitialize]
        //public void SetupTest()
        //{
        //    var mainWindow = Substitute.For<IMainWindow>();
        //    var platformFacade = Substitute.For<IPlatformFacade>();
        //    this.keyMapService = new KeyMapService(mainWindow, platformFacade);
        //    (this.keyMapService as ServiceBase).Start();
        //}

        [TestCleanup]
        public void CleanTest()
        {
            if (this.keyMapService == null)
            {
                return;
            }

            var serviceBase = this.keyMapService as ServiceBase;
            if (serviceBase != null)
            {
                serviceBase.Stop();
            }
        }

        [TestMethod, Priority(0)]
        public void AddKeyMapAddsAHotKey()
        {
            // FIXME Test smell. DRY violation.
            var topKey = Keys.Shift | Keys.Alt | Keys.A;
            var mainWindow = Substitute.For<IMainWindow>();
            mainWindow.GetHandle().Returns(IntPtr.Zero);

            var platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(mainWindow, platformFacade);
            (this.keyMapService as ServiceBase).Start();

            platformFacade.RegisterHotKey(IntPtr.Zero, Keys.A, 0).ReturnsForAnyArgs(true);
            this.keyMapService.AddKeyMap(topKey);
            platformFacade.Received(1)
                          .RegisterHotKey(IntPtr.Zero, topKey, this.keyMapService.GetKeyMap(topKey).GetHashCode());
        }

        [TestMethod, Priority(1)]
        public void AddKeyMapForAlreadyExistingKeyThrowsArgumentException()
        {
            var topKey = Keys.Shift | Keys.Alt | Keys.A;
            var mainWindow = Substitute.For<IMainWindow>();
            mainWindow.GetHandle().Returns(IntPtr.Zero);

            var platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(mainWindow, platformFacade);
            (this.keyMapService as ServiceBase).Start();

            platformFacade.RegisterHotKey(IntPtr.Zero, Keys.A, 0).ReturnsForAnyArgs(true);
            this.keyMapService.AddKeyMap(topKey);
            this.keyMapService.Invoking(t => t.AddKeyMap(topKey)).ShouldThrow<ArgumentException>();
        }

        [TestMethod, Priority(1)]
        public void AddKeyMapWithRuntimeErrorInRegisterThrowsException()
        {
            var topKey = Keys.Shift | Keys.Alt | Keys.A;
            var mainWindow = Substitute.For<IMainWindow>();
            mainWindow.GetHandle().Returns(IntPtr.Zero);

            var platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(mainWindow, platformFacade);
            (this.keyMapService as ServiceBase).Start();

            platformFacade.RegisterHotKey(IntPtr.Zero, Keys.A, 0).ReturnsForAnyArgs(false);
            this.keyMapService
                .Invoking(t => t.AddKeyMap(topKey)).ShouldThrow<Exception>()
                .And.Message.Should().Be("Binding a hot key failed.");

            this.keyMapService.Invoking(t => t.GetKeyMap(topKey)).ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod, Priority(0)]
        public void RemoveKeyMapRemovesTheHotKeyBinding()
        {
            var topKey = Keys.Shift | Keys.Alt | Keys.A;
            var mainWindow = Substitute.For<IMainWindow>();
            mainWindow.GetHandle().Returns(IntPtr.Zero);

            var platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(mainWindow, platformFacade);
            (this.keyMapService as ServiceBase).Start();

            platformFacade.RegisterHotKey(IntPtr.Zero, Keys.A, 0).ReturnsForAnyArgs(true);
            platformFacade.UnregisterHotKey(IntPtr.Zero, 0).ReturnsForAnyArgs(true);
            this.keyMapService.AddKeyMap(topKey);
            var keyMapHash = this.keyMapService.GetKeyMap(topKey).GetHashCode();

            // Validate we unregister the correct KeyMap
            this.keyMapService.RemoveKeyMap(topKey);
            platformFacade.Received(1).UnregisterHotKey(IntPtr.Zero, keyMapHash);

            // Validate that key is removed from internal data structures in KeyMapService
            this.keyMapService.Invoking(t => t.GetKeyMap(topKey)).ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod, Priority(1)]
        public void RemoveKeyMapForFailedUnregisterThrowsException()
        {
            var topKey = Keys.Shift | Keys.Alt | Keys.A;
            var mainWindow = Substitute.For<IMainWindow>();
            mainWindow.GetHandle().Returns(IntPtr.Zero);

            var platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(mainWindow, platformFacade);
            (this.keyMapService as ServiceBase).Start();

            platformFacade.RegisterHotKey(IntPtr.Zero, Keys.A, 0).ReturnsForAnyArgs(true);
            platformFacade.UnregisterHotKey(IntPtr.Zero, 0).ReturnsForAnyArgs(false);
            this.keyMapService.AddKeyMap(topKey);
            this.keyMapService.Invoking(t => t.RemoveKeyMap(topKey))
                .ShouldThrow<Exception>()
                .And.Message.Should()
                .Be("Unbinding a hot key failed.");

            // Validate that GetKeyMap fails with KeyNotFoundException
            this.keyMapService.Invoking(t => t.GetKeyMap(topKey)).ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod, Priority(0)]
        public void GetKeyMapReturnsTheCorrectKeyMapInstance()
        {
            var topKey = Keys.Shift | Keys.Alt | Keys.A;
            var mainWindow = Substitute.For<IMainWindow>();
            mainWindow.GetHandle().Returns(IntPtr.Zero);

            var platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(mainWindow, platformFacade);
            (this.keyMapService as ServiceBase).Start();

            platformFacade.RegisterHotKey(IntPtr.Zero, Keys.A, 0).ReturnsForAnyArgs(true);
            this.keyMapService.AddKeyMap(topKey);
            this.keyMapService.GetKeyMap(topKey).TopKey.Should().Be(topKey);
        }

        [TestMethod, Priority(1)]
        public void GetKeyMapForNotBoundKeyThrowsKeyNotFoundException()
        {
            var topKey = Keys.Shift | Keys.Alt | Keys.A;
            var mainWindow = Substitute.For<IMainWindow>();
            mainWindow.GetHandle().Returns(IntPtr.Zero);

            var platformFacade = Substitute.For<IPlatformFacade>();
            this.keyMapService = new KeyMapService(mainWindow, platformFacade);
            (this.keyMapService as ServiceBase).Start();

            this.keyMapService.Invoking(t => t.GetKeyMap(topKey)).ShouldThrow<KeyNotFoundException>();
        }
    }
}
