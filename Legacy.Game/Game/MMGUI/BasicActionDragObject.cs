using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Game.MMGUI
{
	public class BasicActionDragObject : BaseDragObject
	{
		private SpellView m_view;

		public BasicActionDragObject(SpellView p_view)
		{
			m_view = p_view;
		}

		public SpellView View => m_view;

	    public override void SetActive(Boolean p_active)
		{
			Boolean flag = m_view.ActionType == EQuickActionType.USE_BEST_HEALTHPOTION || m_view.ActionType == EQuickActionType.USE_BEST_MANAPOTION;
			NGUITools.SetActiveSelf(m_sprite.gameObject, p_active && flag);
			NGUITools.SetActiveSelf(m_actionSprite.gameObject, p_active);
			if (p_active)
			{
				m_actionSprite.spriteName = m_view.Icon.spriteName;
				if (flag)
				{
					m_sprite.spriteName = m_view.IconItem.spriteName;
				}
			}
		}
	}
}
