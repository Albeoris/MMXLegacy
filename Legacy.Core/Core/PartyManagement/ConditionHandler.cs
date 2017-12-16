using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.PartyManagement
{
	public class ConditionHandler : ISaveGameObject
	{
		private static readonly MMTime ZeroTime = new MMTime(0, 0, 0);

		private ECondition m_conditions;

		private ECondition m_lastAddedCondition;

		private ECondition m_lastRemovedCondition;

		private Single m_poisonHealthDamagePercent;

		private Single m_poisonEvadeDecrease;

		private Character m_character;

		private MMTime m_lastHandelTime;

		private MMTime m_timeDelta;

		private MMTime m_tick;

		private MMTime m_deficiencySyndromesRest;

		private Int32 m_knockedOutTurnCount;

		private Int32 m_sentEvent;

		private List<LogEntryEventArgs> m_logEntries;

		private List<LogEntryEventArgs> m_getKilledEntries;

		public ConditionHandler(Character p_character)
		{
			m_character = p_character;
			m_lastHandelTime = LegacyLogic.Instance.GameTime.Time;
			m_poisonHealthDamagePercent = ConfigManager.Instance.Game.PoisonHealthDamage;
			m_tick = new MMTime(ConfigManager.Instance.Game.MinutesDeficiencySyndromesTick, 0, 0);
			m_deficiencySyndromesRest = default(MMTime);
			m_deficiencySyndromesRest.AddMinutes(ConfigManager.Instance.Game.HoursDeficiencySyndromesRest * 60);
			m_poisonEvadeDecrease = ConfigManager.Instance.Game.PoisonEvadeDecrease;
			m_sentEvent = 0;
			m_logEntries = new List<LogEntryEventArgs>();
			m_getKilledEntries = new List<LogEntryEventArgs>();
		}

		public Single PoisonEvadeDecrease => m_poisonEvadeDecrease;

	    public Int32 KnockedOutTurnCount => m_knockedOutTurnCount;

	    public Int32 HUDStarved
		{
			get => m_sentEvent;
	        set => m_sentEvent = value;
	    }

		public ECondition LastRemovedCondition
		{
			get
			{
				ECondition lastRemovedCondition = m_lastRemovedCondition;
				m_lastRemovedCondition = ECondition.NONE;
				return lastRemovedCondition;
			}
		}

		public ECondition Condition => m_conditions;

	    public ECondition GetVisibleCondition()
		{
			if (HasCondition(ECondition.DEAD))
			{
				return ECondition.DEAD;
			}
			if (HasCondition(ECondition.UNCONSCIOUS))
			{
				return ECondition.UNCONSCIOUS;
			}
			if (HasCondition(ECondition.PARALYZED))
			{
				return ECondition.PARALYZED;
			}
			if (HasCondition(ECondition.STUNNED))
			{
				return ECondition.STUNNED;
			}
			if (HasCondition(ECondition.SLEEPING))
			{
				return ECondition.SLEEPING;
			}
			if (HasCondition(ECondition.POISONED))
			{
				return ECondition.POISONED;
			}
			if (HasOneCondition(ECondition.CONFUSED | ECondition.WEAK | ECondition.CURSED))
			{
				return m_lastAddedCondition;
			}
			return ECondition.NONE;
		}

		public Boolean CantDoAnything()
		{
			return HasOneCondition(ECondition.DEAD | ECondition.UNCONSCIOUS | ECondition.PARALYZED | ECondition.STUNNED | ECondition.SLEEPING);
		}

		public Boolean CantDoAnythingForGameOver()
		{
			return HasOneCondition(ECondition.DEAD | ECondition.PARALYZED);
		}

		public void AddCondition(ECondition p_condition)
		{
			AddCondition(p_condition, true);
		}

		public void AddCondition(ECondition p_condition, Boolean p_canBeProtected)
		{
			if (HasCondition(p_condition))
			{
				return;
			}
			if (HasCondition(ECondition.DEAD))
			{
				return;
			}
			Boolean p_cantDoAnythingBefore = CantDoAnything();
			Party party = LegacyLogic.Instance.WorldManager.Party;
			PartyBuffHandler buffs = party.Buffs;
			switch (p_condition)
			{
			case ECondition.DEAD:
				m_conditions = ECondition.NONE;
				m_character.BarkHandler.TriggerBark(EBarks.DEAD);
				break;
			case ECondition.UNCONSCIOUS:
				m_character.BarkHandler.TriggerBark(EBarks.UNCONSCIOUS);
				RemoveCondition(ECondition.SLEEPING);
				RemoveCondition(ECondition.STUNNED);
				RemoveCondition(ECondition.PARALYZED);
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.UNCONSCIOUS);
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
									if (p_canBeProtected && Random.Range(0f, 1f) < m_character.FightValues.ConditionProtectionCurses)
									{
										return;
									}
									m_character.BarkHandler.TriggerBark(EBarks.CURSED);
								}
							}
							else
							{
								if (p_canBeProtected && Random.Range(0f, 1f) < m_character.FightValues.ConditionProtectionWeakness)
								{
									return;
								}
								m_character.BarkHandler.TriggerBark(EBarks.WEAK);
							}
						}
						else
						{
							if (p_canBeProtected && Random.Range(0f, 1f) < m_character.FightValues.ConditionProtectionConfusion)
							{
								return;
							}
							m_character.BarkHandler.TriggerBark(EBarks.CONFUSED);
						}
					}
					else
					{
						if (HasCondition(ECondition.UNCONSCIOUS))
						{
							return;
						}
						if (p_canBeProtected && Random.Range(0f, 1f) < m_character.FightValues.ConditionProtectionPoison)
						{
							return;
						}
						m_character.BarkHandler.TriggerBark(EBarks.POISONED);
						LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.CONDITIONS);
					}
				}
				else
				{
					if (buffs.HasBuff(EPartyBuffs.BURNING_DETERMINATION))
					{
						return;
					}
					if (HasOneCondition(ECondition.UNCONSCIOUS | ECondition.PARALYZED | ECondition.STUNNED))
					{
						return;
					}
					if (p_canBeProtected && Random.Range(0f, 1f) < m_character.FightValues.ConditionProtectionSleep)
					{
						return;
					}
					LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.INCAPACITATING);
				}
				break;
			case ECondition.PARALYZED:
				if (buffs.HasBuff(EPartyBuffs.BURNING_DETERMINATION))
				{
					return;
				}
				if (HasCondition(ECondition.UNCONSCIOUS))
				{
					return;
				}
				if (p_canBeProtected && Random.Range(0f, 1f) < m_character.FightValues.ConditionProtectionParalysis)
				{
					return;
				}
				RemoveCondition(ECondition.SLEEPING);
				RemoveCondition(ECondition.STUNNED);
				m_character.BarkHandler.TriggerBark(EBarks.PARALYZED);
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.INCAPACITATING);
				break;
			case ECondition.STUNNED:
				if (buffs.HasBuff(EPartyBuffs.BURNING_DETERMINATION))
				{
					return;
				}
				if (HasOneCondition(ECondition.UNCONSCIOUS | ECondition.PARALYZED))
				{
					return;
				}
				if (p_canBeProtected && Random.Range(0f, 1f) < m_character.FightValues.ConditionProtectionKnockOut)
				{
					return;
				}
				RemoveCondition(ECondition.SLEEPING);
				m_knockedOutTurnCount = 0;
				m_character.BarkHandler.TriggerBark(EBarks.STUNNED);
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.INCAPACITATING);
				break;
			}
			Boolean flag = HasCondition(p_condition);
			if (!flag)
			{
				if (p_condition == ECondition.CONFUSED || p_condition == ECondition.WEAK || p_condition == ECondition.CURSED)
				{
					m_lastAddedCondition = p_condition;
				}
				m_conditions |= p_condition;
				if (m_conditions != ECondition.POISONED)
				{
					m_character.CalculateCurrentAttributes();
				}
				else
				{
					ModifyFightValues();
				}
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
				NotifyGUI(p_condition, p_cantDoAnythingBefore);
			}
			if (p_condition != ECondition.UNCONSCIOUS && p_condition != ECondition.DEAD)
			{
				ConditionChangedEntryEventArgs item = new ConditionChangedEntryEventArgs(m_character, p_condition, true);
				m_logEntries.Add(item);
			}
			else if (!flag)
			{
				ConditionChangedEntryEventArgs item2 = new ConditionChangedEntryEventArgs(m_character, p_condition, true);
				if (party.HasAggro())
				{
					m_getKilledEntries.Add(item2);
				}
				else
				{
					m_logEntries.Add(item2);
				}
			}
			m_character.GetCharacterActiveLeft();
		}

		public void RemoveCondition(ECondition p_condition)
		{
			if (!HasOneCondition(p_condition))
			{
				return;
			}
			m_lastRemovedCondition = p_condition;
			m_conditions &= ~p_condition;
			m_character.CalculateCurrentAttributes();
			ConditionChangedEntryEventArgs p_args = new ConditionChangedEntryEventArgs(m_character, p_condition, false);
			LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
			NotifyGUI();
		}

		public Boolean HasCondition(ECondition p_condition)
		{
			return (m_conditions & p_condition) == p_condition;
		}

		public Boolean HasOneCondition(ECondition p_conditionFlags)
		{
			return (m_conditions & p_conditionFlags) > ECondition.NONE;
		}

		public void ModifyAttributes(ref Attributes p_attributes)
		{
			if (HasCondition(ECondition.WEAK))
			{
				p_attributes.Might = (Int32)Math.Round(m_character.CurrentAttributes.Might * (1f - ConfigManager.Instance.Game.WeakAttribDecrease), MidpointRounding.AwayFromZero);
				p_attributes.Perception = (Int32)Math.Round(m_character.CurrentAttributes.Perception * (1f - ConfigManager.Instance.Game.WeakAttribDecrease), MidpointRounding.AwayFromZero);
				LegacyLogic.Instance.EventManager.InvokeEvent(m_character, EEventType.CHARACTER_STATUS_CHANGED, new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STARVED_WARNING));
			}
			if (HasCondition(ECondition.CURSED))
			{
				p_attributes.Magic = (Int32)(m_character.CurrentAttributes.Magic * (1f - ConfigManager.Instance.Game.CursedAttribDecrease));
				p_attributes.Destiny = (Int32)(m_character.CurrentAttributes.Destiny * (1f - ConfigManager.Instance.Game.CursedAttribDecrease));
			}
		}

		public void ModifyFightValues()
		{
			if (HasCondition(ECondition.POISONED))
			{
				m_character.FightValues.EvadeValue -= m_poisonEvadeDecrease;
				if (m_character.FightValues.EvadeValue < 0f)
				{
					m_character.FightValues.EvadeValue = 0f;
				}
			}
			if (HasCondition(ECondition.CURSED))
			{
				m_character.FightValues.MainHandAttackValue *= ConfigManager.Instance.Game.CursedAttackDecrease;
				m_character.FightValues.OffHandAttackValue *= ConfigManager.Instance.Game.CursedAttackDecrease;
				m_character.FightValues.RangedAttackValue *= ConfigManager.Instance.Game.CursedAttackDecrease;
			}
			if (CantDoAnything())
			{
				m_character.FightValues.EvadeValue = 0f;
				m_character.FightHandler.CurrentGeneralBlockAttempts = 0;
				m_character.FightHandler.CurrentMeleeBlockAttempts = 0;
			}
		}

		public void Update()
		{
			m_timeDelta += LegacyLogic.Instance.GameTime.Time - m_lastHandelTime;
			if (!LegacyLogic.Instance.WorldManager.Party.HasAggro())
			{
				while (m_timeDelta - m_tick >= ZeroTime)
				{
					m_lastHandelTime.AddMinutes(m_tick.Minutes);
					CheckDeficiencies();
					m_timeDelta -= m_tick;
				}
			}
			else
			{
				CheckDeficiencies();
			}
			m_lastHandelTime = LegacyLogic.Instance.GameTime.Time;
		}

		public void DoPoisonDamage()
		{
			if (HasCondition(ECondition.POISONED) && m_character.HealthPoints > 0)
			{
				Single num = m_character.FightValues.Resistance[EDamageType.EARTH].Value;
				Single num2 = 1f - num * 0.01f * ConfigManager.Instance.Game.MagicEvadeFactor;
				if (Random.Range(0f, 1f) < num2)
				{
					Int32 num3 = (Int32)Math.Round(m_character.MaximumHealthPoints * m_poisonHealthDamagePercent, MidpointRounding.AwayFromZero);
					num3 *= (Int32)Math.Round((100f - num) / 100f, MidpointRounding.AwayFromZero);
					if (num3 < 0 || m_character.HealthPoints <= 0)
					{
						num3 = 0;
					}
					if (num3 > m_character.HealthPoints)
					{
						num3 = m_character.HealthPoints;
					}
					if (num3 == 0)
					{
						return;
					}
					m_character.ChangeHP(-num3);
					m_character.CalculateCurrentAttributes();
					AttackResult attackResult = new AttackResult();
					attackResult.Result = EResultType.HIT;
					attackResult.DamageResults.Add(new DamageResult(EDamageType.EARTH, num3, 0, 1f));
					CharacterSpell p_spell = SpellFactory.CreateCharacterSpell(ECharacterSpell.SPELL_EARTH_POISON_SPRAY);
					ConditionEffectTarget item = new ConditionEffectTarget(m_character, ECondition.POISONED, attackResult);
					SpellEffectEntryEventArgs item2 = new SpellEffectEntryEventArgs(this, new SpellEventArgs(p_spell)
					{
						Result = ESpellResult.OK,
						SpellTargets = 
						{
							item
						}
					});
					m_getKilledEntries.Add(item2);
					DamageEventArgs p_eventArgs = new DamageEventArgs(attackResult);
					LegacyLogic.Instance.EventManager.InvokeEvent(m_character, EEventType.CHARACTER_TAKE_POISON_DAMAGE, p_eventArgs);
				}
			}
		}

		public void StartTurn()
		{
			if (HasCondition(ECondition.STUNNED))
			{
				m_knockedOutTurnCount++;
				Boolean flag = m_knockedOutTurnCount >= ConfigManager.Instance.Game.KnockedOutTurnCount;
				flag |= (Random.Value * 100f < m_character.CurrentAttributes.Vitality);
				if (flag)
				{
					RemoveCondition(ECondition.STUNNED);
				}
			}
			if (CantDoAnything())
			{
				m_character.FightHandler.CurrentGeneralBlockAttempts = 0;
				m_character.FightHandler.CurrentMeleeBlockAttempts = 0;
			}
		}

		public void CheckDeficiencies()
		{
			if (HasOneCondition(ECondition.DEAD | ECondition.UNCONSCIOUS))
			{
				return;
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Int32 p_minutes = (Int32)Math.Round(Math.Pow(m_character.CurrentAttributes.Magic, 1.5) + Math.Pow(Random.Range(0, m_character.CurrentAttributes.Destiny), 1.2), MidpointRounding.AwayFromZero);
			Int32 p_minutes2 = (Int32)Math.Round(Math.Pow(m_character.CurrentAttributes.Might, 1.5) + Math.Pow(Random.Range(0, m_character.CurrentAttributes.Destiny), 1.2), MidpointRounding.AwayFromZero);
			MMTime p_t = default(MMTime);
			p_t.AddMinutes(p_minutes);
			MMTime p_t2 = default(MMTime);
			p_t.AddMinutes(p_minutes2);
			if (m_lastHandelTime - m_character.LastRestTime >= new MMTime(0, 20, 0) && m_sentEvent == 0)
			{
				if (party.Buffs.HasBuff(EPartyBuffs.WELL_RESTED))
				{
					party.Buffs.RemoveBuff(EPartyBuffs.WELL_RESTED);
				}
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.RESTING);
				LegacyLogic.Instance.EventManager.InvokeEvent(m_character, EEventType.CHARACTER_STARVED, new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STARVED_WARNING));
				m_sentEvent++;
				m_character.BarkHandler.TriggerBark(EBarks.NOT_RESTED, m_character);
			}
			if (m_lastHandelTime - m_character.LastRestTime >= m_deficiencySyndromesRest + p_t)
			{
				if (party.Buffs.HasBuff(EPartyBuffs.WELL_RESTED))
				{
					party.Buffs.RemoveBuff(EPartyBuffs.WELL_RESTED);
				}
				if (m_sentEvent == 1)
				{
					LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.EXHAUSTION);
					LegacyLogic.Instance.EventManager.InvokeEvent(m_character, EEventType.CHARACTER_STARVED, new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STARVED_ALERT));
					m_sentEvent++;
				}
				AddCondition(ECondition.CONFUSED, false);
			}
			if (m_lastHandelTime - m_character.LastRestTime >= m_deficiencySyndromesRest + p_t2)
			{
				if (party.Buffs.HasBuff(EPartyBuffs.WELL_RESTED))
				{
					party.Buffs.RemoveBuff(EPartyBuffs.WELL_RESTED);
				}
				if (m_sentEvent == 1)
				{
					LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.EXHAUSTION);
					LegacyLogic.Instance.EventManager.InvokeEvent(m_character, EEventType.CHARACTER_STARVED, new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.STARVED_ALERT));
					m_sentEvent++;
				}
				AddCondition(ECondition.WEAK, false);
			}
		}

		public Boolean IsAffectedByExhausted()
		{
			return m_lastHandelTime - m_character.LastRestTime >= m_deficiencySyndromesRest;
		}

		public void Load(SaveGameData p_data)
		{
			m_conditions = (ECondition)p_data.Get<Int32>("Conditions", 0);
			SaveGameData saveGameData = p_data.Get<SaveGameData>("TimeDelta", null);
			if (saveGameData != null)
			{
				m_timeDelta.Load(saveGameData);
			}
			SaveGameData saveGameData2 = p_data.Get<SaveGameData>("LastHandleTime", null);
			if (saveGameData2 != null)
			{
				m_lastHandelTime.Load(saveGameData2);
			}
			m_sentEvent = p_data.Get<Int32>("StarvedEventsSent", 0);
			m_lastAddedCondition = (ECondition)p_data.Get<Int32>("LastAdded", 0);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("Conditions", (Int32)m_conditions);
			SaveGameData saveGameData = new SaveGameData("TimeDelta");
			m_timeDelta.Save(saveGameData);
			p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			SaveGameData saveGameData2 = new SaveGameData("LastHandleTime");
			m_timeDelta.Save(saveGameData2);
			p_data.Set<SaveGameData>(saveGameData2.ID, saveGameData2);
			p_data.Set<Int32>("StarvedEventsSent", m_sentEvent);
			p_data.Set<Int32>("LastAdded", (Int32)m_lastAddedCondition);
		}

		public void FlushDelayedActionLog()
		{
			foreach (LogEntryEventArgs p_args in m_getKilledEntries)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_getKilledEntries.Clear();
		}

		public void FlushActionLog()
		{
			foreach (LogEntryEventArgs p_args in m_logEntries)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_logEntries.Clear();
		}

		private void NotifyGUI()
		{
			StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.CONDITIONS);
			LegacyLogic.Instance.EventManager.InvokeEvent(m_character, EEventType.CHARACTER_STATUS_CHANGED, p_eventArgs);
		}

		private void NotifyGUI(ECondition p_condition, Boolean p_cantDoAnythingBefore)
		{
			StatusChangedEventArgs statusChangedEventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.CONDITIONS, p_condition);
			statusChangedEventArgs.BecameUnableToDoAnything = (CantDoAnything() && !p_cantDoAnythingBefore);
			LegacyLogic.Instance.EventManager.InvokeEvent(m_character, EEventType.CHARACTER_STATUS_CHANGED, statusChangedEventArgs);
		}
	}
}
