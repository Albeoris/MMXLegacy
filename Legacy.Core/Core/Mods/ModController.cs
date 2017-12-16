using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Utilities;

namespace Legacy.Core.Mods
{
	public class ModController
	{
		private const String MODINFO_FILE = "modinfo.xml";

		private const String CONFIG_FILE = "config.txt";

		private const String ASSET_FOLDER = "Asset";

		private const String IMAGE_FOLDER = "Images";

		private const String DIALOG_FOLDER = "Dialog";

		private const String LOCA_FOLDER = "Localisation";

		private const String SAVEGAME_FOLDER = "Savegame";

		private const String STATICDATA_FOLDER = "Staticdata";

		private const String MAP_FOLDER = "Map";

		private ModInfo m_currentMod;

		public Boolean InModMode => m_currentMod != null;

	    public ModInfo CurrentMod => m_currentMod;

	    public List<ModInfo> GetModList()
		{
			String[] directories = Directory.GetDirectories(GamePaths.ModsRootPath);
			List<ModInfo> list = new List<ModInfo>(directories.Length);
			for (Int32 i = 0; i < directories.Length; i++)
			{
				String text = Path.Combine(directories[i], "modinfo.xml");
				try
				{
					ModInfo modInfo;
					if (Helper.Xml.OpenRead<ModInfo>(text, ModInfo.XmlRoot, out modInfo))
					{
						modInfo.RootPath = Path.Combine(GamePaths.ModsRootPath, directories[i]);
						list.Add(modInfo);
					}
					else
					{
						LegacyLogger.Log("ModLoader: ParseError " + text);
					}
				}
				catch (Exception ex)
				{
					LegacyLogger.Log(ex.ToString());
				}
			}
			return list;
		}

		public void LoadMod(ModInfo p_modinfo)
		{
			m_currentMod = p_modinfo;
			WorldManager.CurrentSaveGameFolder = m_currentMod.SavegameFolder;
			ConfigManager.Instance.LoadConfigurations(m_currentMod.RootPath);
			LegacyLogic.Instance.MapLoader.MapFolder = m_currentMod.MapFolder;
			LegacyLogic.Instance.SetConversationPath(m_currentMod.DialogFolder);
			LegacyLogic.Instance.WorldManager.SaveGameManager = new DefaultSaveGameManager();
			StaticDataHandler.Clear();
			LegacyLogic.Instance.LoadStaticData(m_currentMod.StaticdataFolder);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MOD_LOADED, EventArgs.Empty);
		}

		public void Reset(String defaultAssetPath)
		{
			m_currentMod = null;
			WorldManager.CurrentSaveGameFolder = GamePaths.UserGamePath;
			ConfigManager.Instance.LoadConfigurations(defaultAssetPath);
			LegacyLogic.Instance.MapLoader.MapFolder = Path.Combine(defaultAssetPath, "Maps");
			LegacyLogic.Instance.SetConversationPath(Path.Combine(defaultAssetPath, "Dialog"));
			LegacyLogic.Instance.ServiceWrapper.SetSaveGameManager();
			StaticDataHandler.Clear();
			LegacyLogic.Instance.LoadStaticData(Path.Combine(defaultAssetPath, "StaticData"));
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MOD_LOADED, EventArgs.Empty);
		}

		public class ModInfo
		{
			public static readonly XmlRootAttribute XmlRoot = new XmlRootAttribute("Mod");

			[XmlIgnore]
			public String RootPath;

			[XmlElement("Name")]
			public String Name;

			[XmlElement("Creators")]
			public String Creators;

			[XmlElement("Description")]
			public String Description;

			[XmlElement("Version")]
			public String Version;

			[XmlElement("DefaultLanguage")]
			public String DefaultLanguage;

			[XmlElement("TitleImage")]
			public String TitleImage;

			public String AssetFolder => Path.Combine(RootPath, "Asset");

		    public String DialogFolder => Path.Combine(RootPath, "Dialog");

		    public String LocaFolder => Path.Combine(RootPath, "Localisation");

		    public String SavegameFolder => Path.Combine(RootPath, "Savegame");

		    public String StaticdataFolder => Path.Combine(RootPath, "Staticdata");

		    public String ImageFolder => Path.Combine(AssetFolder, "Images");

		    public String MapFolder => Path.Combine(RootPath, "Map");
		}
	}
}
