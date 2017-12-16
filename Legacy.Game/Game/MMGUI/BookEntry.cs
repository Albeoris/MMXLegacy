using System;
using System.Threading;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/BookEntry")]
	public class BookEntry : QuestEntry
	{
		private Int32 m_id;

		private Boolean m_isActive;

		private Boolean m_isClicked;

		private Boolean m_isNew;

		public event EventHandler OnBookSelected;

		public Int32 EntryID => m_id;

	    public Boolean IsActive
		{
			get => m_isActive;
	        set
			{
				m_isActive = value;
				if (!value)
				{
					m_title.color = Color.gray;
				}
				else
				{
					m_title.color = Color.black;
				}
			}
		}

		public Boolean IsNewEntry
		{
			get => m_isNew;
		    set
			{
				if (!value)
				{
					Vector3 localPosition = new Vector3(25f, m_title.transform.localPosition.y, m_title.transform.localPosition.z);
					m_title.transform.localPosition = localPosition;
				}
				else
				{
					Vector3 localPosition2 = new Vector3(45f, m_title.transform.localPosition.y, m_title.transform.localPosition.z);
					m_title.transform.localPosition = localPosition2;
				}
				m_isNew = value;
				NGUITools.SetActive(m_markerNewEntry.gameObject, value);
			}
		}

		public void Init(Int32 loreBookId, String title)
		{
			m_isNew = true;
			if (loreBookId == 0)
			{
				gameObject.SetActive(false);
				return;
			}
			gameObject.SetActive(true);
			m_isActive = true;
			m_id = loreBookId;
			m_title.text = title;
			if (m_questIcon != null)
			{
				m_questIcon.alpha = 0f;
			}
			Vector3 localPosition = new Vector3(25f, m_title.transform.localPosition.y, m_title.transform.localPosition.z);
			if (!m_isNew)
			{
				m_title.transform.localPosition = localPosition;
			}
			m_marker.alpha = 0f;
			m_background.enabled = false;
			m_selected = false;
			m_isClicked = false;
			m_step = null;
		}

		public void SetSelected(Boolean p_isSelected)
		{
			if (p_isSelected)
			{
				m_title.color = Color.white;
				m_marker.alpha = 1f;
				m_background.enabled = true;
				m_selected = true;
				m_isClicked = true;
			}
			else
			{
				m_title.color = Color.black;
				m_marker.alpha = 0f;
				m_background.enabled = false;
				m_selected = false;
				OnHover(false);
				m_isClicked = false;
			}
		}

		public void SetSelected(Int32 p_id)
		{
			if (!m_isActive)
			{
				return;
			}
			if (p_id == m_id)
			{
				m_title.color = Color.white;
				m_marker.alpha = 1f;
				m_background.enabled = true;
				m_selected = true;
				m_isClicked = true;
			}
			else
			{
				m_title.color = Color.black;
				m_marker.alpha = 0f;
				m_background.enabled = false;
				m_selected = false;
				m_isClicked = false;
			}
		}

		public override void OnHover(Boolean isOver)
		{
			if (m_selected || !m_isActive)
			{
				return;
			}
			if (isOver)
			{
				m_background.enabled = true;
				m_title.color = Color.white;
				m_marker.alpha = 1f;
			}
			else
			{
				m_background.enabled = false;
				m_title.color = Color.black;
				m_marker.alpha = 0f;
				if (m_step != null)
				{
					base.OnHover(isOver);
				}
			}
		}

		private void OnClick()
		{
			if (m_questIcon.alpha > 0f)
			{
				OnQuestClick(null);
				return;
			}
			if (OnBookSelected != null && m_isActive && !m_isClicked)
			{
				OnBookSelected(this, EventArgs.Empty);
				m_isClicked = true;
			}
		}
	}
}
