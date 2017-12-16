using System;
using System.Collections.Generic;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Light
{
	public class CharacterSpellRadiantWeapon : CharacterSpell
	{
		public CharacterSpellRadiantWeapon() : base(ECharacterSpell.SPELL_LIGHT_RADIANT_WEAPON)
		{
		}

		protected override void HandleMonsters(Character p_attacker, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			if (p_targets.Count > 0)
			{
				Int32 currentHealth = ((Monster)p_targets[0]).CurrentHealth;
				base.HandleMonsters(p_attacker, p_result, p_targets, p_magicFactor);
				if (((Monster)p_targets[0]).IsAttackable)
				{
					Int32 currentHealth2 = ((Monster)p_targets[0]).CurrentHealth;
					if (currentHealth2 < currentHealth)
					{
						((Monster)p_targets[0]).CurrentGeneralBlockAttempts = 0;
						((Monster)p_targets[0]).CurrentMeleeBlockAttempts = 0;
					}
				}
			}
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
			SetDescriptionValue(1, GetDamageAsString(1, p_magicFactor));
		}
	}
}
