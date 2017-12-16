using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.PartyManagement
{
	public class PartyBuffHandler : ISaveGameObject
	{
		private List<PartyBuff> m_buffs;

		private List<PartyBuff> m_buffsToRemove;

		private List<LogEntryEventArgs> m_logEntries;

		public PartyBuffHandler()
		{
			m_buffs = new List<PartyBuff>();
			m_buffsToRemove = new List<PartyBuff>();
			m_logEntries = new List<LogEntryEventArgs>();
		}

		public List<PartyBuff> Buffs => m_buffs;

	    public void AddBuff(EPartyBuffs p_buffId, Single p_valueFactor)
		{
			if (HasBuff(p_buffId))
			{
				PartyBuff buff = GetBuff(p_buffId);
				buff.ResetDuration(p_valueFactor);
				if (m_buffsToRemove.Contains(buff))
				{
					m_buffsToRemove.Remove(buff);
				}
				return;
			}
			PartyBuff partyBuff = new PartyBuff();
			partyBuff.Init(p_buffId, p_valueFactor);
			m_buffs.Add(partyBuff);
			LegacyLogic.Instance.EventManager.InvokeEvent(partyBuff, EEventType.PARTY_BUFF_ADDED, EventArgs.Empty);
			if (p_buffId != EPartyBuffs.DANGER_SENSE && p_buffId != EPartyBuffs.TORCHLIGHT && p_buffId != EPartyBuffs.LIGHT_ORB && p_buffId != EPartyBuffs.DARK_VISION && p_buffId != EPartyBuffs.CLAIRVOYANCE)
			{
				LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.PARTY_BUFF);
			}
		}

		public void RequestBuffCancel(PartyBuff p_buff)
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(p_buff, EEventType.PARTY_REQUEST_BUFF_CANCEL, EventArgs.Empty);
		}

		private void HandleCrystalBuffs(EPartyBuffs p_buffId)
		{
			if (p_buffId == EPartyBuffs.AIR_WARD)
			{
				RemoveBuff(EPartyBuffs.AIR_WARD_CRYSTAL);
			}
			if (p_buffId == EPartyBuffs.EARTH_WARD)
			{
				RemoveBuff(EPartyBuffs.EARTH_WARD_CRYSTAL);
			}
			if (p_buffId == EPartyBuffs.LIGHT_WARD)
			{
				RemoveBuff(EPartyBuffs.LIGHT_WARD_CRYSTAL);
			}
			if (p_buffId == EPartyBuffs.WATER_WARD)
			{
				RemoveBuff(EPartyBuffs.WATER_WARD_CRYSTAL);
			}
			if (p_buffId == EPartyBuffs.FIRE_WARD)
			{
				RemoveBuff(EPartyBuffs.FIRE_WARD_CRYSTAL);
			}
			if (p_buffId == EPartyBuffs.DARKNESS_WARD)
			{
				RemoveBuff(EPartyBuffs.DARKNESS_WARD_CRYSTAL);
			}
			if (p_buffId == EPartyBuffs.ARCANE_WARD)
			{
				RemoveBuff(EPartyBuffs.ARCANE_WARD_CRYSTAL);
			}
			if (p_buffId == EPartyBuffs.AIR_WARD_CRYSTAL)
			{
				RemoveBuff(EPartyBuffs.AIR_WARD);
			}
			if (p_buffId == EPartyBuffs.EARTH_WARD_CRYSTAL)
			{
				RemoveBuff(EPartyBuffs.EARTH_WARD);
			}
			if (p_buffId == EPartyBuffs.LIGHT_WARD_CRYSTAL)
			{
				RemoveBuff(EPartyBuffs.LIGHT_WARD);
			}
			if (p_buffId == EPartyBuffs.WATER_WARD_CRYSTAL)
			{
				RemoveBuff(EPartyBuffs.WATER_WARD);
			}
			if (p_buffId == EPartyBuffs.FIRE_WARD_CRYSTAL)
			{
				RemoveBuff(EPartyBuffs.FIRE_WARD);
			}
			if (p_buffId == EPartyBuffs.DARKNESS_WARD_CRYSTAL)
			{
				RemoveBuff(EPartyBuffs.DARKNESS_WARD);
			}
			if (p_buffId == EPartyBuffs.ARCANE_WARD_CRYSTAL)
			{
				RemoveBuff(EPartyBuffs.ARCANE_WARD);
			}
			if (p_buffId == EPartyBuffs.HEROIC_DESTINY)
			{
				RemoveBuff(EPartyBuffs.HEROIC_DESTINY_STATUE);
			}
			if (p_buffId == EPartyBuffs.HOUR_OF_POWER)
			{
				RemoveBuff(EPartyBuffs.HOUR_OF_POWER_STATUE);
			}
			if (p_buffId == EPartyBuffs.HEROIC_DESTINY_STATUE)
			{
				RemoveBuff(EPartyBuffs.HEROIC_DESTINY);
			}
			if (p_buffId == EPartyBuffs.HOUR_OF_POWER_STATUE)
			{
				RemoveBuff(EPartyBuffs.HOUR_OF_POWER);
			}
		}

		public void RemoveBuff(EPartyBuffs p_buffId)
		{
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				if (m_buffs[i].StaticData.StaticID == (Int32)p_buffId)
				{
					RemoveBuffByIndex(i);
					break;
				}
			}
			for (Int32 j = m_buffsToRemove.Count - 1; j >= 0; j--)
			{
				if (m_buffsToRemove[j].StaticData.StaticID == (Int32)p_buffId)
				{
					m_buffsToRemove.RemoveAt(j);
					break;
				}
			}
		}

		public Boolean HasBuff(EPartyBuffs p_buffId)
		{
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				if (m_buffs[i].StaticData.StaticID == (Int32)p_buffId)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean HasActiveBuff(EPartyBuffs p_buffId)
		{
			if (m_buffs.Count == 0)
			{
				return false;
			}
			PartyBuff buff = GetBuff(p_buffId);
			if (buff == null)
			{
				return false;
			}
			for (Int32 i = m_buffsToRemove.Count - 1; i >= 0; i--)
			{
				if (m_buffsToRemove[i].StaticData.StaticID == (Int32)p_buffId)
				{
					return false;
				}
			}
			m_buffsToRemove.Add(buff);
			return true;
		}

		public PartyBuff GetBuff(EPartyBuffs p_buffId)
		{
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				if (m_buffs[i].StaticData.StaticID == (Int32)p_buffId)
				{
					return m_buffs[i];
				}
			}
			return null;
		}

		private void RemoveBuffByIndex(Int32 p_index)
		{
			PartyBuff partyBuff = m_buffs[p_index];
			BuffRemovedEventArgs item = new BuffRemovedEventArgs(LegacyLogic.Instance.WorldManager.Party, partyBuff);
			m_logEntries.Add(item);
			m_buffs.RemoveAt(p_index);
			LegacyLogic.Instance.EventManager.InvokeEvent(partyBuff, EEventType.PARTY_BUFF_REMOVED, EventArgs.Empty);
		}

		public void RemoveAllBuffs()
		{
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				RemoveBuffByIndex(i);
			}
			m_buffsToRemove.Clear();
		}

		public void Update()
		{
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				if (!m_buffs[i].DurationIsMinutes)
				{
					m_buffs[i].UpdateExpiration();
				}
				if (m_buffs[i].IsExpired())
				{
					RemoveBuffByIndex(i);
				}
			}
		}

		public void ModifyAttack(Attack attack)
		{
			for (Int32 i = 0; i < m_buffs.Count; i++)
			{
				m_buffs[i].ModifyAttack(attack);
			}
		}

		public void AfterAttackResult(AttackResult p_attackResult)
		{
			for (Int32 i = 0; i < m_buffs.Count; i++)
			{
				m_buffs[i].AfterAttackResult(p_attackResult);
			}
		}

		public Int32 AddHealthPoints(Character p_char)
		{
			Int32 num = 0;
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				num += m_buffs[i].GetHealthPoints(p_char);
			}
			return num;
		}

		public void AddAttributes(ref Attributes p_attributes)
		{
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				m_buffs[i].AddAttributes(ref p_attributes);
			}
		}

		public void AddFightValues(FightValues p_fightValues)
		{
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				m_buffs[i].AddFightValues(p_fightValues);
			}
		}

		public void ModifyAttributes(ref Attributes p_attributes)
		{
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				m_buffs[i].ModifyAttributes(ref p_attributes);
			}
		}

		public void ModifyFightValues(FightValues p_fightValues)
		{
			for (Int32 i = m_buffs.Count - 1; i >= 0; i--)
			{
				m_buffs[i].ModifyFightValues(p_fightValues);
			}
		}

		public void FlushActionLog()
		{
			foreach (LogEntryEventArgs p_args in m_logEntries)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_logEntries.Clear();
		}

		public void AddExternalLogEntry(LogEntryEventArgs p_args)
		{
			m_logEntries.Add(p_args);
		}

		public void Destroy()
		{
			RemoveAllBuffs();
		}

		public void Load(SaveGameData p_data)
		{
			m_buffs.Clear();
			Int32 num = p_data.Get<Int32>("Count", 0);
			for (Int32 i = 0; i < num; i++)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("Buff" + i, null);
				if (saveGameData != null)
				{
					PartyBuff partyBuff = new PartyBuff();
					partyBuff.Load(saveGameData);
					m_buffs.Add(partyBuff);
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("Count", m_buffs.Count);
			for (Int32 i = 0; i < m_buffs.Count; i++)
			{
				SaveGameData saveGameData = new SaveGameData("Buff" + i);
				m_buffs[i].Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			}
		}
	}
}
