using System;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Game.IngameManagement;

namespace Legacy.Game.MMGUI
{
	public class LootContainerView : IIngameContext
	{
		private BilateralScreen m_bilateralScreen;

		public LootContainerView(BilateralScreen p_bilateralScreen)
		{
			m_bilateralScreen = p_bilateralScreen;
		}

		public void SetContainer(Container p_container)
		{
			m_bilateralScreen.LootScreen.ItemContainer.SetInventory(p_container);
		}

		public void Activate()
		{
			m_bilateralScreen.ToggleLoot();
		}

		public void Deactivate()
		{
		}
	}
}
