using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic.Actions;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareSnaringShot : CharacterSpell
	{
		private Boolean m_didHit;

		public CharacterWarfareSnaringShot() : base(ECharacterSpell.WARFARE_SNARING_SHOT)
		{
		}

		public override Boolean CheckSpellConditions(Character p_sorcerer)
		{
			RangedAttackAction rangedAttackAction = new RangedAttackAction(p_sorcerer.Index);
			return rangedAttackAction.ActionAvailable();
		}

		protected override void HandleMonsters(Character p_sorcerer, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			if (m_didHit)
			{
				for (Int32 i = 0; i < p_targets.Count; i++)
				{
					Monster monster = p_targets[i] as Monster;
					if (monster.CurrentHealth > 0 && m_staticData.MonsterBuffs != null)
					{
						for (Int32 j = 0; j < m_staticData.MonsterBuffs.Length; j++)
						{
							if (m_staticData.MonsterBuffs[j] != EMonsterBuffType.NONE)
							{
								AddMonsterBuff(monster, m_staticData.MonsterBuffs[j], p_magicFactor);
								Boolean p_Successful = monster.BuffHandler.HasBuff(m_staticData.MonsterBuffs[j]);
								Boolean p_IsImmune = false;
								if (!monster.AbilityHandler.CanAddBuff(m_staticData.MonsterBuffs[j]))
								{
									p_IsImmune = true;
								}
								p_result.SpellTargets.Add(new MonsterBuffTarget(monster, m_staticData.MonsterBuffs[j], p_Successful, p_IsImmune));
							}
						}
					}
				}
			}
		}

		public override Int32 GetTargets(List<Object> buffer)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Position position = party.Position;
			EDirection direction = party.Direction;
			Int32 rangedAttackRange = party.SelectedCharacter.FightValues.RangedAttackRange;
			Monster monster = party.SelectedMonster;
			if (monster == null || !grid.LineOfSight(party.Position, monster.Position, true))
			{
				monster = grid.GetRandomMonsterInDirection(position, direction, rangedAttackRange);
			}
			if (monster != null && !monster.IsAttackable)
			{
				GridSlot slot = grid.GetSlot(monster.Position);
				foreach (MovingEntity movingEntity in slot.Entities)
				{
					Monster monster2 = movingEntity as Monster;
					if (monster2 != null && monster2.IsAttackable)
					{
						monster = monster2;
						break;
					}
				}
			}
			if (monster != null)
			{
				buffer.Add(monster);
				return 1;
			}
			return 0;
		}

		public override Boolean CastSpell(Character p_sorcerer, Boolean p_fromScroll, Int32 p_scrollTier, List<Object> p_targets)
		{
			foreach (Object obj in p_targets)
			{
				if (obj is Monster)
				{
					Monster p_target = (Monster)obj;
					List<AttackResult> list = p_sorcerer.FightHandler.ExecuteRangedAttackOnTarget(p_target);
					m_didHit = false;
					if (list != null)
					{
						foreach (AttackResult attackResult in list)
						{
							if (attackResult.Result == EResultType.HIT || attackResult.Result == EResultType.CRITICAL_HIT)
							{
								m_didHit = true;
							}
						}
					}
				}
			}
			return base.CastSpell(p_sorcerer, p_fromScroll, p_scrollTier, p_targets);
		}
	}
}
