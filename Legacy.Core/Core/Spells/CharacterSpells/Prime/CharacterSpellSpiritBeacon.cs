using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Prime
{
	public class CharacterSpellSpiritBeacon : CharacterSpell
	{
		public CharacterSpellSpiritBeacon() : base(ECharacterSpell.SPELL_PRIME_SPIRIT_BEACON)
		{
		}

		public override void FinishCastSpell(Character p_sorcerer)
		{
			base.FinishCastSpell(p_sorcerer);
			LegacyLogic.Instance.WorldManager.SpiritBeaconController.ExecuteAction();
		}

		public override Boolean CheckSpellConditions(Character p_sorcerer)
		{
			return !LegacyLogic.Instance.WorldManager.Party.HasAggro();
		}

		protected override void HandlePartyCharacters(Character p_sorcerer, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			base.HandlePartyCharacters(p_sorcerer, p_result, p_targets, p_magicFactor);
			foreach (Object obj in p_targets)
			{
				if (obj is Character)
				{
					p_result.SpellTargets.Add(new SpellTarget(obj));
				}
			}
		}
	}
}
