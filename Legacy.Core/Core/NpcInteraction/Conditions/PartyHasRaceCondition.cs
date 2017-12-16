using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class PartyHasRaceCondition : DialogCondition
	{
		private ERace m_race;

		[XmlAttribute("race")]
		public ERace Race
		{
			get => m_race;
		    set => m_race = value;
		}

		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			for (Int32 i = 0; i < 4; i++)
			{
				if (party.GetMember(i).Class.Race == m_race)
				{
					return EDialogState.NORMAL;
				}
			}
			return FailState;
		}
	}
}
