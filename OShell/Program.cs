﻿//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using OShell.Core.Commands;

    using SimpleInjector.Extensions;

    using OShell.Common;
    using OShell.Core.Contracts;
    using OShell.Core.Services;
    using OShell.Views;

    static class Program
    {
        private static string configFile = Path.Combine(Environment.CurrentDirectory, "oshellrc");
        private static Form mainForm;

        // Command line Options for oshell
        private static readonly GetOpt.Option[] Options = {
            new GetOpt.Option('h', false, "help", "Displays this help"),
            new GetOpt.Option('v', false, "version", "Display version information"),
            new GetOpt.Option('c', true, "command", "Send a command to running instance of OShell"),
            new GetOpt.Option('f', true, "file", "Location of alternate rc file"),
            new GetOpt.Option('r', true, "rescue", "Resets all windows to regular style, in case things get bad")
        };

        private static SimpleInjector.Container container;

        #region Constructor
        static Program()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
        #endregion

        [System.Diagnostics.DebuggerStepThrough]
        public static TService GetInstance<TService>() where TService : class
        {
            return container.GetInstance<TService>();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.ApplicationExit += OnExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Console.CancelKeyPress += OnExit;

            OnStartup(args);
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.GetLogger().Debug("Program: Unhandled exception: " + (Exception)e.ExceptionObject);
            OnExit(sender, e);
        }

        #region Startup and Shutdown
        static void OnExit(object sender, EventArgs e)
        {
            ((IServiceBase)GetInstance<IKeyMapService>()).Start();
            ((IServiceBase)GetInstance<IWindowManagerService>()).Start();
            ((IServiceBase)GetInstance<INotificationService>()).Start();
        }

        static void OnStartup(string[] args)
        {
            // TODO free console for UI execution

            // Bootstrap container
            container = new SimpleInjector.Container();

            container.RegisterSingle<MainWindow, MainWindowImpl>();
            container.RegisterSingle<IWindowManagerService, WindowManagerService>();
            container.RegisterSingle<IKeyMapService, KeyMapService>();
            container.RegisterSingle<INotificationService, NotificationService>();
            
            // Register default command set
            var commandsMap = new Dictionary<string, Func<object>>();
            container.RegisterManyForOpenGeneric(
                typeof(ICommandHandler<>), 
                AccessibilityOption.PublicTypesOnly,
                (closedServiceType, implementations) =>
                    {
                        var commandType = closedServiceType.GenericTypeArguments[0];
                        if (implementations.Length != 1)
                        {
                            throw new Exception(string.Format(
                                "Bootstrap: Duplicate handler found for Command: {0}",
                                commandType));
                        }
                        var commandName = commandType.GetProperty("Name")
                                                     .GetValue(Activator.CreateInstance(commandType))
                                                     .ToString();
                        commandsMap.Add(commandName, () => container.GetInstance(closedServiceType));
                    },
                Assembly.GetExecutingAssembly());

            // Register CommandService
            container.RegisterSingle<ICommandService>(
                new CommandService(
                    container.GetInstance<MainWindow>(),
                    commandsMap));

            container.Verify();

            //var cmdH = container.GetInstance<ICommandProvider>().GetCommandHandler("newkmap");
            //var cmd = container.GetInstance<ICommandProvider>().GetCommand("newkmap");
            var cmdSvc = container.GetInstance<ICommandService>();
            cmdSvc.Run("newkmap foo");

            mainForm = container.GetInstance<MainWindow>();

            // Parse arguments and run the app
            var getopt = new GetOpt(Options);

            char returnChar = ' ';
            int optionIndex = 0;
            string optionArg = null;
            bool startWindowManager = true;

            try
            {
                do
                {
                    // parse the args
                    switch (returnChar)
                    {
                        case 'h':
                            Console.Write(getopt.Description);
                            return;
                        case 'v':
                            Console.WriteLine(Application.ProductVersion);
                            return;
                        case 'f': // parse alternate rc file
                            configFile = optionArg;
                            break;
                        case 'c': // send a command
                            startWindowManager = false;
                            break;
                        case 'r': // reset all windows to decorated regular state, in case things screw up :(
                            startWindowManager = false;
                            WindowManagerService.Reset();
                            break;
                    }
                    returnChar = getopt.Parse(args, ref optionIndex, out optionArg);
                } while (returnChar != ' ');
            }
            catch (GetOpt.InvalidOptionException e)
            {
                Console.WriteLine(e.Message + "\nUse -h or --help for instructions");
                return;
            }

            if (startWindowManager)
            {
                // Start the services
                //((IServiceBase)GetInstance<IKeyMapService>()).Start();
                //((IServiceBase)GetInstance<IWindowManagerService>()).Start();
                //((IServiceBase)GetInstance<INotificationService>()).Start();

                // set the internal variables
                // set the data structures
                // read rc file
                //CommandManager.Execute((int)CommandManager.OtherCommands.source, new string[] { _configFile });
                //GetInstance<IKeyMapService>().AddKeyMap(Keys.Control | Keys.T);
                //Application.Run(mainForm);
            }
            else
            {
                // check if an instance already runs
                // send the commands
                // throw output to console or to the bar
            }
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Allocate a console if application started from within windows GUI.
        /// Detects the presence of an existing console associated with the application and
        /// attaches itself to it if available.
        /// </summary>
        private static void AllocateConsole()
        {
            var notifSvc = GetInstance<INotificationService>();
            if (!Interop.AttachConsole(Interop.ATTACH_PARENT_PROCESS) && Marshal.GetLastWin32Error() == Interop.ERROR_ACCESS_DENIED)
            {
                // A console was not allocated, so we need to make one
                if (!Interop.AllocConsole())
                {
                    notifSvc.NotifyError("Unable to attach console: " + Marshal.GetLastWin32Error());
                }
                else
                {
                    notifSvc.NotifyDebug("Attached console.");
                }

            }
        }
        #endregion
    }
}