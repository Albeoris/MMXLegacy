using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class AutoSaveFunction : DialogFunction
	{
		public override void Trigger(ConversationManager p_manager)
		{
			LegacyLogic.Instance.WorldManager.HighestSaveGameNumber++;
			LegacyLogic.Instance.WorldManager.CurrentSaveGameType = ESaveGameType.AUTO;
			LegacyLogic.Instance.WorldManager.SaveGameName = Localization.Instance.GetText("SAVEGAMETYPE_AUTO");
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SAVEGAME_STARTED_SAVE, EventArgs.Empty);
		}

		private void OnGameSaved(Object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
