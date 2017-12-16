using System;
using System.Collections.Generic;
using System.Linq;
using Legacy.Core.Abilities;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells
{
	public abstract class CharacterSpell : Spell
	{
		private static EMonsterBuffType[] SINGLE_TARGET_BUFFS = new EMonsterBuffType[]
		{
			EMonsterBuffType.SLEEPING,
			EMonsterBuffType.IMPRISONED
		};

		protected CharacterSpellStaticData m_staticData;

		protected Monster m_lastBuffTarget;

		protected Boolean m_didPushEntry;

		protected Boolean m_fromScroll;

		public CharacterSpell(ECharacterSpell p_type) : this((Int32)p_type)
		{
		}

		public CharacterSpell(Int32 p_staticID) : this(StaticDataHandler.GetStaticData<CharacterSpellStaticData>(EDataType.CHARACTER_SPELLS, p_staticID))
		{
		}

		public CharacterSpell(CharacterSpellStaticData p_staticData)
		{
			m_staticData = p_staticData;
		}

		public CharacterSpellStaticData StaticData => m_staticData;

	    public ECharacterSpell SpellType => (ECharacterSpell)m_staticData.StaticID;

	    public override Int32 StaticID => m_staticData.StaticID;

	    public override ETargetType TargetType => m_staticData.TargetType;

	    public override String NameKey => m_staticData.NameKey;

	    public override String EffectKey => m_staticData.EffectKey;

	    public Boolean CastSpell(Character p_sorcerer, Boolean p_fromScroll, Int32 p_scrollTier)
		{
			return CastSpell(p_sorcerer, p_fromScroll, p_scrollTier, GetTargets());
		}

		public Boolean CastSpell(Character p_sorcerer, Boolean p_fromScroll, Int32 p_scrollTier, Character p_target)
		{
			return m_staticData.TargetType == ETargetType.SINGLE_PARTY_MEMBER && CastSpell(p_sorcerer, p_fromScroll, p_scrollTier, new List<Object>
			{
				p_target
			});
		}

		public virtual Single GetMagicFactor(Character p_sorcerer, Boolean p_fromScroll, Int32 p_scrollTier)
		{
			Single result = 1f;
			if (m_staticData.SkillID != ESkillID.SKILL_WARFARE)
			{
				if (p_fromScroll)
				{
					GameConfig game = ConfigManager.Instance.Game;
					if (p_scrollTier == 1)
					{
						result = game.ScrollNoviceMagicFactor;
					}
					else if (p_scrollTier == 2)
					{
						result = game.ScrollExpertMagicFactor;
					}
					else if (p_scrollTier == 3)
					{
						result = game.ScrollMasterMagicFactor;
					}
					else
					{
						result = game.ScrollGrandmasterMagicFactor;
					}
				}
				else
				{
					result = p_sorcerer.FightValues.MagicPowers[m_staticData.SkillID];
				}
			}
			else if (m_staticData.StaticID == 84)
			{
				result = 3.5f + 0.02f * p_sorcerer.CurrentAttributes.Might + 0.02f * p_sorcerer.CurrentAttributes.Perception;
			}
			return result;
		}

		public virtual Boolean CastSpell(Character p_sorcerer, Boolean p_fromScroll, Int32 p_scrollTier, List<Object> p_targets)
		{
			SpellEventArgs eventArgs = GetEventArgs();
			eventArgs.DamageType = ESkillIDToEDamageType(m_staticData.SkillID);
			m_didPushEntry = false;
			m_fromScroll = p_fromScroll;
			Single magicFactor = GetMagicFactor(p_sorcerer, p_fromScroll, p_scrollTier);
			if (m_staticData.SkillID == ESkillID.SKILL_WARFARE)
			{
				p_sorcerer.BarkHandler.TriggerBark(EBarks.ABILITY, p_sorcerer);
			}
			if (m_staticData.Damage != null && (m_staticData.Damage.Length == 0 || m_staticData.Damage[0].Maximum == 0 || m_staticData.Damage[0].Type == EDamageType.HEAL))
			{
				eventArgs.Result = ESpellResult.OK;
				if (m_staticData.RemovedConditions.Length == 0 || m_staticData.RemovedConditions[0] == ECondition.NONE)
				{
					p_sorcerer.FightHandler.FeedActionLog(new CastSpellEntryEventArgs(p_sorcerer, eventArgs));
					m_didPushEntry = true;
				}
			}
			HandleConditions(eventArgs, p_targets, magicFactor);
			HandlePartyBuffs(eventArgs, magicFactor);
			HandleMonsters(p_sorcerer, eventArgs, p_targets, magicFactor);
			if (m_staticData.Damage != null && m_staticData.Damage.Length > 0 && m_staticData.Damage[0].Type == EDamageType.HEAL)
			{
				Int32 value = Damage.Create(m_staticData.Damage[0] * magicFactor, 0f).Value;
				Int32 p_critHealValue = (Int32)(value * p_sorcerer.FightValues.MagicalCriticalDamageMod);
				Single criticalMagicHitChance = p_sorcerer.FightValues.CriticalMagicHitChance;
				HandlePartyMemberHealing(magicFactor, criticalMagicHitChance, p_critHealValue, eventArgs, p_targets);
			}
			else if (SpellType == ECharacterSpell.SPELL_LIGHT_LAY_ON_HANDS)
			{
				HandlePartyMemberHealing(magicFactor, 0f, 0, eventArgs, p_targets);
			}
			HandlePartyCharacters(p_sorcerer, eventArgs, p_targets, magicFactor);
			if (!p_fromScroll && SpellType != ECharacterSpell.SPELL_PRIME_IDENTIFY)
			{
				UseResources(p_sorcerer);
			}
			eventArgs.Result = ESpellResult.OK;
			if (m_staticData.SkillID != ESkillID.SKILL_WARFARE)
			{
				p_sorcerer.BarkHandler.TriggerBark(EBarks.SPELL, p_sorcerer);
			}
			p_sorcerer.CastSpellEvent(this, eventArgs);
			if (!m_didPushEntry)
			{
				p_sorcerer.FightHandler.FeedActionLog(new CastSpellEntryEventArgs(p_sorcerer, eventArgs));
			}
			p_sorcerer.FightHandler.FlushActionLog();
			return true;
		}

		public void CastSpellByInteractiveObject(Single p_magicFactor)
		{
			SpellEventArgs spellEventArgs = new SpellEventArgs(this);
			List<Object> targets = GetTargets();
			if (targets.Count == 0)
			{
				spellEventArgs.Result = ESpellResult.NO_TARGET_FOUND;
				throw new Exception("No target found for spell " + NameKey);
			}
			HandleConditions(spellEventArgs, targets, p_magicFactor);
			HandlePartyBuffs(spellEventArgs, p_magicFactor);
			Single p_critChance = 0f;
			Int32 p_critHealValue = 0;
			HandlePartyMemberHealing(p_magicFactor, p_critChance, p_critHealValue, spellEventArgs, targets);
			spellEventArgs.Result = ESpellResult.OK;
		}

		protected virtual SpellEventArgs GetEventArgs()
		{
			return new SpellEventArgs(this);
		}

		protected virtual void HandleConditions(SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			if (m_staticData.RemovedConditions != null && m_staticData.RemovedConditions.Length != 0)
			{
				for (Int32 i = 0; i < m_staticData.RemovedConditions.Length; i++)
				{
					ECondition econdition = m_staticData.RemovedConditions[i];
					if (econdition != ECondition.NONE)
					{
						for (Int32 j = 0; j < p_targets.Count; j++)
						{
							Character character = p_targets[j] as Character;
							if (character != null)
							{
								if (character.ConditionHandler.HasCondition(econdition))
								{
									p_result.SpellTargets.Add(new RemovedConditionTarget(p_targets[j], econdition));
									RemoveCondition(p_targets[j], econdition);
									m_didPushEntry = true;
								}
							}
						}
					}
				}
			}
		}

		private void HandlePartyBuffs(SpellEventArgs p_result, Single p_magicFactor)
		{
			if (m_staticData.PartyBuff != EPartyBuffs.NONE)
			{
				p_result.AddedPartyBuffs = m_staticData.PartyBuff;
				p_result.SpellTargets.Add(new PartyBuffTarget(LegacyLogic.Instance.WorldManager.Party, m_staticData.PartyBuff));
				AddPartyBuff(p_magicFactor);
			}
		}

		protected virtual void HandlePartyMemberHealing(Single p_magicFactor, Single p_critChance, Int32 p_critHealValue, SpellEventArgs p_result, List<Object> p_targets)
		{
			if (m_staticData.Damage != null && m_staticData.Damage.Length > 0 && m_staticData.Damage[0].Type == EDamageType.HEAL)
			{
				Int32 value = Damage.Create(m_staticData.Damage[0] * p_magicFactor, 0f).Value;
				for (Int32 i = 0; i < p_targets.Count; i++)
				{
					if (!((Character)p_targets[i]).ConditionHandler.HasCondition(ECondition.DEAD))
					{
						Int32 num = value;
						Boolean p_IsCritical = false;
						if (Random.Range(0f, 1f) < p_critChance)
						{
							num = p_critHealValue;
							p_IsCritical = true;
						}
						p_result.SpellTargets.Add(new HealedTarget(p_targets[i], num, p_IsCritical));
						((Character)p_targets[i]).ChangeHP(num);
						DamageEventArgs p_eventArgs = new DamageEventArgs(new AttackResult
						{
							Result = EResultType.HEAL,
							DamageResults = 
							{
								new DamageResult(EDamageType.HEAL, num, 0, 1f)
							}
						});
						LegacyLogic.Instance.EventManager.InvokeEvent((Character)p_targets[i], EEventType.CHARACTER_HEALS, p_eventArgs);
					}
				}
			}
		}

		protected virtual void HandleMonsters(Character p_sorcerer, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			for (Int32 i = 0; i < p_targets.Count; i++)
			{
				Monster monster = p_targets[i] as Monster;
				if (monster != null)
				{
					if (monster.IsAttackable)
					{
						if (m_staticData.SkillID == ESkillID.SKILL_WARFARE)
						{
							if (m_staticData.TargetType == ETargetType.SINGLE_MONSTER || m_staticData.TargetType == ETargetType.ALL_ADJACENT_MONSTERS)
							{
								List<AttackResult> list = MeleeAttackMonster(p_sorcerer, monster);
								if (list == null)
								{
									goto IL_144;
								}
								if (list.Count == 0)
								{
									monster.HitAnimationDone.Trigger();
								}
								Boolean flag = false;
								foreach (AttackResult attackResult in list)
								{
									if (attackResult.Result != EResultType.EVADE && attackResult.Result != EResultType.BLOCK)
									{
										flag = true;
										break;
									}
								}
								if (!flag)
								{
									goto IL_144;
								}
							}
						}
						else
						{
							AttackResult attackResult2 = DoAttackMonster(p_sorcerer, monster, p_magicFactor);
							p_result.SpellTargets.Add(new AttackedTarget(monster, attackResult2));
							if (attackResult2.Result == EResultType.EVADE || attackResult2.Result == EResultType.IMMUNE)
							{
								goto IL_144;
							}
							monster.ApplyDamages(attackResult2, p_sorcerer);
						}
					}
					HandleMonsterBuffs(p_sorcerer, p_result, monster, p_magicFactor);
				}
				IL_144:;
			}
		}

		protected virtual void HandleMonsterBuffs(Character p_sorcerer, SpellEventArgs p_result, Monster p_target, Single p_magicFactor)
		{
			if (p_target.CurrentHealth > 0 && m_staticData.MonsterBuffs != null)
			{
				EMonsterBuffType[] monsterBuffs = m_staticData.MonsterBuffs;
				for (Int32 i = 0; i < monsterBuffs.Length; i++)
				{
					if (monsterBuffs[i] != EMonsterBuffType.NONE)
					{
						AddMonsterBuff(p_target, monsterBuffs[i], p_magicFactor);
						Boolean p_Successful = p_target.BuffHandler.HasBuff(monsterBuffs[i]);
						MonsterBuff monsterBuff = BuffFactory.CreateMonsterBuff(monsterBuffs[i], p_magicFactor);
						Boolean p_IsImmune = !p_target.AbilityHandler.CanAddBuff(monsterBuff.Type);
						p_result.SpellTargets.Add(new MonsterBuffTarget(p_target, monsterBuffs[i], p_Successful, p_IsImmune));
					}
				}
			}
		}

		protected virtual void HandlePartyCharacters(Character p_sorcerer, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
		}

		protected virtual List<AttackResult> MeleeAttackMonster(Character p_sorcerer, Monster p_target)
		{
			return p_sorcerer.FightHandler.ExecuteMeleeAttack();
		}

		public static EDamageType ESkillIDToEDamageType(ESkillID p_skillID)
		{
			switch (p_skillID)
			{
			case ESkillID.SKILL_FIRE_MAGIC:
				return EDamageType.FIRE;
			case ESkillID.SKILL_WATER_MAGIC:
				return EDamageType.WATER;
			case ESkillID.SKILL_AIR_MAGIC:
				return EDamageType.AIR;
			case ESkillID.SKILL_EARTH_MAGIC:
				return EDamageType.EARTH;
			case ESkillID.SKILL_LIGHT_MAGIC:
				return EDamageType.LIGHT;
			case ESkillID.SKILL_DARK_MAGIC:
				return EDamageType.DARK;
			case ESkillID.SKILL_PRIMORDIAL_MAGIC:
				return EDamageType.PRIMORDIAL;
			default:
				return EDamageType.PHYSICAL;
			}
		}

		protected virtual AttackResult DoAttackMonster(Character p_sorcerer, Monster p_target, Single p_magicPower)
		{
			if (p_target == null)
			{
				return null;
			}
			Single criticalMagicHitChance = p_sorcerer.FightValues.CriticalMagicHitChance;
			Single magicalCriticalDamageMod = p_sorcerer.FightValues.MagicalCriticalDamageMod;
			EDamageType edamageType = ESkillIDToEDamageType(m_staticData.SkillID);
			Attack attack = new Attack(0f, criticalMagicHitChance);
			for (Int32 i = 0; i < m_staticData.Damage.Length; i++)
			{
				if (m_staticData.Damage[i].Type != EDamageType.HEAL)
				{
					DamageData p_data = DamageData.Scale(m_staticData.Damage[i], p_magicPower);
					Damage item = Damage.Create(p_data, magicalCriticalDamageMod);
					if (item.Type == edamageType)
					{
						item.IgnoreResistance = p_sorcerer.SkillHandler.GetResistanceIgnoreValue(m_staticData.SkillID);
					}
					attack.Damages.Add(item);
				}
			}
			p_target.AbilityHandler.ExecuteAttack(p_sorcerer, attack, false, EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_BEFORE_DAMAGE_REDUCTION);
			Boolean p_isMagic = m_staticData.SkillID != ESkillID.SKILL_WARFARE;
			return p_target.CombatHandler.AttackMonster(p_sorcerer, attack, false, true, edamageType, p_isMagic, p_sorcerer.SkillHandler.GetResistanceIgnoreValue(m_staticData.SkillID));
		}

		public virtual List<Object> GetTargets()
		{
			List<Object> list = new List<Object>();
			GetTargets(list);
			return list;
		}

		public virtual Int32 GetTargets(List<Object> buffer)
		{
			Int32 range = m_staticData.Range;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Position position = party.Position;
			EDirection direction = party.Direction;
			ETargetType targetType = m_staticData.TargetType;
			switch (targetType)
			{
			case ETargetType.ALL_MONSTERS_ON_TARGET_SLOT:
			{
				Int32 num = buffer.Count;
				grid.GetMonstersOnFirstSlot(position, direction, range, buffer);
				for (Int32 i = buffer.Count - 1; i >= num; i--)
				{
					if (!((Monster)buffer[i]).IsAttackable)
					{
						if (m_staticData.MonsterBuffs.Length <= 0 || m_staticData.MonsterBuffs[0] != EMonsterBuffType.IMPRISONED)
						{
							buffer.RemoveAt(i);
						}
					}
				}
				return buffer.Count - num;
			}
			case ETargetType.PARTY:
			{
				Character[] members = party.Members;
				foreach (Character item in members)
				{
					buffer.Add(item);
				}
				return members.Length;
			}
			default:
				if (targetType != ETargetType.SINGLE_MONSTER)
				{
					Int32 num;
					if (targetType == ETargetType.ALL_ADJACENT_MONSTERS)
					{
						num = buffer.Count;
						grid.GetMonstersInDirection(position, EDirection.NORTH, 1, buffer);
						grid.GetMonstersInDirection(position, EDirection.EAST, 1, buffer);
						grid.GetMonstersInDirection(position, EDirection.SOUTH, 1, buffer);
						grid.GetMonstersInDirection(position, EDirection.WEST, 1, buffer);
						for (Int32 k = buffer.Count - 1; k >= num; k--)
						{
							if (!((Monster)buffer[k]).IsAttackable)
							{
								if (m_staticData.MonsterBuffs.Length <= 0 || m_staticData.MonsterBuffs[0] != EMonsterBuffType.IMPRISONED)
								{
									buffer.RemoveAt(k);
								}
							}
						}
						return buffer.Count - num;
					}
					if (targetType != ETargetType.ALL_MONSTERS)
					{
						return 0;
					}
					num = 0;
					foreach (Monster monster in LegacyLogic.Instance.WorldManager.GetObjectsByTypeIterator<Monster>())
					{
						if (monster.IsAggro && monster.IsAttackable)
						{
							buffer.Add(monster);
							num++;
						}
					}
					return num;
				}
				else
				{
					Monster selectedMonster = party.SelectedMonster;
					if (selectedMonster != null && selectedMonster.CurrentHealth > 0 && selectedMonster.DistanceToParty <= m_staticData.Range && grid.LineOfSight(party.Position, selectedMonster.Position, true) && selectedMonster.IsAttackable)
					{
						buffer.Add(selectedMonster);
						return 1;
					}
					Monster randomMonsterInDirection = grid.GetRandomMonsterInDirection(position, direction, range);
					if (randomMonsterInDirection != null)
					{
						buffer.Add(randomMonsterInDirection);
						return 1;
					}
					return 0;
				}
			case ETargetType.ALL_MONSTERS_IN_LINE_OF_SIGHT:
			{
				Int32 num = buffer.Count;
				grid.GetMonstersInDirection(position, direction, range, buffer);
				for (Int32 l = buffer.Count - 1; l >= num; l--)
				{
					if (!((Monster)buffer[l]).IsAttackable)
					{
						if (m_staticData.MonsterBuffs.Length <= 0 || m_staticData.MonsterBuffs[0] != EMonsterBuffType.IMPRISONED)
						{
							buffer.RemoveAt(l);
						}
					}
				}
				return buffer.Count - num;
			}
			}
		}

		public virtual Boolean HasResources(Character p_sorcerer)
		{
			return m_staticData != null && p_sorcerer != null && p_sorcerer.ManaPoints >= m_staticData.ManaCost;
		}

		public virtual void UseResources(Character p_sorcerer)
		{
			p_sorcerer.ChangeMP(-m_staticData.ManaCost);
		}

		public virtual void RemoveCondition(Object p_target, ECondition p_condition)
		{
			if (p_target != null && p_target is Character)
			{
				((Character)p_target).ConditionHandler.RemoveCondition(p_condition);
				return;
			}
			throw new Exception("Remove condition for a Monster!");
		}

		public virtual void AddPartyBuff(Single p_magicFactor)
		{
			LegacyLogic.Instance.WorldManager.Party.Buffs.AddBuff(m_staticData.PartyBuff, p_magicFactor);
		}

		public void AddMonsterBuff(Monster p_target, EMonsterBuffType p_monsterBuff, Single p_magicFactor)
		{
			if (p_target != null)
			{
				Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
				MonsterBuff monsterBuff = BuffFactory.CreateMonsterBuff(p_monsterBuff, p_magicFactor);
				monsterBuff.Causer = selectedCharacter;
				p_target.AddBuff(monsterBuff);
				if (SINGLE_TARGET_BUFFS.Contains(p_monsterBuff))
				{
					SingleTargetBuff(monsterBuff, p_target);
				}
			}
		}

		protected void SingleTargetBuff(MonsterBuff p_monsterBuff, Monster p_newBuffTarget)
		{
			m_lastBuffTarget = p_newBuffTarget;
		}

		public virtual void FillDescriptionValues(Single p_magicFactor)
		{
		}

		protected String GetDamageAsString(Int32 p_damageId, Single p_magicFactor)
		{
			Int32 num = (Int32)(m_staticData.Damage[p_damageId].Minimum * p_magicFactor + 0.5f);
			Int32 num2 = (Int32)(m_staticData.Damage[p_damageId].Maximum * p_magicFactor + 0.5f);
			if (num == num2)
			{
				return num.ToString();
			}
			return num + " - " + num2;
		}

		public String GetDescription(Single p_magicFactor)
		{
			FillDescriptionValues(p_magicFactor);
			return Localization.Instance.GetText(m_staticData.DescriptionKey, m_descriptionValues);
		}

		public Int32 GetCalculatedCosts()
		{
			Int32 num = m_staticData.GoldPrice;
			if (LegacyLogic.Instance.WorldManager.Difficulty == EDifficulty.HARD)
			{
				num = (Int32)Math.Ceiling(num * ConfigManager.Instance.Game.ItemSpellsFactor);
			}
			return num;
		}

		public virtual Boolean CheckSpellConditions(Character p_sorcerer)
		{
			return ConditionToHealIsPresent() && PartyMemberNeedsHealing();
		}

		public virtual void FinishCastSpell(Character p_sorcerer)
		{
		}

		private Boolean ConditionToHealIsPresent()
		{
			if (m_staticData.RemovedConditions == null || m_staticData.RemovedConditions[0] == ECondition.NONE)
			{
				return true;
			}
			foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
			{
				ECondition econdition = ECondition.NONE;
				for (Int32 j = 0; j < m_staticData.RemovedConditions.Length; j++)
				{
					econdition |= m_staticData.RemovedConditions[j];
				}
				if (character.ConditionHandler.HasOneCondition(econdition))
				{
					return true;
				}
			}
			return false;
		}

		protected virtual Boolean PartyMemberNeedsHealing()
		{
			Boolean flag = false;
			Boolean flag2 = false;
			foreach (DamageData damageData in m_staticData.Damage)
			{
				flag = (damageData.Type != EDamageType.HEAL);
				flag2 = (damageData.Type == EDamageType.HEAL);
			}
			if (flag || !flag2)
			{
				return true;
			}
			if ((m_staticData.RemovedConditions.Length > 0 && m_staticData.RemovedConditions[0] != ECondition.NONE) || m_staticData.PartyBuff != EPartyBuffs.NONE || m_staticData.MonsterBuffs[0] != EMonsterBuffType.NONE)
			{
				return true;
			}
			foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
			{
				if (character.HealthPoints < character.MaximumHealthPoints)
				{
					return true;
				}
			}
			return false;
		}
	}
}
