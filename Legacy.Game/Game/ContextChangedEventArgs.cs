using System;

namespace Legacy.Game
{
	public class ContextChangedEventArgs : EventArgs
	{
		private EContext m_fromContext;

		private EContext m_toContext;

		public ContextChangedEventArgs(EContext p_fromContext, EContext p_toContext)
		{
			m_fromContext = p_fromContext;
			m_toContext = p_toContext;
		}

		public EContext FromContext => m_fromContext;

	    public EContext ToContext => m_toContext;
	}
}
