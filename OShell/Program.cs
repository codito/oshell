//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
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
            new GetOpt.Option('h', false, "help", "Displays this help"),
            new GetOpt.Option('v', false, "version", "Display version information"),
            new GetOpt.Option('c', true, "command", "Send a command to running instance of OShell"),
            new GetOpt.Option('f', true, "file", "Location of alternate rc file"),
            new GetOpt.Option('r', true, "rescue", "Resets all windows to regular style, in case things get bad")
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
            Logger.GetLogger().Debug("Program: Unhandled exception: " + e.ExceptionObject);
            OnExit(sender, e);
        }

        #region Startup and Shutdown
        private static void OnExit(object sender, EventArgs e)
        {
        }

        private static async void OnStartup(string[] args)
        {
            // TODO free console for UI execution

            // Bootstrap container
            container = new SimpleInjector.Container();

            container.RegisterSingle<IPlatformFacade, WindowsPlatform>();
            container.RegisterSingle<IWindowManagerService, WindowManagerService>();
            container.RegisterSingle<IKeyMapService, KeyMapService>();
            container.RegisterSingle<INotificationService, NotificationService>();
            container.RegisterSingle<IMainWindow, MainWindow>();
            
            // Register default command set
            var commands =
                AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(s => s.GetTypes())
                         .Where(p => p != typeof(ICommand) && typeof(ICommand).IsAssignableFrom(p))
                         .ToList();
            container.RegisterAll<ICommand>(commands);
            container.RegisterManyForOpenGeneric(
                typeof(ICommandHandler<>),
                AccessibilityOption.PublicTypesOnly,
                Assembly.GetExecutingAssembly());

            // Register handlers
            var commandHandlers =
                commands.Select(command => container.GetInstance(typeof(ICommandHandler<>).MakeGenericType(command)));
            container.RegisterSingle<ICommandService>(
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
                Console.WriteLine(e.Message + "\nUse -h or --help for instructions");
                return;
            }

            if (startWindowManager)
            {
                // Start the services
                ((IServiceBase)container.GetInstance<IKeyMapService>()).Start();
                ((IServiceBase)container.GetInstance<IWindowManagerService>()).Start();
                ((IServiceBase)container.GetInstance<INotificationService>()).Start();

                // set the internal variables
                // set the data structures
                // read rc file
                ////CommandManager.Execute((int)CommandManager.OtherCommands.source, new string[] { _configFile });
                container.GetInstance<IKeyMapService>().AddKeyMap(Keys.Control | Keys.T);
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