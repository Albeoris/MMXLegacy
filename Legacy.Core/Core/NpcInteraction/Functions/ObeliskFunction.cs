using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class ObeliskFunction : DialogFunction
	{
		private Int32 m_failDialogID;

		private Int32 m_unknownDialogID;

		private String m_dialogText;

		[XmlAttribute("failDialogID")]
		public Int32 FailDialogID
		{
			get => m_failDialogID;
		    set => m_failDialogID = value;
		}

		[XmlAttribute("unknownDialogID")]
		public Int32 UnknownDialogID
		{
			get => m_unknownDialogID;
		    set => m_unknownDialogID = value;
		}

		[XmlAttribute("dialogText")]
		public String DialogText
		{
			get => m_dialogText;
		    set => m_dialogText = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.REQUEST_POPUP_OBELISK, EventArgs.Empty);
		}

		public void EvaluateUserInput(String p_answer)
		{
			EObeliskResult eobeliskResult = EObeliskResult.WRONG_INPUT;
			Position zero = Position.Zero;
			List<ObelisksStaticData> list = new List<ObelisksStaticData>(StaticDataHandler.GetIterator<ObelisksStaticData>(EDataType.OBELISKS));
			foreach (ObelisksStaticData data in list)
			{
				EObeliskResult eobeliskResult2 = CheckObelisk(p_answer, data, ref zero);
				if (eobeliskResult2 != EObeliskResult.WRONG_INPUT)
				{
					eobeliskResult = eobeliskResult2;
					break;
				}
			}
			if (eobeliskResult == EObeliskResult.UNKNOWN)
			{
				LegacyLogic.Instance.ConversationManager._ChangeDialog(LegacyLogic.Instance.ConversationManager.CurrentNpc.StaticID, UnknownDialogID);
			}
			else if (eobeliskResult == EObeliskResult.WRONG_INPUT)
			{
				LegacyLogic.Instance.ConversationManager._ChangeDialog(LegacyLogic.Instance.ConversationManager.CurrentNpc.StaticID, FailDialogID);
			}
			else
			{
				LegacyLogic.Instance.ConversationManager.CloseNpcContainer(null);
				Party party = LegacyLogic.Instance.WorldManager.Party;
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				Position position = party.Position;
				if (grid.AddMovingEntity(zero, party))
				{
					GridSlot slot = grid.GetSlot(zero);
					if (!slot.VisitedByParty)
					{
						slot.VisitedByParty = true;
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UNCOVERED_TILES, EventArgs.Empty);
					}
					grid.GetSlot(position).RemoveEntity(party);
					party.SelectedInteractiveObject = null;
					BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(party, party.Position);
					LegacyLogic.Instance.EventManager.InvokeEvent(party, EEventType.TELEPORT_ENTITY, p_eventArgs);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_TELEPORTER_USED, EventArgs.Empty);
				}
			}
		}

		private EObeliskResult CheckObelisk(String p_answer, ObelisksStaticData data, ref Position target)
		{
			EObeliskResult result = EObeliskResult.WRONG_INPUT;
			if (p_answer.ToUpper() == Localization.Instance.GetText(data.NameKey).ToUpper())
			{
				if (LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(data.TokenID) > 0)
				{
					result = EObeliskResult.TRAVEL;
					target = data.Position;
				}
				else
				{
					result = EObeliskResult.UNKNOWN;
				}
			}
			return result;
		}

		private enum EObeliskResult
		{
			TRAVEL,
			WRONG_INPUT,
			UNKNOWN
		}
	}
}
