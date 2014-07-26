namespace OShell.Test.Doubles
{
    using OShell.Core.Commands;
    using System.Collections.Specialized;

    internal class TestableSetCommandHandler : SetCommandHandler
    {
        public TestableSetCommandHandler(TestableNotificationService notificationService, NameValueCollection configurationMap)
            : base(notificationService, configurationMap)
        {
        }
    }
}