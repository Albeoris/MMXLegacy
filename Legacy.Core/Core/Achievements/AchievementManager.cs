using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;

namespace Legacy.Core.Achievements
{
	public class AchievementManager : ISaveGameObject
	{
		private CharacterSpell m_lastCastedSpell;

		private List<Achievement> m_achievements;

		private Int32 m_oldDays;

		private List<AttacksEventArgs.AttackedTarget> m_attackedTargets;

		private Int32 m_daysWithoutResting;

		private Boolean m_characterDied;

		private BaseItem m_lastGatheredItem;

		private SaveGameData m_globalData;

		private List<ECharacterSpell> m_castedSpellList;

		private List<RelicIdentity> m_relicIDs;

		private List<Boolean> m_actionsClaimed;

		public AchievementManager()
		{
			m_achievements = new List<Achievement>();
			m_attackedTargets = new List<AttacksEventArgs.AttackedTarget>();
			m_actionsClaimed = new List<Boolean>();
			for (Int32 i = 0; i < 4; i++)
			{
				m_actionsClaimed.Add(false);
			}
		}

		public Int32 DaysWithoutResting
		{
			get => m_daysWithoutResting;
		    set => m_daysWithoutResting = value;
		}

		public Boolean CharacterDied => m_characterDied;

	    public List<ECharacterSpell> CastedSpellList
		{
			get
			{
				if (m_castedSpellList == null)
				{
					m_castedSpellList = new List<ECharacterSpell>();
				}
				return m_castedSpellList;
			}
		}

		public List<AttacksEventArgs.AttackedTarget> AttackedTargets => m_attackedTargets;

	    public BaseItem LastGatheredItem => m_lastGatheredItem;

	    public List<RelicIdentity> RelicIDs
		{
			get
			{
				if (m_relicIDs == null)
				{
					m_relicIDs = new List<RelicIdentity>();
				}
				return m_relicIDs;
			}
		}

		public void Initialize()
		{
			Destroy();
			LoadAchievementData();
			LoadGlobal();
			EventManager eventManager = LegacyLogic.Instance.EventManager;
			eventManager.RegisterEvent(EEventType.QUESTLOG_CHANGED, new EventHandler(OnQuestLogChanged));
			eventManager.RegisterEvent(EEventType.INVENTORY_ITEM_ADDED, new EventHandler(OnAddedItem));
			eventManager.RegisterEvent(EEventType.CHARACTER_XP_GAIN, new EventHandler(OnCharacterXpGain));
			eventManager.RegisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnPartyResourcesChanged));
			eventManager.RegisterEvent(EEventType.GAMETIME_TIME_CHANGED, new EventHandler(OnGameTimeChanged));
			eventManager.RegisterEvent(EEventType.CHARACTER_LEARNED_SPELL, new EventHandler(OnLearnedSpell));
			eventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
			eventManager.RegisterEvent(EEventType.UNCOVERED_TILES, new EventHandler(OnUncoveredTile));
			eventManager.RegisterEvent(EEventType.CHARACTER_REVIVED, new EventHandler(OnRevivedCharacter));
			eventManager.RegisterEvent(EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			eventManager.RegisterEvent(EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacks));
			eventManager.RegisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
			eventManager.RegisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpell));
			eventManager.RegisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChanged));
			eventManager.RegisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
		}

		public void Destroy()
		{
			m_achievements.Clear();
			EventManager eventManager = LegacyLogic.Instance.EventManager;
			eventManager.UnregisterEvent(EEventType.QUESTLOG_CHANGED, new EventHandler(OnQuestLogChanged));
			eventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_ADDED, new EventHandler(OnAddedItem));
			eventManager.UnregisterEvent(EEventType.CHARACTER_XP_GAIN, new EventHandler(OnCharacterXpGain));
			eventManager.UnregisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnPartyResourcesChanged));
			eventManager.UnregisterEvent(EEventType.GAMETIME_TIME_CHANGED, new EventHandler(OnGameTimeChanged));
			eventManager.UnregisterEvent(EEventType.CHARACTER_LEARNED_SPELL, new EventHandler(OnLearnedSpell));
			eventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
			eventManager.UnregisterEvent(EEventType.UNCOVERED_TILES, new EventHandler(OnUncoveredTile));
			eventManager.UnregisterEvent(EEventType.CHARACTER_REVIVED, new EventHandler(OnRevivedCharacter));
			eventManager.UnregisterEvent(EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacks));
			eventManager.UnregisterEvent(EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			eventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
			eventManager.UnregisterEvent(EEventType.CHARACTER_CAST_SPELL, new EventHandler(OnCharacterCastSpell));
			eventManager.UnregisterEvent(EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChanged));
			eventManager.UnregisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
		}

		private void OnPartyRested(Object sender, EventArgs e)
		{
			m_daysWithoutResting = 0;
			CheckAchievements(ETriggerType.RESTED);
		}

		private void OnUncoveredTile(Object p_sender, EventArgs p_args)
		{
			CheckAchievements(ETriggerType.UNCOVERED_TILE);
		}

		private void OnQuestLogChanged(Object p_sender, EventArgs p_args)
		{
			QuestChangedEventArgs questChangedEventArgs = (QuestChangedEventArgs)p_args;
			if (questChangedEventArgs.ChangeType == QuestChangedEventArgs.Type.COMPLETED_QUEST)
			{
				CheckAchievements(ETriggerType.QUEST_STEP_COMPLETED);
			}
		}

		private void OnAddedItem(Object p_sender, EventArgs p_args)
		{
			InventoryItemEventArgs inventoryItemEventArgs = (InventoryItemEventArgs)p_args;
			if (inventoryItemEventArgs.Slot.Inventory.GetItemAt(inventoryItemEventArgs.Slot.Slot) is Equipment)
			{
				if (LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory == inventoryItemEventArgs.Slot.Inventory)
				{
					m_lastGatheredItem = inventoryItemEventArgs.Slot.Inventory.GetItemAt(inventoryItemEventArgs.Slot.Slot);
					CheckAchievements(ETriggerType.ITEM_ADDED_TO_INVENTORY);
				}
				Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
				for (Int32 i = 0; i < members.Length; i++)
				{
					if (members[i] != null && members[i].Equipment != null && members[i].Equipment.Equipment == inventoryItemEventArgs.Slot.Inventory)
					{
						CheckAchievements(ETriggerType.ITEM_EQUIPPED);
					}
				}
			}
			else
			{
				m_lastGatheredItem = inventoryItemEventArgs.Slot.Inventory.GetItemAt(inventoryItemEventArgs.Slot.Slot);
				CheckAchievements(ETriggerType.ITEM_ADDED_TO_INVENTORY);
			}
		}

		private void OnCharacterXpGain(Object p_sender, EventArgs p_args)
		{
			XpGainEventArgs xpGainEventArgs = (XpGainEventArgs)p_args;
			if (xpGainEventArgs.LevelUp > 0)
			{
				CheckAchievements(ETriggerType.CHARACTER_LEVEL_UP);
			}
		}

		private void OnPartyResourcesChanged(Object p_sender, EventArgs p_args)
		{
			CheckAchievements(ETriggerType.PARTY_GOLD_CHANGED);
		}

		private void OnGameTimeChanged(Object p_sender, EventArgs p_args)
		{
			MMTime time = LegacyLogic.Instance.GameTime.Time;
			if (time.Days > m_oldDays)
			{
				m_oldDays = time.Days;
				m_daysWithoutResting++;
				CheckAchievements(ETriggerType.NEW_DAY);
			}
		}

		public void OnLearnedSpell(Object p_sender, EventArgs p_args)
		{
			CheckAchievements(ETriggerType.LEARNED_SPELL);
		}

		private void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			if (p_sender is Party)
			{
				CheckAchievements(ETriggerType.PARTY_POSITION_CHANGED);
			}
		}

		private void OnRevivedCharacter(Object p_sender, EventArgs p_args)
		{
			CheckAchievements(ETriggerType.CHARACTER_REVIVED);
		}

		private void OnMonsterAttacks(Object p_sender, EventArgs p_args)
		{
			AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
			m_attackedTargets = attacksEventArgs.Attacks;
			CheckAchievements(ETriggerType.MONSTER_ATTACKED_CHARACTER);
		}

		private void OnMonsterDied(Object p_sender, EventArgs p_args)
		{
			CheckAchievements(ETriggerType.MONSTER_DIED);
		}

		private void OnCharacterCastSpell(Object p_sender, EventArgs p_args)
		{
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			if (spellEventArgs.Spell is CharacterSpell)
			{
				m_lastCastedSpell = (CharacterSpell)spellEventArgs.Spell;
				if (!CastedSpellList.Contains(m_lastCastedSpell.SpellType))
				{
					CastedSpellList.Add(m_lastCastedSpell.SpellType);
				}
				CheckAchievements(ETriggerType.CASTED_SPELL);
			}
		}

		private void OnCharacterStatusChanged(Object p_sender, EventArgs p_args)
		{
			if (((Character)p_sender).ConditionHandler.HasCondition(ECondition.DEAD))
			{
				m_characterDied = true;
				CheckAchievements(ETriggerType.CHARACTER_DIED);
			}
		}

		internal void LoadAchievementData()
		{
			foreach (AchievementStaticData achievementStaticData in StaticDataHandler.GetIterator<AchievementStaticData>(EDataType.ACHIEVEMENT))
			{
				Achievement item = new Achievement(this, achievementStaticData.StaticID);
				m_achievements.Add(item);
			}
		}

		private void CheckAchievements(ETriggerType p_type)
		{
			if (LegacyLogic.Instance.ModController.CurrentMod == null)
			{
				for (Int32 i = 0; i < m_achievements.Count; i++)
				{
					Achievement achievement = m_achievements[i];
					if (!achievement.Aquired && achievement.HasTrigger(p_type))
					{
						achievement.CheckCondition();
						if (achievement.Aquired)
						{
							LegacyLogic.Instance.TrackingManager.TrackAchievementCompleted(achievement);
							AchievementEventArgs p_eventArgs = new AchievementEventArgs(achievement);
							LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ACHIEVEMENT_AQUIRED, p_eventArgs);
						}
					}
				}
			}
		}

		public void ClaimAction(Int32 p_id)
		{
			m_actionsClaimed[p_id] = true;
			SaveGlobal();
		}

		public Boolean IsActionClaimed(Int32 p_id)
		{
			return m_actionsClaimed[p_id];
		}

		public void Load(SaveGameData p_data)
		{
			m_achievements.Clear();
			Int32 num = p_data.Get<Int32>("achievementCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("achievement" + i, null);
				if (saveGameData != null)
				{
					Achievement achievement = new Achievement(this, saveGameData.Get<Int32>("StaticID", 0));
					achievement.Load(saveGameData);
					m_achievements.Add(achievement);
				}
			}
			LoadGlobal();
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("achievementCount", m_achievements.Count);
			for (Int32 i = 0; i < m_achievements.Count; i++)
			{
				SaveGameData saveGameData = new SaveGameData("achievement" + i);
				m_achievements[i].Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			}
			SaveGlobal();
		}

		private void SaveGlobal()
		{
			m_globalData = new SaveGameData("globalAchievements");
			m_globalData.Set<Int32>("relicIDCount", RelicIDs.Count);
			for (Int32 i = 0; i < RelicIDs.Count; i++)
			{
				m_globalData.Set<EDataType>("relicData" + i, RelicIDs[i].dataType);
				m_globalData.Set<Int32>("relicId" + i, RelicIDs[i].staticId);
			}
			for (Int32 j = 0; j < m_achievements.Count; j++)
			{
				m_globalData.Set<Boolean>("aquired" + m_achievements[j].StaticID.ToString(), m_achievements[j].Aquired);
				if (m_achievements[j].IsGlobal)
				{
					m_globalData.Set<Int32>("count" + m_achievements[j].StaticID.ToString(), m_achievements[j].Count);
				}
			}
			for (Int32 k = 0; k < m_actionsClaimed.Count; k++)
			{
				m_globalData.Set<Boolean>("claimed" + (k + 1).ToString(), m_actionsClaimed[k]);
			}
			LegacyLogic.Instance.WorldManager.SaveGameManager.SaveSaveGameData(m_globalData, "global.lsg");
		}

		private void LoadGlobal()
		{
			RelicIDs.Clear();
			m_globalData = new SaveGameData("globalAchievements");
			LegacyLogic.Instance.WorldManager.SaveGameManager.LoadSaveGameData(m_globalData, "global.lsg");
			for (Int32 i = 0; i < m_achievements.Count; i++)
			{
				m_achievements[i].Aquired = m_globalData.Get<Boolean>("aquired" + m_achievements[i].StaticID, false);
				if (m_achievements[i].IsGlobal)
				{
					m_achievements[i].Count = m_globalData.Get<Int32>("count" + m_achievements[i].StaticID, 0);
				}
			}
			Int32 num = m_globalData.Get<Int32>("relicIDCount", 0);
			for (Int32 j = 0; j < num; j++)
			{
				RelicIdentity item = default(RelicIdentity);
				item.dataType = m_globalData.Get<EDataType>("relicData" + j, EDataType.ARMOR);
				item.staticId = m_globalData.Get<Int32>("relicId" + j, 1);
				RelicIDs.Add(item);
			}
			for (Int32 k = 0; k < m_actionsClaimed.Count; k++)
			{
				m_actionsClaimed[k] = m_globalData.Get<Boolean>("claimed" + (k + 1).ToString(), false);
			}
		}

		public Achievement GetAchievement(Int32 p_id)
		{
			for (Int32 i = 0; i < m_achievements.Count; i++)
			{
				if (m_achievements[i].StaticID == p_id)
				{
					return m_achievements[i];
				}
			}
			return null;
		}

		internal void ResetAchievementsForTests()
		{
			m_achievements.Clear();
			LoadAchievementData();
		}

		public void ForceCheck(ETriggerType p_type)
		{
			CheckAchievements(p_type);
		}

		public void ResetRoundData()
		{
			for (Int32 i = 0; i < m_achievements.Count; i++)
			{
				m_achievements[i].Condition.ResetRoundData();
			}
		}

		public struct RelicIdentity
		{
			public Int32 staticId;

			public EDataType dataType;
		}
	}
}
