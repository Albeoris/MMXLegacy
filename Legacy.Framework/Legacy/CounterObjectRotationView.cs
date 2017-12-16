using System;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/CounterObject/Rotation View")]
	public class CounterObjectRotationView : BaseView
	{
		public SoundSource AnimationSound;

		[SerializeField]
		private RotationObjectData[] m_rotateObjects;

		[SerializeField]
		private Single m_timePerCounterChange = 1f;

		[SerializeField]
		private AnimationCurve m_timeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		private Boolean m_running;

		private Single m_timer;

		private Int32 m_currentState = 1;

		private Int32 m_targetState = 1;

		private Boolean m_forward = true;

		[ContextMenu("Count UP")]
		private void CountUP()
		{
			RotateToState(m_currentState, m_currentState + 1, 1);
		}

		[ContextMenu("Count Down")]
		private void CountDown()
		{
			RotateToState(m_currentState, m_currentState - 1, -1);
		}

		protected override void Awake()
		{
			base.Awake();
		}

		protected new CounterObject MyController => (CounterObject)base.MyController;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_COUNTER_CHANGED, new EventHandler(OnCounterChanged));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_COUNTER_CHANGED, new EventHandler(OnCounterChanged));
				RotateToState(StateToInt(MyController.State), StateToInt(MyController.State), 0);
			}
		}

		private void Update()
		{
			if (!m_running)
			{
				enabled = false;
			}
			if (m_running)
			{
				if (m_targetState == m_currentState)
				{
					m_running = false;
				}
				else
				{
					m_timer += 1f / m_timePerCounterChange * Time.deltaTime;
					if (m_timer >= 1f)
					{
						m_timer = 0f;
						m_currentState += ((!m_forward) ? -1 : 1);
					}
					foreach (RotationObjectData rotationObjectData in m_rotateObjects)
					{
						Single t = m_timeCurve.Evaluate(m_timer);
						rotationObjectData.RotateObject.localEulerAngles = Vector3.Lerp(rotationObjectData.GetAxisRotation(m_currentState), rotationObjectData.GetAxisRotation(m_currentState + ((!m_forward) ? -1 : 1)), t);
					}
				}
			}
		}

		private void OnCounterChanged(Object p_sender, EventArgs p_args)
		{
			CounterObjectChangedArgs counterObjectChangedArgs = (CounterObjectChangedArgs)p_args;
			if ((BaseObject)p_sender == MyController)
			{
				RotateToState(counterObjectChangedArgs.OldCount, counterObjectChangedArgs.NewCount, counterObjectChangedArgs.ChangeCount);
			}
		}

		private void RotateToState(Int32 p_oldState, Int32 p_newState, Int32 p_delta)
		{
			if (AnimationSound != null)
			{
				AnimationSound.PlaySound();
			}
			if (p_delta > 0 && p_oldState > p_newState)
			{
				m_currentState = p_oldState;
				m_targetState = p_oldState + p_delta;
				m_forward = true;
			}
			else if (p_delta < 0 && p_oldState < p_newState)
			{
				m_currentState = p_oldState;
				m_targetState = p_oldState + p_delta;
				m_forward = false;
			}
			else if (p_oldState == p_newState)
			{
				m_currentState = p_oldState;
				m_targetState = p_newState;
				foreach (RotationObjectData rotationObjectData in m_rotateObjects)
				{
					rotationObjectData.RotateObject.localEulerAngles = rotationObjectData.GetAxisRotation(m_targetState);
				}
			}
			else
			{
				m_currentState = p_oldState;
				m_targetState = p_newState;
				m_forward = (p_newState > p_oldState);
			}
			m_running = true;
			enabled = true;
			m_timer = 0f;
		}

		private Int32 StateToInt(EInteractiveObjectState p_state)
		{
			switch (p_state)
			{
			case EInteractiveObjectState.COUNTER_1:
				return 1;
			case EInteractiveObjectState.COUNTER_2:
				return 2;
			case EInteractiveObjectState.COUNTER_3:
				return 3;
			case EInteractiveObjectState.COUNTER_4:
				return 4;
			case EInteractiveObjectState.COUNTER_5:
				return 5;
			case EInteractiveObjectState.COUNTER_6:
				return 6;
			case EInteractiveObjectState.COUNTER_7:
				return 7;
			case EInteractiveObjectState.COUNTER_8:
				return 8;
			case EInteractiveObjectState.COUNTER_9:
				return 9;
			default:
				return 1;
			}
		}

		[Serializable]
		public class RotationObjectData
		{
			public Transform RotateObject;

			public Int32 BaseAngle;

			public Int32 AnglesPerCount = 90;

			public Vector3 LocalRotationAxis = Vector3.up;

			public Vector3 GetAxisRotation(Int32 p_count)
			{
				return (BaseAngle + AnglesPerCount * p_count) * LocalRotationAxis;
			}
		}
	}
}
