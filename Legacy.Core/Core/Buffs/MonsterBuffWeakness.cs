using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffWeakness : MonsterBuff
	{
		public MonsterBuffWeakness(Single p_castersMagicFactor) : base(9, p_castersMagicFactor)
		{
		}

		public override void DoEndOfTurnEffect(Monster p_monster)
		{
			Damage damage = new Damage(EDamageType.DARK, (Int32)GetBuffValue(1), 1f, 1f);
			DamageResult item = DamageResult.Create(damage, p_monster.StaticData.MagicResistances);
			AttackResult attackResult = new AttackResult();
			attackResult.Result = EResultType.HIT;
			attackResult.DamageResults.Add(item);
			p_monster.ApplyDamages(attackResult, m_causer);
			MonsterBuffDamageEntryEventArgs p_args = new MonsterBuffDamageEntryEventArgs(this, p_monster, attackResult);
			p_monster.BuffHandler.AddLogEntry(MonsterBuffHandler.ELogEntryPhase.ON_END_TURN, p_args);
			LegacyLogic.Instance.EventManager.InvokeEvent(p_monster, EEventType.MONSTER_BUFF_PERFORM, new BuffPerformedEventArgs(this, attackResult));
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.MeleeDamageModifier -= GetBuffValue(0);
			p_monster.CombatHandler.RangeDamageModifier -= GetBuffValue(0);
		}

		public override Single GetBuffValue(Int32 p_valueIndex)
		{
			if (BuffValues.Length <= p_valueIndex)
			{
				return 0f;
			}
			if (p_valueIndex == 0)
			{
				return BuffValues[p_valueIndex];
			}
			return (Single)Math.Round(BuffValues[p_valueIndex] * m_castersMagicFactor, MidpointRounding.AwayFromZero);
		}

		public override String GetBuffValueForTooltip(Int32 p_valueIndex)
		{
			if (p_valueIndex == 0)
			{
				return GetBuffValue(p_valueIndex) * 100f + " %";
			}
			return base.GetBuffValueForTooltip(p_valueIndex);
		}
	}
}
