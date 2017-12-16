using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;
using Legacy.Game.IngameManagement;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/StandardActionBarView")]
	public class StandardActionBarView : MonoBehaviour
	{
		public void DrinkBestHealPotion()
		{
			DrinkPotion(EPotionType.HEALTH_POTION);
		}

		public void DrinkBestManaPotion()
		{
			DrinkPotion(EPotionType.MANA_POTION);
		}

		private void DrinkPotion(EPotionType p_type)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Potion bestPotion = party.GetBestPotion(p_type, null);
			InventorySlotRef consumableSlot = party.GetConsumableSlot(bestPotion);
			Int32 currentCharacter = LegacyLogic.Instance.WorldManager.Party.CurrentCharacter;
			if (consumableSlot.Slot != -1)
			{
				LegacyLogic.Instance.CommandManager.AddCommand(new ConsumeCommand(consumableSlot, currentCharacter));
			}
		}

		public void Rest()
		{
			LegacyLogic.Instance.CommandManager.AddCommand(RestCommand.Instance);
		}

		public void ToggleSpellBook()
		{
			IngameController.Instance.ToggleSpellbook(this, EventArgs.Empty);
		}
	}
}
