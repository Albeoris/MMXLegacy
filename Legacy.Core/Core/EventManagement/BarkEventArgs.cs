using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.EventManagement
{
	public class BarkEventArgs : EventArgs
	{
		private Character m_char;

		private String m_barkclip;

		private Int32 m_priority;

		private Boolean m_onrecieve;

		public BarkEventArgs(Character p_char, String p_barkclip)
		{
			m_char = p_char;
			m_barkclip = p_barkclip;
		}

		public BarkEventArgs(Character p_char, String p_barkclip, Int32 p_priority, Boolean p_onrecieve)
		{
			m_char = p_char;
			m_barkclip = p_barkclip;
			m_priority = p_priority;
			m_onrecieve = p_onrecieve;
		}

		public Character character => m_char;

	    public String barkclip => m_barkclip;

	    public Int32 priority => m_priority;

	    public Boolean onrecieve => m_onrecieve;
	}
}
