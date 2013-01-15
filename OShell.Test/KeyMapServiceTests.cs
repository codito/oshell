//namespace OShell.Test
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Windows.Forms;

//    using Microsoft.VisualStudio.TestTools.UnitTesting;

//    using NSubstitute;

//    using OShell.Common;
//    using OShell.Core.Contracts;
//    using OShell.Core.Services;

//    [TestClass]
//    public class KeyMapServiceTests
//    {
//        private IKeyMapService keyMapService;

//        [TestInitialize]
//        public void SetupTest()
//        {
//            var mainWindow = Substitute.For<IMainWindow>();
//            this.keyMapService = new KeyMapService(mainWindow);
//            (this.keyMapService as ServiceBase).Start();
//        }

//        [TestCleanup]
//        public void CleanTest()
//        {
//            if (this.keyMapService == null)
//            {
//                return;
//            }

//            var serviceBase = this.keyMapService as ServiceBase;
//            if (serviceBase != null)
//            {
//                serviceBase.Stop();
//            }
//        }

//        [TestMethod, Priority(0)]
//        public void AddKeyMapAddsAHotKey()
//        {
//            var topKey = Keys.Shift | Keys.Alt | Keys.A;
//            var mockInterop = Substitute.For<Interop>();
//            using (ShimsContext.Create())
//            {
//                var keyMapHash = 0;
//                ShimInterop.RegisterHotKeyIntPtrKeysInt32 = (handle, keys, id) =>
//                    {
//                        Assert.AreEqual(topKey, keys);
//                        keyMapHash = id;
//                        return true;
//                    };

//                this.keyMapService.AddKeyMap(topKey);

//                // Compare the key map instances
//                Assert.AreEqual(keyMapHash, this.keyMapService.GetKeyMap(topKey).GetHashCode());
//            }
//        }

//        [TestMethod, Priority(1)]
//        public void AddKeyMapForAlreadyExistingKeyThrowsArgumentException()
//        {
//            var topKey = Keys.Shift | Keys.Alt | Keys.A;
//            using (ShimsContext.Create())
//            {
//                var keyMapHash = 0;
//                ShimInterop.RegisterHotKeyIntPtrKeysInt32 = (handle, keys, id) =>
//                    {
//                        Assert.AreEqual(topKey, keys);
//                        keyMapHash = id;
//                        return true;
//                    };

//                this.keyMapService.AddKeyMap(topKey);

//                try
//                {
//                    this.keyMapService.AddKeyMap(topKey);
//                    Assert.Fail("Expected argument exception is not thrown.");
//                }
//                catch (ArgumentException)
//                {
//                }
//            }
//        }

//        [TestMethod, Priority(1)]
//        public void AddKeyMapWithRuntimeErrorInRegisterThrowsException()
//        {
//            var topKey = Keys.Shift | Keys.Alt | Keys.A;
//            using (ShimsContext.Create())
//            {
//                var keyMapHash = 0;
//                ShimInterop.RegisterHotKeyIntPtrKeysInt32 = (handle, keys, id) =>
//                    {
//                        Assert.AreEqual(topKey, keys);
//                        // Simulate runtime error
//                        return false;
//                    };

//                try
//                {
//                    this.keyMapService.AddKeyMap(topKey);
//                    Assert.Fail("Expected exception is not thrown.");
//                }
//                catch (Exception ex)
//                {
//                    Assert.AreEqual(ex.Message, "Binding a hot key failed.");
//                }

//                // Ensure there is no KeyMap available
//                try
//                {
//                    this.keyMapService.GetKeyMap(topKey);
//                    Assert.Fail("Expected key not found exception is not thrown.");
//                }
//                catch (KeyNotFoundException)
//                {
//                }
//            }

//        }

//        [TestMethod, Priority(0)]
//        public void RemoveKeyMapRemovesTheHotKeyBinding()
//        {
//            var topKey = Keys.Shift | Keys.Alt | Keys.A;
//            using (ShimsContext.Create())
//            {
//                ShimInterop.RegisterHotKeyIntPtrKeysInt32 = (handle, keys, id) => true;
//                this.keyMapService.AddKeyMap(topKey);
//                var keyMapHash = this.keyMapService.GetKeyMap(topKey).GetHashCode();

//                ShimInterop.UnregisterHotKeyIntPtrInt32 = (handle, id) =>
//                    {
//                        Assert.AreEqual(keyMapHash, id);
//                        return true;
//                    };

//                this.keyMapService.RemoveKeyMap(topKey);

//                // Validate that key is removed from internal data structures in KeyMapService
//                try
//                {
//                    this.keyMapService.GetKeyMap(topKey);
//                    Assert.Fail("Expected key not found exception is not thrown.");
//                }
//                catch (KeyNotFoundException)
//                {
//                }
//            }
//        }

//        [TestMethod, Priority(1)]
//        public void RemoveKeyMapForFailedUnregisterThrowsException()
//        {
//            var topKey = Keys.Shift | Keys.Alt | Keys.A;
//            using (ShimsContext.Create())
//            {
//                ShimInterop.RegisterHotKeyIntPtrKeysInt32 = (handle, keys, id) => true;
//                this.keyMapService.AddKeyMap(topKey);
//                var keyMapHash = this.keyMapService.GetKeyMap(topKey).GetHashCode();

//                ShimInterop.UnregisterHotKeyIntPtrInt32 = (handle, id) =>
//                    {
//                        Assert.AreEqual(keyMapHash, id);

//                        // Simulate runtime error
//                        return false;
//                    };


//                try
//                {
//                    this.keyMapService.RemoveKeyMap(topKey);
//                    Assert.Fail("Expected exception is not thrown.");
//                }
//                catch (Exception ex)
//                {
//                    Assert.AreEqual("Unbinding a hot key failed.", ex.Message);
//                }

//                // Validate that GetKeyMap fails with KeyNotFoundException
//                try
//                {
//                    this.keyMapService.GetKeyMap(topKey);
//                    Assert.Fail("Expected key not found exception is not thrown.");
//                }
//                catch (KeyNotFoundException)
//                {
//                }
//            }
//        }

//        [TestMethod, Priority(0)]
//        public void GetKeyMapReturnsTheCorrectKeyMapInstance()
//        {
//            var topKey = Keys.Shift | Keys.Alt | Keys.A;
//            using (ShimsContext.Create())
//            {
//                ShimInterop.RegisterHotKeyIntPtrKeysInt32 = (handle, keys, id) => true;
//                this.keyMapService.AddKeyMap(topKey);
//                var keyMap = this.keyMapService.GetKeyMap(topKey);

//                Assert.AreEqual(topKey, keyMap.TopKey);
//            }
//        }

//        [TestMethod, Priority(1)]
//        public void GetKeyMapForNotBoundKeyThrowsKeyNotFoundException()
//        {
//            var topKey = Keys.Shift | Keys.Alt | Keys.A;

//            try
//            {
//                var keyMap = this.keyMapService.GetKeyMap(topKey);
//                Assert.Fail("Expected key not found exception is not thrown.");
//            }
//            catch (KeyNotFoundException)
//            {
//            }
//        }
//    }
//}
