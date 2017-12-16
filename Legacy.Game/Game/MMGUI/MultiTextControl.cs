using System;
using System.Threading;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/MultiTextControl")]
	public class MultiTextControl : MonoBehaviour
	{
		private String[] m_pages = Arrays<String>.Empty;

		private Int32 m_currentPage;

		private Boolean m_hideButtons = true;

		[SerializeField]
		private UILabel m_main_text;

		[SerializeField]
		private UIButton m_nextButton;

		[SerializeField]
		private UIButton m_prevButton;

		public event EventHandler OnNextPageEvent;

		public event EventHandler OnPrevPageEvent;

		public event EventHandler OnEnd;

		public Int32 CurrentPage => m_currentPage;

	    public Int32 PageCount => m_pages.Length;

	    public Boolean HideButtons
		{
			get => m_hideButtons;
	        set => m_hideButtons = value;
	    }

		public void SetInternalText(String p_value)
		{
			m_currentPage = 0;
			m_pages = p_value.Split(new Char[]
			{
				'@'
			}, StringSplitOptions.RemoveEmptyEntries);
			m_main_text.text = ((m_pages.Length <= 0) ? String.Empty : m_pages[0]);
		}

		public Boolean AtEnd()
		{
			return m_currentPage + 1 == m_pages.Length;
		}

		public void NextPage()
		{
			if (!AtEnd())
			{
				m_currentPage++;
			}
			Show();
			if (OnNextPageEvent != null)
			{
				OnNextPageEvent(this, EventArgs.Empty);
			}
			if (AtEnd() && OnEnd != null)
			{
				OnEnd(this, EventArgs.Empty);
			}
		}

		public void PrevPage()
		{
			if (m_currentPage > 0)
			{
				m_currentPage--;
			}
			if (OnPrevPageEvent != null)
			{
				OnPrevPageEvent(this, EventArgs.Empty);
			}
			Show();
		}

		public void LastPage()
		{
			m_currentPage = Math.Max(0, m_pages.Length - 1);
			if (AtEnd() && OnEnd != null)
			{
				OnEnd(this, EventArgs.Empty);
			}
			Show();
		}

		public void Show()
		{
			m_main_text.text = ((m_currentPage < 0 || m_currentPage >= m_pages.Length) ? String.Empty : m_pages[m_currentPage]);
			if (m_nextButton != null)
			{
				if (m_hideButtons)
				{
					NGUITools.SetActive(m_nextButton.gameObject, !AtEnd());
				}
				else
				{
					NGUITools.SetActive(m_nextButton.gameObject, true);
					m_nextButton.isEnabled = !AtEnd();
				}
			}
			if (m_prevButton != null)
			{
				if (m_hideButtons)
				{
					NGUITools.SetActive(m_prevButton.gameObject, m_currentPage > 0);
				}
				else
				{
					NGUITools.SetActive(m_prevButton.gameObject, true);
					m_prevButton.isEnabled = (m_currentPage > 0);
				}
			}
		}

		public void Hide()
		{
			m_main_text.text = String.Empty;
			if (m_nextButton != null)
			{
				NGUITools.SetActive(m_nextButton.gameObject, false);
			}
			if (m_prevButton != null)
			{
				NGUITools.SetActive(m_prevButton.gameObject, false);
			}
		}

		private void OnClickedNext(GameObject p_sender)
		{
			NextPage();
		}
	}
}
