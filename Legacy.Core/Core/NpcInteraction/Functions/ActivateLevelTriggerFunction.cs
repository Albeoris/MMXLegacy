using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.UpdateLogic.Interactions;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class ActivateLevelTriggerFunction : DialogFunction
	{
		private Int32 m_targetSpawnerID;

		private Int32 m_dialogID;

		private Int32 m_removeTokenID;

		[XmlAttribute("targetSpawnerID")]
		public Int32 TargetSpawnerID
		{
			get => m_targetSpawnerID;
		    set => m_targetSpawnerID = value;
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("removeTokenID")]
		public Int32 RemoveTokenID
		{
			get => m_removeTokenID;
		    set => m_removeTokenID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			foreach (CommandContainer commandContainer in LegacyLogic.Instance.WorldManager.GetObjectsByType<CommandContainer>())
			{
				if (commandContainer.SpawnerID == TargetSpawnerID)
				{
					List<BaseInteraction> list = new List<BaseInteraction>();
					for (Int32 i = 0; i < commandContainer.Commands.Count; i++)
					{
						SpawnCommand spawnCommand = commandContainer.Commands[i];
						if (spawnCommand.Timing == EInteractionTiming.ON_EXECUTE && (spawnCommand.ActivateCount == -1 || spawnCommand.ActivateCount > 0))
						{
							BaseInteraction baseInteraction = InteractionFactory.Create(commandContainer, spawnCommand, TargetSpawnerID, i);
							if (baseInteraction.Valid && (spawnCommand.RequiredState == EInteractiveObjectState.NONE || StateIsMatching(spawnCommand, baseInteraction)))
							{
								list.Add(baseInteraction);
							}
						}
					}
					foreach (BaseInteraction baseInteraction2 in list)
					{
						baseInteraction2.Execute();
					}
					break;
				}
			}
			if (m_removeTokenID != 0)
			{
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.RemoveToken(m_removeTokenID);
			}
			if (m_dialogID > 0)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
			}
			else if (m_dialogID == 0)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, p_manager.CurrentConversation.RootDialog.ID);
			}
			else
			{
				p_manager.CloseNpcContainer(null);
			}
		}

		protected Boolean StateIsMatching(SpawnCommand sc, BaseInteraction b)
		{
			if (b is BaseDoorInteraction)
			{
				return ((BaseDoorInteraction)b).Target.State == sc.RequiredState;
			}
			if (b is BaseLeverInteraction)
			{
				return ((BaseLeverInteraction)b).Target.State == sc.RequiredState;
			}
			if (b is BaseButtonInteraction)
			{
				return ((BaseButtonInteraction)b).Target.State == sc.RequiredState;
			}
			return b is SetStateInteraction && ((SetStateInteraction)b).Target.State == sc.RequiredState;
		}
	}
}
