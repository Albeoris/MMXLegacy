using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Lorebook;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/BestiaryController")]
	public class BestiaryController : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_monsterName;

		[SerializeField]
		private TabController m_categoryTabs;

		[SerializeField]
		private BestiaryInfoController m_infoController;

		[SerializeField]
		private GameObject m_championInfo;

		[SerializeField]
		private UITexture m_renderTexture;

		[SerializeField]
		private UITexture m_renderTextureChampion;

		[SerializeField]
		private UICheckbox m_checkBox;

		[SerializeField]
		private UILabel m_headerLabel;

		[SerializeField]
		private PageableListController m_pageableList;

		[SerializeField]
		private Vector3 m_boxColliderNormalPos;

		[SerializeField]
		private Vector3 m_boxColliderNormalSize;

		[SerializeField]
		private Vector3 m_boxColliderChampionPos;

		[SerializeField]
		private Vector3 m_boxColliderChampionSize;

		private BestiaryHandler m_bestiaryHandler;

		private BestiaryCameraSetup m_fakeScene;

		private RenderTexture m_targetTexture;

		private BoxCollider col;

		public void Init()
		{
			m_bestiaryHandler = LegacyLogic.Instance.WorldManager.BestiaryHandler;
			m_targetTexture = new RenderTexture(504, 700, 24, RenderTextureFormat.ARGB32);
			col = GetComponent<BoxCollider>();
			m_targetTexture.Create();
			m_renderTexture.mainTexture = m_targetTexture;
			m_renderTextureChampion.mainTexture = m_targetTexture;
		}

		public void Open()
		{
			m_categoryTabs.TabIndexChanged += OnCategorySelected;
			UICheckbox checkBox = m_checkBox;
			checkBox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(checkBox.onStateChange, new UICheckbox.OnStateChange(OnCheckBoxChanged));
			m_pageableList.OnNextPageEvent += UpdateMonsters;
			m_pageableList.OnPrevPageEvent += UpdateMonsters;
			if (m_fakeScene == null)
			{
				m_infoController.Init();
				m_fakeScene = Helper.Instantiate<BestiaryCameraSetup>(Helper.ResourcesLoad<BestiaryCameraSetup>("Prefabs/GUI/BestiaryCameraSetup"));
				m_fakeScene.Camera.enabled = true;
				m_fakeScene.InitCamera(m_targetTexture);
				DontDestroyOnLoad(m_fakeScene);
				DontDestroyOnLoad(m_fakeScene.Camera);
			}
			else
			{
				m_fakeScene.Camera.gameObject.SetActive(true);
			}
			for (Int32 i = 0; i < m_categoryTabs.Tabs.Length; i++)
			{
				Tab tab = m_categoryTabs.Tabs[i];
				Dictionary<Int32, Int32> monstersForCategory;
				if (i != m_categoryTabs.Tabs.Length - 1)
				{
					monstersForCategory = m_bestiaryHandler.GetMonstersForCategory((EMonsterClass)i, false);
				}
				else
				{
					monstersForCategory = m_bestiaryHandler.GetMonstersForCategory(EMonsterClass.NONE, true);
				}
				Int32 num = 0;
				foreach (Int32 key in monstersForCategory.Keys)
				{
					if (monstersForCategory[key] > 0)
					{
						num++;
					}
				}
				tab.AddTextToTooltip(" (" + LocaManager.GetText("GUI_STATS_X_OF_Y", num, monstersForCategory.Keys.Count) + ")");
			}
			m_categoryTabs.SelectTab(0, true);
		}

		public void Close()
		{
			m_categoryTabs.TabIndexChanged -= OnCategorySelected;
			UICheckbox checkBox = m_checkBox;
			checkBox.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(checkBox.onStateChange, new UICheckbox.OnStateChange(OnCheckBoxChanged));
			m_pageableList.OnNextPageEvent -= UpdateMonsters;
			m_pageableList.OnPrevPageEvent -= UpdateMonsters;
			if (m_fakeScene != null)
			{
				m_fakeScene.Camera.gameObject.SetActive(false);
			}
			CleanupPageableList();
		}

		public Int32 InitList(EMonsterClass p_monsterClass)
		{
			m_monsterName.enabled = false;
			m_infoController.Hide();
			CleanupPageableList();
			Int32 num = 0;
			Int32 num2 = 0;
			Boolean p_showChampions = m_categoryTabs.CurrentTabIndex == m_categoryTabs.Tabs.Length - 1;
			Dictionary<Int32, Int32> monstersForCategory = m_bestiaryHandler.GetMonstersForCategory(p_monsterClass, p_showChampions);
			Dictionary<Int32, Int32> dictionary = new Dictionary<Int32, Int32>();
			foreach (Int32 num3 in monstersForCategory.Keys)
			{
				if (StaticDataHandler.GetStaticData<MonsterStaticData>(EDataType.MONSTER, num3) != null)
				{
					Int32 num4 = 0;
					if (monstersForCategory.TryGetValue(num3, out num4))
					{
						if (num4 > 0)
						{
							num++;
						}
						if (num4 != 0 || m_checkBox.isChecked)
						{
							dictionary.Add(num3, monstersForCategory[num3]);
						}
					}
				}
			}
			Int32[] array = dictionary.Keys.ToArray<Int32>();
			for (Int32 i = m_pageableList.CurrentIndex; i < m_pageableList.EndIndex; i++)
			{
				if (i < dictionary.Count)
				{
					Int32 num5 = array[i];
					BookEntry entry = m_pageableList.GetEntry();
					MonsterStaticData staticData = StaticDataHandler.GetStaticData<MonsterStaticData>(EDataType.MONSTER, num5);
					if (staticData == null)
					{
						entry.OnBookSelected += OnMonsterSelected;
						entry.Init(num5, "[FF0000]MISSING DATA[-]");
						entry.IsNewEntry = false;
						entry.IsActive = false;
						num2++;
					}
					Int32 num6 = dictionary[num5];
					if (!m_checkBox.isChecked)
					{
						if (num6 > 0)
						{
							entry.OnBookSelected += OnMonsterSelected;
							entry.Init(num5, LocaManager.GetText(staticData.NameKey));
							entry.IsNewEntry = m_bestiaryHandler.NewEntries.Contains(num5);
							num2++;
						}
					}
					else
					{
						entry.OnBookSelected += OnMonsterSelected;
						if (num6 > 0)
						{
							entry.Init(num5, LocaManager.GetText(staticData.NameKey));
							entry.IsNewEntry = m_bestiaryHandler.NewEntries.Contains(num5);
							num2++;
						}
						else
						{
							entry.Init(num5, "?????");
							entry.IsNewEntry = false;
							entry.IsActive = false;
							num2++;
						}
					}
				}
				else
				{
					BookEntry entry2 = m_pageableList.GetEntry();
					entry2.Init(0, String.Empty);
					entry2.IsNewEntry = false;
				}
			}
			BookEntry bookEntry = m_pageableList.TrySelectActiveEntryOnPage();
			if (bookEntry != null)
			{
				OnMonsterSelected(bookEntry, EventArgs.Empty);
			}
			return dictionary.Count;
		}

		private void UpdateMonsters(Object sender, EventArgs e)
		{
			EMonsterClass p_monsterClass = EMonsterClass.NONE;
			if (m_categoryTabs.CurrentTabIndex != m_categoryTabs.Tabs.Length - 1)
			{
				p_monsterClass = (EMonsterClass)m_categoryTabs.CurrentTabIndex;
			}
			InitList(p_monsterClass);
			m_pageableList.Show();
		}

		private void OnDisable()
		{
			Close();
		}

		private void OnCategorySelected(Object p_sender, EventArgs p_args)
		{
			m_pageableList.CurrentIndex = 0;
			EMonsterClass emonsterClass = EMonsterClass.NONE;
			if (m_categoryTabs.CurrentTabIndex != m_categoryTabs.Tabs.Length - 1)
			{
				emonsterClass = (EMonsterClass)m_categoryTabs.CurrentTabIndex;
				String str = (emonsterClass != EMonsterClass.NONE) ? emonsterClass.ToString() : "SHOW_ALL";
				m_headerLabel.text = LocaManager.GetText("TT_BESTIARY_" + str);
			}
			else
			{
				m_headerLabel.text = LocaManager.GetText("TT_BESTIARY_CHAMPION");
			}
			Int32 bookCount = InitList(emonsterClass);
			m_pageableList.Finish(bookCount);
		}

		private void OnMonsterSelected(Object p_sender, EventArgs p_args)
		{
			BookEntry bookEntry = p_sender as BookEntry;
			if (bookEntry == null)
			{
				return;
			}
			m_monsterName.enabled = true;
			Int32 entryID = bookEntry.EntryID;
			foreach (BookEntry bookEntry2 in m_pageableList.EntryList)
			{
				bookEntry2.SetSelected(entryID);
			}
			Int32 num = 0;
			if (m_bestiaryHandler.AllMonsters.TryGetValue(entryID, out num))
			{
				if (num > 0)
				{
					bookEntry.IsNewEntry = false;
					m_bestiaryHandler.RemoveFromNewEntries(entryID);
					MonsterStaticData staticData = StaticDataHandler.GetStaticData<MonsterStaticData>(EDataType.MONSTER, entryID);
					if (staticData.Grade == EMonsterGrade.BOSS || staticData.Grade == EMonsterGrade.CHAMPION)
					{
						NGUITools.SetActiveSelf(m_championInfo, true);
						NGUITools.SetActiveSelf(m_infoController.gameObject, false);
						m_infoController.ShowProgressBar(false);
						m_monsterName.text = LocaManager.GetText(staticData.NameKey);
						col.center = m_boxColliderChampionPos;
						col.size = m_boxColliderChampionSize;
					}
					else
					{
						NGUITools.SetActiveSelf(m_championInfo, false);
						NGUITools.SetActiveSelf(m_infoController.gameObject, true);
						m_infoController.Show();
						m_monsterName.text = LocaManager.GetText(staticData.NameKey);
						m_infoController.SetEntry(staticData, m_bestiaryHandler.AllMonsters[staticData.StaticID]);
						m_infoController.ShowProgressBar(true);
						col.center = m_boxColliderNormalPos;
						col.size = m_boxColliderNormalSize;
					}
					if ((staticData.StaticID >= 35 && staticData.StaticID <= 37) || staticData.Type == EMonsterType.MINOTAUR || staticData.Type == EMonsterType.MINOTAUR_GUARD)
					{
						m_fakeScene.DisplayMonsterPrefab(staticData.Prefab, ESize.MEDIUM);
					}
					else
					{
						m_fakeScene.DisplayMonsterPrefab(staticData.Prefab, staticData.Size);
					}
				}
				else
				{
					m_infoController.Hide();
					m_monsterName.text = "?????";
				}
			}
			else
			{
				m_infoController.Hide();
				m_monsterName.text = "?????";
			}
		}

		private void CleanupPageableList()
		{
			foreach (BookEntry bookEntry in m_pageableList.EntryList)
			{
				bookEntry.Init(-1, String.Empty);
				bookEntry.OnBookSelected -= OnMonsterSelected;
				bookEntry.SetSelected(false);
			}
		}

		public void ResetFakeScene()
		{
			if (m_fakeScene != null)
			{
				m_fakeScene.Reset();
			}
		}

		private void OnPress(Boolean p_isButtonDown)
		{
			if (UICamera.currentTouchID == -2)
			{
				if (p_isButtonDown)
				{
					m_fakeScene.PlayRandomAnimation();
				}
			}
			else
			{
				m_fakeScene.Clicked = p_isButtonDown;
			}
		}

		private void OnAnimButtonPressed()
		{
			m_fakeScene.PlayRandomAnimation();
		}

		private void OnDrag(Vector2 p_delta)
		{
			m_fakeScene.HandleDragging(p_delta);
		}

		private void OnCheckBoxChanged(Boolean state)
		{
			OnCategorySelected(null, EventArgs.Empty);
		}

		public void Cleanup()
		{
			if (m_fakeScene != null)
			{
				m_fakeScene.Camera.enabled = false;
				m_fakeScene.Reset();
				Helper.DestroyImmediate<RenderTexture>(ref m_targetTexture);
				Helper.Destroy<GameObject>(m_fakeScene.gameObject);
			}
			m_categoryTabs.TabIndexChanged -= OnCategorySelected;
			UICheckbox checkBox = m_checkBox;
			checkBox.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(checkBox.onStateChange, new UICheckbox.OnStateChange(OnCheckBoxChanged));
			m_pageableList.OnNextPageEvent -= UpdateMonsters;
			m_pageableList.OnPrevPageEvent -= UpdateMonsters;
			CleanupPageableList();
		}
	}
}
