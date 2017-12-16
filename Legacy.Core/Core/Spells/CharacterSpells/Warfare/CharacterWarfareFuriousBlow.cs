using System;
using System.Collections.Generic;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;
using Legacy.Utilities;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareFuriousBlow : CharacterSpell
	{
		public CharacterWarfareFuriousBlow() : base(ECharacterSpell.WARFARE_FURIOUS_BLOW)
		{
		}

		public override Boolean HasResources(Character p_sorcerer)
		{
			return p_sorcerer.ManaPoints >= 1;
		}

		public override void UseResources(Character p_sorcerer)
		{
			p_sorcerer.ChangeMP(-p_sorcerer.ManaPoints);
		}

		protected override List<AttackResult> MeleeAttackMonster(Character p_attacker, Monster p_target)
		{
			LegacyLogger.Log(p_attacker.ManaPoints * m_staticData.AdditionalValue * 100f + "%");
			return p_attacker.FightHandler.ExecuteMeleeAttack(false, p_attacker.ManaPoints * m_staticData.AdditionalValue, false, false, true);
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, m_staticData.AdditionalValue * 100f);
		}
	}
}
