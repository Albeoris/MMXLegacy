using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffOscillation : MonsterBuff
	{
		private Boolean m_forVisualsOnly;

		public MonsterBuffOscillation(Single p_castersMagicFactor) : base(25, p_castersMagicFactor)
		{
		}

		public Boolean ForVisualsOnly
		{
			get => m_forVisualsOnly;
		    set => m_forVisualsOnly = value;
		}

		public override void DoEndOfTurnEffect(Monster p_monster)
		{
			if (m_duration > 0 || m_forVisualsOnly)
			{
				return;
			}
			Single num = p_monster.CurrentHealth / 4f;
			p_monster.ChangeHP(-p_monster.CurrentHealth, null);
			foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
			{
				Int32 num2 = (Int32)(num * ((100 - character.BaseResistance[EDamageType.PRIMORDIAL].Value) * 0.01f) + 0.5f);
				CombatEntryEventArgs p_args = new CombatEntryEventArgs(p_monster, character, new AttackResult
				{
					Result = EResultType.HIT,
					DamageResults = 
					{
						new DamageResult(EDamageType.PRIMORDIAL, num2, 0, 1f)
					}
				}, null);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
				character.ChangeHP(num2);
			}
		}
	}
}
