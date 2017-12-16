using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Prime
{
	public class CharacterSpellIdentify : CharacterSpell
	{
		public CharacterSpellIdentify() : base(ECharacterSpell.SPELL_PRIME_IDENTIFY)
		{
		}

		public override Boolean CheckSpellConditions(Character p_sorcerer)
		{
			return LegacyLogic.Instance.WorldManager.Party.Inventory.HasUnidentifiedItems() || LegacyLogic.Instance.WorldManager.Party.MuleInventory.HasUnidentifiedItems();
		}
	}
}
