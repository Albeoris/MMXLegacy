using System;

namespace Legacy.Core.UpdateLogic
{
	public abstract class Command
	{
		private ECommandTypes m_type;

		public Command(ECommandTypes p_type)
		{
			m_type = p_type;
		}

		public ECommandTypes Type => m_type;

	    public virtual void CancelCommand()
		{
		}

		public enum ECommandTypes
		{
			MOVE,
			ROTATE,
			INTERACT,
			MELEE_ATTACK,
			RANGE_ATTACK,
			CONSUME,
			REST,
			EQUIP,
			DEFEND,
			CAST_SPELL,
			SELECT_NEXT_INTERACTIVE_OBJECT,
			_MAX_
		}
	}
}
