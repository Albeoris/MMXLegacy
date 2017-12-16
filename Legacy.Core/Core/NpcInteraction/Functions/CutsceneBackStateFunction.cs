using System;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class CutsceneBackStateFunction : DialogFunction
	{
		public override void Trigger(ConversationManager p_manager)
		{
			p_manager._ChangeCutsceneState(p_manager.CutsceneState - 1);
		}
	}
}
