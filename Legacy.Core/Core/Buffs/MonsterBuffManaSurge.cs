using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffManaSurge : MonsterBuff
	{
		public MonsterBuffManaSurge(Single p_castersMagicFactor) : base(3, p_castersMagicFactor)
		{
		}

		public override void DoOnCastSpellEffect(Monster p_monster)
		{
			Int32 p_damageValue = Random.Range((Int32)GetBuffValue(0), (Int32)GetBuffValue(1) + 1);
			Damage damage = new Damage(EDamageType.PRIMORDIAL, p_damageValue, 1f, 1f);
			DamageResult item = DamageResult.Create(damage, p_monster.StaticData.MagicResistances);
			AttackResult attackResult = new AttackResult();
			attackResult.Result = EResultType.HIT;
			attackResult.DamageResults.Add(item);
			m_delayedDamage = attackResult;
			p_monster.BuffHandler.FlagForRemoval(this);
			MonsterBuffDamageEntryEventArgs p_args = new MonsterBuffDamageEntryEventArgs(this, p_monster, attackResult);
			p_monster.BuffHandler.AddLogEntry(MonsterBuffHandler.ELogEntryPhase.ON_CAST_SPELL, p_args);
		}
	}
}
