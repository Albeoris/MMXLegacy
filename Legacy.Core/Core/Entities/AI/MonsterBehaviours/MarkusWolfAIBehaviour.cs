using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class MarkusWolfAIBehaviour : MonsterAIHandler
	{
		public MarkusWolfAIBehaviour(Monster p_owner) : base(p_owner)
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_STATUS_CHANGED, new EventHandler(OnStatusChanged));
		}

		private void OnStatusChanged(Object p_sender, EventArgs p_event)
		{
			StatusChangedEventArgs statusChangedEventArgs = (StatusChangedEventArgs)p_event;
			if (m_owner == p_sender && statusChangedEventArgs.Type == StatusChangedEventArgs.EChangeType.HEALTH_POINTS)
			{
				m_owner.Die();
				m_owner.TriggerLateDieEffects();
				foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
				{
					character.FlushRewardsActionLog();
				}
			}
		}
	}
}
