//-----------------------------------------------------------------------
// <copyright file="IKeyMapService.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OShell.Core.Contracts
{
    using System.Windows.Forms;

    public interface IKeyMapService
    {
        void AddKeyMap(Keys topKey);

        void RemoveKeyMap(Keys topKey);

        KeyMap GetKeyMap(Keys topKey);
    }
}
