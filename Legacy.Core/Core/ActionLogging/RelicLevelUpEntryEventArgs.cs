using System;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.ActionLogging
{
	public class RelicLevelUpEntryEventArgs : LogEntryEventArgs
	{
		public RelicLevelUpEntryEventArgs(Equipment p_oldEquipment, Equipment p_newEquipment)
		{
			OldEquipment = p_oldEquipment;
			NewEquipment = p_newEquipment;
		}

		public Equipment OldEquipment { get; private set; }

		public Equipment NewEquipment { get; private set; }
	}
}
