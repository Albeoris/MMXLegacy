using System;
using Legacy.Core.Map;

namespace Legacy.Core.EventManagement
{
	public class GridCreatedEventArgs : EventArgs
	{
		private Grid m_grid;

		public GridCreatedEventArgs(Grid p_grid)
		{
			m_grid = p_grid;
		}

		public Grid Grid => m_grid;
	}
}
