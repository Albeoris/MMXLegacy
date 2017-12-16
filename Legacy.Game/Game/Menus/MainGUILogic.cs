using System;
using System.Collections.Generic;
using System.IO;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Mods;
using Legacy.Core.SaveGameManagement;
using Legacy.Game.Context;
using Legacy.Game.MMGUI;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/MainGUILogic")]
	public class MainGUILogic : MonoBehaviour
	{
		[SerializeField]
		private Texture m_backgroundTexture43;

		[SerializeField]
		private Texture m_backgroundTexture169;

		[SerializeField]
		private PopupRequest m_popUpRequest;

		[SerializeField]
		private TooltipManager m_tooltipManager;

		[SerializeField]
		private UITexture m_background;

		[SerializeField]
		private Single m_backgroundFlickerTimeMin = 1f;

		[SerializeField]
		private Single m_backgroundFlickerTimeMax = 2f;

		[SerializeField]
		private Single m_backgroundFlickerCurveTopMin = 0.02f;

		[SerializeField]
		private Single m_backgroundFlickerCurveTopMax = 0.05f;

		[SerializeField]
		private Single m_backgroundFlickerCurveBottomMin = -0.01f;

		[SerializeField]
		private Single m_backgroundFlickerCurveBottomMax = -0.03f;

		[SerializeField]
		private SaveGameMenuController m_saveGameController;

		[SerializeField]
		private ModMenu m_modMenu;

		[SerializeField]
		private UILabel m_version;

		[SerializeField]
		private OptionsMenu m_options;

		[SerializeField]
		private UnlockContentManager m_unlockContentManager;

		[SerializeField]
		private GameObject m_veil;

		[SerializeField]
		private UIButton m_continueButton;

		[SerializeField]
		private UIButton m_loadButton;

		[SerializeField]
		private GameObject m_page1;

		[SerializeField]
		private GameObject m_page2;

		private Single m_currentFlickerTotalTime;

		private Single m_currentFlickerCurveTop;

		private Single m_currentFlickerCurveBottom;

		private Single m_currentFlickerTime;

		private Texture2D m_createdBackground;

		public void Awake()
		{
			NGUITools.SetActiveSelf(m_saveGameController.gameObject, false);
			m_saveGameController.OnClose += HandleOnClose;
			m_saveGameController.OnLoadSaveGame += HandleOnLoadSaveGame;
			m_modMenu.OnClose += OnCloseModMenu;
			m_modMenu.OnLoadMod += HandleOnLoadMod;
			m_unlockContentManager.OnClose += OnUnlockContentClose;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOD_LOADED, new EventHandler(OnModLoaded));
			m_version.text = LocaManager.GetText("VERSION_NUMBER", "1.5-16336");
			if (LegacyLogic.Instance.ModController.InModMode)
			{
				ModController.ModInfo currentMod = LegacyLogic.Instance.ModController.CurrentMod;
				if (currentMod != null)
				{
					m_version.text = currentMod.Name + " " + LocaManager.GetText("VERSION_NUMBER", currentMod.Version);
				}
			}
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCloseKeyPressed));
			m_tooltipManager.Init();
			m_options.CloseEvent += OnCloseOptionsMenu;
			if (PopupRequest.Instance != null)
			{
				PopupRequest.Instance.Destroy();
			}
			m_popUpRequest.Init();
		}

		private void Start()
		{
			OnResolutionChange();
			NGUITools.SetActive(m_popUpRequest.gameObject, false);
			NGUITools.SetActive(m_tooltipManager.gameObject, false);
			if (LegacyLogic.Instance.WorldManager.SaveGameManager.GetAllSaveGames(false).Count == 0)
			{
				UIButton[] components = m_loadButton.gameObject.GetComponents<UIButton>();
				for (Int32 i = 0; i < components.Length; i++)
				{
					components[i].isEnabled = false;
					components[i].UpdateColor(false, true);
				}
				components = m_continueButton.gameObject.GetComponents<UIButton>();
				for (Int32 j = 0; j < components.Length; j++)
				{
					components[j].isEnabled = false;
					components[j].UpdateColor(false, true);
				}
			}
			else if (LegacyLogic.Instance.WorldManager.SaveGameManager.CheckForError() != ESaveGameError.NONE)
			{
				String text = LocaManager.GetText("ERROR_POPUP_MESSAGE_CAPTION");
				String text2 = LocaManager.GetText("SAVEGAME_ERROR_COULD_NOT_RECEIVE_SAVEGAMES");
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM, text, text2, null);
			}
		}

		private void OnDestroy()
		{
			Helper.DestroyImmediate<Texture2D>(ref m_createdBackground);
			m_saveGameController.OnClose -= HandleOnClose;
			m_saveGameController.OnLoadSaveGame -= HandleOnLoadSaveGame;
			m_modMenu.OnClose -= OnCloseModMenu;
			m_modMenu.OnLoadMod -= HandleOnLoadMod;
			m_unlockContentManager.OnClose -= OnUnlockContentClose;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOD_LOADED, new EventHandler(OnModLoaded));
			m_options.CloseEvent -= OnCloseOptionsMenu;
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnCloseKeyPressed));
		}

		private void OnModLoaded(Object p_sender, EventArgs p_args)
		{
			if (LegacyLogic.Instance.WorldManager.SaveGameManager.GetAllSaveGames(false).Count == 0)
			{
				UIButton[] components = m_loadButton.gameObject.GetComponents<UIButton>();
				for (Int32 i = 0; i < components.Length; i++)
				{
					components[i].isEnabled = false;
					components[i].UpdateColor(false, true);
				}
				components = m_continueButton.gameObject.GetComponents<UIButton>();
				for (Int32 j = 0; j < components.Length; j++)
				{
					components[j].isEnabled = false;
					components[j].UpdateColor(false, true);
				}
			}
		}

		private void OnResolutionChange()
		{
			EAspectRatio easpectRatio = GraphicsConfigManager.GetAspectRatio();
			if (easpectRatio == EAspectRatio.None)
			{
				easpectRatio = EAspectRatio._4_3;
			}
			Texture mainTexture = (easpectRatio != EAspectRatio._4_3) ? m_backgroundTexture169 : m_backgroundTexture43;
			if (LegacyLogic.Instance.ModController.InModMode)
			{
				ModController.ModInfo currentMod = LegacyLogic.Instance.ModController.CurrentMod;
				if (!String.IsNullOrEmpty(currentMod.TitleImage))
				{
					String path = Path.Combine(currentMod.ImageFolder, currentMod.TitleImage);
					if (File.Exists(path))
					{
						if (m_createdBackground == null)
						{
							m_createdBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
							m_createdBackground.hideFlags = HideFlags.DontSave;
						}
						Byte[] data = File.ReadAllBytes(path);
						if (m_createdBackground.LoadImage(data))
						{
							mainTexture = m_createdBackground;
						}
					}
				}
			}
			m_background.mainTexture = mainTexture;
			if (easpectRatio == EAspectRatio._4_3)
			{
				m_background.transform.localScale = new Vector3(2048f, 1536f, 1f);
			}
			else
			{
				m_background.transform.localScale = new Vector3(2732f, 1536f, 1f);
			}
		}

		private void SetVeil(Boolean p_visible)
		{
			NGUITools.SetActiveSelf(m_veil, p_visible);
		}

		private void HandleOnLoadSaveGame(Object sender, EventArgs e)
		{
			ContextManager.ChangeContext(EContext.Game);
		}

		public void OnStartButtonClick()
		{
			ContextManager.ChangeContext(EContext.PartyCreation);
		}

		public void OnOpenDevButtonClicked()
		{
			Application.OpenURL("https://mightandmagicx-legacy.ubi.com/opendev/blog");
		}

		public void OnLoadButtonClick(GameObject p_sender)
		{
			CloseAll();
			if (m_saveGameController != null)
			{
				SetVeil(true);
				m_saveGameController.ToggleVisiblity(false);
				m_saveGameController.Init(true, false);
			}
		}

		public void OnOptionsButtonClick()
		{
			CloseAll();
			SetVeil(true);
			m_options.Open();
		}

		public void OnExtrasButtonClick()
		{
			NGUITools.SetActive(m_page2, true);
			NGUITools.SetActive(m_page1, false);
		}

		public void OnBackButtonClick()
		{
			NGUITools.SetActive(m_page1, true);
			NGUITools.SetActive(m_page2, false);
		}

		public void OnUnlockContentButtonClick()
		{
			CloseAll();
			SetVeil(true);
			m_unlockContentManager.Open();
		}

		public void OnUnlockContentClose(Object p_sender, EventArgs p_args)
		{
			SetVeil(false);
		}

		public void OnModButtonClick()
		{
			CloseAll();
			SetVeil(true);
			m_modMenu.ToggleVisiblity();
			m_modMenu.Init();
		}

		private void HandleOnClose(Object p_sender, EventArgs p_args)
		{
			m_saveGameController.ToggleVisiblity(false);
			SetVeil(false);
		}

		private void HandleOnLoadMod(Object sender, EventArgs e)
		{
			ModController.ModInfo currentMod = LegacyLogic.Instance.ModController.CurrentMod;
			if (currentMod != null)
			{
				m_version.text = currentMod.Name + " " + LocaManager.GetText("VERSION_NUMBER", currentMod.Version);
			}
			else
			{
				m_version.text = LocaManager.GetText("VERSION_NUMBER", "1.5-16336");
			}
			OnResolutionChange();
		}

		private void OnCloseModMenu(Object p_sender, EventArgs p_args)
		{
			m_modMenu.ToggleVisiblity();
			SetVeil(false);
		}

		private void OnCloseOptionsMenu(Object p_sender, EventArgs p_args)
		{
			SetVeil(false);
		}

		private void CloseAll()
		{
			if (m_options.IsVisible)
			{
				m_options.Close();
			}
			if (m_saveGameController.IsVisible)
			{
				m_saveGameController.ToggleVisiblity(false);
			}
			if (m_modMenu.IsVisible)
			{
				m_modMenu.ToggleVisiblity();
			}
		}

		private void Update()
		{
			Single num = m_currentFlickerTime / m_currentFlickerTotalTime;
			Single num2 = Mathf.Sin(num * 3.14159274f * 2f);
			m_currentFlickerTime += Time.deltaTime;
			if (m_currentFlickerTime >= m_currentFlickerTotalTime)
			{
				m_currentFlickerTime -= m_currentFlickerTotalTime;
				GenerateNewFlickerValues();
			}
			Single num3;
			if (num2 < 0f)
			{
				num3 = 0.5f - num2 * m_currentFlickerCurveBottom;
			}
			else
			{
				num3 = 0.5f + num2 * m_currentFlickerCurveTop;
			}
			Color color = new Color(num3, num3, num3, num3);
			m_background.color = color;
		}

		private void GenerateNewFlickerValues()
		{
			m_currentFlickerTotalTime = Random.Range(m_backgroundFlickerTimeMin, m_backgroundFlickerTimeMax);
			m_currentFlickerCurveTop = Random.Range(m_backgroundFlickerCurveTopMin, m_backgroundFlickerCurveTopMax);
			m_currentFlickerCurveBottom = Random.Range(m_backgroundFlickerCurveBottomMax, m_backgroundFlickerCurveBottomMin);
		}

		public void OnExitButtonClick()
		{
			Main.Instance.QuitGame();
		}

		private void OnCloseKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown)
			{
				if (m_options.IsVisible)
				{
					m_options.Close();
				}
				else if (m_saveGameController.IsVisible)
				{
					m_saveGameController.ToggleVisiblity(false);
					SetVeil(false);
				}
				else if (m_modMenu.IsVisible)
				{
					m_modMenu.ToggleVisiblity();
					SetVeil(false);
				}
				else if (m_unlockContentManager.IsVisible)
				{
					m_unlockContentManager.Close();
				}
			}
		}

		public void OnCreditsButtonClick()
		{
			ContextManager.ChangeContext(EContext.CreditsScreen);
		}

		public void OnContinueButtonClick()
		{
			Dictionary<String, SaveGameMeta> allSaveGames = LegacyLogic.Instance.WorldManager.SaveGameManager.GetAllSaveGames(false);
			List<SaveGameMeta> list = new List<SaveGameMeta>(allSaveGames.Values);
			if (list.Count > 0)
			{
				list.Sort(new Comparison<SaveGameMeta>(SortSaveGamesByDate));
				LegacyLogic.Instance.WorldManager.LoadedFromStartMenu = true;
				LegacyLogic.Instance.WorldManager.IsSaveGame = true;
				LegacyLogic.Instance.WorldManager.SaveGameName = list[0].Name;
				ContextManager.ChangeContext(EContext.Game);
			}
		}

		private Int32 SortMetasByDate(SaveGameMeta p_a, SaveGameMeta p_b)
		{
			Int32 num = p_a.Type.CompareTo(p_b.Type) * -1;
			if (num != 0)
			{
				return num;
			}
			return SortSaveGamesByDate(p_a, p_b);
		}

		private Int32 SortSaveGamesByDate(SaveGameMeta p_a, SaveGameMeta p_b)
		{
			Int32 num = p_a.Time.CompareTo(p_b.Time) * -1;
			if (num != 0)
			{
				return num;
			}
			return p_a.SaveNumber.CompareTo(p_b.SaveNumber) * -1;
		}
	}
}
