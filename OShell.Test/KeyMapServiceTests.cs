namespace OShell.Test
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    using OShell.Core.Contracts;
    using OShell.Core.Services;

    [TestClass]
    public class KeyMapServiceTests
    {
        private const string TestKeyMapName = "testKeyMap";

        private const Keys TestTopKey = Keys.Shift | Keys.Alt | Keys.A;

        private IKeyMapService keyMapService;

        private IPlatformFacade platformFacade;

        [TestInitialize]
        public void SetupTest()
        {
            var mainWindow = Substitute.For<IMainWindow>();
            mainWindow.GetHandle().Returns(IntPtr.Zero);

            this.platformFacade = Substitute.For<IPlatformFacade>();
            this.platformFacade.MainWindow.Returns(mainWindow);
            this.platformFacade.RegisterHotKey(Keys.A, 0).ReturnsForAnyArgs(true);
            this.platformFacade.UnregisterHotKey(0).ReturnsForAnyArgs(true);

            this.keyMapService = new KeyMapService(this.platformFacade);
            (this.keyMapService as ServiceBase).Start();
            this.keyMapService.AddKeyMap(TestKeyMapName);
        }

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
        public void StopClearsKeyMapRegistry()
        {
            this.keyMapService.AddKeyMap("dummyKeyMap");
            this.keyMapService.SetTopKey(TestKeyMapName, TestTopKey);
            this.keyMapService.SetTopKey("dummyKeyMap", Keys.N);

            (this.keyMapService as ServiceBase).Stop();

            this.keyMapService.Invoking(t => t.GetKeyMapByName(TestKeyMapName)).ShouldThrow<NullReferenceException>();
            this.keyMapService.Invoking(t => t.GetKeyMapByName("dummyKeyMap")).ShouldThrow<NullReferenceException>();
            this.keyMapService.Invoking(t => t.GetKeyMapByTopKey(TestTopKey)).ShouldThrow<NullReferenceException>();
            this.keyMapService.Invoking(t => t.GetKeyMapByTopKey(Keys.N)).ShouldThrow<NullReferenceException>();
        }

        [TestMethod, Priority(1)]
        public void StopCalledMultipleTimesShouldNotThrow()
        {
            (this.keyMapService as ServiceBase).Stop();
            (this.keyMapService as ServiceBase).Invoking(t => t.Stop()).ShouldNotThrow();
        }

        [TestMethod, Priority(0)]
        public void AddKeyMapThrowsForNullOrEmptyKeyMapName()
        {
            foreach (var name in new[] { null, string.Empty })
            {
                this.keyMapService.Invoking(t => t.AddKeyMap(name))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should()
                .Be("name");
            }
        }

        [TestMethod, Priority(0)]
        public void SetTopKeyThrowsForNullOrEmptyKeyMapName()
        {
            foreach (var name in new[] { null, string.Empty })
            {
                this.keyMapService.Invoking(t => t.SetTopKey(name, Keys.A))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should()
                .Be("name");
            }
        }

        [TestMethod, Priority(0)]
        public void SetTopKeyThrowsForInvalidTopKey()
        {
            foreach (var key in new[] { Keys.None, Keys.NoName })
            {
                this.keyMapService.Invoking(t => t.SetTopKey(TestKeyMapName, key))
                    .ShouldThrow<ArgumentException>()
                    .And.ParamName.Should()
                    .Be("topKey");
            }
        }

        [TestMethod, Priority(0)]
        public void SetTopKeySetsTheTopKeyForAKeyMap()
        {
            this.keyMapService.SetTopKey(TestKeyMapName, TestTopKey);
            this.platformFacade.Received(1)
                .RegisterHotKey(TestTopKey, this.keyMapService.GetKeyMapByName(TestKeyMapName).GetHashCode());
        }

        [TestMethod, Priority(0)]
        public void SetTopKeyForAlreadyExistingKeyThrowsArgumentException()
        {
            this.keyMapService.SetTopKey(TestKeyMapName, TestTopKey);
            this.keyMapService.Invoking(t => t.SetTopKey(TestKeyMapName, TestTopKey)).ShouldThrow<ArgumentException>();
        }

        [TestMethod, Priority(1)]
        public void SetTopKeyWithRuntimeErrorInRegisterThrowsException()
        {
            this.platformFacade.RegisterHotKey(Keys.None, 0).ReturnsForAnyArgs(false);
            this.keyMapService
                .Invoking(t => t.SetTopKey(TestKeyMapName, TestTopKey)).ShouldThrow<Exception>()
                .And.Message.Should().Be("Binding a hot key failed.");

            this.keyMapService.GetKeyMapByName(TestKeyMapName).TopKey.ShouldBeEquivalentTo(Keys.None);
        }

        [TestMethod, Priority(1)]
        public void RemoveKeyMapThrowsForNullKeyMapName()
        {
            foreach (var name in new[] { null, string.Empty })
            {
                this.keyMapService.Invoking(t => t.RemoveKeyMap(name))
                    .ShouldThrow<ArgumentNullException>()
                    .And.ParamName.Should()
                    .Be("name");
            }
        }

        [TestMethod, Priority(0)]
        public void RemoveKeyMapRemovesHotKeyBindingForTopKeyOfKeyMap()
        {
            this.keyMapService.SetTopKey(TestKeyMapName, TestTopKey);
            var keyMapHash = this.keyMapService.GetKeyMapByName(TestKeyMapName).GetHashCode();

            // Validate we unregister the correct KeyMap
            this.keyMapService.RemoveKeyMap(TestKeyMapName);
            this.platformFacade.Received(1).UnregisterHotKey(keyMapHash);

            // Validate that key is removed from internal data structures in KeyMapService
            this.keyMapService.Invoking(t => t.GetKeyMapByName(TestKeyMapName)).ShouldThrow<KeyNotFoundException>();
            this.keyMapService.Invoking(t => t.GetKeyMapByTopKey(TestTopKey)).ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod, Priority(1)]
        public void RemoveKeyMapForFailedUnregisterThrowsException()
        {
            this.platformFacade.UnregisterHotKey(0).ReturnsForAnyArgs(false);
            this.keyMapService.SetTopKey(TestKeyMapName, TestTopKey);
            this.keyMapService.Invoking(t => t.RemoveKeyMap(TestKeyMapName))
                .ShouldThrow<Exception>()
                .And.Message.Should()
                .Be("Unbinding a hot key failed.");

            // Validate that GetKeyMapByName fails with KeyNotFoundException
            this.keyMapService.Invoking(t => t.GetKeyMapByName(TestKeyMapName)).ShouldThrow<KeyNotFoundException>();
            this.keyMapService.Invoking(t => t.GetKeyMapByTopKey(TestTopKey)).ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod, Priority(0)]
        public void GetKeyMapByNameThrowsForNullKeyMapName()
        {
            foreach (var name in new[] { null, string.Empty })
            {
                this.keyMapService.Invoking(t => t.GetKeyMapByName(name))
                    .ShouldThrow<ArgumentNullException>()
                    .And.ParamName.Should()
                    .Be("name");
            }
        }

        [TestMethod, Priority(0)]
        public void GetKeyMapByNameReturnsTheCorrectKeyMapInstance()
        {
            this.keyMapService.SetTopKey(TestKeyMapName, TestTopKey);
            this.keyMapService.GetKeyMapByName(TestKeyMapName).TopKey.Should().Be(TestTopKey);
        }

        [TestMethod, Priority(1)]
        public void GetKeyMapByNameForNonExistentKeyMapThrowsKeyNotFoundException()
        {
            this.keyMapService.Invoking(t => t.GetKeyMapByName("notExistingKeyMap")).ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod, Priority(0)]
        public void GetKeyMapByTopKeyThrowsForInvalidTopKey()
        {
            foreach (var key in new[] { Keys.None, Keys.NoName })
            {
                this.keyMapService.Invoking(t => t.GetKeyMapByTopKey(key))
                    .ShouldThrow<ArgumentException>()
                    .And.ParamName.Should()
                    .Be("topKey");
            }
        }

        [TestMethod, Priority(0)]
        public void GetKeyMapByTopKeyReturnsAKeyMapForTheTopKey()
        {
            this.keyMapService.SetTopKey(TestKeyMapName, TestTopKey);
            this.keyMapService.GetKeyMapByTopKey(TestTopKey)
                .ShouldBeEquivalentTo(this.keyMapService.GetKeyMapByName(TestKeyMapName));
        }

        [TestMethod, Priority(1)]
        public void GetKeyMapByTopKeyForNonExistentKeyMapThrowsKeyNotFoundException()
        {
            this.keyMapService.Invoking(t => t.GetKeyMapByTopKey(Keys.NumLock)).ShouldThrow<KeyNotFoundException>();
        }
    }
}
