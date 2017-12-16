using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Hints;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class RangedAttackAction : BaseCharacterAction
	{
		private Monster m_target;

		public RangedAttackAction(Int32 characterIndex) : base(characterIndex)
		{
			m_consumeType = EConsumeType.CONSUME_CHARACTER_TURN;
		}

		public override void DoAction(Command p_command)
		{
			m_target = Character.FightHandler.ExecuteRangedAttack();
		}

		public override Boolean IsActionDone()
		{
			return m_target == null || m_target.HitAnimationDone.IsTriggered;
		}

		public override Boolean ActionAvailable()
		{
			Boolean flag = Character.Equipment.IsRangedAttackWeaponEquiped();
			return !Character.DoneTurn && !Character.ConditionHandler.CantDoAnything() && flag;
		}

		public override Boolean CanDoAction(Command p_command)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Monster monster = LegacyLogic.Instance.WorldManager.Party.SelectedMonster;
			if (monster == null || !grid.LineOfSight(party.Position, monster.Position, true))
			{
				monster = Party.GetRandomMonsterInLineOfSight(true);
			}
			if (monster != null)
			{
				GridSlot slot = grid.GetSlot(monster.Position);
				Boolean flag = false;
				foreach (MovingEntity movingEntity in slot.Entities)
				{
					Monster monster2 = movingEntity as Monster;
					if (monster2 != null && monster2.IsAttackable)
					{
						flag = true;
						break;
					}
				}
				Int32 p_range = (Int32)Position.Distance(party.Position, monster.Position);
				Boolean flag2 = Character.InRangedAttackRange(p_range) && flag;
				if (flag2)
				{
					LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.RANGED);
				}
				return flag2;
			}
			return false;
		}

		public override void Finish()
		{
			if (m_target != null)
			{
				m_target.HitAnimationDone.Reset();
			}
			m_target = null;
		}
	}
}
