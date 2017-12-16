using System;
using Legacy.Core.Api;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Actions
{
	public abstract class BaseAction
	{
		protected EConsumeType m_consumeType;

		protected Party Party => LegacyLogic.Instance.WorldManager.Party;

	    protected Grid Grid => LegacyLogic.Instance.MapLoader.Grid;

	    public EConsumeType ConsumeType => m_consumeType;

	    public abstract void DoAction(Command p_command);

		public virtual void DontDoAction(Command p_command)
		{
		}

		public abstract Boolean IsActionDone();

		public abstract Boolean ActionAvailable();

		public virtual Boolean CanDoAction(Command p_command)
		{
			return true;
		}

		public virtual Boolean CanProgressBeforeActionIsDone()
		{
			return false;
		}

		public virtual Boolean CanBeDelayedByLock()
		{
			return true;
		}

		public virtual void Update()
		{
		}

		public virtual void Finish()
		{
		}

		public enum EConsumeType
		{
			NONE,
			CONSUME_CHARACTER_TURN,
			CONSUME_PARTY_TURN
		}
	}
}
