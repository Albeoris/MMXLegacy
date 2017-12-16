using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class PartyHasRaceGenderCondition : DialogCondition
	{
		private ERace m_race;

		private EGender m_gender;

		[XmlAttribute("race")]
		public ERace Race
		{
			get => m_race;
		    set => m_race = value;
		}

		[XmlAttribute("gender")]
		public EGender Gender
		{
			get => m_gender;
		    set => m_gender = value;
		}

		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			for (Int32 i = 0; i < 4; i++)
			{
				if (party.GetMember(i).Class.Race == m_race && party.GetMember(i).Gender == m_gender)
				{
					return EDialogState.NORMAL;
				}
			}
			return FailState;
		}
	}
}
