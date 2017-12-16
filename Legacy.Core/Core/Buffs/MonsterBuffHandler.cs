using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffHandler : ISaveGameObject
	{
		private Monster m_owner;

		private List<MonsterBuff> m_buffList;

		private List<MonsterBuff> m_buffsToRemove;

		private Dictionary<ELogEntryPhase, List<LogEntryEventArgs>> m_logEntries;

		public MonsterBuffHandler(Monster p_owner)
		{
			m_owner = p_owner;
			m_buffList = new List<MonsterBuff>();
			m_buffsToRemove = new List<MonsterBuff>();
			m_logEntries = new Dictionary<ELogEntryPhase, List<LogEntryEventArgs>>();
		}

		public List<MonsterBuff> BuffList => m_buffList;

	    public Int32 BuffCount => m_buffList.Count;

	    public void AddBuff(MonsterBuff p_buff)
		{
			Boolean flag = true;
			if (m_owner.AbilityHandler.CanAddBuff(p_buff))
			{
				if (HasBuff(p_buff))
				{
					for (Int32 i = m_buffList.Count - 1; i >= 0; i--)
					{
						if (m_buffList[i].Type == p_buff.Type)
						{
							if (m_buffList[i].Stackable)
							{
								m_buffList[i].Stack(p_buff, m_owner);
								flag = false;
								LegacyLogic.Instance.EventManager.InvokeEvent(m_buffList[i], EEventType.MONSTER_BUFF_CHANGED, new MonsterBuffUpdateEventArgs(m_owner));
							}
							else
							{
								flag = false;
								m_buffList[i].ResetDuration();
								LegacyLogic.Instance.EventManager.InvokeEvent(m_buffList[i], EEventType.MONSTER_BUFF_CHANGED, new MonsterBuffUpdateEventArgs(m_owner));
							}
						}
					}
				}
				if (!flag)
				{
					return;
				}
				m_buffList.Add(p_buff);
				p_buff.DoImmediateEffect(m_owner);
				LegacyLogic.Instance.EventManager.InvokeEvent(p_buff, EEventType.MONSTER_BUFF_ADDED, new MonsterBuffUpdateEventArgs(m_owner));
			}
			else
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(p_buff, EEventType.MONSTER_BUFF_CANNOT_BE_APPLIED, new MonsterBuffUpdateEventArgs(m_owner));
			}
		}

		public void RemoveBuff(MonsterBuff p_buff)
		{
			if (m_buffList.Remove(p_buff))
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(p_buff, EEventType.MONSTER_BUFF_REMOVED, new MonsterBuffUpdateEventArgs(m_owner));
				if (p_buff is MonsterBuffHourOfJustice)
				{
					m_owner.MagicPowerModifier = 1f;
					m_owner.AttackValueModifier = 1f;
					m_owner.SpellHandler.UpdateMagicPower();
				}
			}
		}

		public void FlagForRemoval(MonsterBuff p_buff)
		{
			m_buffsToRemove.Add(p_buff);
		}

		public void RemoveAllBuffs()
		{
			for (Int32 i = BuffList.Count - 1; i >= 0; i--)
			{
				if (!BuffList[i].IsDebuff)
				{
					RemoveBuff(BuffList[i]);
				}
			}
		}

		public void RemoveFlaggedBuffs()
		{
			for (Int32 i = m_buffsToRemove.Count - 1; i >= 0; i--)
			{
				MonsterBuff monsterBuff = m_buffsToRemove[i];
				if (monsterBuff.DelayedDamage != null)
				{
					m_owner.ApplyDamages(monsterBuff.DelayedDamage, monsterBuff.Causer);
					LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.MONSTER_BUFF_PERFORM, new BuffPerformedEventArgs(monsterBuff, monsterBuff.DelayedDamage));
				}
				RemoveBuff(monsterBuff);
				m_buffsToRemove.RemoveAt(i);
			}
			m_buffsToRemove.Clear();
		}

		public void RemoveBuffByID(Int32 p_buffID)
		{
			for (Int32 i = 0; i < m_buffList.Count; i++)
			{
				if (m_buffList[i].StaticID == p_buffID)
				{
					RemoveBuff(m_buffList[i]);
				}
			}
		}

		public void RemoveRandomDebuff()
		{
			List<MonsterBuff> list = new List<MonsterBuff>();
			for (Int32 i = 0; i < m_buffList.Count; i++)
			{
				if (m_buffList[i].IsDebuff)
				{
					list.Add(m_buffList[i]);
				}
			}
			if (list.Count > 0)
			{
				Int32 index = Random.Range(0, list.Count);
				RemoveBuffByID(list[index].StaticID);
			}
		}

		public void UpdateHandler()
		{
			DoEndOfTurnEffects();
			if (m_buffList.Count > 0)
			{
				for (Int32 i = m_buffList.Count - 1; i >= 0; i--)
				{
					Boolean flag = false;
					if (m_buffList[i].IsDebuff)
					{
						m_buffList[i].DecreaseDuration();
					}
					if (m_buffList[i].IsExpired)
					{
						RemoveBuff(m_buffList[i]);
						flag = true;
					}
					if (!flag && !m_buffList[i].IsDebuff)
					{
						m_buffList[i].DecreaseDuration();
					}
				}
			}
			FlushActionLog(ELogEntryPhase.ON_END_TURN);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTER_BUFF_CHANGED, EventArgs.Empty);
		}

		public Boolean HasBuff(EMonsterBuffType p_buffType)
		{
			foreach (MonsterBuff monsterBuff in m_buffList)
			{
				if (monsterBuff.Type == p_buffType)
				{
					return true;
				}
			}
			return false;
		}

		public MonsterBuff GetBuff(EMonsterBuffType p_buffType)
		{
			foreach (MonsterBuff monsterBuff in m_buffList)
			{
				if (monsterBuff.Type == p_buffType)
				{
					return monsterBuff;
				}
			}
			return null;
		}

		public Boolean HasBuff(MonsterBuff p_buff)
		{
			return m_buffList.Contains(p_buff);
		}

		public void ModifyMonsterValues()
		{
			m_owner.ResetMonsterData();
			for (Int32 i = 0; i < m_buffList.Count; i++)
			{
				if (!m_buffList[i].IsExpired || !m_buffList[i].IsDebuff)
				{
					m_buffList[i].DoEffect(m_owner);
				}
			}
		}

		public void DoEndOfTurnEffects()
		{
			for (Int32 i = 0; i < m_buffList.Count; i++)
			{
				if (!m_buffList[i].IsExpired || !m_buffList[i].IsDebuff)
				{
					m_buffList[i].DoEndOfTurnEffect(m_owner);
				}
			}
		}

		public void DoOnCastSpellEffects()
		{
			for (Int32 i = 0; i < m_buffList.Count; i++)
			{
				if (!m_buffList[i].IsExpired || !m_buffList[i].IsDebuff)
				{
					m_buffList[i].DoOnCastSpellEffect(m_owner);
				}
			}
		}

		public void DoOnGetDamageEffects(List<DamageResult> p_results)
		{
			for (Int32 i = 0; i < m_buffList.Count; i++)
			{
				if (!m_buffList[i].IsExpired || !m_buffList[i].IsDebuff)
				{
					if (m_buffList[i].DontTriggerOnFirstDamage)
					{
						m_buffList[i].DontTriggerOnFirstDamage = false;
					}
					else
					{
						m_buffList[i].DoOnGetDamageEffect(m_owner, p_results);
						m_buffList[i].DoOnGetDamageEffect(m_owner);
					}
				}
			}
		}

		public void ManipulateDamage(Attack p_attack)
		{
			for (Int32 i = 0; i < m_buffList.Count; i++)
			{
				if (!m_buffList[i].IsExpired || !m_buffList[i].IsDebuff)
				{
					m_buffList[i].ManipulateAttack(p_attack, m_owner);
				}
			}
		}

		public void AddLogEntry(ELogEntryPhase p_phase, LogEntryEventArgs p_args)
		{
			List<LogEntryEventArgs> list = new List<LogEntryEventArgs>();
			if (m_logEntries.ContainsKey(p_phase))
			{
				list = m_logEntries[p_phase];
			}
			list.Add(p_args);
			m_logEntries[p_phase] = list;
		}

		public void FlushActionLog(ELogEntryPhase p_phase)
		{
			if (!m_logEntries.ContainsKey(p_phase))
			{
				return;
			}
			List<LogEntryEventArgs> list = m_logEntries[p_phase];
			foreach (LogEntryEventArgs p_args in list)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			list.Clear();
			m_logEntries[p_phase] = list;
		}

		public void Clear()
		{
			m_buffList.Clear();
			m_buffsToRemove.Clear();
			m_logEntries.Clear();
		}

		public void Load(SaveGameData p_data)
		{
			if (p_data != null)
			{
				Int32 num = p_data.Get<Int32>("Count", 0);
				for (Int32 i = 0; i < num; i++)
				{
					Int32 p_type = p_data.Get<Int32>("buffType_" + i, 1);
					Single p_castersMagicFactor = p_data.Get<Single>("castersMagicValue_" + i, 1f);
					m_buffList.Add(BuffFactory.CreateMonsterBuff((EMonsterBuffType)p_type, p_castersMagicFactor));
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			if (p_data != null)
			{
				p_data.Set<Int32>("Count", m_buffList.Count);
				for (Int32 i = 0; i < m_buffList.Count; i++)
				{
					p_data.Set<Int32>("buffType_" + i, (Int32)m_buffList[i].Type);
					p_data.Set<Single>("castersMagicValue_" + i, m_buffList[i].CastersMagicValue);
				}
			}
		}

		public enum ELogEntryPhase
		{
			ON_CAST_SPELL,
			ON_END_TURN,
			ON_GET_DAMAGE
		}
	}
}
