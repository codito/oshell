//-----------------------------------------------------------------------
// <copyright file="Window.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core
{
    using System;
    using System.Drawing;
    using System.Text;

    using OShell.Core.Contracts;
    using OShell.Core.Internal;

    /// <summary>
    /// Represents a Window in the system.
    /// </summary>
    public class Window
    {
        /// <summary>
        /// Style options used for <see cref="Manage"/> and <see cref="Unmanage"/> routines.
        /// </summary>
        private const long WindowStyle = (long)(Interop.GwlStyle.WS_MAXIMIZEBOX | Interop.GwlStyle.WS_MAXIMIZE | Interop.GwlStyle.WS_THICKFRAME);

        /// <summary>
        /// Extended style options used for <see cref="Manage"/> and <see cref="Unmanage"/> routines.
        /// </summary>
        private const long WindowExStyle = (long)(Interop.GwlExStyle.WS_EX_DLGMODALFRAME | Interop.GwlExStyle.WS_EX_CLIENTEDGE | Interop.GwlExStyle.WS_EX_STATICEDGE);

        /// <summary>
        /// Backing field for the <see cref="ApplicationName"/> property.
        /// </summary>
        private string applicationName;

        /// <summary>
        /// Cached size of the window. Used to revert back to the original size
        /// when window is unmanaged.
        /// </summary>
        private Rectangle cachedSize;

        /// <summary>
        /// Cached style for the window. Used to revert back to original style
        /// when window is unmanaged.
        /// </summary>
        private IntPtr cachedStyle;

        /// <summary>
        /// Cached extended style for the window. Used to revert back to original
        /// style when window is unmanaged.
        /// </summary>
        private IntPtr cachedExStyle;

        /// <summary>
        /// Backing field for the <see cref="ClassName"/> property.
        /// </summary>
        private string className;

        /// <summary>
        /// Notification service instance.
        /// </summary>
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes an instance of <see cref="Window"/> class.
        /// </summary>
        /// <param name="frame">Parent Frame for this Window</param>
        /// <param name="hwnd">Native window handle</param>
        /// <param name="notificationService">Instance of Notification Service</param>
        public Window(Frame frame, IntPtr hwnd, INotificationService notificationService)
        {
            this.cachedStyle = IntPtr.Zero;
            this.notificationService = notificationService;
            this.Frame = frame;
            this.Handle = hwnd;
        }

        #region Properties
        /// <summary>
        /// Gets the name of the application which owns this <see cref="Window"/>.
        /// </summary>
        public string ApplicationName
        { 
            get
            {
                if (String.IsNullOrEmpty(this.applicationName))
                {
                    this.applicationName = GetApplicationName(this.Handle);
                }

                return this.applicationName;
            }
        }

        /// <summary>
        /// Gets the class of this <see cref="Window"/>.
        /// </summary>
        public string ClassName
        {
            get
            {
                if (String.IsNullOrEmpty(this.className))
                {
                    this.className = GetClassName(this.Handle);
                }

                return this.className;
            }
        }

        /// <summary>
        /// Gets the parent <see cref="Frame"/> of this <see cref="Window"/>.
        /// </summary>
        public Frame Frame { get; private set; }

        /// <summary>
        /// Gets the native window handle.
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// Gets the caption text in the title bar.
        /// </summary>
        public string Name
        {
            get
            {
                return Window.GetCaptionText(this.Handle);
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        public Rectangle Size { get; private set; }
        #endregion

        #region Public methods
        /// <summary>
        /// Makes this a foreground window.
        /// </summary>
        public void BringToForeground()
        {
            Interop.SwitchToThisWindow(this.Handle, false);
        }

        /// <summary>
        /// Start managing the Window. Strip off unnecessary decorations from the window title bar.
        /// </summary>
        public void Manage()
        {
            // Cache current style
            this.cachedStyle = Interop.GetWindowLongPtr(this.Handle, Interop.GWL_STYLE);
            if (this.cachedStyle == IntPtr.Zero)
            {
                Logger.Instance.Debug("Window: Failed to get style. HWnd = {0}, Name = {1}", this.Handle, this.Name);
                return;
            }

            this.cachedExStyle = Interop.GetWindowLongPtr(this.Handle, Interop.GWL_EXSTYLE);
            if (this.cachedExStyle == IntPtr.Zero)
            {
                Logger.Instance.Debug("Window: Failed to get extended style. HWnd = {0}, Name = {1}", this.Handle, this.Name);
                return;
            }

            Logger.Instance.Debug("Window: Cached attributes: \n{0}", Window.DumpWindowStyle(this.Handle));

            // Apply new style
            var newStyle = this.cachedStyle.ToInt64() & ~Window.WindowStyle;
            if (Interop.SetWindowLongPtr(this.Handle, Interop.GWL_STYLE, new IntPtr(newStyle)) == IntPtr.Zero)
            {
                Logger.Instance.Debug("Window: Failed to undecorate. HWnd = {0}, Name = {1}, Style = {2}", this.Handle, this.Name, newStyle);
                return;
            }

            newStyle = this.cachedExStyle.ToInt64() & ~Window.WindowExStyle;
            if (Interop.SetWindowLongPtr(this.Handle, Interop.GWL_EXSTYLE, new IntPtr(newStyle)) == IntPtr.Zero)
            {
                Logger.Instance
                      .Debug(
                          "Window: Failed to undecorate extended styles. HWnd = {0}, Name = {1}, Style = {2}",
                          this.Handle,
                          this.Name,
                          newStyle);
                return;
            }

            Logger.Instance.Debug("Window: Applied attributes: \n{0}", Window.DumpWindowStyle(this.Handle));

            // Cache current size and position
            this.cachedSize = Window.GetWindowSize(this.Handle);

            // Apply new size and position
            Window.SetWindowPosition(this.Handle, this.Size);
        }

        /// <summary>
        /// Stop managing the window. Add back any decorations to the title bar.
        /// </summary>
        public void Unmanage()
        {
            // Decorate window back
            var newStyle = this.cachedStyle.ToInt64();
            if (Interop.SetWindowLongPtr(this.Handle, Interop.GWL_STYLE, new IntPtr(newStyle)) == IntPtr.Zero)
            {
                Logger.Instance.Debug("Window: Failed to decorate. HWnd = {0}, Name = {1}, Style = {2}", this.Handle, this.Name, newStyle);
            }

            newStyle = this.cachedExStyle.ToInt64();
            if (Interop.SetWindowLongPtr(this.Handle, Interop.GWL_EXSTYLE, new IntPtr(newStyle)) == IntPtr.Zero)
            {
                Logger.Instance.Debug("Window: Failed to decorate extended style. HWnd = {0}, Name = {1}, Style = {2}", this.Handle, this.Name, newStyle);
            }

            // Apply the cached size
            Window.SetWindowPosition(this.Handle, this.cachedSize);
        }

        /// <summary>
        /// Switch the window to a different frame.
        /// </summary>
        /// <param name="f">Target frame</param>
        public void SwitchFrame(Frame f)
        {
            if (!Interop.MoveWindow(this.Handle, f.Size.X, f.Size.Y, f.Size.Width, f.Size.Height, true))
            {
                this.notificationService.NotifyError(
                    String.Format("Window: Unable to move window. HWnd = {0}, Name = {1}", this.Handle, this.Name));
            }

            this.Size = f.Size;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Dumps the current GWL_STYLE and GWL_EXSTYLE for the window.
        /// </summary>
        /// <param name="hwnd">Native windows handle</param>
        /// <returns>Style for the window</returns>
        internal static string DumpWindowStyle(IntPtr hwnd)
        {
            var style = new StringBuilder(512);
            var gwlstyle = Interop.GetWindowLongPtr(hwnd, Interop.GWL_STYLE);
            if (gwlstyle == IntPtr.Zero)
            {
                Logger.Instance.Debug("Window: Failed to get GWL_STYLE. HWnd = {0}, Name = {1}", hwnd, Window.GetCaptionText(hwnd));
                return String.Empty;
            }

            foreach (var s in Enum.GetValues(typeof(Interop.GwlStyle)))
            {
                if ((gwlstyle.ToInt64() & (long)s) == (long)s)
                {
                    style.Append(" " + s);
                }
            }

            style.Append(Environment.NewLine);
            var exstyle = Interop.GetWindowLongPtr(hwnd, Interop.GWL_EXSTYLE);
            if (exstyle == IntPtr.Zero)
            {
                Logger.Instance
                      .Debug(
                          "Window: Failed to get GWL_EXSTYLE. HWnd = {0}, Name = {1}", hwnd, Window.GetCaptionText(hwnd));
                return String.Empty;
            }

            foreach (var s in Enum.GetValues(typeof(Interop.GwlExStyle)))
            {
                if ((exstyle.ToInt64() & (long)s) == (long)s)
                {
                    style.Append(" " + s);
                }
            }

            return style.ToString();
        }

        /// <summary>
        /// Gets the name of the process which owns given window.
        /// </summary>
        /// <param name="hwnd">Native window handle</param>
        /// <returns>Name of the application</returns>
        internal static string GetApplicationName(IntPtr hwnd)
        {
            var sb = new StringBuilder(512);
            uint processId = 0;
            Interop.GetWindowThreadProcessId(hwnd, out processId);

            IntPtr processHandle = Interop.OpenProcess(0x0410, false, processId);
            Interop.GetModuleFileNameEx(processHandle, IntPtr.Zero, sb, sb.Capacity);
            Interop.CloseHandle(processHandle);

            return sb.ToString();
        }

        /// <summary>
        /// Gets the caption text in title bar.
        /// </summary>
        /// <param name="hwnd">Native window handle</param>
        /// <returns>Caption text</returns>
        internal static string GetCaptionText(IntPtr hwnd)
        {
            const uint WM_GETTEXT = 0x000D;
            const uint WM_GETTEXTLENGTH = 0x000E;

            var length = (int)Interop.SendMessage(hwnd, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
            var sb = new StringBuilder(length + 1);
            Interop.SendMessage(hwnd, WM_GETTEXT, (IntPtr)sb.Capacity, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Gets the class name for the window handle.
        /// </summary>
        /// <param name="hwnd">Native window handle</param>
        /// <returns>Class name</returns>
        internal static string GetClassName(IntPtr hwnd)
        {
            var sb = new StringBuilder(512);
            if (Interop.GetClassName(hwnd, sb, sb.Capacity) == 0)
            {
                Logger.Instance.Debug("Window: Failed to get class name. HWnd = {0}", hwnd);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the bounding rectangle for a window.
        /// </summary>
        /// <param name="hwnd">Native window handle</param>
        /// <returns>Bounding rectangle</returns>
        internal static Rectangle GetWindowSize(IntPtr hwnd)
        {
            var size = new Rectangle();
            Interop.RECT rct;

            if (!Interop.GetWindowRect(hwnd, out rct))
            {
                Logger.Instance.Error("Window: Failed to get bounding rectangle. HWnd = " + hwnd);
            }
            else
            {
                size.X = rct.Left;
                size.Y = rct.Top;
                size.Width = rct.Right - rct.Left + 1;
                size.Height = rct.Bottom - rct.Top + 1;
            }

            return size;
        }

        /// <summary>
        /// Validates the window handle with following visibility criteria:
        /// <list type="bullet">
        /// <item>Should be a visible window</item>
        /// <item>Shouldn't be a tool window</item>
        /// </list>
        /// </summary>
        /// <param name="hwnd">Native window handle</param>
        /// <returns>True if the window meets all criteria</returns>
        internal static bool IsVisibleWindow(IntPtr hwnd)
        {
            if (!Interop.IsWindowVisible(hwnd))
            {
                return false;
            }

            // Discard tool windows since they don't appear directly to the user through Alt+Tab or taskbar
            // and don't need management!
            IntPtr style = Interop.GetWindowLongPtr(hwnd, Interop.GWL_EXSTYLE);
            if (style == IntPtr.Zero)
            {
                return false;
            }

            if ((style.ToInt64() & (long)Interop.GwlExStyle.WS_EX_TOOLWINDOW) == (long)Interop.GwlExStyle.WS_EX_TOOLWINDOW)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes any applied custom styles. Use this as a rescue utility in case the program
        /// leaves system in an unrecoverable state. See <c>-r, -reset</c> command line option.
        /// </summary>
        /// <param name="hwnd">Native window handle</param>
        internal static void ResetWindowStyle(IntPtr hwnd)
        {
            if (Window.IsVisibleWindow(hwnd) == false)
            {
                return;
            }

            // Reset GWL_STYLE
            var currentStyle = Interop.GetWindowLongPtr(hwnd, Interop.GWL_STYLE);
            if (currentStyle == IntPtr.Zero)
            {
                Logger.Instance.Error("Window: Failed to get style. HWnd = {0}", hwnd);
            }

            var newStyle = currentStyle.ToInt64() | Window.WindowStyle;
            if (Interop.SetWindowLongPtr(hwnd, Interop.GWL_STYLE, new IntPtr(newStyle)) == IntPtr.Zero)
            {
                Logger.Instance.Error("Window: Failed to decorate. HWnd = {0}, Style = {1}", hwnd, newStyle);
            }

            // Reset GWL_EXSTYLE
            currentStyle = Interop.GetWindowLongPtr(hwnd, Interop.GWL_EXSTYLE);
            if (currentStyle == IntPtr.Zero)
            {
                Logger.Instance.Error("Window: Failed to get extended style. HWnd = {0}", hwnd);
            }

            newStyle = currentStyle.ToInt64() | Window.WindowExStyle;
            if (Interop.SetWindowLongPtr(hwnd, Interop.GWL_STYLE, new IntPtr(newStyle)) == IntPtr.Zero)
            {
                Logger.Instance.Error("Window: Failed to decorate extended style. HWnd = {0}, Style = {1}", hwnd, newStyle);
            }
        }

        /// <summary>
        /// Sets the position of window specified by <paramref name="hwnd"/> to <paramref name="position"/>. Used
        /// after <c>SetWindowLongPtr</c> call to apply the settings.
        /// Doesn't bring the window to foreground. Use <see cref="BringToForeground"/> for that purpose.
        /// </summary>
        /// <param name="hwnd">Native window handle</param>
        /// <param name="position">New position of the window</param>
        internal static void SetWindowPosition(IntPtr hwnd, Rectangle position)
        {
            var windowsPositionFlags = (uint)(Interop.SetWindowPosFlags.FrameChanged | Interop.SetWindowPosFlags.DoNotActivate);
            if (!Interop.SetWindowPos(hwnd, IntPtr.Zero, position.X, position.Y, position.Width, position.Height, windowsPositionFlags))
            {
                Logger.Instance.Error("Window: Unable to set window position. HWnd = " + hwnd);
            }
        }

        #endregion
    }
}
