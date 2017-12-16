using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class CastFunction : DialogFunction
	{
		private Int32 m_dialogID;

		private Int32 m_price;

		private ECharacterSpell[] m_spells;

		public CastFunction(Int32 p_price, ECharacterSpell[] p_spells, Int32 p_dialogID)
		{
			m_spells = p_spells;
			m_price = p_price;
			m_dialogID = p_dialogID;
		}

		public CastFunction()
		{
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("price")]
		public Int32 Price
		{
			get => m_price;
		    set => m_price = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			Int32 num = 0;
			foreach (ECharacterSpell p_spellType in m_spells)
			{
				CharacterSpell characterSpell = SpellFactory.CreateCharacterSpell(p_spellType);
				characterSpell.CastSpellByInteractiveObject(1f);
				num += m_price;
			}
			LegacyLogic.Instance.WorldManager.Party.ChangeGold(-num);
			p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
		}
	}
}
