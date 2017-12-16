using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffRegrowth : MonsterBuff
	{
		public MonsterBuffRegrowth(Single p_casterMagicFactor) : base(20, p_casterMagicFactor)
		{
		}

		public override void DoEndOfTurnEffect(Monster p_monster)
		{
			if (p_monster.CurrentHealth == p_monster.MaxHealth)
			{
				return;
			}
			Int32 num = (Int32)GetBuffValue(0);
			p_monster.ChangeHP(num, m_causer);
			MonsterBuffDamageEntryEventArgs p_args = new MonsterBuffDamageEntryEventArgs(this, p_monster, new AttackResult
			{
				Result = EResultType.HEAL,
				DamageResults = 
				{
					new DamageResult(EDamageType.HEAL, num, 0, 1f)
				}
			});
			p_monster.BuffHandler.AddLogEntry(MonsterBuffHandler.ELogEntryPhase.ON_END_TURN, p_args);
		}
	}
}
