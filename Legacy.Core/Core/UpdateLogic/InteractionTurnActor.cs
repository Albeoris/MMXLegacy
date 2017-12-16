using System;
using System.Collections.Generic;
using System.Text;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic
{
	public class InteractionTurnActor : TurnActor
	{
		private List<InteractiveObject> m_interactiveObjects;

		private List<InteractiveObject> m_additionalObjects;

		private List<InteractiveObject> m_buttons;

		private List<ConditionalTrigger> m_conditionals;

		private Boolean m_conditionsEvaluated;

		private Boolean m_isUpdating;

		public InteractionTurnActor()
		{
			m_interactiveObjects = new List<InteractiveObject>();
			m_additionalObjects = new List<InteractiveObject>();
			m_buttons = new List<InteractiveObject>();
			m_conditionals = new List<ConditionalTrigger>();
		}

		public List<ConditionalTrigger> Conditionals => m_conditionals;

	    public void AddObject(List<InteractiveObject> p_list)
		{
			foreach (InteractiveObject io in p_list)
			{
				AddObject(io);
			}
		}

		public void AddObject(InteractiveObject io)
		{
			if (io == null)
			{
				return;
			}
			if (!m_isUpdating)
			{
				m_interactiveObjects.Add(io);
			}
			else
			{
				m_additionalObjects.Add(io);
			}
			if (io is Button || io is PressurePlate)
			{
				m_buttons.Add(io);
			}
			if (io is ConditionalTrigger)
			{
				m_conditionals.Add((ConditionalTrigger)io);
			}
		}

		public override void StartTurn()
		{
			base.StartTurn();
			m_conditionsEvaluated = false;
		}

		public override void UpdateTurn()
		{
			base.UpdateTurn();
			if (State == EState.IDLE)
			{
				if (m_interactiveObjects.Count > 0)
				{
					Grid grid = LegacyLogic.Instance.MapLoader.Grid;
					foreach (InteractiveObject interactiveObject in m_interactiveObjects)
					{
						interactiveObject.Execute(grid);
					}
				}
				m_stateMachine.ChangeState(EState.RUNNING);
			}
			else if (State == EState.RUNNING)
			{
				Boolean flag = true;
				foreach (InteractiveObject interactiveObject2 in m_interactiveObjects)
				{
					if (interactiveObject2.TurnState != InteractiveObject.EState.FINISHED && interactiveObject2.TurnState != InteractiveObject.EState.IDLE)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					UpdateButtonStates();
					Boolean flag2 = false;
					if (!m_conditionsEvaluated)
					{
						m_interactiveObjects.Clear();
						flag2 = CheckConditionals();
						m_conditionsEvaluated = true;
					}
					if (flag2)
					{
						m_stateMachine.ChangeState(EState.IDLE);
					}
					else
					{
						m_stateMachine.ChangeState(EState.FINISHED);
						m_interactiveObjects.Clear();
					}
				}
			}
			m_isUpdating = true;
			foreach (InteractiveObject interactiveObject3 in m_interactiveObjects)
			{
				interactiveObject3.Update();
			}
			m_isUpdating = false;
		}

		public void UpdateButtonStates()
		{
			for (Int32 i = m_buttons.Count - 1; i >= 0; i--)
			{
				if (m_buttons[i] is Button)
				{
					Button button = (Button)m_buttons[i];
					if (button.UpdateTimer())
					{
						m_buttons.RemoveAt(i);
					}
				}
				else if (m_buttons[i] is PressurePlate)
				{
					PressurePlate pressurePlate = (PressurePlate)m_buttons[i];
					if (pressurePlate.UpdateTimer())
					{
						m_buttons.RemoveAt(i);
					}
				}
			}
		}

		private Boolean CheckConditionals()
		{
			Boolean result = false;
			foreach (ConditionalTrigger conditionalTrigger in m_conditionals)
			{
				if (conditionalTrigger.Enabled)
				{
					m_interactiveObjects.Add(conditionalTrigger);
					result = true;
				}
			}
			return result;
		}

		public override void ExecutionBreak()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("InteractionTurnActor execution aborded!");
			stringBuilder.AppendLine("Left state: " + m_stateMachine.CurrentState);
			stringBuilder.AppendLine("Number of open interactive objects: " + m_interactiveObjects.Count);
			foreach (InteractiveObject interactiveObject in m_interactiveObjects)
			{
				interactiveObject.ExecutionBreak(stringBuilder);
			}
			LegacyLogger.Log(stringBuilder.ToString(), false);
			m_stateMachine.ChangeState(EState.FINISHED);
			m_interactiveObjects.Clear();
		}

		public override void Clear()
		{
			base.Clear();
			m_interactiveObjects.Clear();
			m_buttons.Clear();
			m_conditionals.Clear();
		}

		public override void ClearAndDestroy()
		{
			Clear();
			base.ClearAndDestroy();
		}
	}
}
