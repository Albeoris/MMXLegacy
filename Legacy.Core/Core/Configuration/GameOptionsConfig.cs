using System;
using Legacy.Core.Utilities.Configuration;
using Legacy.Utilities;
using Microsoft.Win32;

namespace Legacy.Core.Configuration
{
	public class GameOptionsConfig
	{
		private const String REG_KEY = "SOFTWARE\\Ubisoft\\Might & Magic X Legacy";

		public String Language;

		public Boolean MonsterHPBarsVisible;

		public Boolean SubTitles;

		public Boolean ShowHints;

		public Boolean HideMinimapTooltips;

		public Boolean ShowAlternativeMonsterModel;

		public EVideoDecoder VideoDecoder;

		public Single EnemyOutlineOpacity;

		public Single ObjectOutlineOpacity;

		public Boolean ShowViewport;

		public Boolean LockActionBar;

		public Boolean FadeLogs;

		public Single FadeLogsDelay;

		public Boolean IsLeftActionBarShowingArrows;

		public Boolean TriggerBarks;

		public Boolean ShowGameMessages;

		public Single LogOpacity = 1f;

		public Single TooltipOpacity = 1f;

		public Boolean ShowFloatingDamageMonsters;

		public Boolean ShowFloatingDamageCharacters;

		public Boolean LockCharacterOrder;

		public Single QuestLogSize;

		public Int32 ActionLogSize;

		public Boolean RetroMode;

		public Int32 RetroScreenDivisor;

		public Single MonsterMovementSpeed = 1f;

		public Boolean ViewAlignedMinimap;

		public Single TooltipDelay = 0.2f;

		public Single MinimapGirdOpacity;

		public Boolean ShowMinimap;

		private Boolean m_onlyChineseAvailable;

		public void SetOnlyChineseAvailable()
		{
			m_onlyChineseAvailable = true;
		}

		public void Load(ConfigReader p_reader)
		{
			if (m_onlyChineseAvailable)
			{
				Language = "cn";
			}
			else
			{
				Language = "en";
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Ubisoft\\Might & Magic X Legacy", true))
				{
					if (registryKey != null)
					{
						Language = registryKey.GetValue("GameLanguageCustom", String.Empty).ToString();
						if (Language == String.Empty)
						{
							Language = registryKey.GetValue("GameLanguage", "en").ToString();
						}
					}
					else
					{
						LegacyLogger.Log("Cant open RegistryKey 'SOFTWARE\\Ubisoft\\Might & Magic X Legacy");
					}
				}
			}
			SubTitles = p_reader["language"]["subtitles"].GetBool(true);
			MonsterHPBarsVisible = p_reader["gameplay"]["monsterHPBarVisible"].GetBool(true);
			ShowHints = p_reader["gameplay"]["showHints"].GetBool(true);
			HideMinimapTooltips = p_reader["gameplay"]["hideMinimapTooltips"].GetBool(false);
			ShowAlternativeMonsterModel = p_reader["gameplay"]["showAlternativeMonsterModel"].GetBool(false);
			EnemyOutlineOpacity = p_reader["gameplay"]["enemyOutlineOpacity"].GetFloat(1f);
			ObjectOutlineOpacity = p_reader["gameplay"]["objectOutlineOpacity"].GetFloat(1f);
			ShowViewport = p_reader["gameplay"]["showViewport"].GetBool(true);
			LockActionBar = p_reader["gameplay"]["lockActionBar"].GetBool(false);
			MonsterMovementSpeed = p_reader["gameplay"]["monsterMovementSpeed"].GetFloat(1f);
			FadeLogs = p_reader["gameplay"]["fadeLogs"].GetBool(false);
			FadeLogsDelay = p_reader["gameplay"]["fadeLogsDelay"].GetFloat(5f);
			IsLeftActionBarShowingArrows = p_reader["gameplay"]["isLeftActionBarWithArrows"].GetBool(false);
			TriggerBarks = p_reader["gameplay"]["triggerBarks"].GetBool(true);
			ShowGameMessages = p_reader["gameplay"]["showMessages"].GetBool(true);
			LogOpacity = p_reader["gameplay"]["logOpacity"].GetFloat(1f);
			TooltipOpacity = p_reader["gameplay"]["tooltipOpacity"].GetFloat(1f);
			ShowFloatingDamageMonsters = p_reader["gameplay"]["showFloatingDamageMonsters"].GetBool(true);
			ShowFloatingDamageCharacters = p_reader["gameplay"]["showFloatingDamageChars"].GetBool(true);
			QuestLogSize = p_reader["gameplay"]["questLogSize"].GetFloat(220f);
			ActionLogSize = p_reader["gameplay"]["actionLogSize"].GetInt(8);
			LockCharacterOrder = p_reader["gameplay"]["lockCharacterOrder"].GetBool(false);
			TooltipDelay = p_reader["gameplay"]["tooltipDelay"].GetFloat(0.2f);
			VideoDecoder = p_reader["general"]["videodecoder"].GetEnum<EVideoDecoder>(EVideoDecoder.System);
			RetroMode = p_reader["general"]["retroMode"].GetBool(false);
			RetroScreenDivisor = p_reader["general"]["retroScreenDivisor"].GetInt(4);
			ViewAlignedMinimap = p_reader["gameplay"]["viewAlignedMinimap"].GetBool(false);
			MinimapGirdOpacity = p_reader["gameplay"]["minimapGirdOpacity"].GetFloat(1f);
			ShowMinimap = p_reader["gameplay"]["showMinimap"].GetBool(true);
		}

		public void Write(ConfigReader p_reader)
		{
			if (!m_onlyChineseAvailable)
			{
				using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Ubisoft\\Might & Magic X Legacy"))
				{
					registryKey.SetValue("GameLanguageCustom", Language);
				}
			}
			p_reader["language"]["subtitles"].SetValue(SubTitles);
			p_reader["gameplay"]["monsterHPBarVisible"].SetValue(MonsterHPBarsVisible);
			p_reader["gameplay"]["showHints"].SetValue(ShowHints);
			p_reader["gameplay"]["hideMinimapTooltips"].SetValue(HideMinimapTooltips);
			p_reader["gameplay"]["showAlternativeMonsterModel"].SetValue(ShowAlternativeMonsterModel);
			p_reader["gameplay"]["enemyOutlineOpacity"].SetValue(EnemyOutlineOpacity);
			p_reader["gameplay"]["objectOutlineOpacity"].SetValue(ObjectOutlineOpacity);
			p_reader["gameplay"]["showViewport"].SetValue(ShowViewport);
			p_reader["gameplay"]["lockActionBar"].SetValue(LockActionBar);
			p_reader["gameplay"]["monsterMovementSpeed"].SetValue(MonsterMovementSpeed);
			p_reader["gameplay"]["fadeLogs"].SetValue(FadeLogs);
			p_reader["gameplay"]["fadeLogsDelay"].SetValue(FadeLogsDelay);
			p_reader["gameplay"]["isLeftActionBarWithArrows"].SetValue(IsLeftActionBarShowingArrows);
			p_reader["gameplay"]["triggerBarks"].SetValue(TriggerBarks);
			p_reader["gameplay"]["showMessages"].SetValue(ShowGameMessages);
			p_reader["gameplay"]["logOpacity"].SetValue(LogOpacity);
			p_reader["gameplay"]["tooltipOpacity"].SetValue(TooltipOpacity);
			p_reader["gameplay"]["showFloatingDamageMonsters"].SetValue(ShowFloatingDamageMonsters);
			p_reader["gameplay"]["showFloatingDamageChars"].SetValue(ShowFloatingDamageCharacters);
			p_reader["gameplay"]["questLogSize"].SetValue(QuestLogSize);
			p_reader["gameplay"]["actionLogSize"].SetValue(ActionLogSize);
			p_reader["gameplay"]["lockCharacterOrder"].SetValue(LockCharacterOrder);
			p_reader["gameplay"]["tooltipDelay"].SetValue(TooltipDelay);
			p_reader["general"]["videodecoder"].SetEnumValue<EVideoDecoder>(VideoDecoder);
			p_reader["general"]["retroMode"].SetValue(RetroMode);
			p_reader["general"]["retroScreenDivisor"].SetValue(RetroScreenDivisor);
			p_reader["gameplay"]["viewAlignedMinimap"].SetValue(ViewAlignedMinimap);
			p_reader["gameplay"]["minimapGirdOpacity"].SetValue(MinimapGirdOpacity);
			p_reader["gameplay"]["showMinimap"].SetValue(ShowMinimap);
		}

		public Boolean HasUnsavedChanges(ConfigReader p_reader)
		{
			Boolean flag = false;
			if (!m_onlyChineseAvailable)
			{
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Ubisoft\\Might & Magic X Legacy"))
				{
					if (registryKey != null)
					{
						flag |= (Language != registryKey.GetValue("GameLanguageCustom", "en").ToString());
					}
				}
			}
			flag |= (SubTitles != p_reader["language"]["subtitles"].GetBool());
			flag |= (MonsterHPBarsVisible != p_reader["gameplay"]["monsterHPBarVisible"].GetBool());
			flag |= (ShowHints != p_reader["gameplay"]["showHints"].GetBool());
			flag |= (HideMinimapTooltips != p_reader["gameplay"]["hideMinimapTooltips"].GetBool());
			flag |= (ShowAlternativeMonsterModel != p_reader["gameplay"]["showAlternativeMonsterModel"].GetBool());
			flag |= (Math.Abs(EnemyOutlineOpacity - p_reader["gameplay"]["enemyOutlineOpacity"].GetFloat()) > 0.001f);
			flag |= (Math.Abs(ObjectOutlineOpacity - p_reader["gameplay"]["objectOutlineOpacity"].GetFloat()) > 0.001f);
			flag |= (ShowViewport != p_reader["gameplay"]["showViewport"].GetBool());
			flag |= (LockActionBar != p_reader["gameplay"]["lockActionBar"].GetBool());
			flag |= (MonsterMovementSpeed != p_reader["gameplay"]["monsterMovementSpeed"].GetFloat());
			flag |= (FadeLogs != p_reader["gameplay"]["fadeLogs"].GetBool());
			flag |= (Math.Abs(FadeLogsDelay - p_reader["gameplay"]["fadeLogsDelay"].GetFloat()) > 0.001f);
			flag |= (IsLeftActionBarShowingArrows != p_reader["gameplay"]["isLeftActionBarWithArrows"].GetBool());
			flag |= (TriggerBarks != p_reader["gameplay"]["triggerBarks"].GetBool());
			flag |= (ShowGameMessages != p_reader["gameplay"]["showMessages"].GetBool());
			flag |= (Math.Abs(LogOpacity - p_reader["gameplay"]["logOpacity"].GetFloat()) > 0.001f);
			flag |= (Math.Abs(TooltipOpacity - p_reader["gameplay"]["tooltipOpacity"].GetFloat()) > 0.001f);
			flag |= (ShowFloatingDamageMonsters != p_reader["gameplay"]["showFloatingDamageMonsters"].GetBool());
			flag |= (ShowFloatingDamageCharacters != p_reader["gameplay"]["showFloatingDamageChars"].GetBool());
			flag |= (QuestLogSize != p_reader["gameplay"]["questLogSize"].GetFloat());
			flag |= (ActionLogSize != p_reader["gameplay"]["actionLogSize"].GetInt());
			flag |= (LockCharacterOrder != p_reader["gameplay"]["lockCharacterOrder"].GetBool());
			flag |= (VideoDecoder != p_reader["general"]["videodecoder"].GetEnum<EVideoDecoder>());
			flag |= (RetroMode != p_reader["general"]["retroMode"].GetBool());
			flag |= (RetroScreenDivisor != p_reader["general"]["retroScreenDivisor"].GetInt());
			flag |= (ViewAlignedMinimap != p_reader["gameplay"]["viewAlignedMinimap"].GetBool(false));
			flag |= (MinimapGirdOpacity != p_reader["gameplay"]["minimapGirdOpacity"].GetFloat(1f));
			flag |= (ShowMinimap != p_reader["gameplay"]["showMinimap"].GetBool(true));
			flag |= (TooltipDelay != p_reader["gameplay"]["tooltipDelay"].GetFloat(0.2f));
			return flag;
		}

		public Boolean HasLanguageChanged(ConfigReader p_reader)
		{
			return Language != p_reader["language"]["language"].GetString();
		}
	}
}
