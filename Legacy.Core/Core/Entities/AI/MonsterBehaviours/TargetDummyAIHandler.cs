using System;
using System.Collections.Generic;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class TargetDummyAIHandler : MonsterAIHandler
	{
		public TargetDummyAIHandler(Monster p_owner) : base(p_owner)
		{
		}

		internal override Boolean CalculatePath(GridSlot p_start, GridSlot p_target, List<GridSlot> p_pathBuffer)
		{
			return false;
		}

		protected override Boolean CanCastSpell()
		{
			return false;
		}

		public override Boolean CastSpell(MonsterSpell p_spell)
		{
			return false;
		}

		protected override void CheckAIEvents()
		{
		}

		protected override Boolean CheckSpellCastChance()
		{
			return false;
		}

		protected override Boolean DoCastSpell()
		{
			return false;
		}

		protected override void DoMelee(Boolean p_isMelee, Party p_party, Grid p_grid, GridSlot p_startSlot)
		{
		}

		protected override void DoRanged(Boolean p_isMagic, Party p_party, Grid p_grid, GridSlot p_startSlot, out Boolean p_isMelee)
		{
			p_isMelee = false;
		}

		public override void DoTurn(Grid p_grid, Party p_party)
		{
			m_owner.SkipMovement.Trigger();
		}

		public override void FilterPossibleTargets(List<Character> p_characters)
		{
		}

		public override List<GridSlot> GetMeleeTargets(Grid p_grid, Party p_party)
		{
			return null;
		}

		public override List<GridSlot> GetRangedTargets(Grid p_grid, Party p_party)
		{
			return null;
		}
	}
}
