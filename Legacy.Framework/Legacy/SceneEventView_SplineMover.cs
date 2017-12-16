using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/Scene/SceneEventView_SplineMover")]
	public class SceneEventView_SplineMover : BaseView
	{
		private SplineTimer mPositionTimer;

		public String ViewListenCommandName = String.Empty;

		public RunMode MoveMode;

		public SplineController PositionSpline;

		public Single Speed = 1f;

		public AnimationCurve SpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		protected SplineTimer PositionTimer
		{
			get
			{
				if (mPositionTimer == null)
				{
					mPositionTimer = new SplineTimer(MoveMode);
				}
				return mPositionTimer;
			}
		}

		[ContextMenu("Activate")]
		private void Activate()
		{
			ActivateMover();
		}

		[ContextMenu("Deactivate")]
		private void Deactivate()
		{
			DeactivateMover();
		}

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
			enabled = false;
			gameObject.SetActive(false);
		}

		private void OnAnimTriggered(Object p_sender, EventArgs p_args)
		{
			StringEventArgs stringEventArgs = p_args as StringEventArgs;
			if (stringEventArgs != null && p_sender == null)
			{
				String[] array = stringEventArgs.text.Split(new Char[]
				{
					'_'
				});
				if (array.Length > 1 && array[0] == ViewListenCommandName && array[1] == "Activate")
				{
					ActivateMover();
				}
				else if (array.Length > 1 && array[0] == ViewListenCommandName && array[1] == "Deactivate")
				{
					DeactivateMover();
				}
			}
		}

		private void ActivateMover()
		{
			mPositionTimer = new SplineTimer(MoveMode);
			enabled = true;
			gameObject.SetActive(true);
		}

		private void DeactivateMover()
		{
			enabled = false;
			gameObject.SetActive(false);
		}

		private void Update()
		{
			if (PositionSpline == null)
			{
				return;
			}
			PositionTimer.Update();
			PositionUpdate();
		}

		protected void PositionUpdate()
		{
			if (PositionSpline == null)
			{
				return;
			}
			Single p_time = SpeedCurve.Evaluate(PositionTimer.Time);
			SplineController.SplineSampleData splineSampleData = PositionSpline.ResolveDataOnSplineTime(p_time);
			PositionTimer.ChangeTime(Mathf.Min(Time.deltaTime * splineSampleData.Speed, PositionSpline.GetMaxTimeStep()) * Speed);
			transform.position = splineSampleData.Position;
			transform.rotation = splineSampleData.Rotation;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
		}

		public enum RunMode
		{
			Forward_Clamp,
			Backward_Clamp,
			Forward_Loop,
			Backward_Loop,
			Forward_PingPong,
			Backward_PingPong
		}

		public class SplineTimer
		{
			public Boolean mForward;

			public Single mTime;

			public RunMode mMoveMode;

			public SplineTimer(RunMode Mode)
			{
				mMoveMode = Mode;
				switch (mMoveMode)
				{
				case RunMode.Forward_Clamp:
				case RunMode.Forward_Loop:
				case RunMode.Forward_PingPong:
					mForward = true;
					mTime = 0f;
					break;
				case RunMode.Backward_Clamp:
				case RunMode.Backward_Loop:
				case RunMode.Backward_PingPong:
					mForward = false;
					mTime = 1f;
					break;
				}
			}

			public Single Time
			{
				get => mTime;
			    set => mTime = value;
			}

			public void ChangeTime(Single p_timeChange)
			{
				mTime += p_timeChange * ((!mForward) ? -1 : 1);
			}

			public void Update()
			{
				if (mTime >= 1f)
				{
					switch (mMoveMode)
					{
					case RunMode.Forward_Clamp:
						mTime = 1f;
						break;
					case RunMode.Forward_Loop:
						mTime = 0f;
						break;
					case RunMode.Forward_PingPong:
					case RunMode.Backward_PingPong:
						mForward = false;
						break;
					}
				}
				else if (mTime <= 0f)
				{
					switch (mMoveMode)
					{
					case RunMode.Backward_Clamp:
						mTime = 0f;
						break;
					case RunMode.Backward_Loop:
						mTime = 1f;
						break;
					case RunMode.Forward_PingPong:
					case RunMode.Backward_PingPong:
						mForward = true;
						break;
					}
				}
			}
		}
	}
}
