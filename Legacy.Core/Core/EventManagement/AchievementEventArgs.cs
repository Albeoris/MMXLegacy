using System;
using Legacy.Core.Achievements;

namespace Legacy.Core.EventManagement
{
	public class AchievementEventArgs : EventArgs
	{
		public AchievementEventArgs(Achievement p_achievement)
		{
			Achievement = p_achievement;
		}

		public Achievement Achievement { get; private set; }
	}
}
