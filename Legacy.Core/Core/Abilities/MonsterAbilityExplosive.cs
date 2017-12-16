using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityExplosive : MonsterAbilityBase
	{
		public MonsterAbilityExplosive() : base(EMonsterAbilityType.EXPLOSIVE)
		{
			m_executionPhase = EExecutionPhase.MONSTER_DIES;
		}

		public override void HandleAttackResults(List<AttackResult> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic, Boolean p_isRanged)
		{
			AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
			p_monster.AbilityHandler.AddEntry(m_executionPhase, p_args);
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_monster.Position);
			List<Monster> list = new List<Monster>();
			Party party = LegacyLogic.Instance.WorldManager.Party;
			IList<MovingEntity> entities = slot.Entities;
			for (Int32 i = 0; i < entities.Count; i++)
			{
				if (entities[i] != null && entities[i] != p_monster && entities[i] is Monster)
				{
					list.Add((Monster)entities[i]);
				}
			}
			Int32 num = (Int32)m_staticData.GetValues(m_level)[0];
			for (Int32 j = 0; j < list.Count; j++)
			{
				list[j].ChangeHP(-num, null);
			}
			if (Position.Distance(party.Position, p_monster.Position) <= 1f)
			{
				for (Int32 k = 0; k < party.Members.Length; k++)
				{
					if (party.Members[k] != null && !party.Members[k].ConditionHandler.HasCondition(ECondition.DEAD))
					{
						party.Members[k].ChangeHP(-num, p_monster);
					}
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
		}

		public override String GetDescription()
		{
			return Localization.Instance.GetText(m_staticData.NameKey + "_INFO", m_staticData.GetValues(m_level)[0]);
		}
	}
}
