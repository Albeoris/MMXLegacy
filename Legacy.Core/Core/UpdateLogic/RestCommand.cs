using System;

namespace Legacy.Core.UpdateLogic
{
	public class RestCommand : Command
	{
		private static RestCommand m_instance;

		private RestCommand() : base(ECommandTypes.REST)
		{
		}

		public static RestCommand Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new RestCommand();
				}
				return m_instance;
			}
		}
	}
}
