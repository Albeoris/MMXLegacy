using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityPiercingStrikes : MonsterAbilityBase
	{
		private Boolean m_isFirstStrike = true;

		public MonsterAbilityPiercingStrikes() : base(EMonsterAbilityType.PIERCING_STRIKES)
		{
			m_executionPhase = EExecutionPhase.AFTER_DAMAGE_CALCULATION;
		}

		public override void ResetAbilityValues()
		{
			m_isFirstStrike = true;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			for (Int32 i = 0; i < p_attackList.Count; i++)
			{
				if ((p_attackList[i].Result == EResultType.HIT || p_attackList[i].Result == EResultType.CRITICAL_HIT) && m_isFirstStrike)
				{
					AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
					p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
					m_isFirstStrike = false;
				}
			}
		}

		public override String GetDescription()
		{
			Int32 num = (Int32)Math.Round(m_staticData.GetValues(m_level)[0] * 100f, MidpointRounding.AwayFromZero);
			Int32 num2 = (Int32)Math.Round(m_staticData.GetValues(m_level)[1], MidpointRounding.AwayFromZero);
			String[] args = new String[]
			{
				"[00FF00]",
				"[80FF80]",
				"[FF0000]",
				"[FFC080]",
				"[FFFF80]",
				"[80FFFF]",
				"[-]",
				num.ToString(),
				num2.ToString(),
				String.Empty,
				String.Empty
			};
			return Localization.Instance.GetText(m_staticData.NameKey + "_INFO", args);
		}
	}
}
