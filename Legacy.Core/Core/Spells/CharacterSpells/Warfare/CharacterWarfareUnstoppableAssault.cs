using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareUnstoppableAssault : CharacterSpell
	{
		public CharacterWarfareUnstoppableAssault() : base(ECharacterSpell.WARFARE_UNSTOPPABLE_ASSAULT)
		{
		}

		protected override List<AttackResult> MeleeAttackMonster(Character p_attacker, Monster p_target)
		{
			return p_attacker.FightHandler.ExecuteMeleeAttack(true, 0f, false, false, false);
		}
	}
}
