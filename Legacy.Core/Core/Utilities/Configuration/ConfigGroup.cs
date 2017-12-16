using System;
using System.Collections.Generic;

namespace Legacy.Core.Utilities.Configuration
{
	public class ConfigGroup
	{
		protected String m_name;

		protected Dictionary<String, ConfigSetting> m_settings = new Dictionary<String, ConfigSetting>(StringComparer.InvariantCultureIgnoreCase);

		internal ConfigGroup(String p_name)
		{
			m_name = p_name;
		}

		public String Name => m_name;

	    public Dictionary<String, ConfigSetting> Settings => m_settings;

	    public ConfigSetting this[String settingName]
		{
			get
			{
				ConfigSetting result;
				if (!m_settings.TryGetValue(settingName, out result))
				{
					result = (m_settings[settingName] = new ConfigSetting(settingName));
				}
				return result;
			}
		}

		internal void AddSetting(ConfigSetting p_setting)
		{
			m_settings.Add(p_setting.Name, p_setting);
		}
	}
}
