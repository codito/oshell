namespace OShell.Test
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OShell.Core;
    using OShell.Core.Services;

    [TestClass]
    public class KeyMapTests
    {
        [TestMethod, Priority(0)]
        public void CreateKeyMapWithTopKey()
        {
            var km = new KeyMap(Keys.A);
            Assert.AreEqual(Keys.A, km.TopKey);
        }

        [TestMethod, Priority(0)]
        public void CreateKeyMapWithTopKeyCombination()
        {
            var keys = Keys.Alt | Keys.Shift | Keys.NumPad0;
            Assert.AreEqual(keys, new KeyMap(keys).TopKey);
        }

        [TestMethod, Priority(0)]
        public async Task ExecuteActionInvokesTheUserAction()
        {
            var topKey = Keys.Alt | Keys.LWin;
            var km = new KeyMap(topKey);

            km.RegisterAction(Keys.A, args => false);
            var result = await km.Execute(Keys.A, string.Empty);
            Assert.IsFalse(result);

            km.RegisterAction(Keys.B, args => args.Equals("Hello"));
            result = await km.Execute(Keys.B, "Hello");
            Assert.IsTrue(result);
        }

        [TestMethod, Priority(1)]
        public async Task ExecuteActionWithUnboundKeyThrowsKeyNotBoundException()
        {
            var topKey = Keys.LWin | Keys.A;
            var km = new KeyMap(topKey);

            try
            {
                await km.Execute(Keys.B, "dummy args");
                Assert.Fail("Expected KeyNotBoundException is not thrown.");
            }
            catch (KeyNotBoundException ex)
            {
                Assert.AreEqual(topKey, ex.Data["TopKey"]);
                Assert.AreEqual(Keys.B, ex.Data["KeyData"]);
            }
        }

        [TestMethod, Priority(1)]
        public void RegisterActionWithExistingKeyThrowsDuplicateKeyBindingException()
        {
            try
            {
                var km = new KeyMap(Keys.Alt);
                km.RegisterAction(Keys.A, args => true);
                km.RegisterAction(Keys.A, args => false);
                Assert.Fail("Expected DuplicateKeyBindingException is not thrown.");
            }
            catch (DuplicateKeyBindingException ex)
            {
                Assert.AreEqual(Keys.A, ex.Data["KeyData"]);
            }
        }

        [TestMethod, Priority(1)]
        public void RegisterActionWithNullThrowsArgumentException()
        {
            try
            {
                var km = new KeyMap(Keys.A);
                km.RegisterAction(Keys.B, null);
                Assert.Fail("Expected ArgumentException is not thrown");
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
