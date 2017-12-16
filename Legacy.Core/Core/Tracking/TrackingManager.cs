using System;
using System.IO;
using System.Reflection;
using Legacy.Core.Achievements;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.Tracking
{
	public class TrackingManager
	{
		public void TrackGameStart(String location)
		{
			TrackingData trackingData = new TrackingData("GAME_START");
			FileInfo fileInfo = new FileInfo(Assembly.GetCallingAssembly().Location);
			FileInfo fileInfo2 = new FileInfo(Assembly.GetExecutingAssembly().Location);
			FileInfo fileInfo3 = new FileInfo("Might and Magic X Legacy.exe");
			FileInfo fileInfo4 = new FileInfo("LegacyRendezVous.dll");
			FileInfo fileInfo5 = new FileInfo("uplay_r1_loader.dll");
			trackingData.AddAttribute("ExeSize", fileInfo3.Length);
			trackingData.AddAttribute("RendezVousDLLSize", fileInfo4.Length);
			trackingData.AddAttribute("UPlayDLLSize", fileInfo5.Length);
			trackingData.AddAttribute("GameDLLSize", fileInfo.Length);
			trackingData.AddAttribute("CoreDLLSize", fileInfo2.Length);
			trackingData.AddAttribute("Language", ConfigManager.Instance.Options.Language);
			trackingData.AddAttribute("Version", "1.5-16336");
			trackingData.AddAttribute("Country", location);
			SendTrackingData(trackingData);
		}

		public void TrackLanguangeChange()
		{
			TrackingData trackingData = new TrackingData("LOC_CHANGE");
			trackingData.AddAttribute("Language", ConfigManager.Instance.Options.Language);
			SendTrackingData(trackingData);
		}

		public void TrackGameStop(Single p_timeSinceStart)
		{
			TrackingData trackingData = new TrackingData("GAME_STOP");
			trackingData.AddAttribute("Time", p_timeSinceStart);
			SendTrackingDataAndWait(trackingData);
		}

		public void TrackQuestStepCompleted(Int32 p_questStepId)
		{
			TrackingData trackingData = new TrackingData("QUEST_STEP_COMPLETED");
			trackingData.AddAttribute("QuestStepId", p_questStepId);
			SendTrackingData(trackingData);
		}

		public void TrackRelicEquipped(Equipment p_equipment, Character p_character)
		{
			TrackingData trackingData = new TrackingData("RELIC_EQUIPPED_FIRST_TIME");
			trackingData.AddAttribute("ItemType", (Int32)p_equipment.GetItemType());
			trackingData.AddAttribute("ItemId", p_equipment.StaticId);
			trackingData.AddAttribute("Class", p_character.Class.StaticData.StaticID);
			SendTrackingData(trackingData);
		}

		public void TrackAchievementCompleted(Achievement p_achievement)
		{
			TrackingData trackingData = new TrackingData("EARNED_ACHIEVMENT");
			Party party = LegacyLogic.Instance.WorldManager.Party;
			trackingData.AddAttribute("AchievementId", p_achievement.StaticID);
			trackingData.AddAttribute("Class1", party.GetMember(0).Class.StaticData.StaticID);
			trackingData.AddAttribute("Class2", party.GetMember(1).Class.StaticData.StaticID);
			trackingData.AddAttribute("Class3", party.GetMember(2).Class.StaticData.StaticID);
			trackingData.AddAttribute("Class4", party.GetMember(3).Class.StaticData.StaticID);
			SendTrackingData(trackingData);
		}

		public void TrackCharacterCreated(Character p_character, Int32[] p_selectedSkills, Attributes p_startAttributes)
		{
			TrackingData trackingData = new TrackingData("CHARACTER_CREATED");
			trackingData.AddAttribute("Class", p_character.Class.StaticData.StaticID);
			trackingData.AddAttribute("Skill1", p_selectedSkills[0]);
			trackingData.AddAttribute("Skill2", p_selectedSkills[1]);
			trackingData.AddAttribute("Skill3", p_selectedSkills[2]);
			trackingData.AddAttribute("Skill4", p_selectedSkills[3]);
			trackingData.AddAttribute("Might", p_startAttributes.Might);
			trackingData.AddAttribute("Magic", p_startAttributes.Magic);
			trackingData.AddAttribute("Perception", p_startAttributes.Perception);
			trackingData.AddAttribute("Destiny", p_startAttributes.Destiny);
			trackingData.AddAttribute("Vitality", p_startAttributes.Vitality);
			trackingData.AddAttribute("Spirit", p_startAttributes.Spirit);
			SendTrackingData(trackingData);
		}

		public void TrackSkillTrained(Skill p_skill, Character p_character)
		{
			TrackingData trackingData = new TrackingData("SKILL_TRAINED");
			trackingData.AddAttribute("SkillId", p_skill.StaticID);
			trackingData.AddAttribute("Rank", (Int32)p_skill.Tier);
			trackingData.AddAttribute("Class", p_character.Class.StaticData.StaticID);
			SendTrackingData(trackingData);
		}

		public void TrackSpellLearned(CharacterSpell p_spell, Character p_character)
		{
			TrackingData trackingData = new TrackingData("SPELL_LEARNED");
			trackingData.AddAttribute("SpellId", p_spell.StaticID);
			trackingData.AddAttribute("Class", p_character.Class.StaticData.StaticID);
			SendTrackingData(trackingData);
		}

		public void TrackAdvancedClassPromoted(Character p_character)
		{
			TrackingData trackingData = new TrackingData("ADVANCED_CLASS_PROMOTED");
			trackingData.AddAttribute("Class", p_character.Class.StaticData.StaticID);
			SendTrackingData(trackingData);
		}

		public void TrackLevelUp(Character p_character)
		{
			TrackingData trackingData = new TrackingData("LEVEL_UP");
			trackingData.AddAttribute("Class", p_character.Class.StaticData.StaticID);
			trackingData.AddAttribute("NewLevel", p_character.Level);
			SendTrackingData(trackingData);
		}

		public void TrackCharacterKilled(Character p_character, Monster p_monster)
		{
			TrackingData trackingData = new TrackingData("CHARACTER_KILLED_BY_MONSTER");
			trackingData.AddAttribute("Class", p_character.Class.StaticData.StaticID);
			trackingData.AddAttribute("Monster", p_monster.StaticID);
			SendTrackingData(trackingData);
		}

		public void TrackPartyDied()
		{
			TrackingData trackingData = new TrackingData("PARTY_DIED");
			Party party = LegacyLogic.Instance.WorldManager.Party;
			trackingData.AddAttribute("Class1", party.GetMember(0).Class.StaticData.StaticID);
			trackingData.AddAttribute("Class2", party.GetMember(1).Class.StaticData.StaticID);
			trackingData.AddAttribute("Class3", party.GetMember(2).Class.StaticData.StaticID);
			trackingData.AddAttribute("Class4", party.GetMember(3).Class.StaticData.StaticID);
			SendTrackingData(trackingData);
		}

		private void SendTrackingData(TrackingData p_data)
		{
			LegacyLogic.Instance.ServiceWrapper.SendTrackingData(p_data.TagName, p_data.GetAttributesAsString());
		}

		private void SendTrackingDataAndWait(TrackingData p_data)
		{
			LegacyLogic.Instance.ServiceWrapper.SendTrackingDataAndWait(p_data.TagName, p_data.GetAttributesAsString());
		}
	}
}
