using System;
using Legacy.Core.Entities;
using Legacy.Core.Map;

namespace Legacy.Core.EventManagement
{
	public class BaseObjectEventArgs : EventArgs
	{
		public BaseObjectEventArgs(BaseObject p_object, Position p_position) : this(p_object, p_position, String.Empty)
		{
		}

		public BaseObjectEventArgs(BaseObject p_object, Position p_position, String p_animation)
		{
			Object = p_object;
			Position = p_position;
			Animation = p_animation;
		}

		public BaseObject Object { get; private set; }

		public Position Position { get; private set; }

		public String Animation { get; private set; }
	}
}
