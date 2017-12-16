using System;

namespace Legacy.Core.UpdateLogic
{
	public class RangeAttackCommand : Command
	{
		private static RangeAttackCommand m_instance;

		private RangeAttackCommand() : base(ECommandTypes.RANGE_ATTACK)
		{
		}

		public static RangeAttackCommand Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new RangeAttackCommand();
				}
				return m_instance;
			}
		}
	}
}
