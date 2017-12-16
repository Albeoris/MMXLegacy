using System;
using System.Xml.Serialization;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class CutsceneStateFunction : DialogFunction
	{
		[XmlAttribute("targetState")]
		public Int32 TargetState { get; set; }

		public override void Trigger(ConversationManager p_manager)
		{
			p_manager._ChangeCutsceneState(TargetState);
		}
	}
}
