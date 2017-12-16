using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	public class GUIButtonTweenWrapper
	{
		private UIButton m_Btn;

		private Color m_EnabledColor;

		private Color m_DisabledColor;

		private UISprite[] m_Sprites;

		public GUIButtonTweenWrapper(UIButton p_Button, Color p_EnabledColor, Color p_DisabledColor)
		{
			m_Btn = p_Button;
			m_EnabledColor = p_EnabledColor;
			m_DisabledColor = p_DisabledColor;
		}

		public UIButton Button => m_Btn;

	    public Color EnabledColor
		{
			get => m_EnabledColor;
	        set => m_EnabledColor = value;
	    }

		public Color DisabledColor
		{
			get => m_DisabledColor;
		    set => m_DisabledColor = value;
		}

		public Boolean isEnabled
		{
			get => m_Btn.isEnabled;
		    set
			{
				Boolean isEnabled = m_Btn.isEnabled;
				m_Btn.isEnabled = value;
				if (isEnabled != value)
				{
					TweenColorOfAllSprites(value);
				}
			}
		}

		public void TweenColorOfAllSprites(Boolean p_IsEnabled)
		{
			TweenColorOfAllSprites((!p_IsEnabled) ? m_DisabledColor : m_EnabledColor);
		}

		public void TweenColorOfAllSprites(Color p_TargetColor)
		{
			CheckSprites();
			if (m_Sprites != null)
			{
				for (Int32 i = 0; i < m_Sprites.Length; i++)
				{
					if (m_Sprites[i].color != p_TargetColor)
					{
						TweenColor.Begin(m_Sprites[i].gameObject, 0.15f, p_TargetColor);
					}
				}
			}
		}

		private void CheckSprites()
		{
			if (m_Sprites == null)
			{
				m_Sprites = m_Btn.GetComponentsInChildren<UISprite>();
			}
		}
	}
}
