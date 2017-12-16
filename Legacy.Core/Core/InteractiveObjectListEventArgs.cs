using System;
using System.Collections.Generic;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core
{
	internal class InteractiveObjectListEventArgs : EventArgs
	{
		public InteractiveObjectListEventArgs(List<InteractiveObject> p_list)
		{
			ObjectList = p_list;
		}

		public List<InteractiveObject> ObjectList { get; private set; }
	}
}
