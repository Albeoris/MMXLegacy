using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using Legacy.Core.StaticData.Items;
using Legacy.Core.UpdateLogic;

namespace Legacy.Core.Entities.Items
{
	public class Scroll : Consumable, IDescribable
	{
		public const String ITEM_SCROLL_ICON = "ITM_consumable_scroll";

		private ScrollStaticData m_staticData;

		private Dictionary<String, String> m_properties;

		private String m_typeDescription = String.Empty;

		protected override BaseItemStaticData BaseData => m_staticData;

	    public Int32 SpellID => m_staticData.SpellID;

	    public Int32 ScrollTier => (Int32)m_staticData.ScrollTier;

	    public override void Init(Int32 p_staticID)
		{
			m_staticData = StaticDataHandler.GetStaticData<ScrollStaticData>(EDataType.SCROLL, p_staticID);
			m_properties = new Dictionary<String, String>();
			InitTypeDescription();
			InitPropertyDescription();
		}

		private void InitTypeDescription()
		{
			switch (ScrollTier)
			{
			case 1:
				m_typeDescription = "SCROLL_TYPE_TIER_1";
				break;
			case 2:
				m_typeDescription = "SCROLL_TYPE_TIER_2";
				break;
			case 3:
				m_typeDescription = "SCROLL_TYPE_TIER_3";
				break;
			case 4:
				m_typeDescription = "SCROLL_TYPE_TIER_4";
				break;
			}
		}

		private void InitPropertyDescription()
		{
			if (SpellID > 0)
			{
				CharacterSpell characterSpell = SpellFactory.CreateCharacterSpell((ECharacterSpell)SpellID);
				if (characterSpell != null)
				{
					GameConfig game = ConfigManager.Instance.Game;
					Single p_magicFactor;
					if (ScrollTier == 1)
					{
						p_magicFactor = game.ScrollNoviceMagicFactor;
					}
					else if (ScrollTier == 2)
					{
						p_magicFactor = game.ScrollExpertMagicFactor;
					}
					else if (ScrollTier == 3)
					{
						p_magicFactor = game.ScrollMasterMagicFactor;
					}
					else
					{
						p_magicFactor = game.ScrollGrandmasterMagicFactor;
					}
					m_properties["SCROLL_EFFECT"] = characterSpell.NameKey;
					m_properties["SCROLL_SPELL_DESCRIPTION"] = characterSpell.GetDescription(p_magicFactor);
				}
			}
		}

		public override EDataType GetItemType()
		{
			return EDataType.SCROLL;
		}

		public override void Consume(InventorySlotRef p_slot, Int32 p_targetCharacter)
		{
			ConsumeCommand p_command = new ConsumeCommand(p_slot, p_targetCharacter);
			LegacyLogic.Instance.CommandManager.AddCommand(p_command);
		}

		public String GetTypeDescription()
		{
			return m_typeDescription;
		}

		public Dictionary<String, String> GetPropertiesDescription()
		{
			return m_properties;
		}
	}
}
