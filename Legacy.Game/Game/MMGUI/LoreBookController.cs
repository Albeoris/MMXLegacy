using System;
using System.Collections.Generic;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Lorebook;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/LoreBookController")]
	public class LoreBookController : MonoBehaviour
	{
		private const String RES_PATH = "LorebookCoverArt/";

		[SerializeField]
		private UILabel m_titleLabel;

		[SerializeField]
		private UILabel m_authorLabel;

		[SerializeField]
		private UILabel m_pageLabel;

		[SerializeField]
		private MultiTextControl m_bookTextLabel;

		[SerializeField]
		private GameObject m_firstPageContent;

		[SerializeField]
		private UITexture m_bookCoverImage;

		[SerializeField]
		private UITexture m_categoryCoverImage;

		[SerializeField]
		private TabController m_categoryTabs;

		[SerializeField]
		private UILabel m_headerLabel;

		[SerializeField]
		private PageableListController m_pageableList;

		[SerializeField]
		private UICheckbox m_checkBox;

		private List<LoreBookStaticData> m_foundBooks;

		private LoreBookHandler m_loreBookHandler;

		public void Init()
		{
			m_bookTextLabel.HideButtons = true;
			m_loreBookHandler = LegacyLogic.Instance.WorldManager.LoreBookHandler;
			m_foundBooks = m_loreBookHandler.FoundBooks;
		}

		public void Open()
		{
			m_bookTextLabel.OnNextPageEvent += OnNextPage;
			m_bookTextLabel.OnPrevPageEvent += OnNextPage;
			m_pageableList.OnNextPageEvent += UpdateBooks;
			m_pageableList.OnPrevPageEvent += UpdateBooks;
			m_categoryTabs.TabIndexChanged += OnCategorySelected;
			UICheckbox checkBox = m_checkBox;
			checkBox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(checkBox.onStateChange, new UICheckbox.OnStateChange(OnCheckBoxChanged));
			for (Int32 i = 0; i < m_categoryTabs.Tabs.Length; i++)
			{
				Int32 count = m_loreBookHandler.GetBooksForCategory((ELoreBookCategories)i, false).Count;
				Int32 numberOfBookForCategory = m_loreBookHandler.GetNumberOfBookForCategory((ELoreBookCategories)i);
				m_categoryTabs.Tabs[i].AddTextToTooltip(" (" + LocaManager.GetText("GUI_STATS_X_OF_Y", count, numberOfBookForCategory) + ")");
			}
			m_categoryTabs.SelectTab(0, true);
		}

		public void Close()
		{
			m_headerLabel.enabled = true;
			CleanupPageableList();
			Cleanup();
		}

		public void OnBookSelected(Int32 p_selected)
		{
			LoreBookStaticData loreBookStaticData = null;
			m_pageLabel.enabled = true;
			m_firstPageContent.SetActive(true);
			m_titleLabel.enabled = true;
			if (m_categoryCoverImage.mainTexture != null)
			{
				Texture mainTexture = m_categoryCoverImage.mainTexture;
				m_categoryCoverImage.mainTexture = null;
				if (mainTexture != null)
				{
					mainTexture.UnloadAsset();
				}
			}
			m_categoryCoverImage.enabled = false;
			m_headerLabel.enabled = false;
			NGUITools.SetActive(m_checkBox.gameObject, false);
			NGUITools.SetActive(m_pageableList.gameObject, false);
			m_pageableList.CurrentIndex = 0;
			foreach (LoreBookStaticData loreBookStaticData2 in m_foundBooks)
			{
				if (loreBookStaticData2.StaticID == p_selected)
				{
					loreBookStaticData = loreBookStaticData2;
					break;
				}
			}
			if (loreBookStaticData == null)
			{
				return;
			}
			Int32 currentTabIndex = m_categoryTabs.CurrentTabIndex;
			m_categoryTabs.Tabs[currentTabIndex].SetActive(false, true);
			m_categoryTabs.CurrentTabIndex = m_categoryTabs.Tabs.Length;
			m_titleLabel.text = LocaManager.GetText(loreBookStaticData.TitleKey);
			m_authorLabel.text = LocaManager.GetText(loreBookStaticData.AuthorKey);
			m_bookTextLabel.SetInternalText(LocaManager.GetText(loreBookStaticData.TextKey));
			String text = "LorebookCoverArt/" + loreBookStaticData.ImageName;
			Texture texture = (Texture)Resources.Load(text);
			if (texture == null)
			{
				Debug.LogError("Could not load texture from: " + text);
			}
			if (m_bookCoverImage.mainTexture != texture)
			{
				Texture mainTexture2 = m_bookCoverImage.mainTexture;
				m_bookCoverImage.mainTexture = texture;
				if (mainTexture2 != null)
				{
					mainTexture2.UnloadAsset();
				}
			}
			if (m_bookTextLabel.PageCount <= 1)
			{
				m_pageLabel.enabled = false;
			}
			else
			{
				m_pageLabel.text = LocaManager.GetText("SPELLBOOK_PAGE", m_bookTextLabel.CurrentPage + 1, m_bookTextLabel.PageCount);
			}
			m_bookTextLabel.Show();
			m_loreBookHandler.RemoveFromNewEntries(p_selected);
			CleanupPageableList();
		}

		private void OnBookSelected(Object p_sender, EventArgs p_args)
		{
			Int32 entryID = (p_sender as BookEntry).EntryID;
			(p_sender as BookEntry).SetSelected(false);
			(p_sender as BookEntry).IsNewEntry = false;
			OnBookSelected(entryID);
		}

		private void OnCategorySelected(Object p_sender, EventArgs p_args)
		{
			m_pageableList.CurrentIndex = 0;
			Int32 bookCount = UpdateBookList();
			m_pageableList.Finish(bookCount);
		}

		private void UpdateBooks(Object sender, EventArgs e)
		{
			UpdateBookList();
			m_pageableList.Show();
		}

		private Int32 UpdateBookList()
		{
			CleanupPageableList();
			m_bookTextLabel.Hide();
			m_firstPageContent.SetActive(false);
			m_bookTextLabel.Hide();
			m_titleLabel.enabled = false;
			if (m_bookCoverImage.mainTexture != null)
			{
				Texture mainTexture = m_bookCoverImage.mainTexture;
				m_bookCoverImage.mainTexture = null;
				if (mainTexture != null)
				{
					mainTexture.UnloadAsset();
				}
			}
			m_categoryCoverImage.enabled = true;
			m_headerLabel.enabled = true;
			NGUITools.SetActive(m_checkBox.gameObject, true);
			NGUITools.SetActive(m_pageableList.gameObject, true);
			Int32 numberOfBookForCategory = m_loreBookHandler.GetNumberOfBookForCategory((ELoreBookCategories)m_categoryTabs.CurrentTabIndex);
			Int32 num = 0;
			String text = "LorebookCoverArt/ART_lore_category_" + ((ELoreBookCategories)m_categoryTabs.CurrentTabIndex).ToString().ToLower();
			Texture texture = (Texture)Resources.Load(text);
			if (texture == null)
			{
				Debug.LogError("Could not load texture from: " + text);
			}
			if (m_categoryCoverImage.mainTexture != texture)
			{
				Texture mainTexture2 = m_categoryCoverImage.mainTexture;
				m_categoryCoverImage.mainTexture = texture;
				if (mainTexture2 != null)
				{
					mainTexture2.UnloadAsset();
				}
			}
			m_pageLabel.enabled = false;
			List<LoreBookStaticData> booksForCategory = m_loreBookHandler.GetBooksForCategory((ELoreBookCategories)m_categoryTabs.CurrentTabIndex, m_checkBox.isChecked);
			for (Int32 i = m_pageableList.CurrentIndex; i < m_pageableList.EndIndex; i++)
			{
				BookEntry entry = m_pageableList.GetEntry();
				entry.IsActive = true;
				if (i < booksForCategory.Count)
				{
					if (!m_checkBox.isChecked)
					{
						entry.Init(booksForCategory[i].StaticID, LocaManager.GetText(booksForCategory[i].TitleKey));
						entry.IsNewEntry = m_loreBookHandler.NewEntries.Contains(booksForCategory[i].StaticID);
						entry.OnBookSelected += OnBookSelected;
						num++;
					}
					else if (m_loreBookHandler.FoundBooks.Contains(booksForCategory[i]))
					{
						entry.Init(booksForCategory[i].StaticID, LocaManager.GetText(booksForCategory[i].TitleKey));
						entry.OnBookSelected += OnBookSelected;
						entry.IsNewEntry = m_loreBookHandler.NewEntries.Contains(booksForCategory[i].StaticID);
						num++;
					}
					else
					{
						entry.Init(booksForCategory[i].StaticID, "?????");
						entry.IsNewEntry = false;
						entry.IsActive = false;
					}
				}
				else
				{
					entry.Init(0, String.Empty);
				}
			}
			m_headerLabel.text = LocaManager.GetText("TT_LORE_CATEGORY_" + ((ELoreBookCategories)m_categoryTabs.CurrentTabIndex).ToString());
			if (!m_checkBox.isChecked)
			{
				return booksForCategory.Count;
			}
			return numberOfBookForCategory;
		}

		private void CleanupPageableList()
		{
			foreach (BookEntry bookEntry in m_pageableList.EntryList)
			{
				bookEntry.OnBookSelected -= OnBookSelected;
				bookEntry.SetSelected(false);
			}
		}

		private void OnCheckBoxChanged(Boolean p_state)
		{
			OnCategorySelected(null, EventArgs.Empty);
		}

		private void OnNextPage(Object sender, EventArgs e)
		{
			m_pageLabel.text = LocaManager.GetText("SPELLBOOK_PAGE", m_bookTextLabel.CurrentPage + 1, m_bookTextLabel.PageCount);
		}

		public void Cleanup()
		{
			m_categoryTabs.TabIndexChanged -= OnCategorySelected;
			m_pageableList.OnNextPageEvent -= UpdateBooks;
			m_pageableList.OnPrevPageEvent -= UpdateBooks;
			m_bookTextLabel.OnNextPageEvent -= OnNextPage;
			m_bookTextLabel.OnPrevPageEvent -= OnNextPage;
			UICheckbox checkBox = m_checkBox;
			checkBox.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(checkBox.onStateChange, new UICheckbox.OnStateChange(OnCheckBoxChanged));
		}
	}
}
