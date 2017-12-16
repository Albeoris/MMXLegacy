using System;

namespace Legacy.Core.EventManagement
{
	public class GameMessageEventArgs : EventArgs
	{
		private Boolean m_isTerrainMessage;

		public GameMessageEventArgs(String p_text) : this(p_text, 0f, true)
		{
		}

		public GameMessageEventArgs(String p_text, Single p_time)
		{
			text = p_text;
			time = p_time;
			isLocaKey = true;
		}

		public GameMessageEventArgs(String p_text, Single p_time, Boolean p_isLoca)
		{
			text = p_text;
			isLocaKey = p_isLoca;
			time = p_time;
		}

		public GameMessageEventArgs(String p_loca, String p_varLoca, Single p_time = 0f, Boolean p_isLoca = true)
		{
			text = p_loca;
			locaVar = p_varLoca;
			isLocaKey = p_isLoca;
			time = p_time;
		}

		public String text { get; private set; }

		public String locaVar { get; private set; }

		public Single time { get; private set; }

		public Boolean isLocaKey { get; private set; }

		public Boolean IsTerrainMessage
		{
			get => m_isTerrainMessage;
		    set => m_isTerrainMessage = value;
		}
	}
}
