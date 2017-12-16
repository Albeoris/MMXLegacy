using System;
using Legacy.Core.Entities;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class MeleeAttackAction : BaseCharacterAction
	{
		private Monster m_target;

		public MeleeAttackAction(Int32 characterIndex) : base(characterIndex)
		{
			m_consumeType = EConsumeType.CONSUME_CHARACTER_TURN;
		}

		public Boolean MonsterWasKilled { get; set; }

		public override void DoAction(Command p_command)
		{
			m_target = Party.SelectedMonster;
			Character.SelectedMonster = Party.SelectedMonster;
			Character.FightHandler.ExecuteMeleeAttack();
			MonsterWasKilled = (m_target != null && m_target.CurrentHealth <= 0);
		}

		public override Boolean IsActionDone()
		{
			return m_target.HitAnimationDone.IsTriggered;
		}

		public override Boolean ActionAvailable()
		{
			Boolean flag = Character.Equipment.IsMeleeAttackWeaponEquiped();
			return !Character.DoneTurn && Party.InCombat && Party.SelectedMonster != null && !Character.ConditionHandler.CantDoAnything() && flag && Party.SelectedMonster.IsAttackable;
		}

		public override Boolean CanDoAction(Command p_command)
		{
			if (Party.SelectedMonster != null)
			{
				EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(Party.Position, Party.SelectedMonster.Position);
				if (lineOfSightDirection != Party.Direction)
				{
					return false;
				}
			}
			return true;
		}

		public override void Finish()
		{
			m_target.HitAnimationDone.Reset();
			m_target = null;
		}
	}
}
