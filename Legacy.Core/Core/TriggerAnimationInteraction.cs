using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.UpdateLogic.Interactions;

namespace Legacy.Core
{
	public class TriggerAnimationInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		protected InteractiveObject m_parent;

		private PrefabContainer m_container;

		private String m_animationName;

		public TriggerAnimationInteraction()
		{
		}

		public TriggerAnimationInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			InteractiveObject interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			m_container = (interactiveObject as PrefabContainer);
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			if (m_container == null)
			{
				StringEventArgs p_eventArgs = new StringEventArgs(m_animationName);
				LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, p_eventArgs);
			}
			else
			{
				m_container.CurrentAnim = m_animationName;
				StringEventArgs p_eventArgs2 = new StringEventArgs(m_animationName);
				LegacyLogic.Instance.EventManager.InvokeEvent(m_container, EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, p_eventArgs2);
			}
			FinishExecution();
		}

		protected override void ParseExtra(String p_extra)
		{
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (array.Length != 1)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					1
				}));
			}
			m_animationName = array[0];
		}

		public override void FinishExecution()
		{
			if (m_parent != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_parent.DecreaseActivate(m_commandIndex);
			}
			base.FinishExecution();
		}
	}
}
