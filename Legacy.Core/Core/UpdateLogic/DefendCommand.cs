using System;

namespace Legacy.Core.UpdateLogic
{
	public class DefendCommand : Command
	{
		private static DefendCommand m_instance;

		private DefendCommand() : base(ECommandTypes.DEFEND)
		{
		}

		public static DefendCommand Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new DefendCommand();
				}
				return m_instance;
			}
		}
	}
}
