using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityManaLeech : MonsterAbilityBase
	{
		public MonsterAbilityManaLeech() : base(EMonsterAbilityType.MANA_LEECH)
		{
			m_executionPhase = EExecutionPhase.AFTER_MONSTER_ATTACK_INSTANT;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			if (p_attackList == null)
			{
				return;
			}
			AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
			Boolean flag = false;
			Boolean flag2 = true;
			if (p_isMagic && p_monster.SpellHandler.LastCastedSpell.TargetType == ETargetType.PARTY)
			{
				for (Int32 i = 0; i < p_monster.SpellHandler.SpellTargetList.Count; i++)
				{
					p_monster.SpellHandler.SpellTargetList[i].ChangeMP(-(Int32)m_staticData.GetValues(m_level)[0]);
				}
				flag2 = false;
				flag = (p_monster.SpellHandler.SpellTargetList.Count > 0);
			}
			if (flag2)
			{
				for (Int32 j = 0; j < p_attackList.Count; j++)
				{
					if (p_attackList[j].Result == EResultType.HIT || p_attackList[j].Result == EResultType.CRITICAL_HIT)
					{
						p_character.ChangeMP(-(Int32)m_staticData.GetValues(m_level)[0]);
						flag = true;
					}
				}
			}
			if (flag)
			{
				p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
			}
		}
	}
}
