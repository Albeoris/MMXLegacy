using System;

namespace Legacy.Core.EventManagement
{
	public class ChangeMapEventArgs : EventArgs
	{
		private String m_level;

		public ChangeMapEventArgs(String p_level)
		{
			m_level = p_level;
		}

		public String Level => m_level;
	}
}
