using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class PartyHasClassCondition : DialogCondition
	{
		private EClass m_class;

		[XmlAttribute("class")]
		public EClass Class
		{
			get => m_class;
		    set => m_class = value;
		}

		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			for (Int32 i = 0; i < 4; i++)
			{
				if (party.GetMember(i).Class.Class == m_class)
				{
					return EDialogState.NORMAL;
				}
			}
			return FailState;
		}
	}
}
