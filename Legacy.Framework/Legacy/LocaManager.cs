using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Mods;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/LocaManager")]
	public class LocaManager : MonoBehaviour
	{
		private const String LOCA_FOLDER = "Localisation";

		private static LocaManager s_Instance;

		[SerializeField]
		private SystemLanguage m_Language = SystemLanguage.English;

		[SerializeField]
		private LanguageConfig[] m_Configs;

		[SerializeField]
		private LanguageFontMap m_FontMapDefault;

		[SerializeField]
		private LanguageFontMap[] m_FontMaps;

		public static SystemLanguage Language
		{
			get
			{
				if (s_Instance != null)
				{
					return s_Instance.m_Language;
				}
				Debug.LogError("LocaManager: s_Instance is null! Have you forgotten to drop LocaManager into the scene?");
				return SystemLanguage.Unknown;
			}
			set
			{
				if (s_Instance != null && s_Instance.m_Language != value)
				{
					s_Instance.m_Language = value;
					s_Instance.LoadLanguage();
				}
			}
		}

		public static String GetText(String id, Object arg)
		{
			return Localization.Instance.GetText(id, arg);
		}

		public static String GetText(String id, Object arg1, Object arg2)
		{
			return Localization.Instance.GetText(id, arg1, arg2);
		}

		public static String GetText(String id, Object arg1, Object arg2, Object arg3)
		{
			return Localization.Instance.GetText(id, arg1, arg2, arg3);
		}

		public static String GetText(String id, params Object[] args)
		{
			return Localization.Instance.GetText(id, args);
		}

		public static String GetText(String id)
		{
			return Localization.Instance.GetText(id);
		}

	    public static void SetText(String id, String text)
	    {
	        Localization.Instance.SetText(id, text);
	    }

        public static void AppendText(StringBuilder builder, String id, Object arg)
		{
			Localization.Instance.AppendText(builder, id, arg);
		}

		public static void AppendText(StringBuilder builder, String id, Object arg1, Object arg2)
		{
			Localization.Instance.AppendText(builder, id, arg1, arg2);
		}

		public static void AppendText(StringBuilder builder, String id, Object arg1, Object arg2, Object arg3)
		{
			Localization.Instance.AppendText(builder, id, arg1, arg2, arg3);
		}

		public static void AppendText(StringBuilder builder, String id, params Object[] args)
		{
			Localization.Instance.AppendText(builder, id, args);
		}

		public static void AppendText(StringBuilder builder, String id)
		{
			Localization.Instance.AppendText(builder, id);
		}

		public static void SetLanguage(String selectedLang)
		{
			Language = StringToLangID(selectedLang);
		}

		private void Awake()
		{
			if (s_Instance != null)
			{
				throw new Exception("Only one LocaManager allowed");
			}
			if (m_Configs == null || m_Configs.Length == 0)
			{
				throw new Exception("No language defination");
			}
			s_Instance = this;
			DontDestroyOnLoad(gameObject);
			HashSet<SystemLanguage> hashSet = new HashSet<SystemLanguage>();
			foreach (LanguageConfig languageConfig in m_Configs)
			{
				if (!hashSet.Add(languageConfig.m_Language))
				{
					throw new Exception("Double Language definition!");
				}
				if (languageConfig.m_LocaFiles == null || languageConfig.m_LocaFiles.Length == 0)
				{
					throw new Exception("LocaFiles not defined in " + languageConfig.m_Language);
				}
			}
			LoadLanguage();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOD_LOADED, new EventHandler(OnModLoaded));
		}

		private void Destroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOD_LOADED, new EventHandler(OnModLoaded));
		}

		private static SystemLanguage StringToLangID(String selectedLang)
		{
			switch (selectedLang)
			{
			case "fr":
				return SystemLanguage.French;
			case "de":
				return SystemLanguage.German;
			case "hu":
				return SystemLanguage.Hugarian;
			case "cz":
				return SystemLanguage.Czech;
			case "it":
				return SystemLanguage.Italian;
			case "ro":
				return SystemLanguage.Romanian;
			case "pl":
				return SystemLanguage.Polish;
			case "ru":
				return SystemLanguage.Russian;
			case "cn":
				return SystemLanguage.Chinese;
			case "es":
				return SystemLanguage.Spanish;
			case "br":
				return SystemLanguage.Portuguese;
			case "jp":
				return SystemLanguage.Japanese;
			case "kr":
				return SystemLanguage.Korean;
			}
			return SystemLanguage.English;
		}

		private void LoadLanguage()
		{
			LanguageConfig languageConfig = null;
			LanguageConfig languageConfig2 = null;
			String path = Path.Combine(Application.streamingAssetsPath, LOCA_FOLDER);
			foreach (LanguageConfig languageConfig3 in m_Configs)
			{
				if (languageConfig3.m_Language == SystemLanguage.English)
				{
					languageConfig = languageConfig3;
				}
				if (languageConfig3.m_Language == m_Language)
				{
					languageConfig2 = languageConfig3;
				}
			}
			if (languageConfig != null)
			{
				Localization.Instance.ClearFallBackContent();
				foreach (String path2 in languageConfig.m_LocaFiles)
				{
					String text = Path.Combine(path, path2);
					if (File.Exists(text))
					{
						Localization.Instance.LoadFallback(text);
					}
				}
			}
			if (languageConfig2 != null)
			{
				Localization.Instance.ClearContent();
				foreach (String path3 in languageConfig2.m_LocaFiles)
				{
					String text2 = Path.Combine(path, path3);
					if (File.Exists(text2))
					{
						Localization.Instance.LoadLanguage(text2);
					}
				}
			}
			LoadFonts();
			if (languageConfig2 == null)
			{
				throw new Exception("Language not defined! " + m_Language);
			}
			RaiseLocaChangedEvent();
		}

		private void LoadFonts()
		{
			foreach (LanguageFontMap languageFontMap in m_FontMaps)
			{
				foreach (FontMap fontMap2 in languageFontMap.m_FontMap)
				{
					if (fontMap2.m_Orignal != null)
					{
						fontMap2.m_Orignal.replacement = null;
					}
				}
			}
			LanguageFontMap languageFontMap2 = null;
			foreach (LanguageFontMap languageFontMap3 in m_FontMaps)
			{
				if (languageFontMap3.m_Language == m_Language)
				{
					languageFontMap2 = languageFontMap3;
					break;
				}
			}
			if (languageFontMap2 == null)
			{
				languageFontMap2 = m_FontMapDefault;
			}
			foreach (FontMap fontMap4 in languageFontMap2.m_FontMap)
			{
				if (fontMap4.m_Orignal != null)
				{
					fontMap4.m_Orignal.replacement = fontMap4.FindReplacement;
				}
			}
			Resources.UnloadUnusedAssets();
		}

		private void OnModLoaded(Object p_sender, EventArgs p_args)
		{
			LoadLanguage();
			ModController modController = (ModController)p_sender;
			if (modController.InModMode)
			{
				LanguageConfig languageConfig = null;
				LanguageConfig languageConfig2 = null;
				SystemLanguage systemLanguage = StringToLangID(modController.CurrentMod.DefaultLanguage);
				String locaFolder = modController.CurrentMod.LocaFolder;
				foreach (LanguageConfig languageConfig3 in m_Configs)
				{
					if (languageConfig3.m_Language == systemLanguage)
					{
						languageConfig = languageConfig3;
					}
					if (languageConfig3.m_Language == m_Language)
					{
						languageConfig2 = languageConfig3;
					}
				}
				if (languageConfig != null)
				{
					foreach (String path in languageConfig.m_LocaFiles)
					{
						String text = Path.Combine(locaFolder, path);
						if (File.Exists(text))
						{
							Localization.Instance.LoadFallback(text);
						}
					}
				}
				if (languageConfig2 != null)
				{
					foreach (String path2 in languageConfig2.m_LocaFiles)
					{
						String text2 = Path.Combine(locaFolder, path2);
						if (File.Exists(text2))
						{
							Localization.Instance.LoadLanguage(text2);
						}
					}
				}
				RaiseLocaChangedEvent();
			}
		}

		private static void RaiseLocaChangedEvent()
		{
			UIRoot.Broadcast("OnLocalize");
		}

		[Serializable]
		private class LanguageConfig
		{
			public SystemLanguage m_Language;

			public String[] m_LocaFiles;
		}

		[Serializable]
		private class LanguageFontMap
		{
			public SystemLanguage m_Language;

			public String m_FontFolder;

			public FontMap[] m_FontMap;
		}

		[Serializable]
		private class FontMap
		{
			public UIFont m_Orignal;

			[SerializeField]
			[LocalResourcePath]
			private String m_replacement;

			public UIFont FindReplacement => LocalResourcePathAttribute.LoadAsset<UIFont>(m_replacement);
		}
	}
}
