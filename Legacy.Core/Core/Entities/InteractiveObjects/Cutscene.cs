using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.StaticData;
using Legacy.Core.UpdateLogic.Interactions;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Cutscene : InteractiveObject
	{
		private static InteractiveObjectStaticData m_cutsceneStaticData = new InteractiveObjectStaticData
		{
			MapVisible = false,
			MinimapVisible = false,
			Edge = true,
			Center = true
		};

		private ECutsceneState m_currentState;

		private CutsceneInteraction m_startCutsceneInteraction;

		public Cutscene() : this(0)
		{
		}

		public Cutscene(Int32 p_spawnerID) : base(0, EObjectType.CUTSCENE, p_spawnerID)
		{
		}

		public ECutsceneState CurrentState => m_currentState;

	    public override EDirection Location
		{
			get
			{
				return EDirection.NORTH;
			}
			set
			{
			}
		}

		protected override void LoadStaticData()
		{
			m_staticData = m_cutsceneStaticData;
		}

		public void StartCutscene(CutsceneInteraction startCutsceneInteraction)
		{
			if (startCutsceneInteraction == null)
			{
				throw new ArgumentNullException();
			}
			if (m_currentState == ECutsceneState.Started)
			{
				throw new InvalidOperationException();
			}
			m_currentState = ECutsceneState.Started;
			m_startCutsceneInteraction = startCutsceneInteraction;
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(this, Position);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CUTSCENE_STARTED, p_eventArgs);
		}

		public void StopCutscene()
		{
			if (m_currentState == ECutsceneState.Stopped)
			{
				throw new InvalidOperationException();
			}
			m_currentState = ECutsceneState.Stopped;
			m_startCutsceneInteraction.FinishExecution();
			m_startCutsceneInteraction = null;
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(this, Position);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CUTSCENE_STOPPED, p_eventArgs);
		}

		public enum ECutsceneState
		{
			Stopped,
			Started
		}
	}
}
