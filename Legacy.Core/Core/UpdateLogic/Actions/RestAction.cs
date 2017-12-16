using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class RestAction : BaseAction
	{
		public RestAction()
		{
			m_consumeType = EConsumeType.CONSUME_PARTY_TURN;
		}

		public override void DoAction(Command p_command)
		{
			if (Party.CanRest)
			{
				PartyBuff buff = Party.Buffs.GetBuff(EPartyBuffs.WELL_RESTED);
				if (buff != null)
				{
					buff.ExpireNow();
				}
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_RESTING, EventArgs.Empty);
			}
		}

		private void OnPartyRested(Object p_sender, EventArgs p_args)
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTED, new EventHandler(OnPartyRested));
			LegacyLogic.Instance.WorldManager.HighestSaveGameNumber++;
			LegacyLogic.Instance.WorldManager.CurrentSaveGameType = ESaveGameType.AUTO;
			LegacyLogic.Instance.WorldManager.SaveGameName = Localization.Instance.GetText("SAVEGAMETYPE_AUTO");
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SAVEGAME_STARTED_SAVE, EventArgs.Empty);
		}

		public override Boolean IsActionDone()
		{
			return Party.RestingDone.IsTriggered;
		}

		public override Boolean ActionAvailable()
		{
			return Party.Supplies > 0 && !Party.HasAggro() && !LegacyLogic.Instance.ConversationManager.IsOpen;
		}

		public override void Finish()
		{
			Party.RestingDone.Reset();
		}

		public override Boolean CanBeDelayedByLock()
		{
			return false;
		}
	}
}
