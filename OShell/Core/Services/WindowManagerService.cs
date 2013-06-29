//-----------------------------------------------------------------------
// <copyright file="WindowManagerService.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using OShell.Core.Contracts;
    using OShell.Core.Internal;

    /// <summary>
    /// Implementation of the Window Manager service.
    /// </summary>
    public class WindowManagerService : ServiceBase, IWindowManagerService
    {
        /// <summary>
        /// The platform facade instance.
        /// </summary>
        private readonly IPlatformFacade platformFacade;

        /// <summary>
        /// Notification service instance.
        /// </summary>
        private readonly INotificationService notificationService;

        #region Constructor

        /// <summary>
        /// Initializes an instance of the <see cref="WindowManagerService"/> class.
        /// </summary>
        /// <param name="platformFacade">Instance of the Platform implementation facade</param>
        /// <param name="notificationService">Instance of Notification Service</param>
        public WindowManagerService(IPlatformFacade platformFacade, INotificationService notificationService)
        {
            this.platformFacade = platformFacade;
            this.notificationService = notificationService;
            this.ManagedWindows = new Dictionary<IntPtr, Window>();
            this.Frames = new List<Frame>();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the dictionary of managed windows handles and references.
        /// </summary>
        public Dictionary<IntPtr, Window> ManagedWindows { get; private set; }

        /// <summary>
        /// Gets the list of managed frames.
        /// </summary>
        public List<Frame> Frames { get; private set; }

        /// <summary>
        /// Gets the currently active frame.
        /// </summary>
        public Frame CurrentFrame { get; private set; }
        #endregion

        #region Start/Stop
        /// <summary>
        /// Starts the window manager service.
        /// </summary>
        public override void Start()
        {
            // TODO Multi monitor
            this.CurrentFrame = new Frame(Screen.PrimaryScreen);
            this.Frames.Add(this.CurrentFrame);

            // Add existing windows to the frame in primary monitor
            if (!Interop.EnumWindows(this.EnumWndProc, IntPtr.Zero))
            {
                Logger.Instance.Debug("WMService: Failed to add existing windows.");
            }

            // Register shell hooks for window created and destroyed
            this.platformFacade.RegisterShellHookWindow();
        }

        /// <summary>
        /// Stops the window manager service.
        /// </summary>
        public override void Stop()
        {
            // Unregister shell hooks
            this.platformFacade.DeregisterShellHookWindow();

            // Unmanaged all managed windows
            foreach (Window window in this.ManagedWindows.Values)
            {
                window.Unmanage();
            }

            this.ManagedWindows.Clear();
            this.Frames.Clear();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Adds a window to currently managed windows list.
        /// </summary>
        /// <param name="handle">Native window handle</param>
        public void AddWindow(IntPtr handle)
        {
            if (Window.IsVisibleWindow(handle) == false)
            {
                return;
            }

            var window = new Window(this.CurrentFrame, handle, this.notificationService);

            try
            {
                window.Manage();
                window.SwitchFrame(this.CurrentFrame);
            }
            catch
            {
                Logger.Instance.Error("WMService: Failed to add existing window. HWnd: " + handle);
                window.Unmanage();
                return;
            }

            this.ManagedWindows.Add(handle, window);
            Logger.Instance
                  .Debug(
                      "WMService: Added window. HWnd = {0}, Name = {1}, Application Name = {2}",
                      window.Handle,
                      window.Name,
                      window.ApplicationName);
        }

        /// <summary>
        /// Removes a window from the currently managed list.
        /// </summary>
        /// <param name="handle">Native window handle</param>
        public void RemoveWindow(IntPtr handle)
        {
            if (this.ManagedWindows.ContainsKey(handle) == false)
            {
                return;
            }

            if (Window.IsVisibleWindow(handle))
            {
                this.ManagedWindows[handle].Unmanage();
            }

            this.ManagedWindows.Remove(handle);
            Logger.Instance.Debug("WMService: Removed window from managed windows list. HWnd = " + handle);
        }
        #endregion

        #region Reset state
        /// <summary>
        /// Resets all visible windows to standard styles. Used as a rescue mechanism in case
        /// windows get into messed up state. See <c>-r or -reset</c> command line parameter.
        /// </summary>
        internal static void Reset()
        {
            if (!Interop.EnumWindows(ResetEnumWndProc, IntPtr.Zero))
            {
                Logger.Instance.Debug("WMService: Failed to reset existing windows.");
            }
        }

        /// <summary>
        /// Delegate for the Reset windows routine.
        /// </summary>
        /// <param name="handle">Native window handle</param>
        /// <param name="lparam">Additional info about window</param>
        /// <returns>True always</returns>
        private static bool ResetEnumWndProc(IntPtr handle, IntPtr lparam)
        {
            Window.ResetWindowStyle(handle);
            return true;
        }

        /// <summary>
        /// Delegate for the initial window look up routine.
        /// </summary>
        /// <param name="handle">Native window handle</param>
        /// <param name="lparam">Additional info about window</param>
        /// <returns>True always</returns>
        private bool EnumWndProc(IntPtr handle, IntPtr lparam)
        {
            this.AddWindow(handle);
            return true;
        }
        #endregion
    }
}
