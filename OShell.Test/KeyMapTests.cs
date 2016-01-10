// <copyright file="KeyMapTests.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

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
        [TestMethod]
        public void KeyMapThrowsForAnEmptyName()
        {
            var action = new Action(() => new KeyMap(null));
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("name");

            action = () => new KeyMap(string.Empty);
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("name");
        }

        [TestMethod]
        public void KeyMapCreatesAKeyMapInstanceWithDefaultTopKey()
        {
            var keymap = new KeyMap("dummyKeyMap");
            keymap.Name.Should().Be("dummyKeyMap");
            keymap.TopKey.ShouldBeEquivalentTo(Keys.None);
        }

        [TestMethod]
        [Priority(0)]
        public void CreateKeyMapWithTopKey()
        {
            var km = CreateKeyMapWithTopKey(Keys.A);
            km.TopKey.Should().Be(Keys.A);
        }

        [TestMethod]
        [Priority(0)]
        public void CreateKeyMapWithTopKeyCombination()
        {
            var keys = Keys.Alt | Keys.Shift | Keys.NumPad0;
            CreateKeyMapWithTopKey(keys).TopKey.Should().Be(keys);
        }

        [TestMethod]
        [Priority(0)]
        public async Task ExecuteActionInvokesTheUserAction()
        {
            var topKey = Keys.Alt | Keys.LWin;
            var km = CreateKeyMapWithTopKey(topKey);

            km.RegisterAction(Keys.A, args => Task.Run(() => false));
            var result = await km.Execute(Keys.A, string.Empty);
            result.Should().BeFalse();

            km.RegisterAction(Keys.B, args => Task.Run(() => args.Equals("Hello")));
            result = await km.Execute(Keys.B, "Hello");
            result.Should().BeTrue();
        }

        [TestMethod]
        [Priority(1)]
        public void ExecuteActionWithUnboundKeyThrowsKeyNotBoundException()
        {
            var topKey = Keys.LWin | Keys.A;
            var km = CreateKeyMapWithTopKey(topKey);

            Func<Task> keyMapExecute = async () =>
                {
                    await km.Execute(Keys.B, "dummy args");
                };
            keyMapExecute.ShouldThrow<KeyNotBoundException>()
                .And.Data.ShouldBeEquivalentTo(new Dictionary<string, object>
                                                   {
                                                       { "TopKey", topKey },
                                                       { "KeyData", Keys.B }
                                                   });
        }

        [TestMethod]
        [Priority(1)]
        public void RegisterActionWithExistingKeyThrowsDuplicateKeyBindingException()
        {
            var km = CreateKeyMapWithTopKey(Keys.Alt);
            km.RegisterAction(Keys.A, args => Task.Run(() => true));
            km.Invoking(t => t.RegisterAction(Keys.A, args => Task.Run(() => false)))
              .ShouldThrow<DuplicateKeyBindingException>()
              .And.Data.ShouldBeEquivalentTo(new Dictionary<string, object> { { "KeyData", Keys.A } });
        }

        [TestMethod]
        [Priority(1)]
        public void RegisterActionWithNullThrowsArgumentException()
        {
            var km = CreateKeyMapWithTopKey(Keys.A);
            km.Invoking(t => t.RegisterAction(Keys.B, null)).ShouldThrow<ArgumentException>();
        }

        private static KeyMap CreateKeyMapWithTopKey(Keys topKey)
        {
            return new KeyMap("testKeyMap") { TopKey = topKey };
        }
    }
}
