using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Lorebook
{
	public class LoreBookHandler : ISaveGameObject
	{
		private List<LoreBookStaticData> m_foundBooks;

		private Dictionary<ELoreBookCategories, Int32> m_bookCounter;

		private List<Int32> m_newEntries;

		public LoreBookHandler()
		{
			m_foundBooks = new List<LoreBookStaticData>();
			m_bookCounter = new Dictionary<ELoreBookCategories, Int32>();
			m_newEntries = new List<Int32>();
		}

		public List<LoreBookStaticData> FoundBooks => m_foundBooks;

	    public List<Int32> NewEntries => m_newEntries;

	    private void InitBookCounterDict()
		{
			List<LoreBookStaticData> list = new List<LoreBookStaticData>(StaticDataHandler.GetIterator<LoreBookStaticData>(EDataType.LOREBOOK));
			foreach (LoreBookStaticData loreBookStaticData in list)
			{
				Int32 num = 0;
				if (m_bookCounter.TryGetValue(loreBookStaticData.Category, out num))
				{
					m_bookCounter[loreBookStaticData.Category] = num + 1;
				}
				else
				{
					m_bookCounter.Add(loreBookStaticData.Category, 1);
				}
			}
			m_bookCounter[ELoreBookCategories.SHOW_ALL] = list.Count;
		}

		public Int32 GetNumberOfBookForCategory(ELoreBookCategories p_category)
		{
			if (m_bookCounter.Keys.Count == 0)
			{
				InitBookCounterDict();
			}
			Int32 result = 0;
			m_bookCounter.TryGetValue(p_category, out result);
			return result;
		}

		public void RemoveFromNewEntries(Int32 p_key)
		{
			if (m_newEntries.Contains(p_key))
			{
				m_newEntries.Remove(p_key);
			}
		}

		public void AddLoreBook(Int32 p_staticID)
		{
			AddLoreBook(p_staticID, false);
		}

		public void AddLoreBook(Int32 p_staticID, Boolean p_fromSaveGame)
		{
			if (p_staticID != 0)
			{
				LoreBookStaticData staticData = StaticDataHandler.GetStaticData<LoreBookStaticData>(EDataType.LOREBOOK, p_staticID);
				if (!m_foundBooks.Contains(staticData))
				{
					m_foundBooks.Add(staticData);
					String text = Localization.Instance.GetText(staticData.TitleKey);
					String text2 = Localization.Instance.GetText("GAME_MESSSAGE_LOREBOOK_ADDED", text);
					GameMessageEventArgs p_eventArgs = new GameMessageEventArgs(text2, 0f, false);
					if (!p_fromSaveGame)
					{
						m_newEntries.Add(p_staticID);
						LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.GAME_MESSAGE, p_eventArgs);
						LegacyLogic.Instance.EventManager.InvokeEvent(staticData, EEventType.ADD_LOREBOOK, EventArgs.Empty);
					}
				}
			}
		}

		public void Cleanup()
		{
			m_foundBooks.Clear();
			m_newEntries.Clear();
			m_bookCounter.Clear();
		}

		public List<LoreBookStaticData> GetBooksForCategory(ELoreBookCategories category, Boolean p_all)
		{
			List<LoreBookStaticData> list;
			if (p_all)
			{
				list = new List<LoreBookStaticData>(StaticDataHandler.GetIterator<LoreBookStaticData>(EDataType.LOREBOOK));
			}
			else
			{
				list = m_foundBooks;
			}
			if (category == ELoreBookCategories.SHOW_ALL)
			{
				return list;
			}
			List<LoreBookStaticData> list2 = new List<LoreBookStaticData>();
			foreach (LoreBookStaticData loreBookStaticData in list)
			{
				if (loreBookStaticData.Category == category)
				{
					list2.Add(loreBookStaticData);
				}
			}
			return list2;
		}

		public void Load(SaveGameData p_data)
		{
			Int32 num = p_data.Get<Int32>("FoundBooksCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				AddLoreBook(p_data.Get<Int32>("FoundBookID" + i, 0), true);
			}
			Int32 num2 = p_data.Get<Int32>("FoundBooksNewEntriesCount", 0);
			for (Int32 j = 0; j < num2; j++)
			{
				m_newEntries.Add(p_data.Get<Int32>("FoundBooksNewEntry" + j, 0));
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("FoundBooksCount", m_foundBooks.Count);
			for (Int32 i = 0; i < m_foundBooks.Count; i++)
			{
				p_data.Set<Int32>("FoundBookID" + i, m_foundBooks[i].StaticID);
			}
			p_data.Set<Int32>("FoundBooksNewEntriesCount", m_newEntries.Count);
			for (Int32 j = 0; j < m_newEntries.Count; j++)
			{
				p_data.Set<Int32>("FoundBooksNewEntry" + j, m_newEntries[j]);
			}
		}
	}
}
