// <copyright file="Variable.cs" company="OShell Development Team">
// Copyright (c) OShell Development Team. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>

/*
 * 8/1/2008 11:29 PM
 *
 * v0.1
 * maxundos number: how many undo levels
 * wingravity: default gravity new windows will get
 * maxsizegravity: default gravity for self-maximized windows
 * bargravity: whr shuld bar appear, default ne
 * font font: use this font
 * padding left top bottom right: default spacing from screen borders
 * border pixels: how thick should frame arnd window be
 * barborder pixels: thickness of border around bar
 * inputwidth pixels: thickness of the input window
 * waitcursor (0|1): should change the cursor while waiting for readkey?
 * winfmt fmt: default info to be shown in window command
 * winname title|name|class: what is the default name of the windows as maintained by rp
 * fgcolor colr: of the windows that we create
 * bgcolor colr: of the windows that we create
 * barpadding x y: horizontal n vertical padding for our bar
 * winliststyle row|column: whether to show windows list as a column or row
 *
 * v0.2
 * resizeunit pixels: amt of pixels resizing will add/subtract
 * barinpadding: if there is padding, shuld bar appear inside window or screen
 * transgravity: where should transient windows appear
 * topkmap kmap: choose the new top keymap
 * framesels selectors: how to access more than 10 frames
 */

 /*
namespace OShell.Core
{
    using System.Drawing;

    /// <summary>
    /// Represent the various variables in a oshell context
    /// </summary>
    public class Variables
    {
        private Variables _instance = new Variables();
        public enum Gravity { NW, W, SW, S, SE, E, NE, N, C };
        public enum Padding { LEFT, TOP, BOTTOM, RIGHT };

        public int MaxUndos
        {
            get { return this.MaxUndos; }
            internal set { this.MaxUndos = value; }
        }

        public int MaxSizeGravity
        {
            get { return this.MaxSizeGravity; }
            internal set { this.MaxSizeGravity = value; }
        }

        public Gravity BarGravity
        {
            get { return this.BarGravity; }
            internal set { this.BarGravity = value; }
        }

        public Font TypeFont
        {
            get { return this.TypeFont; }
            internal set { this.TypeFont = value; }
        }

        public Padding ScreenPadding
        {
            get { return this.ScreenPadding; }
            internal set { this.ScreenPadding = value; }
        }

        public int Border
        {
            get { return this.Border; }
            internal set { this.Border = value; }
        }

        public int BarBorder
        {
            get { return this.BarBorder; }
            internal set { this.BarBorder = value; }
        }

        public uint InputWidth
        {
            get { return this.InputWidth; }
            internal set { this.InputWidth = value; }
        }

        public bool WaitCursor
        {
            get { return this.WaitCursor; }
            internal set { this.WaitCursor = value; }
        }

        public string WinFmt
        {
            get { return this.WinFmt; }
            internal set { this.WinFmt = value; }
        }

        public int BarWidth
        {
            get { return this.BarWidth; }
            internal set { this.BarWidth = value; }
        }


        /// <summary>
        /// Initialize the variables to defaults
        /// </summary>
        protected Variables()
        {

        }

        public Variables GetInstance()
        {
            return this._instance;
        }
    }
}*/
