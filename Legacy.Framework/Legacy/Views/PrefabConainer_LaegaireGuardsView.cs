using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/PrefabConainer_LaegaireGuardsView")]
	internal class PrefabConainer_LaegaireGuardsView : BaseView
	{
		public GameObject Guard;

		public GameObject OneTileLight;

		public GameObject TwoTileLight;

		public GameObject ThreeTileLight;

		public GameObject FourTileLight;

		public GameObject FiveTileLight;

		public new InteractiveObject MyController => base.MyController as InteractiveObject;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnEnabledChanged));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnStateChanged));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnEnabledChanged));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnStateChanged));
				if (Guard != null)
				{
					Guard.SetActive(MyController.Enabled);
					UpdateLightState();
				}
			}
		}

		private void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
			if (moveEntityEventArgs != null && p_sender == MyController)
			{
				Single p_height = 0f;
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(moveEntityEventArgs.Position);
				if (slot != null)
				{
					p_height = slot.Height;
				}
				Vector3 localPosition = Helper.SlotLocalPosition(moveEntityEventArgs.Position, p_height);
				Quaternion localRotation = Helper.GridDirectionToQuaternion(moveEntityEventArgs.TargetOrientation);
				transform.localPosition = localPosition;
				transform.localRotation = localRotation;
			}
		}

		private void OnEnabledChanged(Object p_sender, EventArgs p_args)
		{
			if (p_args != null && p_sender == MyController && Guard != null)
			{
				Guard.SetActive(MyController.Enabled);
				UpdateLightState();
			}
		}

		private void OnStateChanged(Object p_sender, EventArgs p_args)
		{
			if (p_args != null && p_sender == MyController)
			{
				UpdateLightState();
			}
		}

		private void UpdateLightState()
		{
			if (OneTileLight != null)
			{
				OneTileLight.SetActive(MyController.State == EInteractiveObjectState.COUNTER_1 && MyController.Enabled);
			}
			if (TwoTileLight != null)
			{
				TwoTileLight.SetActive(MyController.State == EInteractiveObjectState.COUNTER_2 && MyController.Enabled);
			}
			if (ThreeTileLight != null)
			{
				ThreeTileLight.SetActive(MyController.State == EInteractiveObjectState.COUNTER_3 && MyController.Enabled);
			}
			if (FourTileLight != null)
			{
				FourTileLight.SetActive(MyController.State == EInteractiveObjectState.COUNTER_4 && MyController.Enabled);
			}
			if (FiveTileLight != null)
			{
				FiveTileLight.SetActive(MyController.State == EInteractiveObjectState.COUNTER_5 && MyController.Enabled);
			}
		}
	}
}
