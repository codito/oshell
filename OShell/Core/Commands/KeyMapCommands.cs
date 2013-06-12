//-----------------------------------------------------------------------
// <copyright file="KeyMap.cs" company="OShell Development Team">
//     Copyright (c) OShell Development Team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

// Implementation for following commands:
//
// newkmap, delkmap
// definekey, undefinekey
// bind, unbind
// readkey, describekey
// link

namespace OShell.Core.Commands
{
    using System.Threading.Tasks;

    using OShell.Core.Contracts;

    #region Implementation: delkmap
    public class DelKMapCommand : ICommand
    {
        public string Name
        {
            get
            {
                return "delkmap";
            }
        }

        public string Args { get; set; }

        public string Help
        {
            get
            {
                return "Syntax: delkmap keymap. Deletes the keymap named keymap, that was generated with newkmap."
                       + " The keymaps top (or whatever was specified by set topkmap) and root cannot be deleted.";
            }
        }
    }

    public class DelKMapCommandHandler : ICommandHandler<DelKMapCommand>
    {
        public DelKMapCommandHandler(IKeyMapService keyMapService)
        {
            this.KeyMapService = keyMapService;
        }

        protected IKeyMapService KeyMapService { get; private set; }

        public Task<bool> Execute(DelKMapCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
    #endregion
}
