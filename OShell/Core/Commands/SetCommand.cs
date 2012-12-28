//-----------------------------------------------------------------------
// <copyright file="SetCommand.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Commands
{
    using OShell.Core.Commands;

    public class SetCommand
    {
        public enum Options
        {
            padding,
            topkeymap,  // Key Maps
            waitcursor,

            // Manipulating Windows
            border,
            infofmt,
            maxsizegravity,
            transgravity,
            winfmt,
            wingravity,
            winliststyle,
            winname,

            // Status bar
            barborder,
            bargravity,
            barpadding,
            bgcolor,
            fgcolor,
            font,
            framefmt,
            inputwidth,

            // Resize frames
            resizeunit
        }

        public bool Execute(string args)
        {
            throw new System.NotImplementedException();
        }
    }
}
