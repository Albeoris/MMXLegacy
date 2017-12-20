using System;

namespace Legacy.Core.EventManagement
{
    public class InitUniqueDialogArgs : EventArgs
    {
        public String Caption { get; }

        public InitUniqueDialogArgs(String caption)
        {
            Caption = caption;
        }
    }
}