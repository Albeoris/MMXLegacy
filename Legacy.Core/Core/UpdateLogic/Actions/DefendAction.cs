using System;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class DefendAction : BaseCharacterAction
	{
		public DefendAction(Int32 characterIndex) : base(characterIndex)
		{
			m_consumeType = EConsumeType.CONSUME_CHARACTER_TURN;
		}

		public override void DoAction(Command p_command)
		{
			Character.FightHandler.ExecuteDefend();
		}

		public override Boolean IsActionDone()
		{
			return true;
		}

		public override Boolean ActionAvailable()
		{
			return !Character.DoneTurn && !Character.ConditionHandler.CantDoAnything() && Party.HasAggro();
		}
	}
}
