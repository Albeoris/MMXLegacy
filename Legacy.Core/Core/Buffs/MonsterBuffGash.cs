using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffGash : MonsterBuff
	{
		private Int32 m_damage;

		public MonsterBuffGash(Single p_damage) : base(17, p_damage)
		{
			m_damage = (Int32)Math.Round(GetBuffValue(0) * p_damage, MidpointRounding.AwayFromZero);
		}

		public override void DoEndOfTurnEffect(Monster p_monster)
		{
			Damage damage = new Damage(EDamageType.PHYSICAL, m_damage, 1f, 1f);
			DamageResult item = DamageResult.Create(damage, p_monster.StaticData.MagicResistances);
			AttackResult attackResult = new AttackResult();
			attackResult.Result = EResultType.HIT;
			attackResult.DamageResults.Add(item);
			p_monster.ApplyDamages(attackResult, m_causer);
			MonsterBuffDamageEntryEventArgs p_args = new MonsterBuffDamageEntryEventArgs(this, p_monster, attackResult);
			p_monster.BuffHandler.AddLogEntry(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE, p_args);
			LegacyLogic.Instance.EventManager.InvokeEvent(p_monster, EEventType.MONSTER_BUFF_PERFORM, new BuffPerformedEventArgs(this, attackResult));
		}

		public override Single GetBuffValue(Int32 p_valueIndex)
		{
			if (p_valueIndex < BuffValues.Length)
			{
				return BuffValues[p_valueIndex];
			}
			return 0f;
		}

		public override String GetBuffValueForTooltip(Int32 p_valueIndex)
		{
			return GetBuffValue(p_valueIndex) * 100f + " %";
		}
	}
}
