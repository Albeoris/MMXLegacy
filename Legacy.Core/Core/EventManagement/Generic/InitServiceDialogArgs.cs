using System;

namespace Legacy.Core.EventManagement
{
    public class InitServiceDialogArgs : EventArgs
    {
        public String Caption { get; }
        public String Title { get; }

        public InitServiceDialogArgs(String caption, String title)
        {
            Caption = caption;
            Title = title;
        }
    }
}