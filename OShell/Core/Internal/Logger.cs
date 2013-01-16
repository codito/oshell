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
    internal class Logger: IDisposable
    {
        private FileStream logStream;
        private StreamWriter logWriter;

        private static readonly Logger Instance = new Logger("oshell.log");

        private Logger(string path)
        {
            try
            {
                // TODO Move to TraceListener and Trace messages
                if (!File.Exists(path))
                    this.logStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                else
                    this.logStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);

                this.logWriter = new StreamWriter(this.logStream);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error creating log file.");
                Console.WriteLine("Exception: " + e + ". " + e.Message);

                // write all output to console
                this.logWriter = new StreamWriter(Console.OpenStandardOutput());
            }
        }

        #region Properties
        public bool UseConsole
        {
            get;
            set;
        }
        #endregion

        public static Logger GetLogger()
        {
            return Logger.Instance;
        }

        public void Debug(string message)
        {
            this.Write(Verbosity.Debug, message);
        }

        public void Error(string message)
        {
            this.Write(Verbosity.Error, message);
        }

        public void Info(string message)
        {
            this.Write(Verbosity.Info, message);
        }

        public void Write(Verbosity level, string message)
        {
            string msg = String.Format("{0}: {1}", level, message);

            // Add additional debug data for DEBUG messages
            if (level == Verbosity.Error)
                msg += String.Format(" GLE = {0}", Marshal.GetLastWin32Error());

            if (this.UseConsole)
                Console.WriteLine(msg);
            this.logWriter.WriteLine(msg);
        }

        public void Dispose()
        {
            if (this.logWriter != null)
                this.logWriter.Close();
        }
    }
}