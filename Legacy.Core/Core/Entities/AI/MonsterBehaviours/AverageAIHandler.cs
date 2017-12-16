using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Pathfinding;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class AverageAIHandler : MonsterAIHandler
	{
		public AverageAIHandler(Monster p_owner) : base(p_owner)
		{
		}

		public override void FilterPossibleTargets(List<Character> p_characters)
		{
			for (Int32 i = p_characters.Count - 1; i >= 0; i--)
			{
				if (p_characters[i].ConditionHandler.HasOneCondition(ECondition.UNCONSCIOUS | ECondition.SLEEPING))
				{
					p_characters.RemoveAt(i);
				}
			}
			base.FilterPossibleTargets(p_characters);
		}

		protected override void DoMelee(Boolean p_isMelee, Party p_party, Grid p_grid, GridSlot p_startSlot)
		{
			if (p_grid.GetSlot(m_owner.Position).CheckForSummons())
			{
				List<GridSlot> meleeTargets = GetMeleeTargets(p_grid, p_party);
				meleeTargets.Remove(p_grid.GetSlot(m_owner.Position));
				meleeTargets.Sort(new Comparison<GridSlot>(DistSortAsc));
				if (TryMove(meleeTargets, p_grid, p_startSlot, p_party))
				{
					return;
				}
			}
			base.DoMelee(p_isMelee, p_party, p_grid, p_startSlot);
		}

		protected override void DoRanged(Boolean p_isMagic, Party p_party, Grid p_grid, GridSlot p_startSlot, out Boolean p_isMelee)
		{
			if (p_grid.GetSlot(m_owner.Position).CheckForSummons())
			{
				List<GridSlot> rangedTargets = GetRangedTargets(p_grid, p_party);
				rangedTargets.Remove(p_grid.GetSlot(m_owner.Position));
				rangedTargets.Sort(new Comparison<GridSlot>(DistSortAsc));
				if (TryMove(rangedTargets, p_grid, p_startSlot, p_party))
				{
					p_isMelee = false;
					return;
				}
			}
			base.DoRanged(p_isMagic, p_party, p_grid, p_startSlot, out p_isMelee);
		}

		internal override Boolean CalculatePath(GridSlot p_start, GridSlot p_target, List<GridSlot> p_pathBuffer)
		{
			if (!p_target.CheckForSummons())
			{
				AStarHelper<GridSlot>.Calculate(p_start, p_target, 10, m_owner, false, true, p_pathBuffer);
				return p_pathBuffer.Count > 0;
			}
			List<GridSlot> list;
			if (Position.Distance(p_target.Position, LegacyLogic.Instance.WorldManager.Party.Position) <= 1.1f)
			{
				list = GetMeleeTargets(LegacyLogic.Instance.MapLoader.Grid, LegacyLogic.Instance.WorldManager.Party);
			}
			else
			{
				list = GetRangedTargets(LegacyLogic.Instance.MapLoader.Grid, LegacyLogic.Instance.WorldManager.Party);
			}
			if (list.FindAll(new Predicate<GridSlot>(FindSlotsWithSummons)).Count == list.Count)
			{
				return base.CalculatePath(p_start, p_target, p_pathBuffer);
			}
			list.RemoveAll(new Predicate<GridSlot>(FindSlotsWithSummons));
			list.Sort(new Comparison<GridSlot>(DistSortAsc));
			AStarHelper<GridSlot>.Calculate(p_start, list[0], 10, m_owner, false, false, p_pathBuffer);
			list.Sort(new Comparison<GridSlot>(DistSortAsc));
			return p_pathBuffer.Count > 0;
		}
	}
}
