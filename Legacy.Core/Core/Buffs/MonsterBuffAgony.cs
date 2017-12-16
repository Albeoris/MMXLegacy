using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffAgony : MonsterBuff
	{
		public MonsterBuffAgony(Single p_castersMagicFactor) : base(10, p_castersMagicFactor)
		{
		}

		public override void DoOnGetDamageEffect(Monster p_monster, List<DamageResult> p_results)
		{
			foreach (DamageResult damageResult in p_results)
			{
				if (damageResult.Type == EDamageType.PHYSICAL)
				{
					Int32 p_damageValue = Random.Range((Int32)GetBuffValue(0), (Int32)GetBuffValue(1));
					Damage damage = new Damage(EDamageType.DARK, p_damageValue, 1f, 1f);
					DamageResult item = DamageResult.Create(damage, p_monster.StaticData.MagicResistances);
					AttackResult attackResult = new AttackResult();
					attackResult.Result = EResultType.HIT;
					attackResult.DamageResults.Add(item);
					p_monster.ChangeHP(-attackResult.DamageDone, m_causer);
					MonsterBuffDamageEntryEventArgs p_args = new MonsterBuffDamageEntryEventArgs(this, p_monster, attackResult);
					p_monster.BuffHandler.AddLogEntry(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE, p_args);
					LegacyLogic.Instance.EventManager.InvokeEvent(p_monster, EEventType.MONSTER_BUFF_PERFORM, new BuffPerformedEventArgs(this, attackResult));
				}
			}
		}
	}
}
