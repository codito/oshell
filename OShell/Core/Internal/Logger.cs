//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        private static readonly Logger LoggerInstance = new Logger("oshell.log");

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
        /// Gets an instance of the active logger.
        /// </summary>
        /// <value>The <see cref="Logger"/> instance.</value>
        public static Logger Instance
        {
            get
            {
                return LoggerInstance;
            }
        }

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
        /// Write the log message with Debug verbosity.
        /// </summary>
        /// <param name="format">Message format</param>
        /// <param name="args">Additional arguments</param>
        public void Debug(string format, params object[] args)
        {
            this.Write(Verbosity.Debug, format, args);
        }

        /// <summary>
        /// Write the log message with Error verbosity.
        /// </summary>
        /// <param name="format">Message format</param>
        /// <param name="args">Additional arguments</param>
        public void Error(string format, params object[] args)
        {
            this.Write(Verbosity.Error, format, args);
        }

        /// <summary>
        /// Write the log message with Info verbosity.
        /// </summary>
        /// <param name="format">Message format</param>
        /// <param name="args">Additional arguments</param>
        public void Info(string format, params object[] args)
        {
            this.Write(Verbosity.Info, format, args);
        }

        /// <summary>
        /// Write the log message with <paramref name="level"/> verbosity.
        /// </summary>
        /// <param name="level">Verbosity level.</param>
        /// <param name="format">Message format</param>
        /// <param name="args">Additional arguments</param>
        public void Write(Verbosity level, string format, params object[] args)
        {
            var prettyMessage = string.Format("{0}: {1}", level, string.Format(format, args));

            // Add additional debug data for DEBUG messages
            if (level == Verbosity.Error)
            {
                prettyMessage += string.Format(" GLE = {0}", Marshal.GetLastWin32Error());
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

            if (this.logStream != null)
            {
                this.logStream.Close();
            }
        }

        #endregion
    }
}