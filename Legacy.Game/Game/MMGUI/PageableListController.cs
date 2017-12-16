using System;
using System.Threading;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/PageableListController")]
	public class PageableListController : MonoBehaviour
	{
		private const Int32 VISIBLE_ROWS = 21;

		[SerializeField]
		private UILabel m_pagerLabel;

		[SerializeField]
		private UIButton m_nextButton;

		[SerializeField]
		private UIButton m_prevButton;

		[SerializeField]
		private BookEntry[] m_entryList;

		private Int32 m_currentPage;

		private Int32 m_pageCount;

		private Int32 m_currentIndex;

		private Boolean m_hideButtons = true;

		private Int32 m_listIndex;

		public event EventHandler OnNextPageEvent;

		public event EventHandler OnPrevPageEvent;

		public event EventHandler OnEnd;

		public Int32 CurrentPage => m_currentPage;

	    public Int32 PageCount => m_pageCount;

	    public Int32 CurrentIndex
		{
			get => m_currentIndex;
	        set => m_currentIndex = value;
	    }

		public Int32 EndIndex => 21 + m_currentIndex;

	    public BookEntry[] EntryList => m_entryList;

	    public Boolean HideButtons
		{
			get => m_hideButtons;
	        set => m_hideButtons = value;
	    }

		public void Init()
		{
			m_pageCount = 0;
			m_currentPage = 0;
			m_listIndex = 0;
		}

		public Boolean AtEnd()
		{
			return m_currentPage + 1 == m_pageCount;
		}

		public void PrevPage()
		{
			if (m_currentPage > 0)
			{
				m_currentPage--;
				m_currentIndex = m_currentPage * 21;
				m_listIndex = 0;
				if (OnPrevPageEvent != null)
				{
					OnPrevPageEvent(this, EventArgs.Empty);
				}
				Show();
				return;
			}
		}

		public void NextPage()
		{
			if (!AtEnd())
			{
				m_currentPage++;
				m_currentIndex = m_currentPage * 21;
				m_listIndex = 0;
				if (OnNextPageEvent != null)
				{
					OnNextPageEvent(this, EventArgs.Empty);
				}
				Show();
				if (AtEnd() && OnEnd != null)
				{
					OnEnd(this, EventArgs.Empty);
				}
				return;
			}
		}

		private void UpdatePageLabel()
		{
			m_pagerLabel.text = LocaManager.GetText("SPELLBOOK_PAGE", m_currentPage + 1, m_pageCount);
		}

		public BookEntry GetEntry()
		{
			BookEntry result = m_entryList[m_listIndex];
			m_listIndex++;
			if (m_listIndex >= 21)
			{
				m_listIndex = 0;
			}
			return result;
		}

		public BookEntry TrySelectActiveEntryOnPage()
		{
			for (Int32 i = 0; i < m_entryList.Length; i++)
			{
				BookEntry bookEntry = m_entryList[i];
				if (bookEntry.IsActive && bookEntry.EntryID > 0)
				{
					return bookEntry;
				}
			}
			return null;
		}

		public void Show()
		{
			if (m_nextButton != null)
			{
				if (!m_hideButtons)
				{
					NGUITools.SetActive(m_nextButton.gameObject, true);
					m_nextButton.isEnabled = !AtEnd();
				}
				else
				{
					NGUITools.SetActive(m_nextButton.gameObject, !AtEnd());
				}
			}
			if (m_prevButton != null)
			{
				if (!m_hideButtons)
				{
					NGUITools.SetActive(m_prevButton.gameObject, true);
					m_prevButton.isEnabled = (m_currentPage > 0);
				}
				else
				{
					NGUITools.SetActive(m_prevButton.gameObject, m_currentPage > 0);
				}
			}
			UpdatePageLabel();
		}

		public void Finish(Int32 bookCount)
		{
			m_pageCount = bookCount / 21;
			Int32 num = bookCount % 21;
			if (num > 0)
			{
				m_pageCount++;
			}
			if (m_pageCount <= 1)
			{
				if (m_pageCount == 0)
				{
					m_pageCount++;
				}
				m_pagerLabel.enabled = false;
			}
			else
			{
				m_pagerLabel.enabled = true;
			}
			m_currentPage = 0;
			Show();
		}

		public void Cleanup()
		{
		}

		public void UnselectAll()
		{
			for (Int32 i = 0; i < m_entryList.Length; i++)
			{
				BookEntry bookEntry = m_entryList[i];
				bookEntry.SetSelected(false);
			}
		}
	}
}
