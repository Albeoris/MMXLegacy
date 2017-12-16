using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class TriggerAggroFunction : DialogFunction
	{
		private Int32 m_monsterId;

		[XmlAttribute("monsterID")]
		public Int32 MonsterID
		{
			get => m_monsterId;
		    set => m_monsterId = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			if (m_monsterId > 0)
			{
				p_manager.DialogClosed += SetAggroForMonster;
			}
			p_manager.CloseNpcContainer(null);
			p_manager.CloseDialog();
		}

		public void SetAggroForMonster(Object p_sender, EventArgs p_args)
		{
			((ConversationManager)p_sender).DialogClosed -= SetAggroForMonster;
			if (m_monsterId > 0)
			{
				foreach (Monster monster in LegacyLogic.Instance.WorldManager.GetObjectsByTypeIterator<Monster>())
				{
					if (monster.StaticID == m_monsterId)
					{
						monster.AlwaysTriggerAggro = true;
						monster.AiHandler.UpdateDistanceToParty(LegacyLogic.Instance.WorldManager.Party, LegacyLogic.Instance.MapLoader.Grid);
						monster.CheckAggroRange();
						LegacyLogic.Instance.UpdateManager.SkipPartyTurn = true;
						LegacyLogic.Instance.UpdateManager.ResetTurn();
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
						if (m_monsterId == 506)
						{
							BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(monster, monster.Position);
							LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTERS_TURNED_AGGRO, p_eventArgs);
						}
						break;
					}
				}
			}
		}
	}
}
