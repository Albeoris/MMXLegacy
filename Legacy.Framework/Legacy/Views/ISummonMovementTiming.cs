using System;

namespace Legacy.Views
{
	public interface ISummonMovementTiming
	{
		void OnMoveEntity(Object p_sender, EventArgs p_args, OnSummonMoveFinishedCallback p_callback);
	}
}
