using System;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.UpdateLogic
{
	public class InteractCommand : Command
	{
		private InteractiveObject m_Target;

		public InteractCommand(InteractiveObject p_Target) : base(ECommandTypes.INTERACT)
		{
			m_Target = p_Target;
		}

		public InteractiveObject Target => m_Target;
	}
}
