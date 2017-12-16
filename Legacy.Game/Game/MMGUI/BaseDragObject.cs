using System;
using Legacy.Game.HUD;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	public abstract class BaseDragObject
	{
		protected UISprite m_sprite;

		protected UISprite m_actionSprite;

		protected UISprite m_scrollSprite;

		protected UISprite m_brokenSprite;

		protected UISprite m_itemBackground;

		protected UILabel m_itemCounter;

		protected CharacterHud m_characterHud;

		public UISprite Sprite
		{
			get => m_sprite;
		    set => m_sprite = value;
		}

		public UISprite ActionSprite
		{
			get => m_actionSprite;
		    set => m_actionSprite = value;
		}

		public UISprite ScrollSprite
		{
			get => m_scrollSprite;
		    set => m_scrollSprite = value;
		}

		public UILabel ItemCounter
		{
			get => m_itemCounter;
		    set => m_itemCounter = value;
		}

		public UISprite BrokenSprite
		{
			get => m_brokenSprite;
		    set => m_brokenSprite = value;
		}

		public UISprite ItemBackground
		{
			get => m_itemBackground;
		    set => m_itemBackground = value;
		}

		public CharacterHud CharacterHud
		{
			get => m_characterHud;
		    set => m_characterHud = value;
		}

		public void Update()
		{
			if (m_sprite != null)
			{
				m_sprite.Update();
			}
		}

		public virtual void SetActive(Boolean p_active)
		{
			NGUITools.SetActiveSelf(m_sprite.gameObject, p_active);
			NGUITools.SetActiveSelf(m_actionSprite.gameObject, false);
			NGUITools.SetActiveSelf(m_scrollSprite.gameObject, false);
			NGUITools.SetActiveSelf(m_brokenSprite.gameObject, false);
			NGUITools.SetActiveSelf(m_itemBackground.gameObject, false);
			NGUITools.SetActiveSelf(m_characterHud.gameObject, false);
		}

		public virtual void CancelDragAction()
		{
		}
	}
}
