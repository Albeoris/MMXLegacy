using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityHandler
	{
		private Monster m_owner;

		private List<MonsterAbilityBase> m_abilities;

		private Dictionary<EExecutionPhase, List<LogEntryEventArgs>> m_logEntries;

		private static List<Attack> m_tempA = new List<Attack>
		{
			null
		};

		private static List<AttackResult> m_tempAR = new List<AttackResult>
		{
			null
		};

		public MonsterAbilityHandler(Monster p_owner)
		{
			m_owner = p_owner;
			m_abilities = new List<MonsterAbilityBase>();
			m_logEntries = new Dictionary<EExecutionPhase, List<LogEntryEventArgs>>();
		}

		public Int32 Count => m_abilities.Count;

	    public List<MonsterAbilityBase> Abilities => m_abilities;

	    public List<MonsterAbilityBase> PassiveAbilities
		{
			get
			{
				List<MonsterAbilityBase> list = new List<MonsterAbilityBase>();
				if (m_abilities == null)
				{
					return list;
				}
				if (m_abilities.Count == 0)
				{
					return list;
				}
				foreach (MonsterAbilityBase monsterAbilityBase in m_abilities)
				{
					if (!monsterAbilityBase.StaticData.ActiveAbility)
					{
						list.Add(monsterAbilityBase);
					}
				}
				return list;
			}
		}

		public void Add(MonsterAbilityBase p_ability)
		{
			m_abilities.Add(p_ability);
		}

		public Boolean HasAbility(EMonsterAbilityType p_type)
		{
			foreach (MonsterAbilityBase monsterAbilityBase in m_abilities)
			{
				if (monsterAbilityBase.AbilityType == p_type)
				{
					return true;
				}
			}
			return false;
		}

		public void ExecuteAttack(Character p_character, Attack p_attack, Boolean p_isMagic, EExecutionPhase p_executionPhase)
		{
			if (p_attack == null)
			{
				ExecuteAttacks(p_character, null, p_isMagic, p_executionPhase);
			}
			else
			{
				m_tempA[0] = p_attack;
				ExecuteAttacks(p_character, m_tempA, p_isMagic, p_executionPhase);
				m_tempA[0] = null;
			}
		}

		public void ExecuteAttacks(Character p_character, List<Attack> p_attackList, Boolean p_isMagic, EExecutionPhase p_executionPhase)
		{
			for (Int32 i = 0; i < m_abilities.Count; i++)
			{
				MonsterAbilityBase monsterAbilityBase = m_abilities[i];
				if (monsterAbilityBase != null && (p_executionPhase & monsterAbilityBase.ExecutionPhase) == p_executionPhase && CheckTriggerChance(monsterAbilityBase))
				{
					monsterAbilityBase.CurrentExecutionPhase = p_executionPhase;
					monsterAbilityBase.HandleAttacks(p_attackList, m_owner, p_character, p_isMagic);
				}
			}
		}

		public void ExecuteAttackResult(Character p_character, AttackResult p_attackResult, Boolean p_isMagic, Boolean p_isRanged, EExecutionPhase p_executionPhase)
		{
			if (p_attackResult == null)
			{
				ExecuteAttackResults(p_character, null, p_isMagic, p_isRanged, p_executionPhase);
			}
			else
			{
				m_tempAR[0] = p_attackResult;
				ExecuteAttackResults(p_character, m_tempAR, p_isMagic, p_isRanged, p_executionPhase);
				m_tempAR[0] = null;
			}
		}

		public void ExecuteAttackResults(Character p_character, List<AttackResult> p_attackResultList, Boolean p_isMagic, Boolean p_isRanged, EExecutionPhase p_executionPhase)
		{
			for (Int32 i = 0; i < m_abilities.Count; i++)
			{
				MonsterAbilityBase monsterAbilityBase = m_abilities[i];
				if (monsterAbilityBase != null && (p_executionPhase & monsterAbilityBase.ExecutionPhase) == p_executionPhase && CheckTriggerChance(monsterAbilityBase))
				{
					monsterAbilityBase.CurrentExecutionPhase = p_executionPhase;
					monsterAbilityBase.HandleAttackResults(p_attackResultList, m_owner, p_character, p_isMagic, p_isRanged);
				}
			}
			if (p_executionPhase == EExecutionPhase.AFTER_MONSTER_ATTACK_INSTANT)
			{
				FlushActionLog(p_executionPhase);
			}
		}

		public Int32 CalculateResistanceIgnoreValue(AttackResult p_result)
		{
			MonsterAbilityBase ability = GetAbility(EMonsterAbilityType.PIERCING_STRIKES);
			if (ability == null)
			{
				return 0;
			}
			if (p_result.Result == EResultType.CRITICAL_HIT)
			{
				return (Int32)ability.StaticData.GetValues(ability.Level)[1];
			}
			if (p_result.Result == EResultType.HIT)
			{
				Single num = Random.Range(0f, 1f);
				if (num < ability.StaticData.GetValues(ability.Level)[0])
				{
					return (Int32)ability.StaticData.GetValues(ability.Level)[1];
				}
			}
			return 0;
		}

		private Boolean CheckTriggerChance(MonsterAbilityBase p_monsterAbility)
		{
			return Random.Range(0f, 1f) <= p_monsterAbility.TriggerChance;
		}

		internal MonsterAbilityBase GetAbility(EMonsterAbilityType p_type)
		{
			foreach (MonsterAbilityBase monsterAbilityBase in m_abilities)
			{
				if (monsterAbilityBase.AbilityType == p_type)
				{
					return monsterAbilityBase;
				}
			}
			return null;
		}

		internal Boolean CanAddBuff(EMonsterBuffType p_buffType)
		{
			MonsterBuff monsterBuff = BuffFactory.CreateMonsterBuff(p_buffType, 1f);
			foreach (MonsterAbilityBase monsterAbilityBase in m_abilities)
			{
				if (monsterAbilityBase.IsForbiddenBuff(monsterBuff.StaticID, m_owner, true))
				{
					return false;
				}
			}
			return true;
		}

		internal Boolean CanAddBuff(MonsterBuff p_buff)
		{
			foreach (MonsterAbilityBase monsterAbilityBase in m_abilities)
			{
				if (monsterAbilityBase.IsForbiddenBuff(p_buff.StaticID, m_owner, false))
				{
					return false;
				}
			}
			return true;
		}

		internal void ResetAbilityValues()
		{
			foreach (MonsterAbilityBase monsterAbilityBase in m_abilities)
			{
				monsterAbilityBase.ResetAbilityValues();
			}
		}

		public Boolean HasEntriesForPhase(EExecutionPhase p_phase)
		{
			return m_logEntries.ContainsKey(p_phase) && m_logEntries[p_phase].Count > 0;
		}

		public void FlushActionLog(EExecutionPhase p_phase)
		{
			if (!m_logEntries.ContainsKey(p_phase))
			{
				return;
			}
			List<LogEntryEventArgs> list = m_logEntries[p_phase];
			if (list.Count == 0)
			{
				return;
			}
			foreach (LogEntryEventArgs p_args in list)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			list.Clear();
			m_logEntries[p_phase] = list;
		}

		public void CancelAllEntries()
		{
			m_logEntries.Clear();
		}

		public void AddEntry(EExecutionPhase p_phase, LogEntryEventArgs p_args)
		{
			if (p_args == null)
			{
				return;
			}
			List<LogEntryEventArgs> list = new List<LogEntryEventArgs>();
			if (m_logEntries.ContainsKey(p_phase))
			{
				list = m_logEntries[p_phase];
			}
			list.Add(p_args);
			m_logEntries[p_phase] = list;
		}
	}
}
