// <copyright file="SourceCommandTests.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

namespace OShell.Test.CommandTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OShell.Core;
    using OShell.Core.Commands;
    using OShell.Core.Contracts;
    using OShell.Core.Services;
    using OShell.Test.Doubles;

    [TestClass]
    public class SourceCommandTests
    {
        private readonly SourceCommandHandler sourceCommandHandler;

        private readonly ICommandStub commandStub;

        private readonly ICommandHandlerStub commandHandlerStubReturnsTrue;

        private readonly CommandService commandService;

        public SourceCommandTests()
        {
            this.commandStub = new ICommandStub
                                    {
                                        Args = "arg1 arg2 arg3",
                                        Name = "stubcmd",
                                        Help = "sample help!"
                                    };
            this.commandHandlerStubReturnsTrue = new ICommandHandlerStub
                                                      {
                                                          ExpectedCommandArgs = this.commandStub.Args,
                                                          ExpectedCommandName = this.commandStub.Name,
                                                          ExpectedCommandHelp = this.commandStub.Help,
                                                          ExpectedExecuteResult = () => true
                                                      };
            var failCommandStub = new ICommandStub2
                                    {
                                        Args = "arg1 arg2 arg3",
                                        Name = "stubcmd2",
                                        Help = "sample help!"
                                    };
            var commandHandlerStubReturnsFalse = new ICommandHandlerStub2
                                                      {
                                                          ExpectedCommandArgs = failCommandStub.Args,
                                                          ExpectedCommandName = failCommandStub.Name,
                                                          ExpectedCommandHelp = failCommandStub.Help,
                                                          ExpectedExecuteResult = () => false
                                                      };
            this.commandService = new CommandService(
                new List<ICommand> { this.commandStub, failCommandStub }, new List<object> { this.commandHandlerStubReturnsTrue, commandHandlerStubReturnsFalse });
            this.sourceCommandHandler = new SourceCommandHandler(this.commandService);
        }

        [TestMethod]
        [Priority(0)]
        public void SourceCommandHasNameAsSource()
        {
            var sourceCommand = new SourceCommand();
            sourceCommand.Name.Should().Be("source");
        }

        #region Parsing scenarios
        [TestMethod]
        [Priority(0)]
        public void LineWithCommandAndArgumentIsParsedFromTheSourceFile()
        {
            var configFile = GetDummyConfigFile("stubcmd arg1 arg2 arg3");
            var sourceCmd = new SourceCommand { Args = configFile };
            this.sourceCommandHandler.Execute(sourceCmd).Result.Should().BeTrue();
            CleanupDummyConfigFile(configFile);
        }

        [TestMethod]
        [Priority(0)]
        public void LineStartingWithHashIsTreatedAsComment()
        {
            var configFile = GetDummyConfigFile("#stubcmd arg1 arg2 invalid arg");
            var sourceCmd = new SourceCommand { Args = configFile };
            this.sourceCommandHandler.Execute(sourceCmd).Result.Should().BeTrue();
            CleanupDummyConfigFile(configFile);
        }

        [TestMethod]
        [Priority(0)]
        public void LineWithNullOrEmptySpaceIsIgnored()
        {
            var configFile = GetDummyConfigFile("   \r\n\r\nstubcmd arg1 arg2 arg3");
            var sourceCmd = new SourceCommand { Args = configFile };
            this.sourceCommandHandler.Execute(sourceCmd).Result.Should().BeTrue();
            CleanupDummyConfigFile(configFile);
        }

        [TestMethod]
        [Priority(1)]
        public void LineWithACommandWhichFailsReturnsCorrectLineNumberAndCommandName()
        {
            var configFile = GetDummyConfigFile("stubcmd arg1 arg2 arg3\rstubcmd2 arg1 arg2 arg3");
            var sourceCmd = new SourceCommand { Args = configFile };
            this.sourceCommandHandler.Execute(sourceCmd).Result.Should().BeFalse();
            this.sourceCommandHandler.ErrorMessage.Should().Be("Line: 2: Failed to run command: stubcmd2 arg1 arg2 arg3");
            CleanupDummyConfigFile(configFile);
        }
        #endregion

        #region Exception scenarios
        [TestMethod]
        [Priority(1)]
        public void FileNotFoundExceptionIsThrownIfSourceFileIsNotFound()
        {
            var sourceCmd = new SourceCommand { Args = @"z:\dummydummyfile" };
            var result = new Action(() => this.sourceCommandHandler.Execute(sourceCmd).Result.Should().BeFalse());
            result.ShouldThrow<AggregateException>().And.InnerException.Should().BeOfType<DirectoryNotFoundException>();
        }

        [TestMethod]
        [Priority(1)]
        public void InvalidCommandExceptionIsThrownIfUnrecognizedCommandIsFound()
        {
            var configFile = GetDummyConfigFile("dummycmd arg1 arg2 arg3");
            var sourceCmd = new SourceCommand { Args = configFile };
            var result = new Action(() => this.sourceCommandHandler.Execute(sourceCmd).Result.Should().BeFalse());
            result.ShouldThrow<AggregateException>().And.InnerException.Should().BeOfType<InvalidCommandException>();
            CleanupDummyConfigFile(configFile);
        }
        #endregion

        private static string GetDummyConfigFile(string content)
        {
            var filePath = Path.GetTempFileName();
            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(content);
            }

            return filePath;
        }

        private static void CleanupDummyConfigFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}