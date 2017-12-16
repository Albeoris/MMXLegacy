using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffCelestialArmour : MonsterBuff
	{
		private Int32 m_bufferLeft;

		public MonsterBuffCelestialArmour(Single p_castersMagicFactor) : base(27, p_castersMagicFactor)
		{
			m_bufferLeft = (Int32)(BuffValues[0] * m_castersMagicFactor + 0.5f);
		}

		public Int32 BufferLeft
		{
			get => m_bufferLeft;
		    set => m_bufferLeft = value;
		}

		public override void ManipulateAttack(Attack p_attack, Monster p_monster)
		{
			Int32 num = 0;
			Int32 num2 = BufferLeft;
			for (Int32 i = 0; i < p_attack.Damages.Count; i++)
			{
				Damage value = p_attack.Damages[i];
				Int32 num3 = Math.Min(value.Value, num2);
				num += num3;
				num2 -= num3;
				value.Value -= num3;
				if (value.Value < 0)
				{
					value.Value = 0;
				}
				MonsterDamagePreventedEntryEventArgs p_args = new MonsterDamagePreventedEntryEventArgs(this, num3);
				p_monster.BuffHandler.AddLogEntry(MonsterBuffHandler.ELogEntryPhase.ON_GET_DAMAGE, p_args);
				p_attack.Damages[i] = value;
				if (num2 <= 0)
				{
					break;
				}
			}
			BufferLeft = num2;
			if (BufferLeft <= 0)
			{
				ExpireBuff();
				p_monster.BuffHandler.RemoveBuff(this);
			}
		}
	}
}
