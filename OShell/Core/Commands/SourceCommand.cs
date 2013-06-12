// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceCommand.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace OShell.Core.Commands
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    /// <summary>
    /// The source command.
    /// </summary>
    public class SourceCommand : ICommand
    {
        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "source";
            }
        }

        /// <inheritdoc/>
        public string Args { get; set; }

        /// <inheritdoc/>
        public string Help
        {
            get
            {
                return "source file\nRead file and execute each line as ratpoison command.";
            }
        }
    }

    /// <summary>
    /// The source command handler.
    /// </summary>
    public class SourceCommandHandler : ICommandHandler<SourceCommand>
    {
        private readonly ICommandService commandService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCommandHandler"/> class.
        /// </summary>
        /// <param name="commandService">
        /// The command service.
        /// </param>
        public SourceCommandHandler(ICommandService commandService)
        {
            this.commandService = commandService;
        }

        /// <summary>
        /// Gets the error message for the command execution.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <inheritdoc/>
        public async Task<bool> Execute(SourceCommand command)
        {
            return await Task.Run(
                () =>
                    {
                        try
                        {
                            this.ErrorMessage = this.ReadInitFile(command.Args).Result;
                            return string.IsNullOrEmpty(this.ErrorMessage);
                        }
                        catch (AggregateException ae)
                        {
                            throw ae.InnerException;
                        }
                    });
        }

        /// <summary>
        /// Reads the configuration file.
        /// </summary>
        /// <param name="path">
        /// Path to the file
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<string> ReadInitFile(string path)
        {
            var linenum = 0;
            var errorMessage = string.Empty;

            using (var rcreader = new StreamReader(path))
            {
                String line;
                while (++linenum != 0 && (line = rcreader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    var result = await this.commandService.Run(line);
                    if (result == false)
                    {
                        errorMessage = string.Format(@"Line: {0}: Failed to run command: {1}", linenum, line);
                    }
                }
            }

            return errorMessage;
        }
    }
}