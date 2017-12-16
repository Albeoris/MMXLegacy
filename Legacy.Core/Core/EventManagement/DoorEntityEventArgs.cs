using System;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.EventManagement
{
	public class DoorEntityEventArgs : BaseObjectEventArgs
	{
		public DoorEntityEventArgs(Door p_object, Boolean p_animate, EInteractiveObjectState p_targetState) : base(p_object, p_object.Position)
		{
			Animate = p_animate;
			TargetState = p_targetState;
		}

		public Boolean Animate { get; private set; }

		public EInteractiveObjectState TargetState { get; private set; }
	}
}
