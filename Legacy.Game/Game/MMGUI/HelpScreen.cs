using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Hints;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/HelpScreen")]
	public class HelpScreen : MonoBehaviour
	{
		[SerializeField]
		private PageableListController m_pageableList;

		[SerializeField]
		private TabController m_categoryTabs;

		[SerializeField]
		private UILabel m_hintTitle;

		[SerializeField]
		private UILabel m_hintText;

		[SerializeField]
		private UISprite m_hintBG;

		[SerializeField]
		private UISprite m_hintBorder;

		private Boolean m_isOpen;

		public Boolean IsOpen
		{
			get => m_isOpen;
		    set => m_isOpen = value;
		}

		public void Init()
		{
		}

		public void Open()
		{
			m_pageableList.OnNextPageEvent += UpdateHints;
			m_pageableList.OnPrevPageEvent += UpdateHints;
			m_categoryTabs.TabIndexChanged += OnCategorySelected;
			m_isOpen = true;
			NGUITools.SetActive(gameObject, true);
			m_categoryTabs.SelectTab(0, true);
			AudioController.Play("ButtonClickBookOpen");
		}

		public void CloseScreen()
		{
			CleanupPageableList();
			Cleanup();
			m_isOpen = false;
			NGUITools.SetActive(gameObject, false);
			AudioController.Play("ButtonClickBookClose");
		}

		private void CleanupPageableList()
		{
			foreach (BookEntry bookEntry in m_pageableList.EntryList)
			{
				bookEntry.OnBookSelected -= OnHintSelected;
			}
		}

		public void Cleanup()
		{
			m_pageableList.OnNextPageEvent -= UpdateHints;
			m_pageableList.OnPrevPageEvent -= UpdateHints;
			m_categoryTabs.TabIndexChanged -= OnCategorySelected;
		}

		private void OnCategorySelected(Object sender, EventArgs e)
		{
			m_pageableList.CurrentIndex = 0;
			Int32 bookCount = UpdateHintList();
			m_pageableList.Finish(bookCount);
		}

		private void UpdateHints(Object sender, EventArgs e)
		{
			UpdateHintList();
			m_pageableList.Show();
		}

		private Int32 UpdateHintList()
		{
			Boolean flag = true;
			CleanupPageableList();
			List<Hint> hintsForCategory = LegacyLogic.Instance.WorldManager.HintManager.GetHintsForCategory((EHintCategory)m_categoryTabs.CurrentTabIndex);
			for (Int32 i = m_pageableList.CurrentIndex; i < m_pageableList.EndIndex; i++)
			{
				BookEntry entry = m_pageableList.GetEntry();
				if (i < hintsForCategory.Count)
				{
					entry.Init(hintsForCategory[i].StaticID, hintsForCategory[i].Title);
					entry.OnBookSelected += OnHintSelected;
					if (flag)
					{
						OnHintSelected(entry, EventArgs.Empty);
						flag = false;
					}
				}
				else
				{
					entry.Init(0, String.Empty);
				}
			}
			return hintsForCategory.Count;
		}

		private void OnHintSelected(Object p_sender, EventArgs e)
		{
			m_pageableList.UnselectAll();
			Int32 entryID = (p_sender as BookEntry).EntryID;
			(p_sender as BookEntry).SetSelected(true);
			Hint hintByID = LegacyLogic.Instance.WorldManager.HintManager.GetHintByID(entryID);
			m_hintTitle.text = hintByID.Title;
			m_hintText.text = hintByID.Text;
			Single num = m_hintText.relativeSize.y * m_hintText.transform.localScale.y;
			Single y = Math.Abs(m_hintText.transform.localPosition.y) + num + 45f;
			m_hintBG.transform.localScale = new Vector3(m_hintBG.transform.localScale.x, y, m_hintBG.transform.localScale.z);
			m_hintBorder.transform.localScale = new Vector3(m_hintBorder.transform.localScale.x, y, m_hintBorder.transform.localScale.z);
		}
	}
}
