using System;

namespace Legacy.Game.IngameManagement
{
	public interface IIngameContext
	{
		void Activate();

		void Deactivate();
	}
}
