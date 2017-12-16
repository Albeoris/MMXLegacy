using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Prime
{
	public class CharacterSpellDispelMagic : CharacterSpell
	{
		public CharacterSpellDispelMagic() : base(ECharacterSpell.SPELL_PRIME_DISPEL_MAGIC)
		{
		}

		protected override void HandleConditions(SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			base.HandleConditions(p_result, p_targets, p_magicFactor);
			p_result.SpellTargets.Add(new SpellTarget(LegacyLogic.Instance.WorldManager.Party));
			LegacyLogic.Instance.WorldManager.Party.Buffs.RemoveAllBuffs();
		}

		public override Boolean CheckSpellConditions(Character p_sorcerer)
		{
			if (LegacyLogic.Instance.WorldManager.Party.Buffs.Buffs.Count > 0)
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
	}
}
