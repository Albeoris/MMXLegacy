using System;

namespace Legacy.Core.SaveGameManagement
{
	public struct SaveGameMeta
	{
		private DateTime m_time;

		private TimeSpan m_playTime;

		private Boolean m_loaded;

		private Int32 m_saveNumber;

		private ESaveGameType m_type;

		private EDifficulty m_difficulty;

		public String Name;

		public Int32 SlotId;

		public SaveGameMeta(DateTime p_time, TimeSpan p_playTime, Int32 p_saveNumber, ESaveGameType p_type)
		{
			this = new SaveGameMeta(p_time, p_playTime, p_saveNumber, p_type, EDifficulty.NORMAL);
		}

		public SaveGameMeta(DateTime p_time, TimeSpan p_playTime, Int32 p_saveNumber, ESaveGameType p_type, EDifficulty p_difficulty)
		{
			Name = String.Empty;
			SlotId = 0;
			m_time = p_time;
			m_playTime = p_playTime;
			m_loaded = false;
			m_saveNumber = p_saveNumber;
			m_type = p_type;
			m_difficulty = p_difficulty;
		}

		public DateTime Time => m_time;

	    public TimeSpan PlayTime => m_playTime;

	    public Int32 SaveNumber => m_saveNumber;

	    public ESaveGameType Type => m_type;

	    public EDifficulty Difficulty
		{
			get => m_difficulty;
	        set => m_difficulty = value;
	    }

		public Boolean Loaded
		{
			get => m_loaded;
		    set => m_loaded = value;
		}
	}
}
