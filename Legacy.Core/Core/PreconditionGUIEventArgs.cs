using System;
using Legacy.Core.UpdateLogic.Preconditions;

namespace Legacy.Core
{
	public class PreconditionGUIEventArgs : EventArgs
	{
		public PreconditionGUIEventArgs(BasePrecondition p_condition)
		{
			m_condition = p_condition;
		}

		public BasePrecondition m_condition { get; private set; }
	}
}
