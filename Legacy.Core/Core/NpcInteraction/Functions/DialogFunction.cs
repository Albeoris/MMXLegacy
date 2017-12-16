using System;

namespace Legacy.Core.NpcInteraction.Functions
{
	public abstract class DialogFunction
	{
		public virtual Boolean RequireGold => false;

	    public abstract void Trigger(ConversationManager p_manager);
	}
}
