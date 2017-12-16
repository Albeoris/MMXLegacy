using System;

namespace Legacy.Core.EventManagement
{
	public class ResourcesChangedEventArgs : EventArgs
	{
		public ResourcesChangedEventArgs(EResourceType p_resourceType)
		{
			ResourceType = p_resourceType;
		}

		public EResourceType ResourceType { get; private set; }

		public enum EResourceType
		{
			GOLD,
			SUPPLIES
		}
	}
}
