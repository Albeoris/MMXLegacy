using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Cheats
{
	[AddComponentMenu("MM Legacy/Cheats/CheatsMonsters")]
	public class CheatsMonsters : MonoBehaviour
	{
		[SerializeField]
		private UIPopupList m_monsterList;

		private List<Monster> monsters;

		private Dictionary<String, Monster> m_monsterDict;

		private void Awake()
		{
			monsters = new List<Monster>();
			m_monsterDict = new Dictionary<String, Monster>();
		}

		private void OnEnable()
		{
			if (m_monsterList != null)
			{
				m_monsterList.items.Clear();
				m_monsterDict.Clear();
				monsters.Clear();
				LegacyLogic.Instance.WorldManager.GetObjectsByType<Monster>(monsters);
				Party party = LegacyLogic.Instance.WorldManager.Party;
				monsters.Sort((Monster a, Monster b) => Position.Distance(a.Position, party.Position).CompareTo(Position.Distance(b.Position, party.Position)));
				foreach (Monster monster in monsters)
				{
					Single num = Position.Distance(monster.Position, party.Position);
					Int32 num2 = (Int32)(Math.Atan2(monster.Position.X - party.Position.X, monster.Position.Y - party.Position.Y) * 180.0 / 3.1415926535897931);
					num2 -= (Int32)party.Direction * 90;
					if (num2 < -180)
					{
						num2 += 360;
					}
					String text = LocaManager.GetText(monster.Name);
					String text2 = String.Concat(new Object[]
					{
						"dis:",
						Math.Round(num, 1),
						"  ",
						text,
						"  ",
						num2,
						"° ",
						monster.SpawnerID
					});
					m_monsterList.items.Add(text2);
					m_monsterDict.Add(text2, monster);
				}
				SelectDefaultMonsterListEntry();
			}
		}

		private void OnDisable()
		{
			monsters.Clear();
		}

		public void OnKillMonsterButtonClick()
		{
			if (m_monsterDict.ContainsKey(m_monsterList.selection))
			{
				m_monsterDict[m_monsterList.selection].Die();
				m_monsterDict[m_monsterList.selection].TriggerLateDieEffects();
				foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
				{
					character.FlushRewardsActionLog();
				}
				monsters.Remove(m_monsterDict[m_monsterList.selection]);
				m_monsterList.items.Remove(m_monsterList.selection);
				SelectDefaultMonsterListEntry();
			}
		}

		public void OnKillAllButtonClick()
		{
			foreach (Monster monster in monsters)
			{
				monster.Die();
				monster.TriggerLateDieEffects();
			}
			foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
			{
				character.FlushRewardsActionLog();
			}
			monsters.Clear();
			m_monsterList.items.Clear();
			SelectDefaultMonsterListEntry();
		}

		public void OnKillAllOthersButtonClick()
		{
			String selection = m_monsterList.selection;
			Monster monster = m_monsterDict[selection];
			foreach (Monster monster2 in monsters)
			{
				if (monster2 != monster)
				{
					monster2.Die();
					monster2.TriggerLateDieEffects();
				}
			}
			foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
			{
				character.FlushRewardsActionLog();
			}
			monsters.Clear();
			m_monsterList.items.Clear();
			monsters.Add(monster);
			m_monsterList.items.Add(selection);
			SelectDefaultMonsterListEntry();
		}

		public void OnUnlockBestiaryButtonClick()
		{
			IEnumerable<MonsterStaticData> iterator = StaticDataHandler.GetIterator<MonsterStaticData>(EDataType.MONSTER);
			foreach (MonsterStaticData monsterStaticData in iterator)
			{
				if (monsterStaticData.BestiaryEntry)
				{
					LegacyLogic.Instance.WorldManager.BestiaryHandler.AddKilledMonster(monsterStaticData.StaticID, 50, false);
				}
			}
		}

		private void SelectDefaultMonsterListEntry()
		{
			if (monsters.Count > 0)
			{
				m_monsterList.selection = m_monsterList.items[0];
			}
			else
			{
				m_monsterList.selection = String.Empty;
			}
		}
	}
}
