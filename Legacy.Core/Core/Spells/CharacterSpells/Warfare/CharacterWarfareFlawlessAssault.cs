using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareFlawlessAssault : CharacterSpell
	{
		public CharacterWarfareFlawlessAssault() : base(ECharacterSpell.WARFARE_FLAWLESS_ASSAULT)
		{
		}

		protected override List<AttackResult> MeleeAttackMonster(Character p_attacker, Monster p_target)
		{
			p_target.CombatHandler.CannotBlockThisTurn = true;
			return p_attacker.FightHandler.ExecuteMeleeAttack(true, 0f, true, false, false);
		}
	}
}
