using System;
using Legacy.Core.Utilities.Configuration;

namespace Legacy.Configuration
{
	public static class SoundConfigManager
	{
		public static readonly SoundSettings Settings = new SoundSettings();

		private static String m_path;

		private static String m_defaultsPath;

		public static void SetDefaultsPath(String p_path)
		{
			m_defaultsPath = p_path;
		}

		public static void LoadDefaultSettings()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_defaultsPath);
			Settings.Load(configReader);
			Settings.Apply();
		}

		public static void LoadConfigurations(String p_path)
		{
			m_path = p_path;
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(p_path);
			Settings.Load(configReader);
			Settings.Apply();
		}

		public static void ReloadConfigurations()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_path);
			Settings.Load(configReader);
			Settings.Apply();
		}

		public static void WriteConfigurations()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_path);
			Settings.Save(configReader);
			configReader.WriteData(m_path);
		}

		public static Boolean HasUnsavedChanges()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_path);
			return Settings.HasUnsavedChanges(configReader);
		}
	}
}
