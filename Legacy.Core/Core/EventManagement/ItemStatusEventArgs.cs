using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.EventManagement
{
	public class ItemStatusEventArgs : EventArgs
	{
		public ItemStatusEventArgs(BaseItem p_item, Character p_owner)
		{
			ItemOwner = p_owner;
			AffectedItem = p_item;
		}

		public Character ItemOwner { get; private set; }

		public BaseItem AffectedItem { get; private set; }
	}
}
