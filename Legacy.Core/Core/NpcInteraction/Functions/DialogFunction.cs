using System;

namespace Legacy.Core.NpcInteraction.Functions
{
	public abstract class DialogFunction
	{
		public virtual Boolean RequireGold => false;
	    public virtual void OnShow(Func<String, String> localisation) { }
	    public abstract void Trigger(ConversationManager p_manager);
	}
}
