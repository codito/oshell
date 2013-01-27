namespace OShell.Test
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OShell.Core;

    [TestClass]
    public class KeyMapTests
    {
        [TestMethod, Priority(0)]
        public void CreateKeyMapWithTopKey()
        {
            var km = new KeyMap(Keys.A);
            km.TopKey.Should().Be(Keys.A);
        }

        [TestMethod, Priority(0)]
        public void CreateKeyMapWithTopKeyCombination()
        {
            var keys = Keys.Alt | Keys.Shift | Keys.NumPad0;
            new KeyMap(keys).TopKey.Should().Be(keys);
        }

        [TestMethod, Priority(0)]
        public async Task ExecuteActionInvokesTheUserAction()
        {
            var topKey = Keys.Alt | Keys.LWin;
            var km = new KeyMap(topKey);

            km.RegisterAction(Keys.A, args => false);
            var result = await km.Execute(Keys.A, string.Empty);
            result.Should().BeFalse();

            km.RegisterAction(Keys.B, args => args.Equals("Hello"));
            result = await km.Execute(Keys.B, "Hello");
            result.Should().BeTrue();
        }

        [TestMethod, Priority(1)]
        public void ExecuteActionWithUnboundKeyThrowsKeyNotBoundException()
        {
            var topKey = Keys.LWin | Keys.A;
            var km = new KeyMap(topKey);

            Func<Task> keyMapExecute = async () =>
                {
                    await km.Execute(Keys.B, "dummy args");
                };
            keyMapExecute.ShouldThrow<KeyNotBoundException>()
                .And.Data.ShouldBeEquivalentTo(new Dictionary<string, object>
                                                   {
                                                       {"TopKey", topKey},
                                                       {"KeyData", Keys.B}
                                                   });
        }

        [TestMethod, Priority(1)]
        public void RegisterActionWithExistingKeyThrowsDuplicateKeyBindingException()
        {
            var km = new KeyMap(Keys.Alt);
            km.RegisterAction(Keys.A, args => true);
            km.Invoking(t => t.RegisterAction(Keys.A, args => false))
              .ShouldThrow<DuplicateKeyBindingException>()
              .And.Data.ShouldBeEquivalentTo(new Dictionary<string, object> { { "KeyData", Keys.A } });
        }

        [TestMethod, Priority(1)]
        public void RegisterActionWithNullThrowsArgumentException()
        {
            var km = new KeyMap(Keys.A);
            km.Invoking(t => t.RegisterAction(Keys.B, null)).ShouldThrow<ArgumentException>();
        }
    }
}
