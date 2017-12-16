using System;
using System.Xml.Serialization;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class PrivilegeUnlockedCondition : DialogCondition
	{
		[XmlAttribute("privilegeID")]
		public Int32 PrivilegeID { get; set; }

		public override EDialogState CheckCondition(Npc p_npc)
		{
			if (LegacyLogic.Instance.ServiceWrapper.IsPrivilegeAvailable(PrivilegeID))
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
