//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using OShell.Core;
    using OShell.Core.Contracts;
    using OShell.Core.Internal;
    using OShell.Core.Services;
    using OShell.Views;

    using SimpleInjector.Extensions;

    /// <summary>
    /// OShell entry point.
    /// </summary>
    internal static class Program
    {
        // Command line Options for oshell
        private static readonly GetOpt.Option[] Options =
        {
            new GetOpt.Option('h', false, "help", Properties.Resources.Option_Help_Description),
            new GetOpt.Option('v', false, "version", Properties.Resources.Option_Version_Description),
            new GetOpt.Option('c', true, "command", Properties.Resources.Option_File_Description),
            new GetOpt.Option('f', true, "file", Properties.Resources.Option_File_Description),
            new GetOpt.Option('r', true, "rescue", Properties.Resources.Option_Rescue_Description)
        };

        private static string configFile = Path.Combine(Environment.CurrentDirectory, "oshellrc");
        private static Form mainForm;
        private static SimpleInjector.Container container;

        #region Constructor
        static Program()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.ApplicationExit += OnExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Console.CancelKeyPress += OnExit;

            OnStartup(args);
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Instance.Debug("Program: Unhandled exception: {0}", e.ExceptionObject);
            OnExit(sender, e);
        }

        #region Startup and Shutdown
        private static void OnExit(object sender, EventArgs e)
        {
        }

        private static void OnStartup(string[] args)
        {
            // TODO free console for UI execution

            // Bootstrap container
            container = new SimpleInjector.Container();

            container.RegisterSingleton<IPlatformFacade, WindowsPlatform>();
            container.RegisterSingleton<IWindowManagerService, WindowManagerService>();
            container.RegisterSingleton<IKeyMapService, KeyMapService>();
            container.RegisterSingleton<INotificationService, NotificationService>();
            container.RegisterSingleton<IMainWindow, MainWindow>();

            // Register default command set
            var commands =
                AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(s => s.GetTypes())
                         .Where(p => p != typeof(ICommand) && typeof(ICommand).IsAssignableFrom(p))
                         .ToList();
            container.RegisterCollection<ICommand>(commands);
            container.Register(
                typeof(ICommandHandler<>),
                container.GetTypesToRegister(typeof(ICommandHandler<>), new[] { Assembly.GetExecutingAssembly() })
                    .Where(t => t.IsPublic));

            // Register handlers
            var commandHandlers =
                commands.Select(command => container.GetInstance(typeof(ICommandHandler<>).MakeGenericType(command)));
            container.RegisterSingleton<ICommandService>(
                () => new CommandService(container.GetAllInstances<ICommand>(), commandHandlers));

            container.Verify();

            mainForm = container.GetInstance<IMainWindow>() as Form;

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
                }
                while (returnChar != ' ');
            }
            catch (GetOpt.InvalidOptionException e)
            {
                Console.WriteLine(Properties.Resources.Option_Help_Error);
                Console.WriteLine(e);
                return;
            }

            if (startWindowManager)
            {
                // Start the services
                ((ServiceBase)container.GetInstance<IKeyMapService>()).Start();
                ((ServiceBase)container.GetInstance<IWindowManagerService>()).Start();
                ((ServiceBase)container.GetInstance<INotificationService>()).Start();

                // set the internal variables
                // set the data structures
                // read rc file
                ////CommandManager.Execute((int)CommandManager.OtherCommands.source, new string[] { _configFile });
                var keyMapService = container.GetInstance<IKeyMapService>();

                // Top keymap does not require the window on focus, they are global hot keys
                // Prefix key must reside here
                keyMapService.AddKeyMap("top");
                keyMapService.SetTopKey("top", Keys.Control | Keys.B);

                // Root keymap is the default keymap invoked via Prefix key with readkey command
                // All other default shortcuts reside here
                keyMapService.AddKeyMap("root");

                var commandService = container.GetInstance<ICommandService>();
                commandService.Run("definekey top T readkey root");

                // Set default variables
                // commandService.Run()
                ////keyMapService.SetTopKey("root", Keys.Control | Keys.T);
                Application.Run(mainForm);
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
            var notifSvc = container.GetInstance<INotificationService>();
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