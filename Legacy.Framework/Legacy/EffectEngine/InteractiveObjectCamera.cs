using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.EffectEngine
{
	public class InteractiveObjectCamera : BaseCameraFX
	{
		private Transform m_LookPosition;

		private Vector3 m_LastLookPosition = Vector3.zero;

		private Quaternion mLastLookRotation = Quaternion.identity;

		private Boolean m_doDestroy;

		private Single m_Timer;

		[SerializeField]
		private Single m_Speed = 3f;

		[SerializeField]
		private Transform m_DefaultLookPosition;

		private static InteractiveObjectCamera s_instance;

		public static InteractiveObjectCamera Instance => s_instance;

	    public void Reset()
		{
			m_doDestroy = false;
			enabled = true;
		}

		public void ActivateInteractiveObjectLook(Transform p_lookAtTransform)
		{
			m_Timer = 0f;
			mLastLookRotation = transform.rotation;
			m_LastLookPosition = m_LookPosition.position;
			m_LookPosition = p_lookAtTransform;
			m_doDestroy = false;
			enabled = true;
		}

		private void TurnToDefaultView()
		{
			m_Timer = 0f;
			m_doDestroy = true;
			enabled = true;
			m_LastLookPosition = m_DefaultLookPosition.position;
			mLastLookRotation = transform.rotation;
			m_LookPosition = m_DefaultLookPosition;
		}

		public override void CancelEffect()
		{
			TurnToDefaultView();
		}

		protected override void Awake()
		{
			base.Awake();
			s_instance = this;
			enabled = false;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntityEvent));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ROTATE_ENTITY, new EventHandler(OnRotateOrSetPosEvent));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnRotateOrSetPosEvent));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnTeleported));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTERS_TURNED_AGGRO, new EventHandler(OnAggro));
			m_LookPosition = m_DefaultLookPosition;
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntityEvent));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ROTATE_ENTITY, new EventHandler(OnRotateOrSetPosEvent));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnRotateOrSetPosEvent));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnTeleported));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTERS_TURNED_AGGRO, new EventHandler(OnAggro));
		}

		private void OnEnable()
		{
			FreeRotationCamera component = GetComponent<FreeRotationCamera>();
			if (component != null)
			{
				component.enabled = false;
			}
		}

		private void OnDisable()
		{
			FreeRotationCamera component = GetComponent<FreeRotationCamera>();
			if (component != null)
			{
				component.enabled = true;
			}
		}

		private void LateUpdate()
		{
			m_Timer += Time.deltaTime * m_Speed;
			m_Timer = Mathf.Clamp01(m_Timer);
			transform.LookAt(m_LookPosition.position, Vector3.up);
			Quaternion rotation = transform.rotation;
			transform.rotation = Quaternion.Slerp(mLastLookRotation, rotation, m_Timer);
			if (m_doDestroy && m_Timer == 1f)
			{
				enabled = false;
			}
		}

		private void OnMoveEntityEvent(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
			if (moveEntityEventArgs != null && p_sender == LegacyLogic.Instance.WorldManager.Party && enabled)
			{
				TurnToDefaultView();
			}
		}

		private void OnRotateOrSetPosEvent(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == LegacyLogic.Instance.WorldManager.Party && enabled)
			{
				TurnToDefaultView();
			}
		}

		private void OnTeleported(Object p_sender, EventArgs p_args)
		{
			if (enabled)
			{
				TurnToDefaultView();
			}
		}

		private void OnAggro(Object p_sender, EventArgs p_args)
		{
			if (enabled)
			{
				TurnToDefaultView();
			}
		}
	}
}
