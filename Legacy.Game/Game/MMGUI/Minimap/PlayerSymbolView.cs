using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/PlayerSymbolView")]
	public class PlayerSymbolView : MoveableSymbolView
	{
		private Boolean m_RotateAnchor;

		[SerializeField]
		private Transform m_SymbolAnchor;

		[SerializeField]
		private Single m_MoveTime = 0.2f;

		[SerializeField]
		private Single m_RotateTime = 0.3f;

		private Single m_Time;

		public Boolean RotateAnchor
		{
			get => m_RotateAnchor;
		    set
			{
				if (m_RotateAnchor != value)
				{
					m_RotateAnchor = value;
					OnSetEntityPosition(MyController, null);
				}
			}
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnSetEntityPosition));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnSetEntityPosition));
			}
		}

		protected override void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			base.OnMoveEntity(p_sender, p_args);
			if (p_sender == MyController)
			{
				m_Time = 0f;
			}
		}

		protected override void OnRotateEntity(Object p_sender, EventArgs p_args)
		{
			base.OnRotateEntity(p_sender, p_args);
			if (p_sender == MyController)
			{
				m_Time = 0f;
			}
		}

		protected override void OnSetEntityPosition(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				if (m_RotateAnchor)
				{
					m_SymbolAnchor.localPosition = MyControllerPosition;
					m_SymbolAnchor.localRotation = MyControllerRotation;
					transform.localRotation = Quaternion.identity;
					s_InverseRotation = m_SymbolAnchor.localRotation;
				}
				else
				{
					m_SymbolAnchor.localPosition = MyControllerPosition;
					m_SymbolAnchor.localRotation = Quaternion.identity;
					transform.localRotation = MyControllerRotation;
					s_InverseRotation = Quaternion.identity;
				}
			}
		}

		protected override void Update()
		{
			m_Time += Time.deltaTime;
			Single t = Math.Min(1f, m_Time / m_MoveTime);
			Single t2 = Math.Min(1f, m_Time / m_RotateTime);
			Vector3 vector = m_SymbolAnchor.localPosition;
			Quaternion quaternion = m_SymbolAnchor.localRotation;
			Vector3 myControllerPosition = MyControllerPosition;
			Quaternion myControllerRotation = MyControllerRotation;
			if (m_RotateAnchor)
			{
				vector = (m_SymbolAnchor.localPosition = Vector3.Lerp(vector, myControllerPosition, t));
				quaternion = (m_SymbolAnchor.localRotation = Quaternion.Slerp(quaternion, myControllerRotation, t2));
				transform.localRotation = Quaternion.identity;
				s_InverseRotation = quaternion;
			}
			else
			{
				vector = (m_SymbolAnchor.localPosition = Vector3.Lerp(m_SymbolAnchor.localPosition, myControllerPosition, t));
				m_SymbolAnchor.localRotation = Quaternion.identity;
				quaternion = (transform.localRotation = Quaternion.Slerp(transform.localRotation, myControllerRotation, t2));
				s_InverseRotation = Quaternion.identity;
			}
			if (vector == myControllerPosition && quaternion == myControllerRotation)
			{
				enabled = false;
			}
		}
	}
}
