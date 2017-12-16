using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Legacy.Audio;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.ServiceWrapper;
using Legacy.Core.Utilities;
using Legacy.Core.Utilities.Configuration;
using Legacy.Game.Context;
using Legacy.MMInput;
using UnityEngine;

namespace Legacy.Game
{
	public class Main : MonoBehaviour
	{
		private static Main s_instance;

		private Boolean m_firstUpdate = true;

		[SerializeField]
		private EContext m_StartContextID = EContext.None;

		public static Main Instance => s_instance;

	    private void Awake()
		{
			if (s_instance != null)
			{
				throw new InvalidOperationException("Instance already set!!!!!");
			}
			s_instance = this;
			DontDestroyOnLoad(gameObject);
			String text = "1.5-16336";
			text += " RELEASE";
			UnityEngine.Debug.LogWarning(text);
			Application.runInBackground = false;
			Application.backgroundLoadingPriority = ThreadPriority.Low;
			Shader.WarmupAllShaders();
		}

		private void Start()
		{
			SetupLogic();
			SetupLanguage();
			if (!LegacyLogic.Instance.ServiceWrapper.ForceAppExit())
			{
				SoundConfigManager.Settings.Apply();
				GraphicsConfigManager.Apply();
				foreach (Hotkey hotkeyData in KeyConfigManager.KeyBindings)
				{
					InputManager.SetHotkeyData(hotkeyData);
				}
			}
			StartCoroutine(PreLoadAsset());
		}

		private IEnumerator PreLoadAsset()
		{
			AudioManager.Instance.RequestMainController();
			while (!AudioManager.Instance.MainControllerIsLoaded)
			{
				yield return null;
			}
			Texture2D cur = Helper.ResourcesLoad<Texture2D>("Cursor/CUR_interact");
			Cursor.SetCursor(cur, default(Vector2), CursorMode.Auto);
			ContextManager.ChangeContext(m_StartContextID);
			yield break;
		}

		private void OnApplicationQuit()
		{
			LegacyLogic.Instance.TrackingManager.TrackGameStop(Time.realtimeSinceStartup);
			LegacyLogic.Instance.ServiceWrapper.Close();
		}

		public void QuitGame()
		{
			OnApplicationQuit();
			Process.GetCurrentProcess().Kill();
		}

		private void SetupLogic()
		{
			String streamingAssetsPath = Application.streamingAssetsPath;
			String text;
			String text2;
			if (Helper.Is64BitOperatingSystem())
			{
				text = Path.Combine(streamingAssetsPath, "optionSettings.txt");
				text2 = Path.Combine(GamePaths.UserGamePath, "options64.txt");
			}
			else
			{
				text = Path.Combine(streamingAssetsPath, "optionSettings32.txt");
				text2 = Path.Combine(GamePaths.UserGamePath, "options32.txt");
			}
			KeyConfigManager.SetDefaultsPath(text);
			SoundConfigManager.SetDefaultsPath(text);
			GraphicsConfigManager.SetDefaultsPath(text);
			ConfigManager.Instance.SetOptionDefaultsPath(text);
			Boolean flag = false;
			if (!File.Exists(text2))
			{
				File.Copy(text, text2);
				flag = true;
			}
			else
			{
				ConfigReader configReader = new ConfigReader();
				configReader.ReadData(text2);
				ConfigReader configReader2 = new ConfigReader();
				configReader2.ReadData(text);
				configReader.UnionWith(configReader2);
				configReader.WriteData(text2);
			}
			ConfigManager.Instance.LoadConfigurations(streamingAssetsPath);
			ConfigManager.Instance.LoadOptions(text2);
			KeyConfigManager.LoadConfigurations(text2);
			SoundConfigManager.LoadConfigurations(text2);
			GraphicsConfigManager.LoadConfigurations(text2);
			if (flag)
			{
				GraphicsConfigManager.InitDefaultResolution();
				GraphicsConfigManager.WriteConfigurations();
			}
			LegacyLogic.Instance.ServiceWrapper = new UplayServiceWrapperStrategy();
			if (LegacyLogic.Instance.ServiceWrapper.ForceAppExit() && !LegacyLogic.Instance.ServiceWrapper.UplayNotInstalled())
			{
				QuitGame();
			}
			LegacyLogic.Instance.MapLoader.MapFolder = Path.Combine(streamingAssetsPath, "Maps");
			LegacyLogic.Instance.SetConversationPath(Path.Combine(streamingAssetsPath, "Dialog"));
			LegacyLogic.Instance.LoadStaticData(Path.Combine(streamingAssetsPath, "StaticData"));
		}

		private void SetupLanguage()
		{
			String language = ConfigManager.Instance.Options.Language;
			LocaManager.SetLanguage(language);
		}

		private void Update()
		{
			try
			{
				if (LegacyLogic.Instance.ServiceWrapper.ForceAppExit())
				{
					if (!m_firstUpdate)
					{
						if (LegacyLogic.Instance.ServiceWrapper.UplayNotInstalled())
						{
							String text = LocaManager.GetText("ERROR_POPUP_MESSAGE_UPLAY_NOT_INSTALLED");
							String text2 = LocaManager.GetText("ERROR_POPUP_MESSAGE_CAPTION");
							SystemInvokes.MessageBox(Process.GetCurrentProcess().MainWindowHandle, text, text2, 0);
						}
						QuitGame();
					}
				}
				else
				{
					LegacyLogic.Instance.ServiceWrapper.Update();
					LegacyLogic.Instance.Update();
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception, this);
			}
			m_firstUpdate = false;
		}
	}
}
