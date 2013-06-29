//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Internal
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Log message verbosity.
    /// </summary>
    public enum Verbosity
    {
        /// <summary>
        /// Filter log messages with Debug verbosity
        /// </summary>
        Debug,

        /// <summary>
        /// Filter log messages with Error verbosity
        /// </summary>
        Error,

        /// <summary>
        /// Filter log messages with Info verbosity
        /// </summary>
        Info
    }

    /// <summary>
    /// Utility class to maintain activities log.
    /// </summary>
    internal class Logger : IDisposable
    {
        private static readonly Logger Instance = new Logger("oshell.log");

        private readonly FileStream logStream;
        private readonly StreamWriter logWriter;

        private Logger(string path)
        {
            try
            {
                // TODO Move to TraceListener and Trace messages
                if (!File.Exists(path))
                {
                    this.logStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                }
                else
                {
                    this.logStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
                }

                this.logWriter = new StreamWriter(this.logStream);
            }
            catch (Exception e)
            {
                Console.WriteLine(Properties.Resources.Logger_Error_CreatingFile, e);

                // write all output to console
                this.logWriter = new StreamWriter(Console.OpenStandardOutput());
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to use console for log messages.
        /// </summary>
        public bool UseConsole
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Gets an instance of the active logger.
        /// </summary>
        /// <returns>The <see cref="Logger"/> instance.</returns>
        public static Logger GetLogger()
        {
            return Instance;
        }

        /// <summary>
        /// Write the log message with Debug verbosity.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            this.Write(Verbosity.Debug, message);
        }

        /// <summary>
        /// Write the log message with Error verbosity.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            this.Write(Verbosity.Error, message);
        }

        /// <summary>
        /// Write the log message with Info verbosity.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            this.Write(Verbosity.Info, message);
        }

        /// <summary>
        /// Write the log message with <paramref name="level"/> verbosity.
        /// </summary>
        /// <param name="level">Verbosity level.</param>
        /// <param name="message">Log message.</param>
        public void Write(Verbosity level, string message)
        {
            var prettyMessage = String.Format("{0}: {1}", level, message);

            // Add additional debug data for DEBUG messages
            if (level == Verbosity.Error)
            {
                prettyMessage += String.Format(" GLE = {0}", Marshal.GetLastWin32Error());
            }

            if (this.UseConsole)
            {
                Console.WriteLine(prettyMessage);
            }
            this.logWriter.WriteLine(prettyMessage);
        }

        #region IDisposable implementation

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.logWriter != null)
            {
                this.logWriter.Close();
            }
        }

        #endregion
    }
}