using System;
using System.Xml.Serialization;

namespace Legacy.Core.NpcInteraction.Conditions
{
    public enum EDialogInjectionType
    {
        InsertBefore,
        InsertAfter,
        Replace
    }

    public class DialogEntryInjection
    {
        [XmlAttribute("type")]
        public EDialogInjectionType InjectionType { get; set; }

        [XmlAttribute("target")]
        public String TextKey { get; set; }
    }
}