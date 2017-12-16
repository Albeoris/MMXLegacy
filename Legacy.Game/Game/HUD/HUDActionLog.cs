using System;
using System.Collections.Generic;
using System.Text;
using Legacy.Core;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.Entities.TrapEffects;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.Quests;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;
using Legacy.Core.WorldMap;
using Legacy.Game.MMGUI;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDActionLog")]
	public class HUDActionLog : BaseHUDLog, IResizable, IScrollingListener
	{
		[SerializeField]
		private UIScrollBar m_scrollBar;

		[SerializeField]
		private UISprite m_background;

		[SerializeField]
		private UISprite m_buttonHideIcon;

		[SerializeField]
		private UIButton m_buttonHide;

		[SerializeField]
		private HUDActionLogDragButton m_buttonChangeSize;

		[SerializeField]
		private UILabel m_text;

		[SerializeField]
		private UISprite m_scrollBackground;

		[SerializeField]
		private Color m_colorTargetFriendly = new Color(0f, 1f, 0f);

		[SerializeField]
		private Color m_colorTargetFoe = new Color(1f, 0f, 0f);

		[SerializeField]
		private Color m_colorDamage = new Color(1f, 0f, 0f);

		[SerializeField]
		private Color m_colorMana = new Color(0.075f, 0.88f, 0.88f);

		[SerializeField]
		private Color m_colorHeal = new Color(0f, 1f, 0f);

		[SerializeField]
		private Color m_colorItem = new Color(1f, 0f, 1f);

		[SerializeField]
		private Color m_colorSkill = new Color(0f, 1f, 1f);

		[SerializeField]
		private Color m_colorBuff = new Color(0f, 1f, 1f);

		[SerializeField]
		private Color m_colorDebuff = new Color(0f, 1f, 1f);

		[SerializeField]
		private Color m_colorXP = new Color(1f, 0f, 0f);

		[SerializeField]
		private Color m_colorQuest = new Color(1f, 0f, 0f);

		[SerializeField]
		private Color m_colorGold = new Color(1f, 0f, 0f);

		[SerializeField]
		private Int32 m_buttonScrollLines = 3;

		[SerializeField]
		private Int32 m_minimumLines = 7;

		[SerializeField]
		private Int32 m_maximumLines = 13;

		[SerializeField]
		private Single m_lineHeight = 20f;

		[SerializeField]
		private Single m_scrollBarHeightReduce = 80f;

		[SerializeField]
		private Single m_saveAfterTime = 3f;

		private Int32 m_lineCount = 10;

		private ActionLog m_actionLog;

		private Int32 m_currentLine;

		private StringBuilder m_stringBuilder = new StringBuilder(50);

		private StringBuilder m_tempStringBuilder = new StringBuilder(50);

		private Boolean m_minimized;

		private Boolean m_visible = true;

		private Single m_buttonChangeSizeOffset;

		private Single m_backgroundOffset;

		private Single m_collisionBoxOffset;

		private Single m_collisionBoxScaleOffset;

		private Vector3 m_textOffset;

		private List<String> m_logEntries;

		private BoxCollider m_boxCollider;

		private UIRoot m_uiRoot;

		private Single m_sizeChangeSaveTimer;

		private String m_colorTargetFriendlyHex;

		private String m_colorTargetFoeHex;

		private String m_colorItemHex;

		private String m_colorSkillHex;

		private String m_colorBuffHex;

		private String m_colorDebuffHex;

		private String m_colorDamageHex;

		private String m_colorHealHex;

		private String m_colorManaHex;

		private String m_colorXPHex;

		private String m_colorQuestHex;

		private String m_colorGoldHex;

		public void Init(ActionLog p_actionLog)
		{
			base.Init();
			m_lineCount = ConfigManager.Instance.Options.ActionLogSize;
			m_actionLog = p_actionLog;
			m_logEntries = new List<String>(ConfigManager.Instance.Game.ActionLogMaxEntries);
			m_stringBuilder.Length = 0;
			m_tempStringBuilder.Length = 0;
			m_boxCollider = gameObject.GetComponent<BoxCollider>();
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			Single y = m_scrollBackground.transform.localScale.y;
			m_backgroundOffset = m_background.transform.localScale.y - y;
			m_collisionBoxScaleOffset = m_boxCollider.size.y - y;
			m_collisionBoxOffset = m_boxCollider.center.y + y / 2f;
			m_buttonChangeSizeOffset = m_buttonChangeSize.transform.localPosition.y + y;
			m_textOffset = m_text.cachedTransform.localPosition;
			m_buttonChangeSize.Init(this);
			m_buttonChangeSize.ButtonReleased += OnButtonReleased;
			m_actionLog.ReceivedLogEntry += new EventHandler<LogEntryEventArgs>(OnReceivedLogEntry);
			m_colorTargetFriendlyHex = "[" + NGUITools.EncodeColor(m_colorTargetFriendly) + "]";
			m_colorTargetFoeHex = "[" + NGUITools.EncodeColor(m_colorTargetFoe) + "]";
			m_colorItemHex = "[" + NGUITools.EncodeColor(m_colorItem) + "]";
			m_colorSkillHex = "[" + NGUITools.EncodeColor(m_colorSkill) + "]";
			m_colorBuffHex = "[" + NGUITools.EncodeColor(m_colorBuff) + "]";
			m_colorDebuffHex = "[" + NGUITools.EncodeColor(m_colorDebuff) + "]";
			m_colorDamageHex = "[" + NGUITools.EncodeColor(m_colorDamage) + "]";
			m_colorHealHex = "[" + NGUITools.EncodeColor(m_colorHeal) + "]";
			m_colorManaHex = "[" + NGUITools.EncodeColor(m_colorMana) + "]";
			m_colorXPHex = "[" + NGUITools.EncodeColor(m_colorXP) + "]";
			m_colorQuestHex = "[" + NGUITools.EncodeColor(m_colorQuest) + "]";
			m_colorGoldHex = "[" + NGUITools.EncodeColor(m_colorGold) + "]";
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAME_LOADED, new EventHandler(OnGameLoaded));
			UpdateSize();
			m_initialBackgroundAlpha = ConfigManager.Instance.Options.LogOpacity;
			m_initialScrollBarAlpha = m_scrollBar.alpha;
			m_currentAlpha = 1f;
			m_fadeState = EFadeState.WAITING;
			ResetWaitTime();
		}

		public void CleanUp()
		{
			Cleanup();
			m_actionLog.ReceivedLogEntry -= new EventHandler<LogEntryEventArgs>(OnReceivedLogEntry);
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Remove(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			m_buttonChangeSize.ButtonReleased -= OnButtonReleased;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAME_LOADED, new EventHandler(OnGameLoaded));
		}

		private void OnButtonReleased(Object sender, EventArgs e)
		{
			m_buttonChangeSize.FadeButton(m_initialScrollBarAlpha);
			ResetWaitTime();
		}

		private void Start()
		{
			if (m_uiRoot == null)
			{
				m_uiRoot = NGUITools.FindInParents<UIRoot>(gameObject);
			}
		}

		protected override void Update()
		{
			if (m_sizeChangeSaveTimer > 0f)
			{
				m_sizeChangeSaveTimer -= Time.deltaTime;
				if (m_sizeChangeSaveTimer <= 0f && m_lineCount != ConfigManager.Instance.Options.ActionLogSize)
				{
					ConfigManager.Instance.Options.ActionLogSize = m_lineCount;
					ConfigManager.Instance.WriteConfigurations();
				}
			}
			if (!m_configSaysFade)
			{
				if (m_currentAlpha < 1f)
				{
					m_currentAlpha = 1f;
					FadeComponents(m_currentAlpha);
					m_fadeState = EFadeState.WAITING;
					m_locked = true;
				}
				return;
			}
			if (LegacyLogic.Instance.WorldManager.Party.HasAggro() && !m_locked)
			{
				m_currentAlpha = 1f;
				FadeComponents(m_currentAlpha);
				m_locked = true;
			}
			else if (m_locked)
			{
				m_waitTime = Time.time + m_configFadeDelay * 2f;
				m_fadeState = EFadeState.WAITING;
				m_locked = false;
			}
			if (m_buttonChangeSize.IsPressed && m_fadeState == EFadeState.FADE_OUT)
			{
				m_fadeState = EFadeState.FADE_IN;
			}
			base.Update();
		}

		public override void OnOptionsChanged()
		{
			m_initialBackgroundAlpha = ConfigManager.Instance.Options.LogOpacity;
			if (m_configSaysFade)
			{
				m_locked = false;
				m_fadeState = EFadeState.WAITING;
				ResetWaitTime();
			}
			else
			{
				m_background.alpha = m_initialBackgroundAlpha;
			}
		}

		public override void FadeComponents(Single p_alpha)
		{
			base.FadeComponents(p_alpha);
			if (p_alpha < m_initialScrollBarAlpha)
			{
				m_scrollBar.alpha = p_alpha;
				m_buttonChangeSize.FadeButton(p_alpha);
			}
			else
			{
				m_scrollBar.alpha = m_initialScrollBarAlpha;
				m_buttonChangeSize.FadeButton(m_initialScrollBarAlpha);
			}
			if (p_alpha <= m_initialBackgroundAlpha)
			{
				m_background.alpha = p_alpha;
			}
			else
			{
				m_background.alpha = m_initialBackgroundAlpha;
			}
			m_text.alpha = p_alpha;
		}

		private void OnGameLoaded(Object p_sender, EventArgs p_args)
		{
			m_logEntries.Clear();
			UpdateScrollBar();
			UpdateText();
		}

		public void SetVisible(Boolean p_visible)
		{
			m_visible = p_visible;
			UpdateVisibility();
			ScrollingHelper.InitScrollListeners(this, m_scrollBar.gameObject);
		}

		public void OnReceivedLogEntry(Object p_sender, EventArgs p_args)
		{
			m_fadeState = EFadeState.FADE_IN;
			AddActionLogText((LogEntryEventArgs)p_args);
			UpdateScrollBar();
			UpdateText();
		}

		private void UpdateText()
		{
			if (m_logEntries.Count == 0)
			{
				m_text.text = String.Empty;
				return;
			}
			m_stringBuilder.Rest();
			Int32 num = Math.Min(m_lineCount, m_logEntries.Count);
			for (Int32 i = num - 1; i >= 0; i--)
			{
				m_stringBuilder.AppendLine(m_logEntries[m_logEntries.Count - 1 - i - m_currentLine]);
			}
			m_text.text = m_stringBuilder.ToString();
		}

		private void UpdateScrollBar()
		{
			if (m_logEntries.Count == 0)
			{
				m_scrollBar.barSize = 1f;
			}
			else
			{
				m_scrollBar.barSize = m_lineCount / (Single)m_logEntries.Count;
			}
			Int32 num = m_logEntries.Count - m_lineCount;
			if (num > 0)
			{
				if (m_currentLine > num)
				{
					m_currentLine = num;
				}
				m_scrollBar.scrollValue = m_currentLine / (Single)num;
			}
			else
			{
				m_scrollBar.scrollValue = 0f;
			}
		}

		private void UpdateSize()
		{
			Single num = m_lineCount * m_lineHeight - m_scrollBarHeightReduce;
			Vector3 localScale = m_scrollBackground.transform.localScale;
			localScale.y = num;
			m_scrollBackground.transform.localScale = localScale;
			Vector3 localPosition = m_buttonChangeSize.gameObject.transform.localPosition;
			localPosition.y = -num + m_buttonChangeSizeOffset;
			m_buttonChangeSize.gameObject.transform.localPosition = localPosition;
			Vector3 localScale2 = m_background.gameObject.transform.localScale;
			localScale2.y = num + m_backgroundOffset;
			m_background.gameObject.transform.localScale = localScale2;
			Vector3 size = m_boxCollider.size;
			size.y = m_collisionBoxScaleOffset + num;
			m_boxCollider.size = size;
			Vector3 center = m_boxCollider.center;
			center.y = m_collisionBoxOffset - num / 2f;
			m_boxCollider.center = center;
			Vector3 textOffset = m_textOffset;
			textOffset.y -= m_lineHeight * m_lineCount;
			m_text.cachedTransform.localPosition = textOffset;
			m_scrollBar.barSize = 0f;
			UpdateScrollBar();
			UpdateText();
		}

		private void UpdateVisibility()
		{
			NGUITools.SetActive(m_scrollBar.gameObject, !m_minimized && m_visible);
			NGUITools.SetActive(m_text.gameObject, !m_minimized && m_visible);
			NGUITools.SetActive(m_buttonChangeSize.gameObject, !m_minimized && m_visible);
			m_buttonChangeSize.FadeButton(m_currentAlpha * 0.5f);
			NGUITools.SetActive(m_background.gameObject, !m_minimized && m_visible);
			m_boxCollider.enabled = (!m_minimized && m_visible);
			if (!m_minimized)
			{
				m_buttonHideIcon.spriteName = "ICO_window_minimize";
				m_buttonHideIcon.Update();
			}
			else
			{
				m_buttonHideIcon.spriteName = "ICO_window_maximize";
				m_buttonHideIcon.Update();
			}
			m_buttonHide.isEnabled = m_visible;
		}

		private void AddActionLogText(LogEntryEventArgs p_logEntry)
		{
			if (p_logEntry is CombatEntryEventArgs)
			{
				AddCombatText((CombatEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is MessageEventArgs)
			{
				AddText((MessageEventArgs)p_logEntry);
			}
			else if (p_logEntry is ConditionChangedEntryEventArgs)
			{
				AddConditionChangedText((ConditionChangedEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is PotionUsedEntryEventArgs)
			{
				AddPotionUsedText((PotionUsedEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is BloodMagicEventArgs)
			{
				AddBloodMagicText((BloodMagicEventArgs)p_logEntry);
			}
			else if (p_logEntry is LevelUpEntryEventArgs)
			{
				AddLevelUpText((LevelUpEntryEventArgs)p_logEntry);
				AddAttributeChangeText((LevelUpEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is ExpEntryEventArgs)
			{
				AddExpText((ExpEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is RestEntryEventArgs)
			{
				AddRestText((RestEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is GameTimeEntryEventArgs)
			{
				AddGameTimeText((GameTimeEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is SkillEffectEventArgs)
			{
				AddSkillEffectText((SkillEffectEventArgs)p_logEntry);
			}
			else if (p_logEntry is CastSpellEntryEventArgs)
			{
				AddCastSpellText((CastSpellEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is SpellEffectEntryEventArgs)
			{
				AddSpellEffectText((SpellEffectEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is MonsterLootEntryEventArgs)
			{
				AddMonsterLootText((MonsterLootEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is QuestLootEntryEventArgs)
			{
				AddQuestLootText((QuestLootEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is EnchantmentEffectEventArgs)
			{
				AddEnchantmentEffectText((EnchantmentEffectEventArgs)p_logEntry);
			}
			else if (p_logEntry is AbilityTriggeredEventArgs)
			{
				AddAbilityTriggeredText((AbilityTriggeredEventArgs)p_logEntry);
			}
			else if (p_logEntry is ChestLootEventArgs)
			{
				AddChestLootText((ChestLootEventArgs)p_logEntry);
			}
			else if (p_logEntry is MonsterBuffDamageEntryEventArgs)
			{
				AddMonsterBuffDamageText((MonsterBuffDamageEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is RelicLevelUpEntryEventArgs)
			{
				AddRelicLevelUpText((RelicLevelUpEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is TokenAcquiredEventArgs)
			{
				AddTokenText((TokenAcquiredEventArgs)p_logEntry);
			}
			else if (p_logEntry is TokenRemovedEventArgs)
			{
				AddTokenRemovedText((TokenRemovedEventArgs)p_logEntry);
			}
			else if (p_logEntry is TrapTriggeredEventArgs)
			{
				AddTrapTriggeredText((TrapTriggeredEventArgs)p_logEntry);
			}
			else if (p_logEntry is TrapDamageEventArgs)
			{
				AddTrapDamageText((TrapDamageEventArgs)p_logEntry);
			}
			else if (p_logEntry is TrapManaLostEventArgs)
			{
				AddTrapManaLostText((TrapManaLostEventArgs)p_logEntry);
			}
			else if (p_logEntry is TrapFoodLostEventArgs)
			{
				AddTrapFoodLostText((TrapFoodLostEventArgs)p_logEntry);
			}
			else if (p_logEntry is MapPointAddedEventArgs)
			{
				AddMapPointAddedText((MapPointAddedEventArgs)p_logEntry);
			}
			else if (p_logEntry is ExtraAttackEventArgs)
			{
				AddExtraAttackText((ExtraAttackEventArgs)p_logEntry);
			}
			else if (p_logEntry is MonsterBuffEntryEventArgs)
			{
				AddMonsterBuffText((MonsterBuffEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is BuffRemovedEventArgs)
			{
				AddBuffRemovedText((BuffRemovedEventArgs)p_logEntry);
			}
			else if (p_logEntry is DamagePreventedEntryEventArgs)
			{
				AddDamagePreventedText((DamagePreventedEntryEventArgs)p_logEntry);
			}
			else if (p_logEntry is SkillTierBonusEventArgs)
			{
				AddSkillTierBonusText((SkillTierBonusEventArgs)p_logEntry);
			}
			else if (p_logEntry is MonsterDamagePreventedEntryEventArgs)
			{
				AddMonsterDamagePreventedText((MonsterDamagePreventedEntryEventArgs)p_logEntry);
			}
		}

		private void AddLogEntry(StringBuilder p_stringBuilder)
		{
			AddLogEntry(p_stringBuilder.ToString());
			m_stringBuilder.Rest();
		}

		private void AddLogEntry(String p_string)
		{
			m_text.lineWidth = m_text.lineWidth - 20;
			m_text.text = p_string;
			String processedText = m_text.processedText;
			Int32 num = processedText.IndexOf("\n");
			m_text.lineWidth = m_text.lineWidth + 20;
			if (m_logEntries.Count >= ConfigManager.Instance.Game.ActionLogMaxEntries)
			{
				m_logEntries.RemoveAt(0);
			}
			if (num > 0)
			{
				m_logEntries.Add(processedText.Substring(0, num));
				AddLogEntry("    " + processedText.Substring(num + 1));
			}
			else
			{
				m_logEntries.Add(p_string);
			}
		}

		private void AddCombatText(CombatEntryEventArgs p_logEntry)
		{
			m_stringBuilder.Rest();
			if (p_logEntry.Result.Result == EResultType.BLOCK)
			{
				if (LegacyLogic.Instance.WorldManager.Party.LastBlockSubstitute != null && p_logEntry.Target is Character)
				{
					LocaManager.AppendText(m_stringBuilder, "ACTION_LOG_COMBAT_ATTACK_BLOCKED_BY_OTHER", GetColoredName(p_logEntry.Source), GetColoredName(p_logEntry.Target), GetColoredName(LegacyLogic.Instance.WorldManager.Party.LastBlockSubstitute));
				}
				else
				{
					LocaManager.AppendText(m_stringBuilder, "ACTION_LOG_COMBAT_ATTACK_BLOCK", GetColoredName(p_logEntry.Source), GetColoredName(p_logEntry.Target));
				}
				LegacyLogic.Instance.WorldManager.Party.LastBlockSubstitute = null;
			}
			else if (p_logEntry.Result.Result == EResultType.EVADE)
			{
				LocaManager.AppendText(m_stringBuilder, "ACTION_LOG_COMBAT_ATTACK_MISSES", GetColoredName(p_logEntry.Source), GetColoredName(p_logEntry.Target));
			}
			else
			{
				if (p_logEntry.Result.DamageResults.Count == 1)
				{
					if (p_logEntry.Result.DamageResults[0].Type == EDamageType.PRIMORDIAL)
					{
						LocaManager.AppendText(m_stringBuilder, "ACTION_LOG_COMBAT_ATTACK_HIT_MULTISOURCE", new Object[]
						{
							GetColoredName(p_logEntry.Source),
							GetColoredName(p_logEntry.Target),
							GetColoredDamage(p_logEntry.Result),
							GetSplitDamage(p_logEntry.Result.DamageResults)
						});
					}
					else
					{
						LocaManager.AppendText(m_stringBuilder, "ACTION_LOG_COMBAT_ATTACK_HIT", GetColoredName(p_logEntry.Source), GetColoredName(p_logEntry.Target), GetColoredDamage(p_logEntry.Result));
					}
				}
				else
				{
					LocaManager.AppendText(m_stringBuilder, "ACTION_LOG_COMBAT_ATTACK_HIT_MULTISOURCE", new Object[]
					{
						GetColoredName(p_logEntry.Source),
						GetColoredName(p_logEntry.Target),
						GetColoredDamage(p_logEntry.Result),
						GetSplitDamage(p_logEntry.Result.DamageResults)
					});
				}
				String resistedDamage = GetResistedDamage(p_logEntry.Result.DamageResults);
				if (resistedDamage != String.Empty)
				{
					LocaManager.AppendText(m_stringBuilder, "ACTION_LOG_COMBAT_RESISTED_DAMAGE", resistedDamage);
				}
			}
			AddLogEntry(m_stringBuilder);
			if (p_logEntry.Result.ReflectedDamage != null && p_logEntry.Result.ReflectedDamage.Damages != null && p_logEntry.Result.ReflectedDamage.Damages.Count > 0)
			{
				foreach (Damage damage in p_logEntry.Result.ReflectedDamage.Damages)
				{
					if (p_logEntry.Result.ReflectedDamageSource is PartyBuff && ((PartyBuff)p_logEntry.Result.ReflectedDamageSource).Type == EPartyBuffs.FIRE_SHIELD)
					{
						LocaManager.AppendText(m_stringBuilder, "ACTION_LOG_COMBAT_ATTACK_COUNTER_DAMAGE", GetColoredName(p_logEntry.Source), GetColoredReflect(damage.Value), GetColoredBuff("PARTYBUFF_FIRE_SHIELD_NAME"));
						AddLogEntry(m_stringBuilder);
					}
				}
			}
			if (p_logEntry.Result.BrokenItem != null)
			{
				Character p_object = (!(p_logEntry.Source is Character)) ? ((Character)p_logEntry.Target) : ((Character)p_logEntry.Source);
				AddLogEntry(LocaManager.GetText("ACTION_LOG_ITEM_BROKEN", GetColoredItemName(p_logEntry.Result.BrokenItem), GetColoredName(p_object)));
			}
			if (p_logEntry.BloodMagicEventArgs != null)
			{
				AddBloodMagicText(p_logEntry.BloodMagicEventArgs);
			}
		}

		private void AddText(MessageEventArgs messageEventArgs)
		{
			if (messageEventArgs.IsLocalized)
			{
				AddLogEntry(messageEventArgs.Message);
			}
			else
			{
				AddLogEntry(LocaManager.GetText(messageEventArgs.Message));
			}
		}

		private void AddConditionChangedText(ConditionChangedEntryEventArgs p_logEntry)
		{
			String conditionText = GetConditionText(p_logEntry.Condition, p_logEntry.Object, p_logEntry.Set);
			AddLogEntry(LocaManager.GetText(conditionText, GetColoredName(p_logEntry.Object)));
		}

		private String GetConditionText(ECondition p_condition, Object target, Boolean p_set)
		{
			String text;
			if (p_set)
			{
				text = "ACTION_LOG_CONDITION_CHANGED_";
			}
			else
			{
				text = "ACTION_LOG_CONDITION_REMOVED_";
			}
			switch (p_condition)
			{
			case ECondition.DEAD:
				text += "DEAD";
				break;
			case ECondition.UNCONSCIOUS:
				text += "UNCONSCIOUS";
				break;
			default:
				if (p_condition != ECondition.SLEEPING)
				{
					if (p_condition != ECondition.POISONED)
					{
						if (p_condition != ECondition.CONFUSED)
						{
							if (p_condition != ECondition.WEAK)
							{
								if (p_condition == ECondition.CURSED)
								{
									text += "CURSED";
								}
							}
							else
							{
								text += "WEAK";
							}
						}
						else
						{
							text += "CONFUSED";
						}
					}
					else
					{
						text += "POISONED";
					}
				}
				else
				{
					text += "SLEEPING";
				}
				break;
			case ECondition.PARALYZED:
				text += "PARALYZED";
				break;
			case ECondition.STUNNED:
				text += "KNOCKED_OUT";
				break;
			}
			if (target is Character)
			{
				if (((Character)target).Gender == EGender.MALE)
				{
					text += "_M";
				}
				else
				{
					text += "_F";
				}
			}
			else if (target is Monster)
			{
				if (((Monster)target).StaticData.Gender == EGender.MALE)
				{
					text += "_M";
				}
				else
				{
					text += "_F";
				}
				if (((Monster)target).StaticData.Type == EMonsterType.CAGE && p_condition == ECondition.DEAD)
				{
					text = "ACTION_LOG_DEAD_CAGE";
				}
				else if (((Monster)target).StaticData.Type == EMonsterType.TARGET && p_condition == ECondition.DEAD)
				{
					text = "ACTION_LOG_DEAD_TARGET_DUMMY";
				}
			}
			else
			{
				text += "_M";
			}
			return text;
		}

		private void AddPotionUsedText(PotionUsedEntryEventArgs p_logEntry)
		{
			if (p_logEntry.Potion.Target == EPotionTarget.HP)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_POTION_RESTORE_HP", GetColoredItemName(p_logEntry.Potion), GetColoredName(p_logEntry.Character), GetColoredHealValue(p_logEntry.Value)));
			}
			else if (p_logEntry.Potion.Target == EPotionTarget.MANA)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_POTION_RESTORE_MP", GetColoredItemName(p_logEntry.Potion), GetColoredName(p_logEntry.Character), p_logEntry.Value));
			}
			else if (p_logEntry.Potion.Target == EPotionTarget.MIGHT)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_POTION_INCREASE_MIGHT", GetColoredItemName(p_logEntry.Potion), GetColoredName(p_logEntry.Character), p_logEntry.Value));
			}
			else if (p_logEntry.Potion.Target == EPotionTarget.MAGIC)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_POTION_INCREASE_MAGIC", GetColoredItemName(p_logEntry.Potion), GetColoredName(p_logEntry.Character), p_logEntry.Value));
			}
			else if (p_logEntry.Potion.Target == EPotionTarget.DESTINY)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_POTION_INCREASE_DESTINY", GetColoredItemName(p_logEntry.Potion), GetColoredName(p_logEntry.Character), p_logEntry.Value));
			}
			else if (p_logEntry.Potion.Target == EPotionTarget.PERCEPTION)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_POTION_INCREASE_PERCEPTION", GetColoredItemName(p_logEntry.Potion), GetColoredName(p_logEntry.Character), p_logEntry.Value));
			}
		}

		private void AddBloodMagicText(BloodMagicEventArgs p_logEntry)
		{
			String text = "ACTION_LOG_ENTRY_MANA_GAINED";
			if (p_logEntry.Character.Gender == EGender.MALE)
			{
				text += "_M";
			}
			else
			{
				text += "_F";
			}
			AddLogEntry(LocaManager.GetText(text, GetColoredName(p_logEntry.Character), GetColoredMana(p_logEntry.Value)));
		}

		private void AddLevelUpText(LevelUpEntryEventArgs p_logEntry)
		{
			String id = "ACTION_LOG_CHARACTER_LEVEL_UP_M";
			if (p_logEntry.Character.Gender == EGender.FEMALE)
			{
				id = "ACTION_LOG_CHARACTER_LEVEL_UP_F";
			}
			AddLogEntry(LocaManager.GetText(id, GetColoredName(p_logEntry.Character), p_logEntry.Character.Level));
		}

		private void AddRelicLevelUpText(RelicLevelUpEntryEventArgs p_logEntry)
		{
			AddLogEntry(LocaManager.GetText("ACTION_LOG_RELICR_LEVEL_UP", GetColoredItemName(p_logEntry.OldEquipment)));
		}

		private void AddAttributeChangeText(LevelUpEntryEventArgs p_logEntry)
		{
			String id = "ACTION_LOG_CHARACTER_ATTRIBUTE_CHANGED_M";
			String id2 = "ACTION_LOG_CHARACTER_SKILLPOINTS_CHANGED_M";
			if (p_logEntry.Character.Gender == EGender.FEMALE)
			{
				id = "ACTION_LOG_CHARACTER_ATTRIBUTE_CHANGED_F";
				id2 = "ACTION_LOG_CHARACTER_SKILLPOINTS_CHANGED_F";
			}
			AddLogEntry(LocaManager.GetText(id2, GetColoredName(p_logEntry.Character), LocaManager.GetText("ATTRIBUTE_SKILLPOINTS"), p_logEntry.Character.SkillPoints));
			AddLogEntry(LocaManager.GetText(id, GetColoredName(p_logEntry.Character), p_logEntry.Character.AttributePoints));
		}

		private void AddExpText(ExpEntryEventArgs p_logEntry)
		{
			String id = "ACTION_LOG_CHARACTER_EXP_GAINED_M";
			if (p_logEntry.Character.Gender == EGender.FEMALE)
			{
				id = "ACTION_LOG_CHARACTER_EXP_GAINED_F";
			}
			AddLogEntry(LocaManager.GetText(id, GetColoredName(p_logEntry.Character), GetColoredXP(p_logEntry.Exp)));
		}

		private void AddRestText(RestEntryEventArgs p_logEntry)
		{
			AddLogEntry(LocaManager.GetText("ACTION_LOG_PARTY_REST", m_colorHealHex + p_logEntry.ConsumedFoodAmount + "[-]"));
		}

		private void AddGameTimeText(GameTimeEntryEventArgs p_logEntry)
		{
			if (p_logEntry.NewDayState == EDayState.DAWN)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_DAY_BEGINS"));
			}
			else
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_NIGHT_BEGINS"));
			}
		}

		private void AddSkillEffectText(SkillEffectEventArgs p_logEntry)
		{
			AddLogEntry(LocaManager.GetText(p_logEntry.EffectData.DynamicDescription));
		}

		private void AddEnchantmentEffectText(EnchantmentEffectEventArgs p_logEntry)
		{
			if (p_logEntry.EffectData.Effect == ESuffixEffect.LOSE_ALL_BLOCK_ATTEMPTS)
			{
				AddLogEntry(LocaManager.GetText("SUFFIX_EFFECT_REMOVED_ALL_BLOCKS"));
			}
			else if (p_logEntry.EffectData.Effect == ESuffixEffect.GAIN_HP_PERCENTAGE_FROM_DEALT_DAMAGE)
			{
				AddLogEntry(LocaManager.GetText("SUFFIX_EFFECT_GAINED_HP_FROM_DAMAGE", p_logEntry.EffectValue));
			}
			else if (p_logEntry.EffectData.Effect == ESuffixEffect.GAIN_MP_PERCENTAGE_FROM_DEALT_DAMAGE)
			{
				AddLogEntry(LocaManager.GetText("SUFFIX_EFFECT_GAINED_MP_FROM_DAMAGE", p_logEntry.EffectValue));
			}
			else if (p_logEntry.EffectData.Effect == ESuffixEffect.HIT_TILE_MONSTERS_MAGIC_DAMAGE_FIRE)
			{
				AddLogEntry(LocaManager.GetText("SUFFIX_EFFECT_HIT_TILE_MONSTERS_MAGIC_DAMAGE_FIRE"));
			}
			else if (p_logEntry.EffectData.Effect == ESuffixEffect.ATTACKING_MONSTER_DAMAGED)
			{
				Int32 p_dmg = (Int32)p_logEntry.EffectValue;
				EDamageType magicSchool = p_logEntry.EffectData.MagicSchool;
				switch (magicSchool + 1)
				{
				case EDamageType.PHYSICAL:
				case EDamageType.AIR:
					AddLogEntry(LocaManager.GetText("SUFFIX_THORNS_LOG_ENTRY", GetColoredReflect(p_dmg)));
					break;
				case EDamageType.EARTH:
					AddLogEntry(LocaManager.GetText("SUFFIX_AIR_ARMOR_SHORT", GetColoredReflect(p_dmg)));
					break;
				case EDamageType.FIRE:
					AddLogEntry(LocaManager.GetText("SUFFIX_EARTH_ARMOR_SHORT", GetColoredReflect(p_dmg)));
					break;
				case EDamageType.WATER:
					AddLogEntry(LocaManager.GetText("SUFFIX_FIRE_ARMOR_SHORT", GetColoredReflect(p_dmg)));
					break;
				case EDamageType.DARK:
					AddLogEntry(LocaManager.GetText("SUFFIX_WATER_ARMOR_SHORT", GetColoredReflect(p_dmg)));
					break;
				case EDamageType.LIGHT:
					AddLogEntry(LocaManager.GetText("SUFFIX_DARK_ARMOR_SHORT", GetColoredReflect(p_dmg)));
					break;
				case EDamageType.PRIMORDIAL:
					AddLogEntry(LocaManager.GetText("SUFFIX_LIGHT_ARMOR_SHORT", GetColoredReflect(p_dmg)));
					break;
				case EDamageType.HEAL:
					AddLogEntry(LocaManager.GetText("SUFFIX_PRIME_ARMOR_SHORT", GetColoredReflect(p_dmg)));
					break;
				}
			}
			else
			{
				Debug.LogError("No specific text for suffix effect " + p_logEntry.EffectData.Effect);
			}
		}

		private void AddCastSpellText(CastSpellEntryEventArgs p_logEntry)
		{
			if (p_logEntry.Source is Summon)
			{
				Summon summon = (Summon)p_logEntry.Source;
				if (p_logEntry.SpellResult.Result == ESpellResult.OK)
				{
					foreach (AttackedTarget attackedTarget in p_logEntry.SpellResult.SpellTargetsOfType<AttackedTarget>())
					{
						if (attackedTarget.Result.Result == EResultType.EVADE)
						{
							AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_SPELL_MISSES", GetColoredSpell(summon.StaticData.NameKey), GetColoredName(attackedTarget.Target)));
						}
						else if (attackedTarget.Result.Result == EResultType.IMMUNE)
						{
							AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_IS_IMMUNE_AGAINST", GetColoredName(attackedTarget.Target), GetColoredSpell(summon.StaticData.NameKey)));
						}
						else
						{
							String text = LocaManager.GetText("ACTION_LOG_COMBAT_SPELL_HIT", GetColoredSpell(summon.StaticData.NameKey), GetColoredName(attackedTarget.Target), GetColoredDamage(attackedTarget.Result));
							String resistedDamage = GetResistedDamage(attackedTarget.Result.DamageResults);
							if (resistedDamage != String.Empty)
							{
								text += LocaManager.GetText("ACTION_LOG_COMBAT_RESISTED_DAMAGE", resistedDamage);
							}
							AddLogEntry(text);
						}
					}
				}
				return;
			}
			CharacterSpell characterSpell = p_logEntry.SpellResult.Spell as CharacterSpell;
			String id;
			if (p_logEntry.SpellResult.Result == ESpellResult.NO_TARGET_FOUND)
			{
				id = "ACTION_LOG_COMBAT_CAST_SPELL_NO_TARGET_M";
				if (p_logEntry.Source is Character && ((Character)p_logEntry.Source).Gender == EGender.FEMALE)
				{
					id = "ACTION_LOG_COMBAT_CAST_SPELL_NO_TARGET_F";
				}
			}
			else if (characterSpell != null && characterSpell.StaticData.SkillID == ESkillID.SKILL_WARFARE)
			{
				id = "ACTION_LOG_COMBAT_WARFARE_M";
				if (p_logEntry.Source is Character && ((Character)p_logEntry.Source).Gender == EGender.FEMALE)
				{
					id = "ACTION_LOG_COMBAT_WARFARE_F";
				}
			}
			else
			{
				id = "ACTION_LOG_COMBAT_CAST_SPELL_M";
				if (p_logEntry.Source is Character && ((Character)p_logEntry.Source).Gender == EGender.FEMALE)
				{
					id = "ACTION_LOG_COMBAT_CAST_SPELL_F";
				}
			}
			AddLogEntry(LocaManager.GetText(id, GetColoredName(p_logEntry.Source), GetColoredSpell(p_logEntry.SpellResult.Spell.NameKey)));
		}

		private void AddSpellEffectText(SpellEffectEntryEventArgs p_logEntry)
		{
			if (p_logEntry.SpellResult.AddedPartyBuffs != EPartyBuffs.NONE)
			{
				EPartyBuffs addedPartyBuffs = p_logEntry.SpellResult.AddedPartyBuffs;
				PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)addedPartyBuffs);
				AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_APPLY_BUFF", GetColoredBuff(staticData.Name), LocaManager.GetText("PARTY")));
			}
			for (Int32 i = 0; i < p_logEntry.SpellResult.SpellTargets.Count; i++)
			{
				SpellTarget spellTarget = p_logEntry.SpellResult.SpellTargets[i];
				if (spellTarget is HealedTarget)
				{
					HealedTarget healedTarget = (HealedTarget)spellTarget;
					String text = healedTarget.Value.ToString();
					if (healedTarget.IsCritical)
					{
						text = text + " (" + LocaManager.GetText("ACTION_LOG_COMBAT_CRITICAL") + ")";
					}
					text = GetColoredHealValue(text);
					if (p_logEntry.Source is PartyBuff)
					{
						AddLogEntry(LocaManager.GetText("ACTION_LOG_POTION_RESTORE_HP", GetColoredBuff(((PartyBuff)p_logEntry.Source).StaticData.Name), GetColoredName(healedTarget.Target), text));
					}
					else
					{
						AddLogEntry(LocaManager.GetText("ACTION_LOG_POTION_RESTORE_HP", GetColoredSpell(p_logEntry.SpellResult.Spell.NameKey), GetColoredName(healedTarget.Target), text));
					}
				}
				else if (spellTarget is ConditionEffectTarget)
				{
					ConditionEffectTarget conditionEffectTarget = (ConditionEffectTarget)spellTarget;
					String p_debuffName = "CHARACTER_CONDITION_" + conditionEffectTarget.Source.ToString();
					AddLogEntry(LocaManager.GetText("CONDITION_NEGATIVE_EFFECT_LOG_TEXT", GetColoredDebuff(p_debuffName), GetColoredDamage(conditionEffectTarget.Damage), GetColoredName(conditionEffectTarget.Target)));
				}
				else if (spellTarget is AttackedTarget)
				{
					AttackedTarget attackedTarget = (AttackedTarget)spellTarget;
					if (attackedTarget.Result.Result == EResultType.EVADE)
					{
						AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_SPELL_MISSES", GetColoredSpell(p_logEntry.SpellResult.Spell.NameKey), GetColoredName(attackedTarget.Target)));
					}
					else if (attackedTarget.Result.Result == EResultType.IMMUNE)
					{
						AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_IS_IMMUNE_AGAINST", GetColoredName(attackedTarget.Target), GetColoredSpell(p_logEntry.SpellResult.Spell.NameKey)));
					}
					else if (attackedTarget.Result.DamageDone > 0)
					{
						String text2 = LocaManager.GetText("ACTION_LOG_COMBAT_SPELL_HIT", GetColoredSpell(p_logEntry.SpellResult.Spell.NameKey), GetColoredName(attackedTarget.Target), GetColoredDamage(attackedTarget.Result));
						String resistedDamage = GetResistedDamage(attackedTarget.Result.DamageResults);
						if (resistedDamage != String.Empty)
						{
							text2 += LocaManager.GetText("ACTION_LOG_COMBAT_RESISTED_DAMAGE", resistedDamage);
						}
						AddLogEntry(text2);
					}
				}
				else if (spellTarget is MonsterBuffTarget)
				{
					MonsterBuffTarget monsterBuffTarget = (MonsterBuffTarget)spellTarget;
					MonsterBuffStaticData staticData2 = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)monsterBuffTarget.Buff);
					String nameKey = staticData2.NameKey;
					if (monsterBuffTarget.IsImmune)
					{
						if (staticData2.IsDebuff)
						{
							AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_IS_IMMUNE_AGAINST", GetColoredName(monsterBuffTarget.Target), GetColoredDebuff(nameKey)));
						}
						else
						{
							AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_IS_IMMUNE_AGAINST", GetColoredName(monsterBuffTarget.Target), GetColoredBuff(nameKey)));
						}
					}
					else if (monsterBuffTarget.Target is Monster)
					{
						if (monsterBuffTarget.Successful)
						{
							if (staticData2.IsDebuff)
							{
								AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_APPLY_BUFF", GetColoredDebuff(nameKey), GetColoredName(monsterBuffTarget.Target)));
							}
							else
							{
								AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_APPLY_BUFF", GetColoredBuff(nameKey), GetColoredName(monsterBuffTarget.Target)));
							}
						}
						else if (staticData2.IsDebuff)
						{
							AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_IS_IMMUNE_AGAINST", GetColoredName(monsterBuffTarget.Target), GetColoredDebuff(nameKey)));
						}
						else
						{
							AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_IS_IMMUNE_AGAINST", GetColoredName(monsterBuffTarget.Target), GetColoredBuff(nameKey)));
						}
					}
				}
				else if (spellTarget is PushedTarget)
				{
					PushedTarget pushedTarget = (PushedTarget)spellTarget;
					if (!pushedTarget.Success)
					{
						AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_PUSH_BACK_FAILED", GetColoredName(pushedTarget.Target)));
					}
				}
			}
		}

		private void AddMonsterBuffText(MonsterBuffEntryEventArgs p_logEntry)
		{
			if (p_logEntry.Successful)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_APPLY_BUFF", GetColoredDebuff(p_logEntry.Buff.NameKey), GetColoredName(p_logEntry.Target)));
			}
			else
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_IS_IMMUNE_AGAINST", GetColoredName(p_logEntry.Target), GetColoredDebuff(p_logEntry.Buff.NameKey)));
			}
		}

		private void AddAbilityTriggeredText(AbilityTriggeredEventArgs p_logEntry)
		{
			AddLogEntry(LocaManager.GetText("ACTION_LOG_ABILITY_TRIGGERED", GetColoredName(p_logEntry.Monster), GetColoredSpell(p_logEntry.Ability.StaticData.NameKey)));
		}

		private void AddMapPointAddedText(MapPointAddedEventArgs p_mapPointAddedEventArgs)
		{
			AddLogEntry(LocaManager.GetText("ACTION_LOG_NEW_LOCATION_ADDED_TO_WORLD_MAP", GetColoredName(p_mapPointAddedEventArgs.Point)));
		}

		private void AddMonsterLootText(MonsterLootEntryEventArgs p_logEntry)
		{
			if (p_logEntry.Item != null)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_LOOT_ITEM", GetColoredName(p_logEntry.Monster), GetColoredItemName(p_logEntry.Item)));
			}
			else if (p_logEntry.Gold > 0)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_LOOT_GOLD", GetColoredGoldAmount(p_logEntry.Gold)));
			}
			else
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_LOOT_INVENTORY_FULL"));
			}
		}

		private void AddQuestLootText(QuestLootEntryEventArgs p_logEntry)
		{
			if (p_logEntry.Item != null)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_QUESTLOOT_ITEM", GetColoredItemName(p_logEntry.Item), GetColoredName(p_logEntry.Quest)));
			}
			else if (p_logEntry.Gold > 0)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_QUESTLOOT_GOLD", GetColoredGoldAmount(p_logEntry.Gold), GetColoredName(p_logEntry.Quest)));
			}
			else
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_QUESTLOOT_INVENTORY_FULL"));
			}
		}

		private void AddChestLootText(ChestLootEventArgs p_logEntry)
		{
			if (p_logEntry.Items != null)
			{
				foreach (BaseItem baseItem in p_logEntry.Items)
				{
					if (baseItem is Consumable && ((Consumable)baseItem).Counter > 1)
					{
						AddLogEntry(LocaManager.GetText("ACTION_LOG_CHESTLOOT_ITEM_STACK", m_colorItemHex + ((Consumable)baseItem).Counter + "[-]", GetColoredItemName(baseItem)));
					}
					else
					{
						AddLogEntry(LocaManager.GetText("ACTION_LOG_CHESTLOOT_ITEM", GetColoredItemName(baseItem)));
					}
				}
			}
			if (p_logEntry.Gold > 0)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_CHESTLOOT_GOLD", GetColoredGoldAmount(p_logEntry.Gold)));
			}
		}

		private void AddMonsterBuffDamageText(MonsterBuffDamageEntryEventArgs p_logEntry)
		{
			if (p_logEntry.Buff.IsDebuff)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_SPELL_HIT", GetColoredDebuff(p_logEntry.Buff.NameKey), GetColoredName(p_logEntry.Target), GetColoredReflect(p_logEntry.AttackResult.DamageDone)));
			}
			else if (p_logEntry.AttackResult.DamageResults[0].Type == EDamageType.HEAL)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_POTION_RESTORE_HP", GetColoredBuff(p_logEntry.Buff.NameKey), GetColoredName(p_logEntry.Target), GetColoredHealValue(p_logEntry.AttackResult.DamageDone)));
			}
		}

		private void AddTokenText(TokenAcquiredEventArgs p_args)
		{
			if (p_args.TokenID >= 1 && p_args.TokenID <= 6)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_BLESSING_ACQUIRED", GetColoredBlessingName(p_args.Name)));
			}
			else if (p_args.TokenID >= 7 && p_args.TokenID <= 10)
			{
				AddLogEntry((String)GetColoredBlessingName(p_args.Name));
			}
			else if (p_args.TokenID >= 11 && p_args.TokenID <= 22)
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_CLASS_UNLOCKED", GetColoredBlessingName(p_args.Name)));
			}
			else
			{
				AddLogEntry(LocaManager.GetText("ACTION_LOG_TOKEN_ACQUIRED", GetColoredBlessingName(p_args.Name)));
			}
		}

		private void AddTokenRemovedText(TokenRemovedEventArgs p_args)
		{
			AddLogEntry(LocaManager.GetText("ACTION_LOG_TOKEN_REMOVED", GetColoredBlessingName(p_args.Name)));
		}

		private void AddTrapTriggeredText(TrapTriggeredEventArgs p_args)
		{
			AddLogEntry(LocaManager.GetText("TRAP_TRIGGERED", GetColoredName(p_args.Trap)));
		}

		private void AddTrapDamageText(TrapDamageEventArgs p_logEntry)
		{
			m_stringBuilder.Rest();
			String id = "ACTION_LOG_TRAP_DAMAGE_M";
			if (p_logEntry.Target.Gender == EGender.FEMALE)
			{
				id = "ACTION_LOG_TRAP_DAMAGE_F";
			}
			LocaManager.AppendText(m_stringBuilder, id, GetColoredName(p_logEntry.Target), GetColoredDamage(p_logEntry.Result));
			AddLogEntry(m_stringBuilder);
			if (p_logEntry.ConditionReceived != ECondition.NONE)
			{
				String conditionText = GetConditionText(p_logEntry.ConditionReceived, p_logEntry.Target, true);
				AddLogEntry(LocaManager.GetText(conditionText, GetColoredName(p_logEntry.Target)));
			}
		}

		private void AddTrapManaLostText(TrapManaLostEventArgs p_args)
		{
			m_stringBuilder.Rest();
			String id = "ACTION_LOG_TRAP_MANA_LOST_M";
			if (p_args.Target.Gender == EGender.FEMALE)
			{
				id = "ACTION_LOG_TRAP_MANA_LOST_F";
			}
			LocaManager.AppendText(m_stringBuilder, id, GetColoredName(p_args.Target), GetColoredMana(p_args.Amount));
			AddLogEntry(m_stringBuilder);
		}

		private void AddTrapFoodLostText(TrapFoodLostEventArgs p_args)
		{
			AddLogEntry(LocaManager.GetText("ACTION_LOG_TRAP_FOOD_LOST", m_colorHealHex + p_args.Amount + "[-]"));
		}

		private void AddExtraAttackText(ExtraAttackEventArgs p_logEntry)
		{
			String id = "ACTION_LOG_EXTRA_ATTACK_M";
			if (p_logEntry.Character.Gender == EGender.FEMALE)
			{
				id = "ACTION_LOG_EXTRA_ATTACK_F";
			}
			AddLogEntry(LocaManager.GetText(id, GetColoredName(p_logEntry.Character)));
		}

		private void AddBuffRemovedText(BuffRemovedEventArgs p_logEntry)
		{
			if (p_logEntry.LostBy is Party)
			{
				String name = (p_logEntry.Buff as PartyBuff).StaticData.Name;
				AddLogEntry(LocaManager.GetText("ACTION_LOG_COMBAT_REMOVED_PARTYBUFF", GetColoredBuff(name), LocaManager.GetText("PARTY")));
			}
		}

		private void AddDamagePreventedText(DamagePreventedEntryEventArgs p_logEntry)
		{
			String name = p_logEntry.Buff.StaticData.Name;
			AddLogEntry(LocaManager.GetText("ACTION_LOG_PARTY_BUFF_PREVENTED_DAMAGE", GetColoredBuff(name), m_colorDamageHex + p_logEntry.DamagePrevented.ToString() + "[-]"));
		}

		private void AddMonsterDamagePreventedText(MonsterDamagePreventedEntryEventArgs p_logEntry)
		{
			String nameKey = p_logEntry.Buff.NameKey;
			AddLogEntry(LocaManager.GetText("ACTION_LOG_PARTY_BUFF_PREVENTED_DAMAGE", GetColoredBuff(nameKey), m_colorDamageHex + p_logEntry.DamagePrevented.ToString() + "[-]"));
		}

		private void AddSkillTierBonusText(SkillTierBonusEventArgs p_logEntry)
		{
			String text = "ACTION_LOG_COMBAT_SKILL_BONUS_TRIGGERED";
			if (p_logEntry.Chara.Gender == EGender.FEMALE)
			{
				text += "_F";
			}
			else
			{
				text += "_M";
			}
			String text2 = LocaManager.GetText("SKILL_TIER_" + ((Int32)p_logEntry.Tier).ToString());
			AddLogEntry(LocaManager.GetText(text, GetColoredName(p_logEntry.Chara), GetColoredSpell(p_logEntry.SkillName), text2));
		}

		private String GetColoredName(Object p_object)
		{
			m_tempStringBuilder.Rest();
			if (p_object is Character)
			{
				Character character = (Character)p_object;
				m_tempStringBuilder.Append(m_colorTargetFriendlyHex);
				m_tempStringBuilder.Append(character.Name);
				m_tempStringBuilder.Append("[-]");
			}
			else if (p_object is Monster)
			{
				Monster monster = (Monster)p_object;
				m_tempStringBuilder.Append(m_colorTargetFoeHex);
				LocaManager.AppendText(m_tempStringBuilder, monster.Name);
				m_tempStringBuilder.Append("[-]");
			}
			else if (p_object is QuestStep)
			{
				QuestStep questStep = (QuestStep)p_object;
				m_tempStringBuilder.Append(m_colorQuestHex);
				LocaManager.AppendText(m_tempStringBuilder, questStep.StaticData.Name);
				m_tempStringBuilder.Append("[-]");
			}
			else if (p_object is MonsterBuff)
			{
				MonsterBuff monsterBuff = (MonsterBuff)p_object;
				m_tempStringBuilder.Append(m_colorTargetFriendlyHex);
				LocaManager.AppendText(m_tempStringBuilder, monsterBuff.NameKey);
				m_tempStringBuilder.Append("[-]");
			}
			else if (p_object is BaseTrapEffect)
			{
				BaseTrapEffect baseTrapEffect = (BaseTrapEffect)p_object;
				m_tempStringBuilder.Append(m_colorTargetFoeHex);
				LocaManager.AppendText(m_tempStringBuilder, baseTrapEffect.StaticData.Name);
				m_tempStringBuilder.Append("[-]");
			}
			else if (p_object is WorldMapPoint)
			{
				m_tempStringBuilder.Append(m_colorTargetFriendlyHex);
				WorldMapPoint worldMapPoint = (WorldMapPoint)p_object;
				m_tempStringBuilder.Append(LocaManager.GetText(worldMapPoint.StaticData.NameKey));
				m_tempStringBuilder.Append("[-]");
			}
			else if (p_object is Party)
			{
				m_tempStringBuilder.Append(m_colorTargetFriendlyHex);
				m_tempStringBuilder.Append(LocaManager.GetText("PARTY"));
				m_tempStringBuilder.Append("[-]");
			}
			else if (p_object is Npc)
			{
				Npc npc = (Npc)p_object;
				m_tempStringBuilder.Append(m_colorTargetFriendlyHex);
				m_tempStringBuilder.Append(LocaManager.GetText(npc.StaticData.NameKey));
				m_tempStringBuilder.Append("[-]");
			}
			else
			{
				Debug.LogError("No Target for GetColoredName. Objects class was " + ((p_object == null) ? "NULL" : p_object.GetType().ToString()));
				m_tempStringBuilder.Append("[882200]Unspecified Object![-]");
			}
			return m_tempStringBuilder.ToString();
		}

		private String GetColoredItemName(BaseItem p_item)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorItemHex);
			m_tempStringBuilder.Append(p_item.Name);
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private String GetColoredGoldAmount(Int32 p_value)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorGoldHex);
			m_tempStringBuilder.Append(p_value);
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private String GetColoredHealValue(Int32 p_value)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorHealHex);
			m_tempStringBuilder.Append(p_value);
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private String GetColoredHealValue(String p_value)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorHealHex);
			m_tempStringBuilder.Append(p_value);
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private String GetColoredDamage(AttackResult p_attack)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorDamageHex);
			m_tempStringBuilder.Append(p_attack.DamageDone);
			if (p_attack.Result == EResultType.CRITICAL_HIT)
			{
				m_tempStringBuilder.Append(" (");
				LocaManager.AppendText(m_tempStringBuilder, "ACTION_LOG_COMBAT_CRITICAL");
				m_tempStringBuilder.Append(")");
			}
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private String GetColoredReflect(Int32 p_dmg)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorDamageHex);
			m_tempStringBuilder.Append(p_dmg.ToString());
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private String GetColoredMana(Int32 p_mana)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorManaHex);
			m_tempStringBuilder.Append(p_mana.ToString());
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private Object GetColoredSpell(String castedSpell)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorSkillHex);
			LocaManager.AppendText(m_tempStringBuilder, castedSpell);
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private Object GetColoredBuff(String p_buffName)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorBuffHex);
			LocaManager.AppendText(m_tempStringBuilder, p_buffName);
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private Object GetColoredDebuff(String p_debuffName)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorDebuffHex);
			LocaManager.AppendText(m_tempStringBuilder, p_debuffName);
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private Object GetColoredBlessingName(String p_name)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorItemHex);
			LocaManager.AppendText(m_tempStringBuilder, p_name);
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private Object GetColoredXP(Int32 p_experience)
		{
			m_tempStringBuilder.Rest();
			m_tempStringBuilder.Append(m_colorXPHex);
			m_tempStringBuilder.Append(p_experience.ToString());
			m_tempStringBuilder.Append("[-]");
			return m_tempStringBuilder.ToString();
		}

		private String GetResistedDamage(List<DamageResult> p_damageResults)
		{
			StringBuilder stringBuilder = new StringBuilder(String.Empty);
			Boolean flag = true;
			foreach (DamageResult damageResult in p_damageResults)
			{
				if (damageResult.ResistedValue > 0)
				{
					if (!flag)
					{
						stringBuilder.Append(" + ");
					}
					flag = false;
					String id = "SINGLE_DAMAGE_PHYSICAL";
					if (damageResult.Type == EDamageType.AIR)
					{
						id = "SINGLE_DAMAGE_AIR";
					}
					if (damageResult.Type == EDamageType.EARTH)
					{
						id = "SINGLE_DAMAGE_EARTH";
					}
					if (damageResult.Type == EDamageType.FIRE)
					{
						id = "SINGLE_DAMAGE_FIRE";
					}
					if (damageResult.Type == EDamageType.WATER)
					{
						id = "SINGLE_DAMAGE_WATER";
					}
					if (damageResult.Type == EDamageType.DARK)
					{
						id = "SINGLE_DAMAGE_DARK";
					}
					if (damageResult.Type == EDamageType.LIGHT)
					{
						id = "SINGLE_DAMAGE_LIGHT";
					}
					if (damageResult.Type == EDamageType.PRIMORDIAL)
					{
						id = "SINGLE_DAMAGE_PRIMORDIAL";
					}
					stringBuilder.Append(LocaManager.GetText(id, damageResult.ResistedValue));
				}
			}
			return stringBuilder.ToString();
		}

		private String GetSplitDamage(List<DamageResult> damageResults)
		{
			String text = String.Empty;
			Boolean flag = true;
			foreach (DamageResult damageResult in damageResults)
			{
				if (!flag)
				{
					text += " + ";
				}
				flag = false;
				String id = "SINGLE_DAMAGE_PHYSICAL";
				if (damageResult.Type == EDamageType.AIR)
				{
					id = "SINGLE_DAMAGE_AIR";
				}
				if (damageResult.Type == EDamageType.EARTH)
				{
					id = "SINGLE_DAMAGE_EARTH";
				}
				if (damageResult.Type == EDamageType.FIRE)
				{
					id = "SINGLE_DAMAGE_FIRE";
				}
				if (damageResult.Type == EDamageType.WATER)
				{
					id = "SINGLE_DAMAGE_WATER";
				}
				if (damageResult.Type == EDamageType.DARK)
				{
					id = "SINGLE_DAMAGE_DARK";
				}
				if (damageResult.Type == EDamageType.LIGHT)
				{
					id = "SINGLE_DAMAGE_LIGHT";
				}
				if (damageResult.Type == EDamageType.PRIMORDIAL)
				{
					id = "SINGLE_DAMAGE_PRIMORDIAL";
				}
				text += LocaManager.GetText(id, damageResult.EffectiveValue);
			}
			return text;
		}

		public void OnScrollBarChange(UIScrollBar p_scrollBar)
		{
			Int32 currentLine = m_currentLine;
			Single num = m_logEntries.Count - m_lineCount;
			if (num > 0f)
			{
				m_currentLine = Mathf.RoundToInt(m_scrollBar.scrollValue * num);
			}
			else
			{
				m_currentLine = 0;
			}
			UpdateScrollBar();
			if (currentLine != m_currentLine)
			{
				UpdateText();
			}
		}

		public void OnHideButtonClicked()
		{
			m_minimized = !m_minimized;
			UpdateVisibility();
		}

		public void OnScrollBottomButtonClicked()
		{
			if (m_scrollBar.barSize < 1f)
			{
				m_currentLine = 0;
				UpdateScrollBar();
				UpdateText();
			}
		}

		public void OnSizeChanged()
		{
			Single pixelSizeAdjustment = m_uiRoot.pixelSizeAdjustment;
			Single num = m_buttonChangeSize.DragYTotalDelta * pixelSizeAdjustment;
			Int32 num2 = -(Int32)(num / m_lineHeight);
			m_sizeChangeSaveTimer = m_saveAfterTime;
			if (m_lineCount + num2 > m_maximumLines)
			{
				num2 = m_maximumLines - m_lineCount;
			}
			else if (m_lineCount + num2 < m_minimumLines)
			{
				num2 = m_minimumLines - m_lineCount;
			}
			if (num2 != 0)
			{
				m_lineCount += num2;
				m_buttonChangeSize.DragYTotalDelta += num2 * m_lineHeight / pixelSizeAdjustment;
				UpdateSize();
			}
		}

		public void OnScroll(Single p_delta)
		{
			if (!m_minimized)
			{
				Int32 num = m_logEntries.Count - m_lineCount;
				m_scrollBar.scrollValue += 1f / num * m_buttonScrollLines * p_delta * 10f;
			}
		}
	}
}
