using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Lorebook
{
	public class BestiaryHandler : ISaveGameObject
	{
		public const Int32 MAXIMUM_KILLED_AMOUNT = 20;

		private Dictionary<Int32, Int32> m_allMonsters;

		private Dictionary<Int32, Int32> m_killedMonsters;

		private Dictionary<Int32, Int32> m_questMonsters;

		private List<Int32> m_newEntries;

		private Boolean m_initialized;

		private Int32 m_killedCoreMonsters;

		internal BestiaryHandler()
		{
			m_allMonsters = new Dictionary<Int32, Int32>();
			m_killedMonsters = new Dictionary<Int32, Int32>();
			m_questMonsters = new Dictionary<Int32, Int32>();
			m_newEntries = new List<Int32>();
			m_killedCoreMonsters = 0;
			m_initialized = false;
		}

		public Dictionary<Int32, Int32> AllMonsters => m_allMonsters;

	    public Dictionary<Int32, Int32> KilledMonsters => m_killedMonsters;

	    public List<Int32> NewEntries => m_newEntries;

	    public void Initialize()
		{
			if (m_initialized)
			{
				return;
			}
			m_initialized = true;
			if (m_allMonsters.Count < 1)
			{
				foreach (MonsterStaticData monsterStaticData in StaticDataHandler.GetIterator<MonsterStaticData>(EDataType.MONSTER))
				{
					m_allMonsters.Add(monsterStaticData.StaticID, 0);
				}
			}
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
		}

		public void Cleanup()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
			m_initialized = false;
		}

		private void OnMonsterDied(Object sender, EventArgs e)
		{
			Monster monster = sender as Monster;
			if (monster == null)
			{
				return;
			}
			if (monster.StaticData.BestiaryEntry)
			{
				AddKilledMonster(monster.StaticID, 1, false);
			}
			else
			{
				AddKilledQuestMonster(monster.StaticID, 1, false);
			}
		}

		public void AddKilledQuestMonster(Int32 p_key, Int32 p_val, Boolean p_fromLoad)
		{
			Int32 num = 0;
			if (!p_fromLoad)
			{
				if (m_questMonsters.ContainsKey(p_key))
				{
					num = m_questMonsters[p_key];
				}
				else
				{
					m_questMonsters.Add(p_key, num);
				}
				Int32 value = num + p_val;
				m_questMonsters[p_key] = value;
			}
			else
			{
				m_questMonsters.Add(p_key, p_val);
			}
		}

		public void AddKilledMonster(Int32 p_key, Int32 p_val, Boolean p_fromLoad)
		{
			Int32 num = 0;
			if (!p_fromLoad)
			{
				if (m_allMonsters.ContainsKey(p_key))
				{
					num = m_allMonsters[p_key];
				}
				else
				{
					m_allMonsters.Add(p_key, num);
				}
				Int32 num2 = num + p_val;
				MonsterStaticData staticData = StaticDataHandler.GetStaticData<MonsterStaticData>(EDataType.MONSTER, p_key);
				String nameKey = staticData.NameKey;
				if (num == 0)
				{
					BestiaryEntryEventArgs p_eventArgs = new BestiaryEntryEventArgs(nameKey, BestiaryEntryEventArgs.EEntryState.ENTRY_NEW);
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.BESTIARY_ENTRY_CHANGED, p_eventArgs);
					m_newEntries.Add(p_key);
					if (staticData.Grade == EMonsterGrade.CORE)
					{
						m_killedCoreMonsters++;
					}
					if (nameKey == "MONSTER_GIANT_SPIDER")
					{
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.STORY_2);
					}
				}
				else if (num2 == staticData.BestiaryThresholds[0] || num2 == staticData.BestiaryThresholds[1] || num2 == staticData.BestiaryThresholds[2])
				{
					BestiaryEntryEventArgs p_eventArgs2 = new BestiaryEntryEventArgs(nameKey, BestiaryEntryEventArgs.EEntryState.ENRTY_UPDATED);
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.BESTIARY_ENTRY_CHANGED, p_eventArgs2);
				}
				m_allMonsters[p_key] = num2;
				m_killedMonsters[p_key] = num2;
				CheckForQuestProgress();
			}
			else
			{
				m_allMonsters.Add(p_key, p_val);
				if (p_val > 0)
				{
					m_killedMonsters.Add(p_key, p_val);
				}
			}
		}

		private void CheckForQuestProgress()
		{
			TokenHandler tokenHandler = LegacyLogic.Instance.WorldManager.Party.TokenHandler;
			if (m_killedCoreMonsters >= 10 && tokenHandler.GetTokens(557) == 0)
			{
				tokenHandler.AddToken(557);
			}
			else if (m_killedCoreMonsters >= 20 && tokenHandler.GetTokens(558) == 0)
			{
				tokenHandler.AddToken(558);
			}
			else if (m_killedCoreMonsters >= 30 && tokenHandler.GetTokens(559) == 0)
			{
				tokenHandler.AddToken(559);
			}
			else if (m_killedCoreMonsters >= 40 && tokenHandler.GetTokens(560) == 0)
			{
				tokenHandler.AddToken(560);
			}
			else if (m_killedCoreMonsters >= 50 && tokenHandler.GetTokens(561) == 0)
			{
				tokenHandler.AddToken(561);
			}
		}

		public Int32 GetKilledMonstersOfType(EMonsterType p_type)
		{
			Int32 num = 0;
			foreach (Int32 num2 in m_questMonsters.Keys)
			{
				if (m_questMonsters[num2] != 0)
				{
					MonsterStaticData staticData = StaticDataHandler.GetStaticData<MonsterStaticData>(EDataType.MONSTER, num2);
					if (staticData.Type == p_type)
					{
						num += m_questMonsters[num2];
					}
				}
			}
			return num;
		}

		public Dictionary<Int32, Int32> GetMonstersForCategory(EMonsterClass p_class, Boolean p_showChampions)
		{
			Dictionary<Int32, Int32> dictionary = new Dictionary<Int32, Int32>();
			foreach (Int32 num in m_allMonsters.Keys)
			{
				MonsterStaticData staticData = StaticDataHandler.GetStaticData<MonsterStaticData>(EDataType.MONSTER, num);
				if (staticData != null && staticData.BestiaryEntry)
				{
					if (!p_showChampions)
					{
						if (staticData.Class != p_class && p_class != EMonsterClass.NONE)
						{
							continue;
						}
					}
					else if (staticData.Grade != EMonsterGrade.CHAMPION)
					{
						continue;
					}
					dictionary.Add(num, m_allMonsters[num]);
				}
			}
			return dictionary;
		}

		public void RemoveFromNewEntries(Int32 p_key)
		{
			if (m_newEntries.Contains(p_key))
			{
				m_newEntries.Remove(p_key);
			}
		}

		public void Load(SaveGameData p_data)
		{
			m_allMonsters.Clear();
			m_killedMonsters.Clear();
			m_questMonsters.Clear();
			m_newEntries.Clear();
			m_killedCoreMonsters = p_data.Get<Int32>("BestiaryKilledCoreMonsters", 0);
			Int32 num = p_data.Get<Int32>("BestiaryLength", 0);
			for (Int32 i = 0; i < num; i++)
			{
				Int32 p_key = p_data.Get<Int32>("BestiaryEntryMonsterKey" + i, 0);
				Int32 p_val = p_data.Get<Int32>("BestiaryEntryMonsterValue" + i, 0);
				AddKilledMonster(p_key, p_val, true);
			}
			Int32 num2 = p_data.Get<Int32>("BestiaryNewEntries", 0);
			for (Int32 j = 0; j < num2; j++)
			{
				m_newEntries.Add(p_data.Get<Int32>("BestiaryNewEntryKey" + j, 0));
			}
			Int32 num3 = p_data.Get<Int32>("BestiaryQuestMonsters", 0);
			for (Int32 k = 0; k < num3; k++)
			{
				Int32 p_key2 = p_data.Get<Int32>("BestiaryQuestMonsterKey" + k, 0);
				Int32 p_val2 = p_data.Get<Int32>("BestiaryQuestMonsterValue" + k, 0);
				AddKilledQuestMonster(p_key2, p_val2, true);
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("BestiaryLength", m_allMonsters.Count);
			p_data.Set<Int32>("BestiaryKilledCoreMonsters", m_killedCoreMonsters);
			Int32 num = 0;
			foreach (Int32 num2 in m_allMonsters.Keys)
			{
				p_data.Set<Int32>("BestiaryEntryMonsterKey" + num, num2);
				p_data.Set<Int32>("BestiaryEntryMonsterValue" + num, m_allMonsters[num2]);
				num++;
			}
			p_data.Set<Int32>("BestiaryNewEntries", m_newEntries.Count);
			for (Int32 i = 0; i < m_newEntries.Count; i++)
			{
				p_data.Set<Int32>("BestiaryNewEntryKey" + i, m_newEntries[i]);
			}
			p_data.Set<Int32>("BestiaryQuestMonsters", m_questMonsters.Count);
			Int32 num3 = 0;
			foreach (Int32 num4 in m_questMonsters.Keys)
			{
				p_data.Set<Int32>("BestiaryQuestMonsterKey" + num3, num4);
				p_data.Set<Int32>("BestiaryQuestMonsterValue" + num3, m_questMonsters[num4]);
				num3++;
			}
		}
	}
}
