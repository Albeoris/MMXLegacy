using System;
using Legacy.Core.NpcInteraction.Conditions;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction
{
	public class TradingSpellOffer
	{
		private SpellOfferStaticData m_offerData;

		private DialogCondition[] m_conditions;

		public TradingSpellOffer(Int32 p_offerId, DialogCondition[] p_conditions)
		{
			m_offerData = StaticDataHandler.GetStaticData<SpellOfferStaticData>(EDataType.SPELL_OFFERS, p_offerId);
			m_conditions = p_conditions;
		}

		public SpellOfferStaticData OfferData => m_offerData;

	    public EDialogState CheckConditions(Npc p_npc)
		{
			EDialogState edialogState = EDialogState.NORMAL;
			if (m_conditions != null)
			{
				for (Int32 i = 0; i < m_conditions.Length; i++)
				{
					EDialogState edialogState2 = m_conditions[i].CheckCondition(p_npc);
					if (edialogState2 > edialogState)
					{
						edialogState = edialogState2;
						if (edialogState == EDialogState.HIDDEN)
						{
							break;
						}
					}
				}
			}
			return edialogState;
		}
	}
}
