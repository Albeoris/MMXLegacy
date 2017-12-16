using System;

namespace Legacy.Core.UpdateLogic
{
	public class MeleeAttackCommand : Command
	{
		private static MeleeAttackCommand m_instance;

		private MeleeAttackCommand() : base(ECommandTypes.MELEE_ATTACK)
		{
		}

		public static MeleeAttackCommand Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new MeleeAttackCommand();
				}
				return m_instance;
			}
		}
	}
}
