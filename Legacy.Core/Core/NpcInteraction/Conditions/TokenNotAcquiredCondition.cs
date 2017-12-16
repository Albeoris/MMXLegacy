using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class TokenNotAcquiredCondition : DialogCondition
	{
		[XmlAttribute("tokenID")]
		public Int32 TokenID { get; set; }

		public override EDialogState CheckCondition(Npc p_npc)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.TokenHandler.GetTokens(TokenID) == 0)
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
