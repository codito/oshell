namespace OShell.Test
{
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OShell.Core.Contracts.Fakes;
    using OShell.Core.Services;

    [TestClass]
    public class CommandServiceTests
    {
        [TestMethod, Priority(0)]
        public async Task RunSingleCommand()
        {
            //var cmdsvc = new CommandService(new StubMainWindow());
            //await cmdsvc.Run("dummycmd arg1 arg2");
        }

        [TestMethod, Priority(0)]
        public void RunMultipleCommandWithDifferentRunTime()
        {
            Assert.Fail();
        }

        [TestMethod, Priority(1)]
        public void NullCommandSpecThrowsArgumentException()
        {
            Assert.Fail();
        }

        [TestMethod, Priority(1)]
        public void EmptyCommandSpecThrowsArgumentException()
        {
            Assert.Fail();
        }

        [TestMethod, Priority(1)]
        public void UnregisteredCommandInCommandSpecThrowsInvalidCommandException()
        {
            Assert.Fail();
        }

        [TestMethod, Priority(1)]
        public void MalformedCommandSpecThrowsInvalidCommandException()
        {
            Assert.Fail();
        }
    }
}
