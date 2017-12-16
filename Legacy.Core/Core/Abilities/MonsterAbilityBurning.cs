using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityBurning : MonsterAbilityBase
	{
		public MonsterAbilityBurning() : base(EMonsterAbilityType.BURNING)
		{
			m_executionPhase = EExecutionPhase.BEFORE_MONSTER_ATTACK;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			if (p_attackList == null)
			{
				return;
			}
			AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
			p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
			for (Int32 i = 0; i < p_attackList.Count; i++)
			{
				List<Damage> damages = p_attackList[i].Damages;
				Int32 j = 0;
				Int32 count = damages.Count;
				while (j < count)
				{
					if (damages[j].Type == EDamageType.PHYSICAL)
					{
						Damage value = damages[j];
						value.Value = (Int32)(value.Value * 0.5f);
						damages[j] = value;
						Damage item = new Damage(EDamageType.FIRE, value.Value, value.CriticalBonusMod, 1f);
						damages.Add(item);
					}
					j++;
				}
			}
		}
	}
}
