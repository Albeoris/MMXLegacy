using System;
using System.Xml.Serialization;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class QuitFunction : DialogFunction
	{
		[XmlAttribute("cutsceneVideoID")]
		public String CutsceneVideoID { get; set; }

		public override void Trigger(ConversationManager p_manager)
		{
			p_manager.CloseNpcContainer(CutsceneVideoID);
		}
	}
}
