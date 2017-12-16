using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class MaxSuppliesCondition : DialogCondition
	{
		private Int32 m_maxSupplies;

		[XmlAttribute("maxSupllies")]
		public Int32 MaxSupplies
		{
			get => m_maxSupplies;
		    set => m_maxSupplies = value;
		}

		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.Supplies < m_maxSupplies)
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
