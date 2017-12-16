using System;
using Legacy.Core.Abilities;
using Legacy.Core.Entities.AI.Events;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class TaralethAIHandler : MonsterAIHandler
	{
		private Single m_currentTreshholdWitheringBreath;

		private Single RESET_TRESHHOLD = 0.2f;

		private Single MAX_TRESHHOLD = 0.85f;

		private Single TRESHHOLD_INCREMENT = 0.08f;

		private Boolean m_castedSpell;

		public TaralethAIHandler(Monster p_owner) : base(p_owner)
		{
			m_aiEvents.Add(new AIEventHealthPercent(0.75f, m_owner));
			m_aiEvents[0].OnTrigger += OnLifeEventTriggered;
			m_aiEvents.Add(new AIEventHealthPercent(0.5f, m_owner));
			m_aiEvents[1].OnTrigger += OnLifeEventTriggered;
			m_aiEvents.Add(new AIEventHealthPercent(0.25f, m_owner));
			m_aiEvents[2].OnTrigger += OnLifeEventTriggered;
		}

		private void OnLifeEventTriggered()
		{
			MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(29);
			if (CastSpell(p_spell))
			{
				m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
			}
		}

		public override void DoTurn(Grid p_grid, Party p_party)
		{
			m_owner.AbilityHandler.ExecuteAttacks(null, null, false, EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			m_owner.AbilityHandler.FlushActionLog(EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			m_owner.BuffHandler.ModifyMonsterValues();
			m_owner.CombatHandler.MeleeStrikes += m_owner.CombatHandler.MeleeStrikesRoundBonus;
			m_owner.CombatHandler.MeleeStrikesRoundBonus = 0;
			CheckAIEvents();
			if (!m_castedSpell && Position.Distance(m_owner.Position, p_party.Position) < 1.1f)
			{
				DoMelee(true, p_party, p_grid, p_grid.GetSlot(m_owner.Position));
			}
		}

		public override Boolean CastSpell(MonsterSpell p_spell)
		{
			if (base.CastSpell(p_spell))
			{
				m_castedSpell = true;
				m_owner.CombatHandler.CastedSpell = p_spell;
				m_owner.CombatHandler.CastSpell = true;
				if (p_spell.TargetType == ETargetType.PARTY)
				{
					m_owner.DivideAttacksToPartyCharacters = true;
				}
				m_owner.BuffHandler.DoOnCastSpellEffects();
				m_owner.CombatHandler.DoAttack();
				return true;
			}
			return false;
		}

		protected override void CheckAIEvents()
		{
			m_castedSpell = false;
			for (Int32 i = 0; i < m_aiEvents.Count; i++)
			{
				m_aiEvents[i].Update();
			}
			m_currentTreshholdWitheringBreath += TRESHHOLD_INCREMENT;
			if (!m_castedSpell && Random.Value <= m_currentTreshholdWitheringBreath)
			{
				MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(22);
				if (CastSpell(p_spell))
				{
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					m_currentTreshholdWitheringBreath = RESET_TRESHHOLD;
				}
			}
		}

		public override void Destroy()
		{
			m_aiEvents[0].OnTrigger -= OnLifeEventTriggered;
			m_aiEvents[1].OnTrigger -= OnLifeEventTriggered;
			m_aiEvents[2].OnTrigger -= OnLifeEventTriggered;
			base.Destroy();
		}
	}
}
