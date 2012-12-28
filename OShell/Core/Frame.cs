﻿//-----------------------------------------------------------------------
// <copyright file="Frame.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Represents the concept of a Frame. A <see cref="Frame"/> can contain multiple 
    /// <see cref="Window"/> instances. User can split a frame vertically horizontally.
    /// </summary>
    /// <remarks>See <url>http://www.nongnu.org/ratpoison/doc/Concepts.html#Concepts</url> for details</remarks>
    public class Frame
    {
        /// <summary>
        /// Initializes an instance of <see cref="Frame"/> class.
        /// </summary>
        /// <param name="screen">Parent screen for the <see cref="Frame"/></param>
        public Frame(Screen screen)
        {
            this.Screen = screen;
            this.Size = this.Screen.WorkingArea;
        }

        #region Properties
        /// <summary>
        /// Gets the parent screen for the frame.
        /// </summary>
        public Screen Screen { get; private set; }

        /// <summary>
        /// Gets the size of the frame.
        /// </summary>
        public Rectangle Size { get; private set; }
        #endregion
    }
}
