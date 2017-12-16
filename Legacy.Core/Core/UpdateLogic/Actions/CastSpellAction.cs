using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Skills;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class CastSpellAction : BaseCharacterAction
	{
		private CharacterSpell m_spell;

		private List<Object> m_targets = new List<Object>();

		private List<Monster> m_finishedMonsters = new List<Monster>();

		public CastSpellAction(Int32 charaIndex) : base(charaIndex)
		{
			m_consumeType = EConsumeType.CONSUME_CHARACTER_TURN;
		}

		public override void DoAction(Command p_command)
		{
			CastSpellCommand castSpellCommand = (CastSpellCommand)p_command;
			m_spell = castSpellCommand.Spell;
			m_targets.Clear();
			if (castSpellCommand.TargetCharacter != null)
			{
				m_targets.Add(castSpellCommand.TargetCharacter);
			}
			else
			{
				m_spell.GetTargets(m_targets);
			}
			if (castSpellCommand.Scroll != null)
			{
				castSpellCommand.Spell.CastSpell(Character, true, castSpellCommand.Scroll.ScrollTier, m_targets);
				Party.Inventory.ConsumeSuccess(castSpellCommand.Scroll);
				Party.MuleInventory.ConsumeSuccess(castSpellCommand.Scroll);
			}
			else
			{
				castSpellCommand.Spell.CastSpell(Character, false, 0, m_targets);
			}
		}

		public override Boolean IsActionDone()
		{
			if (m_spell.StaticID == 76 || m_spell.StaticID == 88)
			{
				return true;
			}
			if (m_spell.TargetType == ETargetType.PARTY || m_spell.TargetType == ETargetType.SINGLE_PARTY_MEMBER || m_spell.TargetType == ETargetType.NONE || m_spell.TargetType == ETargetType.SUMMON)
			{
				return true;
			}
			Boolean result = true;
			for (Int32 i = m_targets.Count - 1; i >= 0; i--)
			{
				Monster monster = m_targets[i] as Monster;
				if (monster != null)
				{
					if (monster.HitAnimationDone.IsTriggered || !monster.IsAttackable || m_spell.SpellType == ECharacterSpell.SPELL_DARK_PURGE)
					{
						m_targets.RemoveAt(i);
						m_finishedMonsters.Add(monster);
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		public override Boolean ActionAvailable()
		{
			return !Character.DoneTurn && !Character.ConditionHandler.CantDoAnything();
		}

		public override Boolean CanDoAction(Command p_command)
		{
			CastSpellCommand castSpellCommand = (CastSpellCommand)p_command;
			CharacterSpell spell = castSpellCommand.Spell;
			m_spell = spell;
			Boolean flag = Character.ConditionHandler.HasCondition(ECondition.CONFUSED) && spell.StaticData.SkillID != ESkillID.SKILL_WARFARE;
			if (flag)
			{
				return false;
			}
			Boolean flag2 = !Character.Equipment.IsMeleeAttackWeaponEquiped() && spell.StaticData.SkillID == ESkillID.SKILL_WARFARE;
			if (flag2)
			{
				return false;
			}
			Boolean flag3 = spell.CheckSpellConditions(Character);
			if (flag3 && m_spell.TargetType == ETargetType.SUMMON && m_spell.HasResources(Character))
			{
				return true;
			}
			List<Object> targets = spell.GetTargets();
			if (targets != null && targets.Count == 1 && targets[0] is Monster)
			{
				if (LegacyLogic.Instance.WorldManager.Party.SelectedMonster != null && !LegacyLogic.Instance.WorldManager.Party.SelectedMonster.IsAttackable)
				{
					return false;
				}
				if (!((Monster)targets[0]).IsAttackable)
				{
					return false;
				}
			}
			return (spell.TargetType == ETargetType.SINGLE_PARTY_MEMBER || targets.Count > 0) && (spell.HasResources(Character) || castSpellCommand.Scroll != null) && flag3;
		}

		public override void Finish()
		{
			foreach (Monster monster in m_finishedMonsters)
			{
				monster.HitAnimationDone.Reset();
			}
			m_finishedMonsters.Clear();
			m_targets.Clear();
			m_spell = null;
		}
	}
}
