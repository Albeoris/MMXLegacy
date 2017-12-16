using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Dark
{
	public class CharacterSpellSleep : CharacterSpell
	{
		public CharacterSpellSleep() : base(ECharacterSpell.SPELL_DARK_SLEEP)
		{
		}

		protected override void HandleMonsters(Character p_attacker, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			foreach (Monster monster in LegacyLogic.Instance.WorldManager.GetObjectsByTypeIterator<Monster>())
			{
				monster.BuffHandler.RemoveBuffByID(19);
			}
			base.HandleMonsters(p_attacker, p_result, p_targets, p_magicFactor);
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, StaticData.Range);
		}
	}
}
