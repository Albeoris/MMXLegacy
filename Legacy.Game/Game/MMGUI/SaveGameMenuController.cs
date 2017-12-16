using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Game.IngameManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SaveGameMenuController")]
	public class SaveGameMenuController : MonoBehaviour
	{
		[SerializeField]
		private UITexture m_screenshot;

		[SerializeField]
		private GameObject m_saveButton;

		[SerializeField]
		private GameObject m_loadButton;

		[SerializeField]
		private GameObject m_deleteButton;

		[SerializeField]
		private UIGrid m_saveGameEntryList;

		[SerializeField]
		private UILabel m_saveGameName;

		[SerializeField]
		private GameObject m_saveGameEntryPrefab;

		[SerializeField]
		private Texture m_defaultTexture;

		[SerializeField]
		private Int32 m_entriesPerPage = 12;

		[SerializeField]
		private UILabel m_pageLabel;

		[SerializeField]
		private GameObject m_nextPageButton;

		[SerializeField]
		private GameObject m_previousPageButton;

		[SerializeField]
		private UILabel m_gameTimeLabel;

		[SerializeField]
		private UILabel m_difficultyLabel;

		[SerializeField]
		private UILabel m_header;

		private List<SaveGameEntry> m_entries = new List<SaveGameEntry>();

		private SaveGameEntry m_selectedEntry;

		private SaveGameEntry m_createNewSlotEntry;

		private Boolean m_isVisible;

		private Int32 m_page;

		private Int32 m_maxPage;

		private String m_overrideName = String.Empty;

		private Boolean m_confirmOverride;

		private Boolean m_inMenu;

		private Boolean m_saveMenu;

		private Texture2D m_SavegamePreview;

		public event EventHandler OnClose;

		public event EventHandler OnSave;

		public event EventHandler OnLoadSaveGame;

		public Boolean IsVisible => m_isVisible;

	    public void Init(Boolean p_inMenu, Boolean p_saveMenu)
		{
			m_inMenu = p_inMenu;
			m_saveMenu = p_saveMenu;
			m_saveGameName.text = LocaManager.GetText("SAVEGAMEMENU_SELECT_SAVEGAME");
			LoadSaveGameList();
			if (m_inMenu)
			{
				NGUITools.SetActive(m_saveButton.gameObject, false);
			}
			else if (m_saveMenu)
			{
				NGUITools.SetActive(m_saveButton.gameObject, true);
				NGUITools.SetActive(m_loadButton.gameObject, false);
			}
			else
			{
				NGUITools.SetActive(m_saveButton.gameObject, false);
				NGUITools.SetActive(m_loadButton.gameObject, true);
			}
			if (m_entries.Count == 0)
			{
				NGUITools.SetActive(m_saveButton.gameObject, false);
			}
			if (m_saveMenu)
			{
				m_header.text = LocaManager.GetText("Gui/IngameMenu/Save");
			}
			else
			{
				m_header.text = LocaManager.GetText("Gui/Mainmenu/Load");
			}
			UpdateDeleteButton();
		}

		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(OnGameSaved));
			for (Int32 i = 0; i < m_entriesPerPage; i++)
			{
				GameObject gameObject = NGUITools.AddChild(m_saveGameEntryList.gameObject, m_saveGameEntryPrefab);
				SaveGameEntry component = gameObject.GetComponent<SaveGameEntry>();
				component.SetSelected(false);
				m_entries.Add(component);
			}
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(OnGameSaved));
			Helper.DestroyImmediate<Texture2D>(ref m_SavegamePreview);
		}

		private void OnEnable()
		{
			if (!m_isVisible)
			{
				NGUITools.SetActiveChildren(gameObject, m_isVisible);
			}
		}

		private void OnClickedClose(GameObject p_sender)
		{
			if (OnClose != null)
			{
				OnClose(this, EventArgs.Empty);
			}
		}

		private void OnGameSaved(Object sender, EventArgs e)
		{
			if (m_isVisible)
			{
				OnClickedClose(null);
			}
		}

		public void ToggleVisiblity(Boolean p_saveMenu)
		{
			m_isVisible = !m_isVisible;
			NGUITools.SetActive(gameObject, m_isVisible);
			if (m_isVisible)
			{
				Init(false, p_saveMenu);
			}
		}

		private void UpdateDeleteButton()
		{
			NGUITools.SetActive(m_deleteButton.gameObject, m_selectedEntry != null && m_selectedEntry != m_createNewSlotEntry);
		}

		public void LoadSaveGameList()
		{
			Dictionary<String, SaveGameMeta> allSaveGames = LegacyLogic.Instance.WorldManager.SaveGameManager.GetAllSaveGames(m_saveMenu);
			List<SaveGameMeta> list = new List<SaveGameMeta>(allSaveGames.Values);
			if (list.Count == 0)
			{
				NGUITools.SetActive(m_loadButton, false);
			}
			m_maxPage = (Int32)(list.Count / (Single)m_entriesPerPage + 1f);
			if (list.Count % m_entriesPerPage == 0)
			{
				m_maxPage--;
			}
			m_page = Math.Min(m_page, m_maxPage);
			m_pageLabel.text = m_page + 1 + "/" + m_maxPage;
			NGUITools.SetActive(m_nextPageButton, m_page < m_maxPage - 1);
			NGUITools.SetActive(m_previousPageButton, m_page > 0);
			Int32 num = 0;
			Int32 num2 = 0;
			if (m_saveMenu)
			{
				num2 = 1;
			}
			if (!m_inMenu && m_saveMenu && m_page == 0)
			{
				m_entries[num].Init(this, LocaManager.GetText("SAVEGAME_MENU_NEW_SLOT"), default(SaveGameMeta), false);
				m_createNewSlotEntry = m_entries[num];
				m_entries[num].SetVisible(true);
				ClickedEntry(m_entries[num]);
				num++;
			}
			list.Sort(new Comparison<SaveGameMeta>(SortMetasByDate));
			for (Int32 i = 0; i < list.Count; i++)
			{
				SaveGameMeta p_meta = list[i];
				if (i >= m_page * m_entriesPerPage - num2)
				{
					m_entries[num].Init(this, p_meta.Name, p_meta, true);
					m_entries[num].SetVisible(true);
					num++;
				}
				if (num >= m_entriesPerPage)
				{
					break;
				}
			}
			m_saveGameEntryList.Reposition();
			for (Int32 j = num; j < m_entriesPerPage; j++)
			{
				m_entries[j].SetVisible(false);
			}
			if (!m_saveMenu || m_page > 0)
			{
				if (num > 0)
				{
					ClickedEntry(m_entries[0]);
				}
				else
				{
					ClickedEntry(null);
				}
			}
			UpdateDeleteButton();
			if (LegacyLogic.Instance.WorldManager.SaveGameManager.CheckForError() != ESaveGameError.NONE)
			{
				String text = LocaManager.GetText("ERROR_POPUP_MESSAGE_CAPTION");
				String text2 = LocaManager.GetText("SAVEGAME_ERROR_COULD_NOT_RECEIVE_SAVEGAMES");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM, text, text2, null);
			}
		}

		private Int32 SortMetasByDate(SaveGameMeta p_a, SaveGameMeta p_b)
		{
			Int32 num = p_a.Type.CompareTo(p_b.Type) * -1;
			if (num != 0)
			{
				return num;
			}
			Int32 num2 = p_a.Time.CompareTo(p_b.Time) * -1;
			if (num2 != 0)
			{
				return num2;
			}
			return p_a.SaveNumber.CompareTo(p_b.SaveNumber) * -1;
		}

		public void ClickedEntry(SaveGameEntry p_saveGameEntry)
		{
			Texture mainTexture = m_defaultTexture;
			if (m_selectedEntry != null)
			{
				m_selectedEntry.SetSelected(false);
			}
			m_selectedEntry = p_saveGameEntry;
			if (m_selectedEntry != null)
			{
				m_selectedEntry.SetSelected(true);
				m_saveGameName.text = m_selectedEntry.Name;
				SaveGameMeta meta = p_saveGameEntry.Meta;
				TimeSpan playTime = meta.PlayTime;
				m_gameTimeLabel.text = LocaManager.GetText("SAVEGAMEMENU_GAMETIME_FORMAT", playTime.Days, playTime.Hours);
				m_difficultyLabel.text = LocaManager.GetText((meta.Difficulty != EDifficulty.NORMAL) ? "GUI_DIFFICULTY_WARRIOR" : "GUI_DIFFICULTY_ADVENTURER");
				Byte[] saveGameImage = LegacyLogic.Instance.WorldManager.SaveGameManager.GetSaveGameImage(p_saveGameEntry.name);
				if (saveGameImage != null)
				{
					if (m_SavegamePreview == null)
					{
						m_SavegamePreview = new Texture2D(1, 1, TextureFormat.RGBA32, false);
						m_SavegamePreview.hideFlags = HideFlags.DontSave;
					}
					if (m_SavegamePreview.LoadImage(saveGameImage))
					{
						mainTexture = m_SavegamePreview;
					}
				}
				else
				{
					m_gameTimeLabel.text = " - ";
					m_difficultyLabel.text = String.Empty;
				}
				Boolean state = LegacyLogic.Instance.WorldManager.SaveGameManager.SaveGameExists(p_saveGameEntry.name);
				if (!m_saveMenu)
				{
					NGUITools.SetActive(m_loadButton.gameObject, state);
				}
			}
			else
			{
				if (!m_saveMenu)
				{
					NGUITools.SetActive(m_loadButton.gameObject, false);
				}
				m_gameTimeLabel.text = " - ";
				m_difficultyLabel.text = String.Empty;
				m_saveGameName.text = LocaManager.GetText("SAVEGAMEMENU_SELECT_SAVEGAME");
			}
			m_screenshot.mainTexture = mainTexture;
			UpdateDeleteButton();
		}

		public void OnDoubleClick(SaveGameEntry p_entry)
		{
			if (m_selectedEntry != p_entry)
			{
				ClickedEntry(p_entry);
			}
			if (m_saveMenu)
			{
				if (m_saveButton.activeSelf)
				{
					OnClickedSave(gameObject);
				}
			}
			else if (m_loadButton.activeSelf)
			{
				OnClickedLoad(gameObject);
			}
		}

		public void OnClickedSave(GameObject p_sender)
		{
			if (m_selectedEntry != null && LegacyLogic.Instance.WorldManager.SaveGameManager.SaveGameExists(m_selectedEntry.name))
			{
				IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, String.Format(LocaManager.GetText("SAVEGAME_MENU_OVERRIDE_REQUEST_POPUP"), m_selectedEntry.name), new PopupRequest.RequestCallback(OnRequestClosed));
			}
			else if (m_selectedEntry != null && m_selectedEntry == m_createNewSlotEntry)
			{
				String inputFieldText = CheckDefaultSaveGameName(LocaManager.GetText(LegacyLogic.Instance.MapLoader.Grid.LocationLocaName));
				IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.TEXTFIELD_CONFIRM, String.Empty, LocaManager.GetText("SAVEGAME_MENU_CREATE_NEW_SAVEGAME"), new PopupRequest.RequestCallback(OnSaveNewSlotRequestClosed));
				IngameController.Instance.PopupRequest.InputFieldText = inputFieldText;
			}
		}

		private String CheckDefaultSaveGameName(String p_gridName)
		{
			if (p_gridName.Contains("@"))
			{
				p_gridName = p_gridName.Split(new Char[]
				{
					'@'
				})[0];
			}
			if (p_gridName.Length <= 20)
			{
				return p_gridName;
			}
			if (p_gridName.Contains("-"))
			{
				p_gridName = p_gridName.Split(new Char[]
				{
					'-'
				})[0];
			}
			else if (p_gridName.Contains(" "))
			{
				p_gridName = p_gridName.Split(new Char[]
				{
					' '
				})[0];
			}
			return p_gridName;
		}

		private void OnSaveNewSlotRequestClosed(PopupRequest.EResultType p_result, String p_saveGameName)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				if (CheckFileName(p_saveGameName))
				{
					if (LegacyLogic.Instance.WorldManager.SaveGameManager.SaveGameExists(p_saveGameName))
					{
						m_overrideName = p_saveGameName;
						m_confirmOverride = true;
					}
					else
					{
						SaveGame(p_saveGameName);
						m_selectedEntry = null;
					}
				}
				else
				{
					Debug.Log("Wrong Filename");
				}
			}
		}

		private Boolean CheckFileName(String p_name)
		{
			if (p_name == LocaManager.GetText("SAVEGAME_MENU_NEW_SLOT") || p_name == LocaManager.GetText("SAVEGAMETYPE_AUTO") || p_name == LocaManager.GetText("SAVEGAMETYPE_QUICK"))
			{
				IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.CONFIRM, String.Empty, LocaManager.GetText("SAVEGAME_ERROR_NAME_NOT_ALLOWED", p_name), new PopupRequest.RequestCallback(OnErrorConfirmClosed));
				return false;
			}
			if (p_name.Length > 30)
			{
				IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.CONFIRM, String.Empty, LocaManager.GetText("SAVEGAME_ERROR_MORE_THAN_30_CHARACTERS"), new PopupRequest.RequestCallback(OnErrorConfirmClosed));
				return false;
			}
			if (p_name.Length == 0 || p_name.Replace(" ", String.Empty).Replace("\t", String.Empty).Length == 0)
			{
				IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.CONFIRM, String.Empty, LocaManager.GetText("SAVEGAME_ERROR_INPUT_EMPTY"), new PopupRequest.RequestCallback(OnErrorConfirmClosed));
				return false;
			}
			Char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			for (Int32 i = 0; i < invalidFileNameChars.Length; i++)
			{
				if (p_name.IndexOf(invalidFileNameChars[i]) != -1)
				{
					IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.CONFIRM, String.Empty, LocaManager.GetText("SAVEGAME_ERROR_INVALID_CHARACTERS"), new PopupRequest.RequestCallback(OnErrorConfirmClosed));
					return false;
				}
			}
			return true;
		}

		private void OnErrorConfirmClosed(PopupRequest.EResultType p_result, String p_inputString)
		{
			String inputFieldText = IngameController.Instance.PopupRequest.InputFieldText;
			IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.TEXTFIELD_CONFIRM, String.Empty, LocaManager.GetText("SAVEGAME_MENU_CREATE_NEW_SAVEGAME"), new PopupRequest.RequestCallback(OnSaveNewSlotRequestClosed));
			IngameController.Instance.PopupRequest.InputFieldText = inputFieldText;
		}

		private void OnRequestClosed(PopupRequest.EResultType p_result, String p_text)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				if (m_selectedEntry != m_createNewSlotEntry || m_page > 0)
				{
					SaveGame(m_selectedEntry.name);
					ClickedEntry(null);
				}
				else
				{
					SaveGame(m_overrideName);
				}
			}
		}

		private void SaveGame(String saveName)
		{
			LegacyLogic.Instance.WorldManager.SaveGameName = saveName;
			LegacyLogic.Instance.WorldManager.CurrentSaveGameType = ESaveGameType.NORMAL;
			LegacyLogic.Instance.WorldManager.HighestSaveGameNumber++;
			if (OnSave != null)
			{
				OnSave(this, EventArgs.Empty);
			}
		}

		public void OnClickedLoad(GameObject p_sender)
		{
			if (m_inMenu)
			{
				LegacyLogic.Instance.WorldManager.LoadedFromStartMenu = true;
				LegacyLogic.Instance.WorldManager.IsSaveGame = true;
				if (m_selectedEntry != null)
				{
					LegacyLogic.Instance.WorldManager.SaveGameName = m_selectedEntry.name;
					if (OnClose != null)
					{
						OnClose(this, EventArgs.Empty);
					}
					if (OnLoadSaveGame != null)
					{
						OnLoadSaveGame(this, EventArgs.Empty);
					}
				}
			}
			else
			{
				IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, LocaManager.GetText("SAVEGAME_MENU_LOAD_REQUEST_POPUP"), new PopupRequest.RequestCallback(OnLoadRequestClosed));
			}
		}

		private void OnLoadRequestClosed(PopupRequest.EResultType p_result, String p_text)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				LegacyLogic.Instance.WorldManager.IsSaveGame = true;
				LegacyLogic.Instance.WorldManager.Load(m_selectedEntry.name);
				if (LegacyLogic.Instance.WorldManager.SaveGameManager.CheckForError() == ESaveGameError.COULD_NOT_LOAD_SAVEGAME)
				{
					String text = LocaManager.GetText("ERROR_POPUP_MESSAGE_CAPTION");
					String text2 = LocaManager.GetText("SAVEGAME_ERROR_LOADING_SAVEGAME");
					PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM, text, text2, null);
				}
				else if (OnClose != null)
				{
					OnClose(this, EventArgs.Empty);
				}
			}
		}

		public void OnClickedDelete(GameObject p_sender)
		{
			if (!m_inMenu)
			{
				IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, String.Format(LocaManager.GetText("SAVEGAME_MENU_DELETE_REQUEST_POPUP"), m_selectedEntry.name), new PopupRequest.RequestCallback(OnDeleteRequestClosed));
			}
			else
			{
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, String.Format(LocaManager.GetText("SAVEGAME_MENU_DELETE_REQUEST_POPUP"), m_selectedEntry.name), new PopupRequest.RequestCallback(OnDeleteRequestClosed));
			}
		}

		public void OnClickedNextPage(GameObject p_sender)
		{
			if (m_page < m_maxPage)
			{
				m_page++;
				LoadSaveGameList();
			}
		}

		public void OnClickedPreviousPage(GameObject p_sender)
		{
			if (m_page > 0)
			{
				m_page--;
				LoadSaveGameList();
			}
		}

		private void OnDeleteRequestClosed(PopupRequest.EResultType p_result, String p_text)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				DeleteSelectedSaveGame();
				m_selectedEntry.SetSelected(false);
				m_selectedEntry = null;
				m_saveGameName.text = LocaManager.GetText("SAVEGAMEMENU_SELECT_SAVEGAME");
				m_screenshot.mainTexture = m_defaultTexture;
				m_gameTimeLabel.text = " - ";
				m_difficultyLabel.text = String.Empty;
				LoadSaveGameList();
			}
		}

		private void DeleteSelectedSaveGame()
		{
			if (m_selectedEntry != null)
			{
				LegacyLogic.Instance.WorldManager.SaveGameManager.DeleteSaveGame(m_selectedEntry.name);
			}
		}

		private void Update()
		{
			if (m_confirmOverride)
			{
				IngameController.Instance.PopupRequest.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, String.Format(LocaManager.GetText("SAVEGAME_MENU_OVERRIDE_REQUEST_POPUP"), m_overrideName), new PopupRequest.RequestCallback(OnRequestClosed));
				m_confirmOverride = false;
			}
		}
	}
}
