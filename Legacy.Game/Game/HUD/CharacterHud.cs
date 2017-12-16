using System;
using System.Collections;
using System.Threading;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;
using Legacy.EffectEngine;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/CharacterHud")]
	public class CharacterHud : MonoBehaviour
	{
		private const Single SPEED_UPDATE_DAMAGE_BAR = 0.5f;

		private const Single TIME_SHOW_FULL_DAMAGE = 0.5f;

		private const Single ENABLE_FADE_TIME = 0.25f;

		private const String BACKGROUND_SELECTED_CHARACTER = "BTN_square_152_highlight";

		private const String BACKGROUND_NOT_SELECTED_CHARACTER = "BTN_square_152";

		[SerializeField]
		private UISprite m_portrait;

		[SerializeField]
		private UISprite m_body;

		[SerializeField]
		private UISprite m_portraitBackground;

		[SerializeField]
		private UIFilledSprite m_hpBar;

		[SerializeField]
		private UIFilledSprite m_mpBar;

		[SerializeField]
		private ParticleSystem m_selectionFlame;

		[SerializeField]
		private ParticleSystem m_selectionCombatFlame;

		[SerializeField]
		private GameObject m_levelUpFX;

		[SerializeField]
		private Color m_disabledColor = Color.white;

		[SerializeField]
		private PortraitXpBar m_xpBar;

		[SerializeField]
		private UIFilledSprite m_recentDamageHPBar;

		[SerializeField]
		private UIButton m_skillButton;

		[SerializeField]
		private UIButton m_attributeButton;

		[SerializeField]
		private CharacterHudInfoTooltip m_tooltipHp;

		[SerializeField]
		private CharacterHudInfoTooltip m_tooltipMp;

		[SerializeField]
		private UISprite m_bridge;

		[SerializeField]
		private GameObject m_mainPortrait;

		[SerializeField]
		private BoxCollider m_portraitCollider;

		[SerializeField]
		private GameObject m_root;

		[SerializeField]
		private Color m_damageBarNormalColor = new Color(255f, 0f, 0f, 255f);

		[SerializeField]
		private Color m_damageBarUnconsciousColor = new Color(255f, 0f, 0f, 128f);

		[SerializeField]
		private UILabel m_levelUpLabel;

		[SerializeField]
		private Single SHOW_LEVELUP_TEXT_DURATION;

		[SerializeField]
		private HUDDamageText m_HUDDamageText;

		[SerializeField]
		private CharacterHudStatManager m_statManager;

		[SerializeField]
		private Color m_hoverColor;

		[SerializeField]
		private Color m_selectionColor;

		[SerializeField]
		private Color m_selectionHoverColor;

		private Character m_character;

		private Boolean m_selected = true;

		private Boolean m_isDoubleClickAllowed = true;

		private Int32 m_index;

		private PortraitFXView m_fx;

		private Int32 m_lastHitFrame = -10;

		private Single m_timeStartUpdateDamagebar;

		private Boolean m_isHovered;

		private Color m_portraitBackgroundNormalColor;

		private Boolean m_enabled = true;

		private Boolean m_waitForEndOfConversation;

		private Boolean m_showLvlUpText;

		private Boolean m_isForDragging;

		private Boolean m_showingUnconsciousHealthBar;

		private Boolean m_isInitialized;

		private Boolean m_animateHPBar;

		public event EventHandler OnCharacterClicked;

		public Boolean Enabled => m_enabled;

	    public Int32 Index => m_index;

	    public Character Owner => m_character;

	    public Boolean IsForDragging
		{
			get => m_isForDragging;
	        set => m_isForDragging = value;
	    }

		public Boolean IsInitialized => m_isInitialized;

	    public void Init(Character p_character, Int32 p_index)
		{
			m_isDoubleClickAllowed = true;
			if (m_levelUpLabel != null)
			{
				m_levelUpLabel.enabled = false;
			}
			m_character = p_character;
			m_index = p_index;
			m_portrait.spriteName = p_character.Portrait;
			m_body.spriteName = p_character.Body;
			if (!m_isForDragging)
			{
				if (m_fx == null)
				{
					m_fx = gameObject.AddComponent<PortraitFXView>();
					m_fx.Init(m_character, m_HUDDamageText);
				}
				else
				{
					m_fx.ChangeCharacter(m_character);
				}
			}
			if (!m_isInitialized)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_DONE_TURN_UPDATE, new EventHandler(OnCharacterDoneTurn));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_SKILL_POINTS_CHANGED, new EventHandler(UpdateSkillButton));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTRIBUTE_POINTS_CHANGED, new EventHandler(UpdateAttributeButton));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacks));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggered));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChanged));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacksDelayed));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksDelayed));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpellDelayed));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FX_HIT, EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggeredDelayed));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FIXED_DELAY, EEventType.CHARACTER_XP_GAIN, new EventHandler(OnXpWon));
				LegacyLogic.Instance.ConversationManager.BeginConversation += HandleBeginConversation;
				LegacyLogic.Instance.ConversationManager.EndConversation += HandleEndConversation;
				m_isInitialized = true;
			}
			m_hpBar.fillAmount = m_character.HealthPoints / (Single)m_character.MaximumHealthPoints;
			m_mpBar.fillAmount = m_character.ManaPoints / (Single)m_character.MaximumManaPoints;
			m_portraitBackgroundNormalColor = m_portraitBackground.color;
			m_statManager.Init(m_character);
			OnConditionChange();
			m_statManager.ForceUpdate(m_character);
			m_tooltipHp.Init(CharacterHudInfoTooltip.ETooltipObject.HP_BAR, m_character);
			m_tooltipMp.Init(CharacterHudInfoTooltip.ETooltipObject.MP_BAR, m_character);
		}

		public void ChangeCharacter(Character p_character)
		{
			m_character = p_character;
			if (!m_isForDragging)
			{
				m_fx.ChangeCharacter(m_character);
			}
			m_statManager.ChangeCharacter(m_character);
			m_tooltipHp.ChangeCharacter(m_character);
			m_tooltipMp.ChangeCharacter(m_character);
			m_portrait.spriteName = p_character.Portrait;
			m_body.spriteName = p_character.Body;
			OnConditionChange();
			m_statManager.ForceUpdate(m_character);
			m_hpBar.fillAmount = m_character.HealthPoints / (Single)m_character.MaximumHealthPoints;
			m_mpBar.fillAmount = m_character.ManaPoints / (Single)m_character.MaximumManaPoints;
			m_recentDamageHPBar.fillAmount = 0f;
			SetSelected(LegacyLogic.Instance.WorldManager.Party.SelectedCharacter == m_character);
			m_xpBar.Clear();
			Boolean state = true;
			if (DragDropManager.Instance.DraggedItem is PortraitDragObject)
			{
				PortraitDragObject portraitDragObject = (PortraitDragObject)DragDropManager.Instance.DraggedItem;
				if (portraitDragObject.Index == m_index && portraitDragObject.Character == m_character)
				{
					state = false;
				}
			}
			NGUITools.SetActiveSelf(m_root, state);
			UpdateCharacterDoneTurnState();
		}

		public void Cleanup()
		{
			if (!m_isForDragging && m_fx != null)
			{
				m_fx.CleanUp();
			}
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_DONE_TURN_UPDATE, new EventHandler(OnCharacterDoneTurn));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_SKILL_POINTS_CHANGED, new EventHandler(UpdateSkillButton));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_ATTRIBUTE_POINTS_CHANGED, new EventHandler(UpdateAttributeButton));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacks));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacks));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpell));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggered));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.CHARACTER_STATUS_CHANGED, new EventHandler(OnCharacterStatusChanged));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS, new EventHandler(OnMonsterAttacksDelayed));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_ATTACKS_RANGED, new EventHandler(OnMonsterAttacksDelayed));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.MONSTER_CAST_SPELL, new EventHandler(OnMonsterCastSpellDelayed));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FX_HIT, EEventType.TRAP_TRIGGERED, new EventHandler(OnTrapTriggeredDelayed));
			DelayedEventManager.UnregisterEvent(EDelayType.ON_FIXED_DELAY, EEventType.CHARACTER_XP_GAIN, new EventHandler(OnXpWon));
			LegacyLogic.Instance.ConversationManager.BeginConversation -= HandleBeginConversation;
			LegacyLogic.Instance.ConversationManager.EndConversation -= HandleEndConversation;
			m_statManager.Cleanup();
		}

		private void HandleBeginConversation(Object sender, EventArgs e)
		{
			m_waitForEndOfConversation = true;
			m_isDoubleClickAllowed = false;
		}

		private void HandleEndConversation(Object sender, EventArgs e)
		{
			m_waitForEndOfConversation = false;
			m_isDoubleClickAllowed = true;
		}

		private void Update()
		{
			if (m_showLvlUpText && !m_waitForEndOfConversation)
			{
				m_showLvlUpText = false;
				ShowLevelUp();
			}
			if (m_hpBar != null)
			{
				if (m_showingUnconsciousHealthBar && m_hpBar.fillAmount > 0f)
				{
					m_showingUnconsciousHealthBar = false;
					m_recentDamageHPBar.fillAmount = m_hpBar.fillAmount;
					m_recentDamageHPBar.color = m_damageBarNormalColor;
				}
				if (m_timeStartUpdateDamagebar == 0f)
				{
					if (m_hpBar.fillAmount < m_recentDamageHPBar.fillAmount)
					{
						if (m_animateHPBar)
						{
							m_timeStartUpdateDamagebar = Time.time + 0.5f;
						}
						else if (!m_showingUnconsciousHealthBar)
						{
							m_recentDamageHPBar.fillAmount = m_hpBar.fillAmount;
							m_timeStartUpdateDamagebar = 0f;
						}
					}
					else if (m_hpBar.fillAmount > m_recentDamageHPBar.fillAmount)
					{
						m_recentDamageHPBar.fillAmount = m_hpBar.fillAmount;
					}
				}
				else
				{
					if (Time.time > m_timeStartUpdateDamagebar)
					{
						m_recentDamageHPBar.fillAmount -= Time.deltaTime * 0.5f;
					}
					if (m_showingUnconsciousHealthBar)
					{
						Single deadHealthPercent = m_character.GetDeadHealthPercent();
						if (deadHealthPercent > m_recentDamageHPBar.fillAmount)
						{
							m_recentDamageHPBar.fillAmount = deadHealthPercent;
							m_timeStartUpdateDamagebar = 0f;
							m_animateHPBar = false;
						}
					}
					else if (m_hpBar.fillAmount >= m_recentDamageHPBar.fillAmount)
					{
						if (m_hpBar.fillAmount == 0f)
						{
							m_recentDamageHPBar.fillAmount = 1f;
							m_showingUnconsciousHealthBar = true;
							m_recentDamageHPBar.color = m_damageBarUnconsciousColor;
						}
						else
						{
							m_recentDamageHPBar.fillAmount = m_hpBar.fillAmount;
							m_timeStartUpdateDamagebar = 0f;
							m_animateHPBar = false;
						}
					}
				}
			}
		}

		private void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (enabled)
			{
				if (m_selected)
				{
					TweenColor.Begin(m_portraitBackground.gameObject, 0.1f, (!p_isOver) ? m_selectionColor : m_selectionHoverColor);
				}
				else
				{
					TweenColor.Begin(m_portraitBackground.gameObject, 0.1f, (!p_isOver) ? m_portraitBackgroundNormalColor : m_hoverColor);
				}
			}
		}

		private void OnTooltip(Boolean p_show)
		{
			if (p_show)
			{
				String text = m_character.Class.NameKey;
				if (m_character.Gender == EGender.MALE)
				{
					text += "_M";
				}
				else
				{
					text += "_F";
				}
				String p_tooltipText = LocaManager.GetText("CHARACTER_LEVEL", m_character.Level) + " " + LocaManager.GetText(text);
				TooltipManager.Instance.Show(this, m_character.Name, p_tooltipText, TextTooltip.ESize.MEDIUM, transform.position, transform.localScale * 0.5f);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		private void OnMonsterAttacks(Object p_sender, EventArgs p_args)
		{
			AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
			foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
			{
				if (attackedTarget.AttackTarget == m_character)
				{
					m_lastHitFrame = Time.frameCount;
				}
				if (attacksEventArgs.Counterattack && !m_isForDragging)
				{
					m_fx.OnMonsterCounterAttack(attacksEventArgs);
				}
			}
		}

		private void OnMonsterAttacksDelayed(Object p_sender, EventArgs p_args)
		{
			AttacksEventArgs attacksEventArgs = (AttacksEventArgs)p_args;
			foreach (AttacksEventArgs.AttackedTarget attackedTarget in attacksEventArgs.Attacks)
			{
				if (attackedTarget.AttackTarget == m_character)
				{
					UpdateHPBar();
					OnConditionChange();
				}
			}
		}

		private void OnMonsterCastSpell(Object p_sender, EventArgs p_args)
		{
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			foreach (AttackedTarget attackedTarget in spellEventArgs.SpellTargetsOfType<AttackedTarget>())
			{
				if (attackedTarget.Target == m_character)
				{
					m_lastHitFrame = Time.frameCount;
				}
			}
		}

		private void OnMonsterCastSpellDelayed(Object p_sender, EventArgs p_args)
		{
			SpellEventArgs spellEventArgs = (SpellEventArgs)p_args;
			foreach (AttackedTarget attackedTarget in spellEventArgs.SpellTargetsOfType<AttackedTarget>())
			{
				if (attackedTarget.Target == m_character)
				{
					UpdateHPBar();
					OnConditionChange();
				}
			}
		}

		private void OnTrapTriggered(Object p_sender, EventArgs p_args)
		{
			m_lastHitFrame = Time.frameCount;
		}

		public void OnTrapTriggeredDelayed(Object p_sender, EventArgs p_args)
		{
			UpdateHPBar();
			OnConditionChange();
		}

		private void OnCharacterSelected(Object p_sender, EventArgs p_args)
		{
			if (p_sender == m_character)
			{
				SetSelected(true);
			}
			else if (m_selected)
			{
				SetSelected(false);
			}
		}

		private void OnCharacterDoneTurn(Object p_sender, EventArgs p_args)
		{
			if (p_sender == m_character)
			{
				UpdateCharacterDoneTurnState();
			}
		}

		private void OnXpWon(Object p_sender, EventArgs p_args)
		{
			if (!m_isForDragging && p_sender == m_character)
			{
				XpGainEventArgs xpGainEventArgs = (XpGainEventArgs)p_args;
				m_xpBar.AddXpWon(xpGainEventArgs.XpWon, xpGainEventArgs.LevelUp, xpGainEventArgs.PercentXPFilled);
				if (xpGainEventArgs.LevelUp > 0)
				{
					m_showLvlUpText = true;
				}
			}
		}

		private void ShowLevelUp()
		{
			if (!LegacyLogic.Instance.MapLoader.IsLoading)
			{
				if (m_levelUpFX != null)
				{
					Instantiate(m_levelUpFX, transform.position, transform.rotation);
					ShowLevelUpText();
					AudioManager.Instance.RequestPlayAudioID("LevelUp", 0);
				}
				else
				{
					Debug.LogError("Missing level up FX!");
				}
			}
		}

		private void ShowLevelUpText()
		{
			if (m_levelUpLabel != null)
			{
				m_levelUpLabel.enabled = true;
				m_levelUpLabel.StopAllCoroutines();
				m_levelUpLabel.StartCoroutine(InvokeDisableLevelUpText(SHOW_LEVELUP_TEXT_DURATION));
			}
		}

		private void OnDisable()
		{
			if (m_levelUpLabel != null)
			{
				m_levelUpLabel.StopAllCoroutines();
				m_levelUpLabel.enabled = false;
			}
		}

		private void OnStartSceneLoad(Object sender, EventArgs e)
		{
			if (m_levelUpLabel != null)
			{
				m_levelUpLabel.StopAllCoroutines();
				m_levelUpLabel.enabled = false;
			}
		}

		private IEnumerator InvokeDisableLevelUpText(Single waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			m_levelUpLabel.enabled = false;
			yield break;
		}

		public void SetSelected(Boolean p_selected)
		{
			m_selected = p_selected;
			if (m_selectionFlame != null)
			{
				if (m_selected)
				{
					m_selectionFlame.Play();
				}
				else
				{
					m_selectionFlame.Stop();
				}
			}
			if (m_selectionCombatFlame != null)
			{
				m_selectionCombatFlame.Stop();
			}
			if (m_bridge != null)
			{
				NGUITools.SetActiveSelf(m_bridge.gameObject, m_selected);
			}
			if (m_selected)
			{
				m_portraitBackground.spriteName = "BTN_square_152_highlight";
				TweenColor.Begin(m_portraitBackground.gameObject, 0.1f, (!m_isHovered) ? m_selectionColor : m_selectionHoverColor);
			}
			else
			{
				m_portraitBackground.spriteName = "BTN_square_152";
				TweenColor.Begin(m_portraitBackground.gameObject, 0.1f, (!m_isHovered) ? m_portraitBackgroundNormalColor : m_hoverColor);
			}
		}

		public void ShowFlames()
		{
			SetSelected(m_selected);
		}

		public void HideFlames()
		{
			m_selectionCombatFlame.Stop();
			m_selectionFlame.Stop();
		}

		public void SetEnabled(Boolean p_enabled)
		{
			m_enabled = p_enabled;
			if (p_enabled)
			{
				TweenColor.Begin(m_portrait.gameObject, 0.25f, Color.white);
				TweenColor.Begin(m_body.gameObject, 0.25f, Color.white);
			}
			else
			{
				TweenColor.Begin(m_portrait.gameObject, 0.25f, m_disabledColor);
				TweenColor.Begin(m_body.gameObject, 0.25f, m_disabledColor);
			}
		}

		private void UpdateCharacterDoneTurnState()
		{
			if (m_character.DoneTurn)
			{
				SetEnabled(false);
			}
			else
			{
				SetEnabled(true);
			}
		}

		private void OnCharacterStatusChanged(Object p_sender, EventArgs p_args)
		{
			StatusChangedEventArgs statusChangedEventArgs = (StatusChangedEventArgs)p_args;
			if (p_sender == m_character)
			{
				if (statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.HEALTH_POINTS)
				{
					if (m_lastHitFrame != Time.frameCount)
					{
						UpdateHPBar();
					}
				}
				else if (statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.MANA_POINTS)
				{
					m_mpBar.fillAmount = m_character.ManaPoints / (Single)m_character.MaximumManaPoints;
				}
				else if (statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.STATUS)
				{
					if (LegacyLogic.Instance.WorldManager.Party.HasAggro())
					{
						if (m_lastHitFrame != Time.frameCount)
						{
							m_mpBar.fillAmount = m_character.ManaPoints / (Single)m_character.MaximumManaPoints;
							UpdateHPBar();
							OnConditionChange();
						}
					}
					else
					{
						m_hpBar.fillAmount = m_character.HealthPoints / (Single)m_character.MaximumHealthPoints;
						m_mpBar.fillAmount = m_character.ManaPoints / (Single)m_character.MaximumManaPoints;
					}
				}
				else if (statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.CONDITIONS && m_lastHitFrame != Time.frameCount)
				{
					OnConditionChange();
				}
			}
		}

		private void UpdateHPBar()
		{
			m_animateHPBar = true;
			m_hpBar.fillAmount = m_character.HealthPoints / (Single)m_character.MaximumHealthPoints;
		}

		private void OnConditionChange()
		{
			ECondition visibleCondition = m_character.ConditionHandler.GetVisibleCondition();
			NGUITools.SetActive(m_body.gameObject, visibleCondition != ECondition.DEAD);
			ECondition econdition = visibleCondition;
			switch (econdition)
			{
			case ECondition.NONE:
				m_portrait.spriteName = m_character.Portrait;
				break;
			case ECondition.DEAD:
				m_portrait.spriteName = "PIC_skull";
				break;
			case ECondition.UNCONSCIOUS:
			{
				UISprite portrait = m_portrait;
				String spriteName = m_character.Portrait.Replace("_idle", "_unconscious");
				m_portrait.spriteName = spriteName;
				portrait.spriteName = spriteName;
				break;
			}
			default:
				if (econdition != ECondition.SLEEPING)
				{
					if (econdition != ECondition.POISONED)
					{
						if (econdition != ECondition.CONFUSED)
						{
							if (econdition != ECondition.WEAK)
							{
								if (econdition == ECondition.CURSED)
								{
									UISprite portrait2 = m_portrait;
									String spriteName = m_character.Portrait.Replace("_idle", "_curse");
									m_portrait.spriteName = spriteName;
									portrait2.spriteName = spriteName;
								}
							}
							else
							{
								UISprite portrait3 = m_portrait;
								String spriteName = m_character.Portrait.Replace("_idle", "_weak");
								m_portrait.spriteName = spriteName;
								portrait3.spriteName = spriteName;
							}
						}
						else
						{
							UISprite portrait4 = m_portrait;
							String spriteName = m_character.Portrait.Replace("_idle", "_weak");
							m_portrait.spriteName = spriteName;
							portrait4.spriteName = spriteName;
						}
					}
					else
					{
						UISprite portrait5 = m_portrait;
						String spriteName = m_character.Portrait.Replace("_idle", "_poison");
						m_portrait.spriteName = spriteName;
						portrait5.spriteName = spriteName;
					}
				}
				else
				{
					UISprite portrait6 = m_portrait;
					String spriteName = m_character.Portrait.Replace("_idle", "_sleep");
					m_portrait.spriteName = spriteName;
					portrait6.spriteName = spriteName;
				}
				break;
			case ECondition.PARALYZED:
			{
				UISprite portrait7 = m_portrait;
				String spriteName = m_character.Portrait.Replace("_idle", "_paralyze");
				m_portrait.spriteName = spriteName;
				portrait7.spriteName = spriteName;
				break;
			}
			case ECondition.STUNNED:
			{
				UISprite portrait8 = m_portrait;
				String spriteName = m_character.Portrait.Replace("_idle", "_ko");
				m_portrait.spriteName = spriteName;
				portrait8.spriteName = spriteName;
				break;
			}
			}
			m_statManager.ForceUpdate(m_character);
		}

		public void OnClick()
		{
			if (OnCharacterClicked != null)
			{
				OnCharacterClicked(this, EventArgs.Empty);
			}
		}

		private void OnDoubleClick()
		{
			if (m_isDoubleClickAllowed)
			{
				OnClick();
				IngameController.Instance.ToggleInventory(this, EventArgs.Empty);
			}
		}

		private void OnDrag()
		{
			if (ConfigManager.Instance.Options.LockCharacterOrder)
			{
				return;
			}
			if (DragDropManager.Instance.DraggedItem == null && UICamera.currentTouchID == -1)
			{
				DragDropManager.Instance.StartDrag(new PortraitDragObject(m_character, m_index));
				NGUITools.SetActiveSelf(m_root, false);
			}
		}

		private void OnPress(Boolean p_isDown)
		{
			if (ConfigManager.Instance.Options.LockCharacterOrder)
			{
				return;
			}
			if (UICamera.currentTouchID == -1 && !p_isDown && DragDropManager.Instance.DraggedItem is PortraitDragObject && ((PortraitDragObject)DragDropManager.Instance.DraggedItem).Index == m_index)
			{
				DragDropManager.Instance.StopDrag();
				EndDrag();
			}
		}

		public void EndDrag()
		{
			NGUITools.SetActiveSelf(m_root, true);
		}

		public virtual void OnDragHover(DragHoverEventArgs p_eventArgs)
		{
			if (DragDropManager.Instance.DraggedItem is PortraitDragObject)
			{
				PortraitDragObject portraitDragObject = (PortraitDragObject)DragDropManager.Instance.DraggedItem;
				if (p_eventArgs.IsHovered)
				{
					if (portraitDragObject.Index != m_index)
					{
						LegacyLogic.Instance.WorldManager.Party.ChangeCharacterOrder(portraitDragObject.Character, portraitDragObject.Index);
						LegacyLogic.Instance.WorldManager.Party.ChangeCharacterOrder(portraitDragObject.Character, m_index);
						NGUITools.SetActiveSelf(m_root, false);
					}
				}
				else
				{
					LegacyLogic.Instance.WorldManager.Party.ChangeCharacterOrder(portraitDragObject.Character, portraitDragObject.Index);
				}
			}
		}

		private void OnDrop()
		{
			if (UICamera.currentTouchID == -1)
			{
				if (DragDropManager.Instance.DraggedItem is ItemDragObject)
				{
					ItemDragObject itemDragObject = (ItemDragObject)DragDropManager.Instance.DraggedItem;
					if (itemDragObject.ItemSlot != null && !m_character.DoneTurn && !m_character.ConditionHandler.CantDoAnything())
					{
						LegacyLogic.Instance.WorldManager.Party.SelectCharacter(m_character.Index);
						Int32 index = itemDragObject.ItemSlot.Index;
						Potion potion = itemDragObject.Item as Potion;
						Equipment equipment = itemDragObject.Item as Equipment;
						PartyInventoryController partyInventoryController = itemDragObject.ItemSlot.Parent.Inventory as PartyInventoryController;
						if (potion != null && partyInventoryController != null)
						{
							partyInventoryController.ConsumeItem(index, m_character.Index);
						}
						if (equipment != null)
						{
							Int32 autoSlot = (Int32)m_character.Equipment.GetAutoSlot(equipment);
							if (autoSlot >= 0 && m_character.Equipment.IsItemPlaceableAt(equipment, autoSlot))
							{
								EquipCommand p_command = new EquipCommand(m_character.Equipment, partyInventoryController, m_character.Equipment.GetItemAt((EEquipSlots)autoSlot), equipment, autoSlot, index);
								if (LegacyLogic.Instance.UpdateManager.PartyTurnActor.CanDoCommand(p_command, LegacyLogic.Instance.WorldManager.Party.CurrentCharacter))
								{
									LegacyLogic.Instance.CommandManager.AddCommand(p_command);
								}
							}
						}
					}
					DragDropManager.Instance.CancelDragAction();
				}
				else if (DragDropManager.Instance.DraggedItem is PortraitDragObject)
				{
					PortraitDragObject portraitDragObject = (PortraitDragObject)DragDropManager.Instance.DraggedItem;
					LegacyLogic.Instance.WorldManager.Party.ChangeCharacterOrder(portraitDragObject.Character, m_index);
					DragDropManager.Instance.EndDragAction();
				}
				else
				{
					DragDropManager.Instance.CancelDragAction();
				}
			}
		}

		private void OnAttributeButtonClick()
		{
			if (m_isDoubleClickAllowed)
			{
				IngameController.Instance.OpenAttributePointsWindow(this, EventArgs.Empty);
			}
		}

		public void UpdateAttributeButton(Object p_sender, EventArgs p_args)
		{
			if (p_sender == m_character)
			{
				UpdateAttributeButton();
			}
		}

		public void UpdateAttributeButton()
		{
			if (m_attributeButton != null)
			{
				NGUITools.SetActive(m_attributeButton.gameObject, m_character.AttributePoints > 0);
			}
		}

		private void OnSkillButtonClick()
		{
			if (m_isDoubleClickAllowed)
			{
				OnClick();
				IngameController.Instance.ToggleSkillMenu(this, EventArgs.Empty);
			}
		}

		public void UpdateSkillButton(Object p_sender, EventArgs p_args)
		{
			if (p_sender == m_character)
			{
				UpdateSkillButton();
			}
		}

		public void UpdateSkillButton()
		{
			if (m_skillButton != null)
			{
				NGUITools.SetActive(m_skillButton.gameObject, m_character.SkillPoints > 0);
			}
		}

		public void MovePortraitInFront()
		{
			Vector3 localPosition = m_mainPortrait.transform.localPosition;
			localPosition.z = -10f;
			m_mainPortrait.transform.localPosition = localPosition;
			Vector3 center = m_portraitCollider.center;
			center.z = -10f;
			m_portraitCollider.center = center;
		}

		public void MovePortraitBack()
		{
			Vector3 localPosition = m_mainPortrait.transform.localPosition;
			localPosition.z = 0f;
			m_mainPortrait.transform.localPosition = localPosition;
			Vector3 center = m_portraitCollider.center;
			center.z = 0f;
			m_portraitCollider.center = center;
		}
	}
}
