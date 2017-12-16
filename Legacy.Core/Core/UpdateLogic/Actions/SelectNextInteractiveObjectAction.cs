using System;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class SelectNextInteractiveObjectAction : BaseAction
	{
		public SelectNextInteractiveObjectAction()
		{
			m_consumeType = EConsumeType.NONE;
		}

		public override void DoAction(Command p_command)
		{
			if (ActionAvailable())
			{
				Party.SelectNextInteractiveObject();
			}
		}

		public override Boolean IsActionDone()
		{
			return true;
		}

		public override Boolean ActionAvailable()
		{
			return Party.SelectedInteractiveObject != null;
		}
	}
}
