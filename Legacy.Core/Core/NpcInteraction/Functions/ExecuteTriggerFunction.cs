using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Utilities;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class ExecuteTriggerFunction : DialogFunction
	{
		[XmlAttribute("targetSpawnerID")]
		public Int32 TargetSpawnerID { get; set; }

		public override void Trigger(ConversationManager p_manager)
		{
			InteractiveObject interactiveObject = LegacyLogic.Instance.WorldManager.FindObjectBySpawnerId<InteractiveObject>(TargetSpawnerID);
			if (interactiveObject != null)
			{
				interactiveObject.ClearInteractions();
				interactiveObject.Execute(LegacyLogic.Instance.MapLoader.Grid);
				interactiveObject.Update();
			}
			else
			{
				LegacyLogger.LogError("InteractiveObject spawnerID not found! ID: " + TargetSpawnerID);
			}
		}
	}
}
