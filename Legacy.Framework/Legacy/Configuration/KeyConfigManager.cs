using System;
using Legacy.Core.Utilities.Configuration;

namespace Legacy.Configuration
{
	public static class KeyConfigManager
	{
		public static readonly KeyBindings KeyBindings = new KeyBindings();

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
			KeyBindings.Load(configReader);
		}

		public static void LoadConfigurations(String p_path)
		{
			m_path = p_path;
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(p_path);
			KeyBindings.Load(configReader);
		}

		public static void ReloadConfigurations()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_path);
			KeyBindings.Load(configReader);
		}

		public static void WriteConfigurations()
		{
			ConfigReader configReader = new ConfigReader();
			configReader.ReadData(m_path);
			KeyBindings.Save(configReader);
			configReader.WriteData(m_path);
		}

		public static Boolean HasUnsavedChanges()
		{
			return KeyBindings.HasUnsavedChanges();
		}
	}
}
