using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Map;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.UpdateLogic.Interactions;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class ConditionalTrigger : InteractiveObject
	{
		private List<ObjectCondition> m_conditions;

		private Boolean m_allSatisfied;

		public ConditionalTrigger() : this(0, 0)
		{
		}

		public ConditionalTrigger(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.CONDITIONAL_TRIGGER, p_spawnerID)
		{
			m_conditions = new List<ObjectCondition>();
			m_allSatisfied = false;
		}

		public Boolean AllSatisfied => m_allSatisfied;

	    public List<ObjectCondition> TargetConditions => m_conditions;

	    public override void SetData(EInteractiveObjectData p_key, String p_value)
		{
			if (p_key == EInteractiveObjectData.CONDITIONAL_DATA)
			{
				String[] array = p_value.Split(new Char[]
				{
					','
				});
				Int32 id = Convert.ToInt32(array[0]);
				if (Enum.IsDefined(typeof(EInteractiveObjectState), array[1]))
				{
					EInteractiveObjectState wantedState = (EInteractiveObjectState)Enum.Parse(typeof(EInteractiveObjectState), array[1]);
					ObjectCondition item;
					item.id = id;
					item.wantedState = wantedState;
					m_conditions.Add(item);
				}
			}
			else
			{
				base.SetData(p_key, p_value);
			}
		}

		public override void OnAddedToGrid()
		{
			LegacyLogic.Instance.UpdateManager.InteractionsActor.Conditionals.Add(this);
		}

		public override void Execute(Grid p_grid)
		{
			m_allSatisfied = (m_conditions.Count > 0);
			foreach (ObjectCondition objectCondition in m_conditions)
			{
				InteractiveObject interactiveObject = p_grid.FindInteractiveObject(objectCondition.id);
				if (interactiveObject == null)
				{
					m_allSatisfied = false;
					break;
				}
				if (interactiveObject.State != objectCondition.wantedState)
				{
					m_allSatisfied = false;
					break;
				}
			}
			EInteractionTiming einteractionTiming = EInteractionTiming.ON_FAIL;
			if (m_allSatisfied)
			{
				einteractionTiming = EInteractionTiming.ON_SUCCESS;
			}
			for (Int32 i = 0; i < Commands.Count; i++)
			{
				SpawnCommand spawnCommand = Commands[i];
				if ((spawnCommand.Timing == einteractionTiming || spawnCommand.Timing == EInteractionTiming.ON_EXECUTE) && (spawnCommand.ActivateCount == -1 || spawnCommand.ActivateCount > 0))
				{
					BaseInteraction baseInteraction = InteractionFactory.Create(this, spawnCommand, SpawnerID, i);
					if (baseInteraction.Valid && (spawnCommand.RequiredState == EInteractiveObjectState.NONE || StateIsMatching(spawnCommand, baseInteraction)))
					{
						Interactions.Add(baseInteraction);
					}
				}
				m_stateMachine.ChangeState(EState.IDLE);
			}
			if (einteractionTiming == EInteractionTiming.ON_SUCCESS && QuestObjectives != null && QuestObjectives.Count > 0)
			{
				LegacyLogic.Instance.WorldManager.QuestHandler.ObjectInteraction(this);
			}
		}

		public override void OnAfterExecute(EInteractionTiming p_success)
		{
			m_stateMachine.ChangeState(EState.FINISHED);
		}

		public override void Load(SaveGameData p_data)
		{
			m_conditions.Clear();
			base.Load(p_data);
			Int32 num = p_data.Get<Int32>("ConditionCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				ObjectCondition item;
				item.id = p_data.Get<Int32>("ID" + i, 0);
				item.wantedState = p_data.Get<EInteractiveObjectState>("State" + i, EInteractiveObjectState.NONE);
				m_conditions.Add(item);
			}
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<Int32>("ConditionCount", m_conditions.Count);
			Int32 num = 0;
			foreach (ObjectCondition objectCondition in m_conditions)
			{
				p_data.Set<Int32>("ID" + num, objectCondition.id);
				p_data.Set<Int32>("State" + num, (Int32)objectCondition.wantedState);
				num++;
			}
		}
	}
}
