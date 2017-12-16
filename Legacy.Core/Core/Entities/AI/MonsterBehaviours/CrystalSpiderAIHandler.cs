using System;
using Legacy.Core.Abilities;
using Legacy.Core.Entities.AI.Events;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class CrystalSpiderAIHandler : MonsterAIHandler
	{
		private const Single RESET_TRESHHOLD_BLINK = 0f;

		private const Single TRESHHOLD_INCREMENT = 0.03f;

		private Single m_currentTreshholBlink;

		private Boolean m_castedSpell;

		private Boolean m_castShellNextTurn = true;

		public CrystalSpiderAIHandler(Monster p_owner) : base(p_owner)
		{
			m_aiEvents.Add(new AIEventHealthPercent(0.7f, m_owner));
			m_aiEvents[0].OnTrigger += OnLifeEvent1Triggered;
			m_aiEvents.Add(new AIEventHealthPercent(0.3f, m_owner));
			m_aiEvents[1].OnTrigger += OnLifeEvent2Triggered;
			m_currentTreshholBlink = 0f;
		}

		private void OnLifeEvent2Triggered()
		{
			MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(26);
			if (CastSpell(p_spell))
			{
				m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
			}
			m_castedSpell = true;
		}

		private void OnLifeEvent1Triggered()
		{
			MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(25);
			if (CastSpell(p_spell))
			{
				m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
			}
			m_castedSpell = true;
		}

		public override void DoTurn(Grid p_grid, Party p_party)
		{
			m_owner.AbilityHandler.ExecuteAttacks(null, null, false, EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			m_owner.AbilityHandler.FlushActionLog(EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			m_owner.BuffHandler.ModifyMonsterValues();
			m_owner.CombatHandler.MeleeStrikes += m_owner.CombatHandler.MeleeStrikesRoundBonus;
			m_owner.CombatHandler.MeleeStrikesRoundBonus = 0;
			CheckAIEvents();
			if (!m_castedSpell)
			{
				if (Position.Distance(m_owner.Position, p_party.Position) < 1.1f)
				{
					DoMelee(true, p_party, p_grid, p_grid.GetSlot(m_owner.Position));
				}
				else if (p_grid.LineOfSight(m_owner.Position, p_party.Position))
				{
					MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(23);
					if (CastSpell(p_spell))
					{
						m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					}
				}
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
			if (m_castShellNextTurn)
			{
				MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(24);
				if (CastSpell(p_spell))
				{
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					m_castShellNextTurn = false;
				}
			}
			m_currentTreshholBlink += 0.03f;
			if (!m_castedSpell && Random.Value <= m_currentTreshholBlink)
			{
				MonsterSpell p_spell2 = m_owner.SpellHandler.SelectSpellByID(27);
				if (CastSpell(p_spell2))
				{
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					m_currentTreshholBlink = 0f;
					m_castShellNextTurn = true;
				}
			}
		}

		public override void Destroy()
		{
			m_aiEvents[0].OnTrigger -= OnLifeEvent1Triggered;
			m_aiEvents[1].OnTrigger -= OnLifeEvent2Triggered;
			base.Destroy();
		}
	}
}
