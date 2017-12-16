using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Game.MMGUI
{
	public class QuickActionDragObject : BaseDragObject
	{
		private const String BEST_HEALTH_POTION_DEFAULT = "ITM_consumable_potion_health_1";

		private const String BEST_MANA_POTION_DEFAULT = "ITM_consumable_potion_mana_1";

		private EQuickActionType m_type;

		private Consumable m_item;

		private CharacterSpell m_spell;

		private String m_spritename = String.Empty;

		private ActionButtonView m_view;

		public QuickActionDragObject(ActionButtonView p_view)
		{
			m_view = p_view;
			m_type = p_view.Type;
			m_item = p_view.Item;
			m_spell = p_view.Spell;
			m_spritename = p_view.Icon;
		}

		public ActionButtonView View => m_view;

	    public EQuickActionType Type => m_type;

	    public Consumable Item => m_item;

	    public CharacterSpell Spell => m_spell;

	    public String Spritename => m_spritename;

	    public override void SetActive(Boolean p_active)
		{
			Boolean flag = m_type == EQuickActionType.USE_BEST_HEALTHPOTION || m_type == EQuickActionType.USE_BEST_MANAPOTION || m_type == EQuickActionType.USE_ITEM;
			NGUITools.SetActiveSelf(m_sprite.gameObject, p_active && flag);
			NGUITools.SetActiveSelf(m_actionSprite.gameObject, p_active && m_type != EQuickActionType.USE_ITEM);
			if (p_active)
			{
				m_actionSprite.spriteName = m_spritename;
				if (flag)
				{
					if (m_item != null)
					{
						if (m_item is Scroll)
						{
							NGUITools.SetActiveSelf(m_scrollSprite.gameObject, true);
							m_sprite.spriteName = "ITM_consumable_scroll";
							m_scrollSprite.spriteName = m_item.Icon;
						}
						else
						{
							NGUITools.SetActiveSelf(m_scrollSprite.gameObject, false);
							m_sprite.spriteName = m_item.Icon;
						}
						NGUITools.SetActiveSelf(m_itemCounter.gameObject, true);
						m_itemCounter.text = m_item.Counter.ToString();
					}
					else if (m_type == EQuickActionType.USE_BEST_HEALTHPOTION)
					{
						m_sprite.spriteName = "ITM_consumable_potion_health_1";
					}
					else if (m_type == EQuickActionType.USE_BEST_MANAPOTION)
					{
						m_sprite.spriteName = "ITM_consumable_potion_mana_1";
					}
				}
			}
			else
			{
				NGUITools.SetActiveSelf(m_itemCounter.gameObject, false);
			}
		}
	}
}
