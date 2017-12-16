using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views.Cutscenes
{
	[AddComponentMenu("MM Legacy/Views/CutsceneView")]
	public class CutsceneView : BaseView
	{
		public const String METHOD_START_BROADCAST = "OnCutsceneStart";

		public const String METHOD_STOP_BROADCAST = "OnCutsceneStop";

		public new Cutscene MyController => (Cutscene)base.MyController;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			if (MyController != null && !(MyController is Cutscene))
			{
				throw new NotSupportedException("Only Cutscene objects\n" + MyController.GetType().FullName);
			}
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CUTSCENE_STARTED, new EventHandler(OnCutsceneStarted));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CUTSCENE_STOPPED, new EventHandler(OnCutsceneStopped));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CUTSCENE_STARTED, new EventHandler(OnCutsceneStarted));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CUTSCENE_STOPPED, new EventHandler(OnCutsceneStopped));
				Cutscene.ECutsceneState currentState = MyController.CurrentState;
				if (currentState != Cutscene.ECutsceneState.Stopped)
				{
					if (currentState == Cutscene.ECutsceneState.Started)
					{
						OnCutsceneStarted(MyController, null);
					}
				}
				else
				{
					OnCutsceneStopped(MyController, null);
				}
			}
		}

		protected override void OnDestroy()
		{
			ViewManager.DestroyView(MyController);
			base.OnDestroy();
		}

		private void OnCutsceneStarted(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				UnityEventArgs parameter = new UnityEventArgs(this);
				BroadcastMessage("OnCutsceneStart", parameter, SendMessageOptions.DontRequireReceiver);
			}
		}

		private void OnCutsceneStopped(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				UnityEventArgs parameter = new UnityEventArgs(this);
				BroadcastMessage("OnCutsceneStop", parameter, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
