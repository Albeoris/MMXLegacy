using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/Moveable Symbol View")]
	public class MoveableSymbolView : SimpleSymbolView
	{
		[SerializeField]
		protected Boolean m_rotateSymbol = true;

		protected override void Awake()
		{
			base.Awake();
			enabled = false;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ROTATE_ENTITY, new EventHandler(OnRotateEntity));
			}
			if (MyController != null)
			{
				OnSetEntityPosition(MyController, null);
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ROTATE_ENTITY, new EventHandler(OnRotateEntity));
			}
		}

		protected virtual void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				enabled = true;
				CheckVisibility(false);
			}
		}

		protected virtual void OnRotateEntity(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				enabled = true;
			}
		}

		protected override void OnSetEntityPosition(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				GameObject gameObject = ViewManager.Instance.FindView(MyController);
				if (gameObject != null)
				{
					transform.localPosition = GetMinimapPosition(gameObject);
				}
				else
				{
					transform.localPosition = MyControllerPosition;
				}
				Quaternion quaternion = (!m_rotateSymbol) ? Quaternion.identity : MyControllerRotation;
				if (m_asymmetricSymbol)
				{
					quaternion *= s_InverseRotation;
				}
				transform.localRotation = quaternion;
				CheckVisibility(false);
			}
		}

		protected override void Update()
		{
			Vector3 vector = transform.localPosition;
			Vector3 vector2 = MyControllerPosition;
			GameObject gameObject = ViewManager.Instance.FindView(MyController);
			if (gameObject != null)
			{
				vector = (transform.localPosition = (vector2 = GetMinimapPosition(gameObject)));
			}
			else
			{
				vector = (transform.localPosition = Vector3.MoveTowards(vector, vector2, 40f * Time.deltaTime));
			}
			Quaternion quaternion = transform.localRotation;
			Quaternion myControllerRotation = MyControllerRotation;
			if (m_asymmetricSymbol)
			{
				quaternion *= s_InverseRotation;
			}
			if (m_rotateSymbol)
			{
				quaternion = (transform.localRotation = Quaternion.RotateTowards(quaternion, myControllerRotation, 270f * Time.deltaTime));
			}
			else
			{
				transform.localRotation = Quaternion.identity;
				quaternion = myControllerRotation;
			}
			if (!m_asymmetricSymbol && gameObject == null && vector == vector2 && quaternion == myControllerRotation)
			{
				enabled = false;
			}
		}

		private static Vector3 GetMinimapPosition(GameObject view)
		{
			Vector3 vector = view.transform.localPosition;
			LargeLevelEntityView component = view.GetComponent<LargeLevelEntityView>();
			if (component != null)
			{
				vector -= component.transform.localRotation * component.PositionOffset;
			}
			vector.x = (Int32)((vector.x - 5f) / 10f * 24f);
			vector.y = (Int32)((vector.z - 5f) / 10f * 24f);
			vector.z = 0f;
			return vector;
		}
	}
}
