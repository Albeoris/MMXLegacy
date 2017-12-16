using System;
using Legacy.Core.Utilities.Configuration;

namespace Legacy.Core.Configuration
{
	public class ConfigManager
	{
		private static ConfigManager m_instance;

		private ConfigLoader m_loader;

		private GameOptionsConfig m_optionsConfig;

		private GameConfig m_logicsConfig;

		private String m_optionsPath;

		private String m_optionDefaultsPath;

		private ConfigManager()
		{
			m_logicsConfig = new GameConfig();
			m_optionsConfig = new GameOptionsConfig();
		}

		public static ConfigManager Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new ConfigManager();
				}
				return m_instance;
			}
		}

		public GameConfig Game => m_logicsConfig;

	    public GameOptionsConfig Options => m_optionsConfig;

	    public Boolean IsLoaded { get; private set; }

		public void LoadConfigurations(String p_path)
		{
			IsLoaded = true;
			m_loader = new ConfigLoader(p_path);
			m_loader.LoadConfiguration("config.txt", Game);
		}

		public void SetOptionDefaultsPath(String p_path)
		{
			m_optionDefaultsPath = p_path;
		}

		public void SetOnlyChineseAvailable()
		{
			m_optionsConfig.SetOnlyChineseAvailable();
		}

		public void LoadDefaultOptions()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_optionDefaultsPath);
			m_optionsConfig.Load(configReader);
		}

		public void LoadOptions(String p_path)
		{
			m_optionsPath = p_path;
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(p_path);
			m_optionsConfig.Load(configReader);
		}

		public void ReloadGameOptions()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_optionsPath);
			m_optionsConfig.Load(configReader);
		}

		public void WriteConfigurations()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_optionsPath);
			m_optionsConfig.Write(configReader);
			configReader.WriteData(m_optionsPath);
		}

		public Boolean HasUnsavedChanges()
		{
			if (m_optionsPath != null)
			{
				ConfigReader configReader = new ConfigReader();
				configReader.ReadData(m_optionsPath);
				return m_optionsConfig.HasUnsavedChanges(configReader);
			}
			return false;
		}

		public Boolean HasLanguageChanged()
		{
			if (m_optionsPath != null)
			{
				ConfigReader configReader = new ConfigReader();
				configReader.ReadData(m_optionsPath);
				return m_optionsConfig.HasLanguageChanged(configReader);
			}
			return false;
		}
	}
}
