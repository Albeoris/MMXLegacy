using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.Quests;

namespace Legacy.Core.ActionLogging
{
	public class QuestLootEntryEventArgs : LogEntryEventArgs
	{
		public QuestLootEntryEventArgs(QuestStep p_quest)
		{
			Quest = p_quest;
		}

		public Int32 Gold { get; set; }

		public QuestStep Quest { get; private set; }

		public BaseItem Item { get; set; }
	}
}
