using System;
using System.IO;

namespace Legacy.Core.Utilities.Configuration
{
	public class ConfigLoader
	{
		protected String m_rootPath;

		public ConfigLoader(String p_path)
		{
			m_rootPath = p_path;
		}

		public void LoadConfiguration(String p_fileName, IConfigDataContainer p_targetConfig)
		{
			String path = Path.Combine(m_rootPath, p_fileName);
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				p_targetConfig.Load(fileStream);
			}
		}

		public void WriteConfiguration(String p_fileName, IConfigDataContainer p_targetConfig)
		{
			String path = Path.Combine(m_rootPath, p_fileName);
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
			{
				p_targetConfig.Write(fileStream);
			}
		}
	}
}
