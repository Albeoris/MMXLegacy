using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Game.MMGUI
{
	public class PortraitDragObject : BaseDragObject
	{
		private Character m_character;

		private Int32 m_index;

		public PortraitDragObject(Character p_character, Int32 p_index)
		{
			m_character = p_character;
			m_index = p_index;
		}

		public Character Character => m_character;

	    public Int32 Index => m_index;

	    public override void SetActive(Boolean p_active)
		{
			NGUITools.SetActiveSelf(m_sprite.gameObject, false);
			NGUITools.SetActiveSelf(m_actionSprite.gameObject, false);
			NGUITools.SetActiveSelf(m_scrollSprite.gameObject, false);
			NGUITools.SetActiveSelf(m_brokenSprite.gameObject, false);
			NGUITools.SetActiveSelf(m_itemBackground.gameObject, false);
			if (!m_characterHud.IsInitialized)
			{
				m_characterHud.IsForDragging = true;
				m_characterHud.Init(m_character, -1);
			}
			else
			{
				m_characterHud.ChangeCharacter(m_character);
			}
			m_characterHud.SetSelected(LegacyLogic.Instance.WorldManager.Party.SelectedCharacter == m_character);
			NGUITools.SetActiveSelf(m_characterHud.gameObject, p_active);
		}
	}
}
