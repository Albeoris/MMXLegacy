using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Skills;
using Legacy.Core.Internationalization;
using Legacy.Core.NpcInteraction.Conditions;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction
{
	public class DialogText
	{
		private NpcConversationStaticData.DialogText m_staticData;

		private EDialogState m_state;

		public DialogText(NpcConversationStaticData.DialogText p_textData)
		{
			m_staticData = p_textData;
		}

		public EDialogState State => m_state;

	    public String LocaKey => m_staticData.m_locaKey;

	    public String VoiceID => m_staticData.m_voiceID;

	    public EDialogReplacement Replacement => m_staticData.m_replacement;

	    internal void CheckCondition(Npc p_npc)
		{
			DialogCondition[] conditions = m_staticData.m_conditions;
			m_state = EDialogState.NORMAL;
			if (conditions != null)
			{
				for (Int32 i = 0; i < conditions.Length; i++)
				{
					EDialogState edialogState = conditions[i].CheckCondition(p_npc);
					if (edialogState > m_state)
					{
						m_state = edialogState;
						if (m_state == EDialogState.HIDDEN)
						{
							break;
						}
					}
				}
			}
		}

		public String ReplaceWithRace()
		{
			Character character = null;
			if (Replacement == EDialogReplacement.RACE_HUMAN)
			{
				character = GetCharByRace(ERace.HUMAN);
			}
			else if (Replacement == EDialogReplacement.RACE_DWARF)
			{
				character = GetCharByRace(ERace.DWARF);
			}
			else if (Replacement == EDialogReplacement.RACE_ELF)
			{
				character = GetCharByRace(ERace.ELF);
			}
			else if (Replacement == EDialogReplacement.RACE_ORC)
			{
				character = GetCharByRace(ERace.ORC);
			}
			if (character.Gender == EGender.FEMALE)
			{
				return Localization.Instance.GetText(LocaKey + "_F", character.Name);
			}
			return Localization.Instance.GetText(LocaKey, character.Name);
		}

		private Character GetCharByRace(ERace p_race)
		{
			List<Character> list = new List<Character>();
			Party party = LegacyLogic.Instance.WorldManager.Party;
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = party.GetMember(i);
				if (member.Class.Race == p_race)
				{
					list.Add(member);
				}
			}
			if (list.Count > 0)
			{
				return list[Random.Range(0, list.Count - 1)];
			}
			return party.GetMember(Random.Range(0, 3));
		}

		public String ReplaceWithClass()
		{
			Character character = null;
			if (Replacement == EDialogReplacement.CLASS_MERCENARY)
			{
				character = GetCharByClass(EClass.MERCENARY);
			}
			else if (Replacement == EDialogReplacement.CLASS_CRUSADER)
			{
				character = GetCharByClass(EClass.CRUSADER);
			}
			else if (Replacement == EDialogReplacement.CLASS_FREEMAGE)
			{
				character = GetCharByClass(EClass.FREEMAGE);
			}
			else if (Replacement == EDialogReplacement.CLASS_BLADEDANCER)
			{
				character = GetCharByClass(EClass.BLADEDANCER);
			}
			else if (Replacement == EDialogReplacement.CLASS_RANGER)
			{
				character = GetCharByClass(EClass.RANGER);
			}
			else if (Replacement == EDialogReplacement.CLASS_DRUID)
			{
				character = GetCharByClass(EClass.DRUID);
			}
			else if (Replacement == EDialogReplacement.CLASS_DEFENDER)
			{
				character = GetCharByClass(EClass.DEFENDER);
			}
			else if (Replacement == EDialogReplacement.CLASS_SCOUT)
			{
				character = GetCharByClass(EClass.SCOUT);
			}
			else if (Replacement == EDialogReplacement.CLASS_RUNEPRIEST)
			{
				character = GetCharByClass(EClass.RUNEPRIEST);
			}
			else if (Replacement == EDialogReplacement.CLASS_BARBARIAN)
			{
				character = GetCharByClass(EClass.BARBARIAN);
			}
			else if (Replacement == EDialogReplacement.CLASS_HUNTER)
			{
				character = GetCharByClass(EClass.HUNTER);
			}
			else if (Replacement == EDialogReplacement.CLASS_SHAMAN)
			{
				character = GetCharByClass(EClass.SHAMAN);
			}
			if (character.Gender == EGender.FEMALE)
			{
				return Localization.Instance.GetText(LocaKey + "_F", character.Name);
			}
			return Localization.Instance.GetText(LocaKey, character.Name);
		}

		private Character GetCharByClass(EClass p_class)
		{
			List<Character> list = new List<Character>();
			Party party = LegacyLogic.Instance.WorldManager.Party;
			for (Int32 i = 0; i < 4; i++)
			{
				Character member = party.GetMember(i);
				if (member.Class.Class == p_class)
				{
					list.Add(member);
				}
			}
			if (list.Count > 0)
			{
				return list[Random.Range(0, list.Count - 1)];
			}
			return party.GetMember(Random.Range(0, 3));
		}

		public String ReplaceWithAllChars()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			return Localization.Instance.GetText(LocaKey, new Object[]
			{
				party.GetMember(0).Name,
				party.GetMember(1).Name,
				party.GetMember(2).Name,
				party.GetMember(3).Name
			});
		}

		public String ReplacementWithFeature(NpcConversationStaticData.DialogFeature p_feature)
		{
			Int32 num = p_feature.m_price;
			if (num == -1)
			{
				if (p_feature.m_type == EDialogFeature.IDENTIFY)
				{
					num = ConfigManager.Instance.Game.CostIdentify;
				}
				else if (p_feature.m_type == EDialogFeature.REPAIR)
				{
					num = ConfigManager.Instance.Game.CostRepair;
				}
				else if (p_feature.m_type == EDialogFeature.RESTORE)
				{
					num = ConfigManager.Instance.Game.CostRestoration;
				}
				else if (p_feature.m_type == EDialogFeature.CAST)
				{
					num = ConfigManager.Instance.Game.CostCast;
				}
				else if (p_feature.m_type == EDialogFeature.RESURRECT)
				{
					num = ConfigManager.Instance.Game.CostResurrect;
				}
				else if (p_feature.m_type == EDialogFeature.CURE)
				{
					num = ConfigManager.Instance.Game.CostCure;
				}
				else if (p_feature.m_type == EDialogFeature.SUPPLIES)
				{
					num = ConfigManager.Instance.Game.CostSupplies;
				}
				else if (p_feature.m_type == EDialogFeature.REST)
				{
					num = ConfigManager.Instance.Game.CostRest;
				}
				else if (p_feature.m_type == EDialogFeature.TRAVEL)
				{
					num = ConfigManager.Instance.Game.CostTravel;
				}
				else if (p_feature.m_type == EDialogFeature.TRAINING)
				{
					if (p_feature.m_skillRank == ETier.EXPERT)
					{
						num = ConfigManager.Instance.Game.SkillExpertPrice;
					}
					else if (p_feature.m_skillRank == ETier.MASTER)
					{
						num = ConfigManager.Instance.Game.SkillMasterPrice;
					}
					else
					{
						num = ConfigManager.Instance.Game.SkillGrandmasterPrice;
					}
				}
			}
			Int32 num2 = 0;
			if (p_feature.m_type == EDialogFeature.IDENTIFY)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcItemIdentifyFactor);
			}
			else if (p_feature.m_type == EDialogFeature.REPAIR)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcItemRepairFactor);
			}
			else if (p_feature.m_type == EDialogFeature.RESTORE)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcRestoreFactor);
			}
			else if (p_feature.m_type == EDialogFeature.CAST)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcRestoreFactor);
			}
			else if (p_feature.m_type == EDialogFeature.RESURRECT)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcResurrectFactor);
			}
			else if (p_feature.m_type == EDialogFeature.CURE)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcCureFactor);
			}
			else if (p_feature.m_type == EDialogFeature.SUPPLIES)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcSuppliesFactor);
			}
			else if (p_feature.m_type == EDialogFeature.REST)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcRestFactor);
			}
			else if (p_feature.m_type == EDialogFeature.TRAINING)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcSkillTrainingFactor);
			}
			else if (p_feature.m_type == EDialogFeature.TRAVEL)
			{
				num2 = CalculatePriceForDifficulty(num, ConfigManager.Instance.Game.NpcTravelFactor);
			}
			else if (p_feature.m_type == EDialogFeature.BUY_TOKEN)
			{
				num2 = num;
			}
			return Localization.Instance.GetText(LocaKey, num2);
		}

		private Int32 CalculatePriceForDifficulty(Int32 p_price, Single p_factor)
		{
			if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
			{
				p_price = (Int32)Math.Ceiling(p_price * p_factor);
			}
			return p_price;
		}

		public String ReplacementWithPrice(Int32 p_price)
		{
			return Localization.Instance.GetText(LocaKey, p_price);
		}

		public String ReplacementWithHirelingAbsolute(Int32 p_npcID)
		{
			Npc npc = LegacyLogic.Instance.WorldManager.NpcFactory.Get(p_npcID);
			return Localization.Instance.GetText(LocaKey, npc.GetHirePrice());
		}

		public String ReplacementWithHirelingAbsolutePercent(Int32 p_npcID)
		{
			Npc npc = LegacyLogic.Instance.WorldManager.NpcFactory.Get(p_npcID);
			return Localization.Instance.GetText(LocaKey, npc.GetHirePrice(), npc.GetSharePricePercent());
		}

		public String ReplaceLinebreaksWithSpace()
		{
			String text = Localization.Instance.GetText(LocaKey);
			return text.Replace("\\n", " ");
		}
	}
}
