using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Legacy.Core.Utilities.Configuration
{
	public class ConfigReader
	{
		public const String DEFAULT_CONFIG_GROUP_NAME = "default";

		private Dictionary<String, ConfigGroup> m_groups = new Dictionary<String, ConfigGroup>(StringComparer.InvariantCultureIgnoreCase);

		public Dictionary<String, ConfigGroup> Groups => m_groups;

	    public ConfigGroup this[String groupName]
		{
			get
			{
				ConfigGroup result;
				if (!m_groups.TryGetValue(groupName, out result))
				{
					result = (m_groups[groupName] = new ConfigGroup(groupName));
				}
				return result;
			}
		}

		public void WriteData(String p_file)
		{
			if (p_file == null)
			{
				throw new ArgumentNullException("p_file");
			}
			try
			{
				using (FileStream fileStream = new FileStream(p_file, FileMode.Create, FileAccess.Write))
				{
					WriteData(fileStream);
				}
			}
			catch (IOException p_innerException)
			{
				throw new ConfigException(String.Format("Could not write file {0]", p_file), p_innerException);
			}
		}

		public void WriteData(Stream p_stream)
		{
			if (p_stream == null)
			{
				throw new ArgumentNullException("p_stream");
			}
			using (StreamWriter streamWriter = new StreamWriter(p_stream))
			{
				foreach (KeyValuePair<String, ConfigGroup> keyValuePair in m_groups)
				{
					streamWriter.WriteLine("[{0}]", keyValuePair.Key);
					foreach (KeyValuePair<String, ConfigSetting> keyValuePair2 in keyValuePair.Value.Settings)
					{
						streamWriter.WriteLine("{0} = {1}", keyValuePair2.Key, keyValuePair2.Value.Value);
					}
					streamWriter.WriteLine();
				}
			}
		}

		public void ReadData(String p_file)
		{
			if (p_file == null)
			{
				throw new ArgumentNullException("p_stream");
			}
			try
			{
				using (FileStream fileStream = new FileStream(p_file, FileMode.Open, FileAccess.Read))
				{
					ReadData(fileStream);
				}
			}
			catch (IOException p_innerException)
			{
				throw new ConfigException(String.Format("Could not read file {0}", p_file), p_innerException);
			}
		}

		public void ReadData(Stream p_stream)
		{
			if (p_stream == null)
			{
				throw new ArgumentNullException("p_stream");
			}
			m_groups.Clear();
			Int32 num = 0;
			ConfigGroup configGroup = null;
			using (StreamReader streamReader = new StreamReader(p_stream))
			{
				Regex regex = new Regex("\\[[a-zA-Z\\d\\s]+\\]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
				Char[] separator = new Char[]
				{
					'='
				};
				while (!streamReader.EndOfStream)
				{
					String text = streamReader.ReadLine();
					num++;
					if (text.Contains("#"))
					{
						if (text.IndexOf("#") == 0)
						{
							continue;
						}
						text = text.Substring(0, text.IndexOf("#"));
					}
					text = text.Trim();
					Match match = regex.Match(text);
					if (match.Success)
					{
						if (match.Value.Length == 2)
						{
							throw new ConfigException(String.Format("Group must have a name (line {0})", num));
						}
						if (configGroup != null)
						{
							m_groups.Add(configGroup.Name, configGroup);
						}
						String text2 = match.Value.Substring(1, match.Length - 2);
						if (!m_groups.TryGetValue(text2, out configGroup))
						{
							configGroup = new ConfigGroup(text2);
						}
					}
					else if (text.IndexOf('=') != -1)
					{
						if (configGroup == null)
						{
							configGroup = new ConfigGroup("default");
						}
						String[] array = text.Split(separator, 2);
						configGroup.AddSetting(new ConfigSetting(array[0].Trim(), array[1].Trim()));
					}
				}
				m_groups.Add(configGroup.Name, configGroup);
			}
		}

		public void UnionWith(ConfigReader other)
		{
			List<ConfigGroup> list = new List<ConfigGroup>();
			List<ConfigSetting> list2 = new List<ConfigSetting>();
			List<ConfigGroup> list3 = new List<ConfigGroup>();
			List<ConfigSetting> list4 = new List<ConfigSetting>();
			foreach (KeyValuePair<String, ConfigGroup> keyValuePair in other.m_groups)
			{
				ConfigGroup configGroup;
				if (!m_groups.TryGetValue(keyValuePair.Key, out configGroup))
				{
					list.Add(keyValuePair.Value);
				}
				else
				{
					foreach (KeyValuePair<String, ConfigSetting> keyValuePair2 in keyValuePair.Value.Settings)
					{
						ConfigSetting configSetting;
						if (!configGroup.Settings.TryGetValue(keyValuePair2.Key, out configSetting))
						{
							list2.Add(keyValuePair2.Value);
						}
					}
					foreach (ConfigSetting configSetting2 in list2)
					{
						configGroup.Settings.Add(configSetting2.Name, configSetting2);
					}
					list2.Clear();
					foreach (KeyValuePair<String, ConfigSetting> keyValuePair3 in configGroup.Settings)
					{
						ConfigSetting configSetting;
						if (!keyValuePair.Value.Settings.TryGetValue(keyValuePair3.Key, out configSetting))
						{
							list4.Add(keyValuePair3.Value);
						}
					}
					foreach (ConfigSetting configSetting3 in list4)
					{
						configGroup.Settings.Remove(configSetting3.Name);
					}
					list4.Clear();
				}
			}
			foreach (KeyValuePair<String, ConfigGroup> keyValuePair4 in m_groups)
			{
				ConfigGroup configGroup2;
				if (!other.m_groups.TryGetValue(keyValuePair4.Key, out configGroup2))
				{
					list3.Add(keyValuePair4.Value);
				}
			}
			foreach (ConfigGroup configGroup3 in list3)
			{
				m_groups.Remove(configGroup3.Name);
			}
			foreach (ConfigGroup configGroup4 in list)
			{
				m_groups.Add(configGroup4.Name, configGroup4);
			}
		}
	}
}
