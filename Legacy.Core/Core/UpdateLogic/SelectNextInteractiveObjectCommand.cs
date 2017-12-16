using System;

namespace Legacy.Core.UpdateLogic
{
	public class SelectNextInteractiveObjectCommand : Command
	{
		private static SelectNextInteractiveObjectCommand m_instance;

		private SelectNextInteractiveObjectCommand() : base(ECommandTypes.SELECT_NEXT_INTERACTIVE_OBJECT)
		{
		}

		public static SelectNextInteractiveObjectCommand Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new SelectNextInteractiveObjectCommand();
				}
				return m_instance;
			}
		}
	}
}
