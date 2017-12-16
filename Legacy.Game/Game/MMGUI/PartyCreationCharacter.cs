using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyCreationCharacter")]
	public class PartyCreationCharacter : MonoBehaviour
	{
		private const String BACKGROUND_SELECTED_CHARACTER = "BTN_square_152_highlight";

		private const String BACKGROUND_NOT_SELECTED_CHARACTER = "BTN_square_152";

		[SerializeField]
		private UISprite m_portrait;

		[SerializeField]
		private UISprite m_portraitBG;

		[SerializeField]
		private UISprite m_tick;

		[SerializeField]
		private UISprite m_body;

		[SerializeField]
		private Color m_hoverColor;

		[SerializeField]
		private Color m_selectionColor;

		[SerializeField]
		private Color m_selectionHoverColor;

		private Boolean m_selected = true;

		private Int32 m_index;

		private Boolean m_isHovered;

		private Color m_portraitBackgroundNormalColor;

		private DummyCharacter m_character;

		public event EventHandler OnCharacterClicked;

		public Int32 Index => m_index;

	    public void Init(DummyCharacter p_character, Int32 p_index)
		{
			m_portraitBackgroundNormalColor = m_portraitBG.color;
			m_character = p_character;
			m_index = p_index;
			m_portrait.spriteName = p_character.GetPortrait();
			String bodySprite = m_character.GetBodySprite();
			if (bodySprite == String.Empty)
			{
				NGUITools.SetActive(m_body.gameObject, false);
			}
			else
			{
				m_body.spriteName = bodySprite;
				NGUITools.SetActive(m_body.gameObject, true);
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DUMMY_CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DUMMY_CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
		}

		public void Cleanup()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DUMMY_CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DUMMY_CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
		}

		private void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (enabled)
			{
				if (m_selected)
				{
					TweenColor.Begin(m_portraitBG.gameObject, 0.1f, (!p_isOver) ? m_selectionColor : m_selectionHoverColor);
				}
				else
				{
					TweenColor.Begin(m_portraitBG.gameObject, 0.1f, (!p_isOver) ? m_portraitBackgroundNormalColor : m_hoverColor);
				}
			}
		}

		private void OnClick()
		{
			if (OnCharacterClicked != null)
			{
				OnCharacterClicked(this, EventArgs.Empty);
			}
		}

		private void OnCharacterSelected(Object p_sender, EventArgs p_args)
		{
			if (p_sender == m_character)
			{
				SetSelected(true);
			}
			else if (m_selected)
			{
				SetSelected(false);
			}
		}

		public void SetSelected(Boolean p_selected)
		{
			m_selected = p_selected;
			if (m_selected)
			{
				m_portraitBG.spriteName = "BTN_square_152_highlight";
				TweenColor.Begin(m_portraitBG.gameObject, 0.1f, (!m_isHovered) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				m_portraitBG.spriteName = "BTN_square_152";
				TweenColor.Begin(m_portraitBG.gameObject, 0.1f, (!m_isHovered) ? m_portraitBackgroundNormalColor : m_hoverColor);
			}
		}

		private void OnCharacterStatusChanged(Object sender, EventArgs e)
		{
			m_portrait.spriteName = m_character.GetPortrait();
			String bodySprite = m_character.GetBodySprite();
			if (bodySprite == String.Empty)
			{
				NGUITools.SetActive(m_body.gameObject, false);
			}
			else
			{
				m_body.spriteName = bodySprite;
				NGUITools.SetActive(m_body.gameObject, true);
			}
		}

		public void SetTickState(Boolean p_visible)
		{
			NGUITools.SetActive(m_tick.gameObject, p_visible);
		}

		public void ResetPortrait()
		{
			m_portrait.spriteName = m_character.GetPortrait();
			NGUITools.SetActive(m_body.gameObject, false);
		}
	}
}
