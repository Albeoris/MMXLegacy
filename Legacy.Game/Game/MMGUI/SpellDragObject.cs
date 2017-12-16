using System;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Game.MMGUI
{
	public class SpellDragObject : BaseDragObject
	{
		private CharacterSpell m_spell;

		private SpellView m_view;

		public SpellDragObject(SpellView p_view, CharacterSpell p_spell)
		{
			m_spell = p_spell;
			m_view = p_view;
		}

		public CharacterSpell Spell => m_spell;

	    public SpellView View => m_view;

	    public override void SetActive(Boolean p_active)
		{
			NGUITools.SetActiveSelf(m_sprite.gameObject, false);
			NGUITools.SetActiveSelf(m_actionSprite.gameObject, p_active);
			if (p_active)
			{
				m_actionSprite.spriteName = m_spell.StaticData.Icon;
			}
		}
	}
}
