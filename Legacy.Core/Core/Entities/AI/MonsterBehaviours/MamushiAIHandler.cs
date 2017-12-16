using System;
using Legacy.Core.Abilities;
using Legacy.Core.Api;
using Legacy.Core.Entities.AI.Events;
using Legacy.Core.EventManagement;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class MamushiAIHandler : MonsterAIHandler
	{
		private static Single[] CURLING_TRIGGER_CHANCE = new Single[]
		{
			0.1f,
			0.3f,
			0.1f
		};

		private static Single[] PUSH_TRIGGER_CHANCE = new Single[]
		{
			0f,
			0.15f,
			0.45f
		};

		private Single MAX_TRESHHOLD = 0.85f;

		private Single TRESHHOLD_INCREMENT = 0.08f;

		private Single m_currentTreshholdCurling;

		private Single m_currentTreshholdPush;

		private Int32 m_phase;

		private Boolean m_firstHitDone;

		public MamushiAIHandler(Monster p_owner) : base(p_owner)
		{
			if (m_owner.AbilityHandler.HasAbility(EMonsterAbilityType.CURLING) && m_owner.AbilityHandler.HasAbility(EMonsterAbilityType.PUSH))
			{
				m_currentTreshholdCurling = 0f;
				m_currentTreshholdPush = 1f;
			}
			m_aiEvents.Add(new AIEventHealthPercent(0.75f, m_owner));
			m_aiEvents[0].OnTrigger += StartSecondPhase;
			m_aiEvents.Add(new AIEventHealthPercent(0.25f, m_owner));
			m_aiEvents[1].OnTrigger += StartFinalPhase;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ENTITY_ABILITY_ADDED, new EventHandler(OnAbilityTriggered));
		}

		private void OnAbilityTriggered(Object p_sender, EventArgs p_args)
		{
			AbilityEventArgs abilityEventArgs = (AbilityEventArgs)p_args;
			if (abilityEventArgs.Monster == m_owner)
			{
				if (abilityEventArgs.Ability.AbilityType == EMonsterAbilityType.CURLING)
				{
					m_firstHitDone = true;
					m_currentTreshholdCurling = CURLING_TRIGGER_CHANCE[m_phase];
				}
				else if (abilityEventArgs.Ability.AbilityType == EMonsterAbilityType.PUSH)
				{
					m_firstHitDone = true;
					m_currentTreshholdPush = PUSH_TRIGGER_CHANCE[m_phase];
				}
			}
		}

		protected override void CheckAIEvents()
		{
			UpdateAbilityChances();
			MonsterAbilityBase ability = m_owner.AbilityHandler.GetAbility(EMonsterAbilityType.CURLING);
			ability.TriggerChance = m_currentTreshholdCurling;
			MonsterAbilityBase ability2 = m_owner.AbilityHandler.GetAbility(EMonsterAbilityType.PUSH);
			ability2.TriggerChance = m_currentTreshholdPush;
			for (Int32 i = 0; i < m_aiEvents.Count; i++)
			{
				m_aiEvents[i].Update();
			}
		}

		private void UpdateAbilityChances()
		{
			if (m_firstHitDone)
			{
				m_currentTreshholdCurling += TRESHHOLD_INCREMENT;
				m_currentTreshholdCurling = Math.Min(m_currentTreshholdCurling, MAX_TRESHHOLD);
				m_currentTreshholdPush += TRESHHOLD_INCREMENT;
				m_currentTreshholdPush = Math.Min(m_currentTreshholdPush, MAX_TRESHHOLD);
			}
		}

		private void StartSecondPhase()
		{
			m_phase = 1;
		}

		private void StartFinalPhase()
		{
			m_phase = 2;
		}

		public override void Destroy()
		{
			m_aiEvents[0].OnTrigger -= StartSecondPhase;
			m_aiEvents[1].OnTrigger -= StartFinalPhase;
			base.Destroy();
		}
	}
}
