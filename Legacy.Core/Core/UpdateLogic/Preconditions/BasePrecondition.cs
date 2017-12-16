using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Preconditions
{
	public abstract class BasePrecondition
	{
		private EPreconditionType m_type;

		private EPreconditionDecision m_decision;

		private String m_mainText;

		protected PreconditionGUIEventArgs m_eventArgs;

		protected Boolean m_result;

		protected Boolean m_cancelled;

		public BasePrecondition(EPreconditionType p_type)
		{
			m_type = p_type;
			m_result = true;
		}

		public EPreconditionType Type => m_type;

	    public EPreconditionDecision Decision
		{
			get => m_decision;
	        set => m_decision = value;
	    }

		public String Maintext
		{
			get => m_mainText;
		    set => m_mainText = value;
		}

		public virtual void Trigger()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PRECONDITION_GUI_DONE, new EventHandler(OnResult));
			m_eventArgs = new PreconditionGUIEventArgs(this);
			switch (m_decision)
			{
			case EPreconditionDecision.NONE:
				if (m_mainText != String.Empty)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.REQUEST_POPUP_CONFIRM, m_eventArgs);
				}
				else
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PRECONDITION_GUI_DONE, EventArgs.Empty);
				}
				break;
			case EPreconditionDecision.YES_NO:
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.REQUEST_POPUP_YES_NO, m_eventArgs);
				break;
			case EPreconditionDecision.WHO_WILL:
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.REQUEST_POPUP_WHO_WILL, m_eventArgs);
				break;
			case EPreconditionDecision.TEXT_INPUT:
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.REQUEST_POPUP_INPUT, m_eventArgs);
				break;
			}
		}

		public virtual void OnResult(Object p_sender, EventArgs p_eventArgs)
		{
			Character character = null;
			if (p_eventArgs is PreconditionResultWhoWillArgs)
			{
				character = ((PreconditionResultWhoWillArgs)p_eventArgs).m_selectedCharacter;
				LegacyLogic.Instance.WorldManager.Party.LastWhoWillCharacter = character;
			}
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PRECONDITION_GUI_DONE, new EventHandler(OnResult));
			PreconditionEvaluateArgs p_eventArgs2 = new PreconditionEvaluateArgs(m_result, m_cancelled, character);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PRECONDITION_EVALUATED, p_eventArgs2);
		}
	}
}
