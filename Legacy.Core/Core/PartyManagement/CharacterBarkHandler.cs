using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.StaticData;

namespace Legacy.Core.PartyManagement
{
	public class CharacterBarkHandler
	{
		private BarksCharacterStaticData m_CharData;

		private BarksPartyStaticData m_PartyData;

		private EBarks m_BarkType;

		private Character m_Character;

		private PartyBarkData m_CurrentPartyBarkData;

		private PartyBarkData[] m_PartyBarkData = new PartyBarkData[20];

		private Int32 m_currentNextSlot;

		private ExplorationData m_CurrentExploreData;

		private ExplorationData[] m_Explorationdata = new ExplorationData[12];

		public CharacterBarkHandler()
		{
		}

		public CharacterBarkHandler(EBarks p_type)
		{
			m_CharData = StaticDataHandler.GetStaticData<BarksCharacterStaticData>(EDataType.BARKS_CHARACTER, (Int32)p_type);
			m_PartyData = StaticDataHandler.GetStaticData<BarksPartyStaticData>(EDataType.BARKS_PARTY, (Int32)p_type);
			m_BarkType = p_type;
		}

		public CharacterBarkHandler(Character character)
		{
			m_Character = character;
		}

		public void InitializeData()
		{
			m_CurrentPartyBarkData = new PartyBarkData();
			for (Int32 i = 0; i < m_PartyBarkData.Length; i++)
			{
				m_PartyBarkData[i] = new PartyBarkData();
			}
			m_CurrentExploreData = new ExplorationData();
			for (Int32 j = 0; j < m_Explorationdata.Length; j++)
			{
				m_Explorationdata[j] = new ExplorationData();
				switch (j)
				{
				case 0:
					m_Explorationdata[j].Region = ETerrainType.PASSABLE;
					m_Explorationdata[j].ID = 0;
					m_Explorationdata[j].MapType = EMapType.OUTDOOR;
					m_Explorationdata[j].Bark = EBarks.PLAINS_DAY;
					m_Explorationdata[j].OptionalBark = EBarks.PLAINS_NIGHT;
					break;
				case 1:
					m_Explorationdata[j].Region = ETerrainType.FOREST;
					m_Explorationdata[j].ID = 1;
					m_Explorationdata[j].MapType = EMapType.OUTDOOR;
					m_Explorationdata[j].Bark = EBarks.FOREST_DAY;
					m_Explorationdata[j].OptionalBark = EBarks.FOREST_NIGHT;
					break;
				case 2:
					m_Explorationdata[j].Region = ETerrainType.ROUGH;
					m_Explorationdata[j].ID = 2;
					m_Explorationdata[j].MapType = EMapType.OUTDOOR;
					m_Explorationdata[j].Bark = EBarks.MOUNTAINSIDE_DAY;
					m_Explorationdata[j].OptionalBark = EBarks.MOUNTAINSIDE_NIGHT;
					break;
				case 3:
					m_Explorationdata[j].MapType = EMapType.CITY;
					m_Explorationdata[j].ID = 3;
					m_Explorationdata[j].MapName = "Sorpigal";
					m_Explorationdata[j].Bark = EBarks.SORPIGAL_DAY;
					m_Explorationdata[j].OptionalBark = EBarks.SORPIGAL_NIGHT;
					break;
				case 4:
					m_Explorationdata[j].MapType = EMapType.CITY;
					m_Explorationdata[j].ID = 4;
					m_Explorationdata[j].MapName = "Seahaven";
					m_Explorationdata[j].Bark = EBarks.SEAHAVEN_DAY;
					m_Explorationdata[j].OptionalBark = EBarks.SEAHAVEN_NIGHT;
					break;
				case 5:
					m_Explorationdata[j].MapType = EMapType.CITY;
					m_Explorationdata[j].ID = 5;
					m_Explorationdata[j].MapName = "TheCrag";
					m_Explorationdata[j].Bark = EBarks.THE_CRAG_DAY;
					m_Explorationdata[j].OptionalBark = EBarks.THE_CRAG_NIGHT;
					break;
				case 6:
					m_Explorationdata[j].MapType = EMapType.CITY;
					m_Explorationdata[j].ID = 6;
					m_Explorationdata[j].MapName = "Karthal";
					m_Explorationdata[j].Bark = EBarks.KARTHAL_DAY;
					m_Explorationdata[j].OptionalBark = EBarks.KARTHAL_NIGHT;
					break;
				case 7:
					m_Explorationdata[j].MapType = EMapType.DUNGEON;
					m_Explorationdata[j].ID = 7;
					m_Explorationdata[j].DungeonStyle = EDungeonStyle.CASTLE;
					m_Explorationdata[j].Bark = EBarks.CASTLE_START;
					break;
				case 8:
					m_Explorationdata[j].MapType = EMapType.DUNGEON;
					m_Explorationdata[j].ID = 8;
					m_Explorationdata[j].DungeonStyle = EDungeonStyle.CAVES;
					m_Explorationdata[j].Bark = EBarks.CAVE_START;
					break;
				case 9:
					m_Explorationdata[j].MapType = EMapType.DUNGEON;
					m_Explorationdata[j].ID = 9;
					m_Explorationdata[j].DungeonStyle = EDungeonStyle.PALACE;
					m_Explorationdata[j].Bark = EBarks.KARTHAL_START;
					break;
				case 10:
					m_Explorationdata[j].MapType = EMapType.DUNGEON;
					m_Explorationdata[j].ID = 10;
					m_Explorationdata[j].DungeonStyle = EDungeonStyle.RUINS;
					m_Explorationdata[j].Bark = EBarks.SHANTIRI_START;
					break;
				case 11:
					m_Explorationdata[j].MapType = EMapType.DUNGEON;
					m_Explorationdata[j].ID = 11;
					m_Explorationdata[j].DungeonStyle = EDungeonStyle.TEMPLE;
					m_Explorationdata[j].Bark = EBarks.FACELESS_START;
					break;
				}
			}
		}

		public BarksCharacterStaticData BarkCharacterStaticData => m_CharData;

	    public BarksPartyStaticData BarkPartyStaticData => m_PartyData;

	    public EBarks BarkType => m_BarkType;

	    public BarkEventArgs GenerateBarkEventArgs(EBarks p_BarkType)
		{
			return GenerateBarkEventArgs(p_BarkType, m_Character);
		}

		public BarkEventArgs GenerateBarkEventArgs(EBarks p_BarkType, Character p_character)
		{
			if (!p_character.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS) && !p_character.ConditionHandler.HasCondition(ECondition.DEAD))
			{
				if (p_BarkType < EBarks.DEAD)
				{
					BarksPartyStaticData staticData = StaticDataHandler.GetStaticData<BarksPartyStaticData>(EDataType.BARKS_PARTY, (Int32)p_BarkType);
					return new BarkEventArgs(p_character, staticData.Clipname);
				}
				BarksCharacterStaticData staticData2 = StaticDataHandler.GetStaticData<BarksCharacterStaticData>(EDataType.BARKS_CHARACTER, (Int32)p_BarkType);
				Single value = Random.Value;
				if (value <= staticData2.Probability)
				{
					return new BarkEventArgs(p_character, staticData2.Clipname, staticData2.Priority, staticData2.OnRecieve);
				}
			}
			return null;
		}

		public void TriggerBark(EBarks p_BarkType)
		{
			TriggerBark(p_BarkType, m_Character);
		}

		public void TriggerBark(EBarks p_BarkType, Character p_character)
		{
			BarkEventArgs barkEventArgs = GenerateBarkEventArgs(p_BarkType, p_character);
			if (!p_character.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS) && !p_character.ConditionHandler.HasCondition(ECondition.DEAD) && barkEventArgs != null)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(p_character, EEventType.CHARACTER_BARK, barkEventArgs);
			}
		}

		public void UpdateCurrenExplorationData(GridSlot p_GridSlotData)
		{
			GetSlotRegionData(p_GridSlotData);
			GetDayPhase();
			GetMapType();
			GetDungeonStyle();
			GetID();
			GetExlorationBark();
			CheckCooldown();
		}

		private void GetSlotRegionData(GridSlot p_GridSlotData)
		{
			m_CurrentExploreData.Region = p_GridSlotData.TerrainType;
		}

		private void GetDayPhase()
		{
			m_CurrentExploreData.Dayphase = LegacyLogic.Instance.GameTime.DayState;
		}

		private void GetMapType()
		{
			m_CurrentExploreData.MapType = LegacyLogic.Instance.MapLoader.Grid.Type;
		}

		private void GetDungeonStyle()
		{
			m_CurrentExploreData.DungeonStyle = LegacyLogic.Instance.MapLoader.Grid.Style;
		}

		private void GetID()
		{
			Int32 num;
			if ((m_CurrentExploreData.Region & ETerrainType.PASSABLE) != ETerrainType.NONE)
			{
				num = 0;
			}
			else if ((m_CurrentExploreData.Region & ETerrainType.FOREST) != ETerrainType.NONE)
			{
				num = 1;
			}
			else
			{
				if ((m_CurrentExploreData.Region & ETerrainType.ROUGH) == ETerrainType.NONE)
				{
					return;
				}
				num = 2;
			}
			if (m_CurrentExploreData.MapType == EMapType.OUTDOOR)
			{
				m_CurrentExploreData.ID = m_Explorationdata[num].ID;
			}
			if (m_CurrentExploreData.MapType == EMapType.CITY)
			{
				GetCityName();
			}
			if (m_CurrentExploreData.MapType == EMapType.DUNGEON)
			{
				GetDungeonType();
			}
		}

		private void GetCityName()
		{
			m_CurrentExploreData.MapName = LegacyLogic.Instance.MapLoader.Grid.SceneName;
			String mapName = m_CurrentExploreData.MapName;
			switch (mapName)
			{
			case "Sorpigal":
				m_CurrentExploreData.ID = m_Explorationdata[3].ID;
				break;
			case "SeaHaven":
				m_CurrentExploreData.ID = m_Explorationdata[4].ID;
				break;
			case "TheCrag":
				m_CurrentExploreData.ID = m_Explorationdata[5].ID;
				break;
			case "KarthalCity":
				m_CurrentExploreData.ID = m_Explorationdata[6].ID;
				break;
			case "Karthal_Harbour":
				m_CurrentExploreData.ID = m_Explorationdata[6].ID;
				break;
			case "Karthal_Palace":
				m_CurrentExploreData.ID = m_Explorationdata[6].ID;
				break;
			case "Karthal_Slums":
				m_CurrentExploreData.ID = m_Explorationdata[6].ID;
				break;
			}
		}

		private void GetDungeonType()
		{
			switch (m_CurrentExploreData.DungeonStyle)
			{
			case EDungeonStyle.CASTLE:
				m_CurrentExploreData.ID = m_Explorationdata[7].ID;
				break;
			case EDungeonStyle.CAVES:
				m_CurrentExploreData.ID = m_Explorationdata[8].ID;
				break;
			case EDungeonStyle.RUINS:
				m_CurrentExploreData.ID = m_Explorationdata[10].ID;
				break;
			case EDungeonStyle.PALACE:
				m_CurrentExploreData.ID = m_Explorationdata[9].ID;
				break;
			case EDungeonStyle.TEMPLE:
				m_CurrentExploreData.ID = m_Explorationdata[11].ID;
				break;
			}
		}

		private void GetExlorationBark()
		{
			if (m_CurrentExploreData.MapType != EMapType.DUNGEON)
			{
				if (m_CurrentExploreData.Dayphase == EDayState.NIGHT)
				{
					m_CurrentExploreData.Bark = m_Explorationdata[m_CurrentExploreData.ID].OptionalBark;
				}
				else
				{
					m_CurrentExploreData.Bark = m_Explorationdata[m_CurrentExploreData.ID].Bark;
				}
			}
			else
			{
				m_CurrentExploreData.Bark = m_Explorationdata[m_CurrentExploreData.ID].Bark;
			}
		}

		private void CalculateChance()
		{
			UpdateChance();
			m_CurrentExploreData.Chance = m_Explorationdata[m_CurrentExploreData.ID].Chance;
			Single num = m_Explorationdata[m_CurrentExploreData.ID].Chance;
			Single value = Random.Value;
			if (m_CurrentExploreData.MapType == EMapType.DUNGEON)
			{
				String locationLocaName = LegacyLogic.Instance.MapLoader.Grid.LocationLocaName;
				if (locationLocaName != null && locationLocaName.LastIndexOf('@') != -1)
				{
					String a = locationLocaName.Substring(locationLocaName.LastIndexOf('@'));
					if (a != "1")
					{
						num = 0f;
					}
				}
			}
			if (!LegacyLogic.Instance.WorldManager.Party.HasAggro() && (m_CurrentExploreData.Region & ETerrainType.NO_PARTY_BARK) == ETerrainType.NONE && value < num)
			{
				ExplorationRandomPartyMember();
				m_Explorationdata[m_CurrentExploreData.ID].Chance = 0f;
			}
		}

		private void CheckPartyBarkCooldown()
		{
			Int32 totalMinutes = LegacyLogic.Instance.GameTime.Time.TotalMinutes;
			Boolean flag = false;
			for (Int32 i = 0; i < m_PartyBarkData.Length; i++)
			{
				if (m_PartyBarkData[i].Bark == m_CurrentPartyBarkData.Bark && m_PartyBarkData[i].CoolDown > totalMinutes)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				ChooseRandomPartyMember();
				UpdateCoolDownPartyBark();
			}
		}

		private void UpdateCoolDownPartyBark()
		{
			Int32 totalMinutes = LegacyLogic.Instance.GameTime.Time.TotalMinutes;
			m_PartyBarkData[m_currentNextSlot].Bark = m_CurrentPartyBarkData.Bark;
			m_PartyBarkData[m_currentNextSlot].CoolDown = totalMinutes + m_CurrentPartyBarkData.CoolDown;
			if (m_currentNextSlot == 19)
			{
				m_currentNextSlot = 0;
			}
			else
			{
				m_currentNextSlot++;
			}
		}

		private void CheckCooldown()
		{
			BarksPartyStaticData staticData = StaticDataHandler.GetStaticData<BarksPartyStaticData>(EDataType.BARKS_PARTY, (Int32)m_CurrentExploreData.Bark);
			if (staticData.DayPhase != "BOTH")
			{
				if (m_CurrentExploreData.Dayphase != EDayState.NIGHT)
				{
					m_CurrentExploreData.CoolDown = m_Explorationdata[m_CurrentExploreData.ID].CoolDown;
				}
				if (m_CurrentExploreData.Dayphase == EDayState.NIGHT)
				{
					m_CurrentExploreData.CoolDown = m_Explorationdata[m_CurrentExploreData.ID].CoolDownOptional;
				}
			}
			else
			{
				m_CurrentExploreData.CoolDown = m_Explorationdata[m_CurrentExploreData.ID].CoolDown;
			}
			if (m_CurrentExploreData.CoolDown < LegacyLogic.Instance.GameTime.Time.TotalMinutes)
			{
				CalculateChance();
			}
		}

		private void UpdateCooldown()
		{
			Int32 totalMinutes = LegacyLogic.Instance.GameTime.Time.TotalMinutes;
			BarksPartyStaticData staticData = StaticDataHandler.GetStaticData<BarksPartyStaticData>(EDataType.BARKS_PARTY, (Int32)m_CurrentExploreData.Bark);
			if (staticData.DayPhase != "BOTH")
			{
				if (m_CurrentExploreData.Dayphase != EDayState.NIGHT)
				{
					m_Explorationdata[m_CurrentExploreData.ID].CoolDown = totalMinutes + staticData.CoolDown;
				}
				if (m_CurrentExploreData.Dayphase == EDayState.NIGHT)
				{
					m_Explorationdata[m_CurrentExploreData.ID].CoolDownOptional = totalMinutes + staticData.CoolDown;
				}
			}
			else
			{
				m_Explorationdata[m_CurrentExploreData.ID].CoolDown = totalMinutes + staticData.CoolDown;
			}
		}

		private void UpdateChance()
		{
			BarksPartyStaticData staticData = StaticDataHandler.GetStaticData<BarksPartyStaticData>(EDataType.BARKS_PARTY, (Int32)m_CurrentExploreData.Bark);
			m_Explorationdata[m_CurrentExploreData.ID].Chance += staticData.Probability;
		}

		private void ExplorationRandomPartyMember()
		{
			Int32 p_idx = Random.Range(0, 4);
			Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(p_idx);
			TriggerExplorationBark(member);
		}

		private void ChooseRandomPartyMember()
		{
			Int32 p_idx = Random.Range(0, 4);
			Character member = LegacyLogic.Instance.WorldManager.Party.GetMember(p_idx);
			TriggerPartyBark(member);
		}

		public void RandomPartyMemberBark(EBarks p_Bark)
		{
			BarksPartyStaticData staticData = StaticDataHandler.GetStaticData<BarksPartyStaticData>(EDataType.BARKS_PARTY, (Int32)p_Bark);
			m_CurrentPartyBarkData.CoolDown = staticData.CoolDown;
			m_CurrentPartyBarkData.Bark = p_Bark;
			if (m_CurrentPartyBarkData.CoolDown == 0)
			{
				ChooseRandomPartyMember();
			}
			if (m_CurrentPartyBarkData.CoolDown > 0)
			{
				CheckPartyBarkCooldown();
			}
		}

		private void TriggerExplorationBark(Character p_Char)
		{
			if (!p_Char.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS) && !p_Char.ConditionHandler.HasCondition(ECondition.DEAD))
			{
				p_Char.BarkHandler.TriggerBark(m_CurrentExploreData.Bark, p_Char);
				m_Explorationdata[m_CurrentExploreData.ID].Chance = 0f;
				UpdateCooldown();
			}
			else
			{
				ExplorationRandomPartyMember();
			}
		}

		private void TriggerPartyBark(Character p_Char)
		{
			if (!p_Char.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS) && !p_Char.ConditionHandler.HasCondition(ECondition.DEAD))
			{
				TriggerBark(m_CurrentPartyBarkData.Bark, p_Char);
			}
			else
			{
				ChooseRandomPartyMember();
			}
		}

		private class PartyBarkData
		{
			public EBarks Bark;

			public Int32 CoolDown;
		}

		private class ExplorationData
		{
			public Int32 ID;

			public ETerrainType Region;

			public EMapType MapType;

			public String MapName;

			public EDungeonStyle DungeonStyle;

			public EDayState Dayphase;

			public Single Chance;

			public Int32 CoolDown;

			public Int32 CoolDownOptional;

			public EBarks Bark;

			public EBarks OptionalBark;
		}
	}
}
