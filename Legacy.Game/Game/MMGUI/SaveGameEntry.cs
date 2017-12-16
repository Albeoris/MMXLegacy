using System;
using Legacy.Core.SaveGameManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SaveGameEntry")]
	public class SaveGameEntry : MonoBehaviour
	{
		private const Single MAX_SIZE_TITLE_LABEL = 18f;

		[SerializeField]
		private UILabel m_title;

		[SerializeField]
		private UILabel m_subHeadLine;

		[SerializeField]
		private Color m_selectionHoverColor = new Color(1f, 1f, 1f, 0.625f);

		[SerializeField]
		private Color m_selectionColor = new Color(1f, 1f, 1f, 0.5f);

		[SerializeField]
		private Color m_hoverColor = new Color(1f, 1f, 1f, 0.25f);

		[SerializeField]
		private Color m_notSelectedColor = new Color(1f, 1f, 1f, 0f);

		[SerializeField]
		private GameObject m_background;

		private SaveGameMenuController m_controller;

		private SaveGameMeta m_meta;

		private String m_name = String.Empty;

		private String m_shortName = String.Empty;

		private Boolean m_isHovered;

		private Boolean m_selected;

		private Boolean m_showDate;

		public String Name => m_name;

	    public SaveGameMeta Meta => m_meta;

	    public void Init(SaveGameMenuController p_controller, String p_name, SaveGameMeta p_meta, Boolean p_showDate)
		{
			m_showDate = p_showDate;
			m_controller = p_controller;
			m_name = p_name;
			m_meta = p_meta;
			name = p_name;
			InitData();
		}

		public void SetVisible(Boolean p_visible)
		{
			NGUITools.SetActiveSelf(gameObject, p_visible);
		}

		private void OnDisable()
		{
			OnHover(false);
		}

		private void InitData()
		{
			if (m_meta.SaveNumber == 0)
			{
				m_title.text = m_name;
			}
			else if (m_meta.Type == ESaveGameType.AUTO)
			{
				m_title.text = m_name;
			}
			else
			{
				String text = String.Concat(new Object[]
				{
					GetSaveGameType(),
					" ",
					m_meta.SaveNumber,
					" - ",
					m_name
				});
				m_title.text = text;
				String text2 = ShortenTitle(text);
				if (text2.Length < text.Length)
				{
					UILabel title = m_title;
					title.text += "...";
				}
			}
			if (m_showDate)
			{
				String text3 = LocaManager.GetText("SAVEGAMEMENU_DATE_FORMAT", m_meta.Time.Date.Day, m_meta.Time.Date.Month, m_meta.Time.Date.Year);
				String text4 = LocaManager.GetText("SAVEGAMEMENU_TIME_FORMAT", m_meta.Time.TimeOfDay.Hours, m_meta.Time.TimeOfDay.Minutes.ToString("00"));
				m_subHeadLine.text = text3 + " - " + text4;
			}
			else
			{
				m_title.text = m_name;
				m_subHeadLine.text = "-";
			}
		}

		private String ShortenTitle(String p_title)
		{
			if (p_title.Length == 0)
			{
				return String.Empty;
			}
			m_title.text = p_title;
			if (m_title.relativeSize.x > 18f)
			{
				String p_title2 = p_title.Substring(0, p_title.Length - 2);
				return ShortenTitle(p_title2);
			}
			return p_title;
		}

		private String GetSaveGameType()
		{
			switch (m_meta.Type)
			{
			case ESaveGameType.NORMAL:
				return LocaManager.GetText("SAVEGAMETYPE_NORMAL");
			case ESaveGameType.QUICK:
				return LocaManager.GetText("SAVEGAMETYPE_QUICK");
			case ESaveGameType.AUTO:
				return LocaManager.GetText("SAVEGAMETYPE_AUTO");
			default:
				return String.Empty;
			}
		}

		public void OnClickedSaveGame(GameObject p_sender)
		{
			m_controller.ClickedEntry(this);
		}

		public void CleanUp()
		{
			m_controller = null;
		}

		public void SetSelected(Boolean p_selected)
		{
			m_selected = p_selected;
			if (m_selected)
			{
				TweenColor.Begin(m_background.gameObject, 0.1f, (!m_isHovered) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				TweenColor.Begin(m_background.gameObject, 0.1f, (!m_isHovered) ? m_notSelectedColor : m_hoverColor);
			}
		}

		private void OnDoubleClick()
		{
			m_controller.OnDoubleClick(this);
		}

		private void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (enabled)
			{
				if (m_selected)
				{
					TweenColor.Begin(m_background.gameObject, 0.1f, (!p_isOver) ? m_selectionColor : m_selectionHoverColor);
				}
				else
				{
					TweenColor.Begin(m_background.gameObject, 0.1f, (!p_isOver) ? m_notSelectedColor : m_hoverColor);
				}
			}
		}
	}
}
