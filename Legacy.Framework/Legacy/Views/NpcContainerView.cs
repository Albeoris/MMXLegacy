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
	[AddComponentMenu("MM Legacy/Views/NpcContainerView")]
	internal class NpcContainerView : BaseView
	{
		[SerializeField]
		private GameObject[] m_npcObjects;

		public GameObject[] NPCObjects
		{
			get => m_npcObjects;
		    set => m_npcObjects = value;
		}

		protected override void Awake()
		{
			base.Awake();
			if (m_npcObjects != null)
			{
				foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
				{
					renderer.useLightProbes = true;
				}
			}
		}

		public new InteractiveObject MyController => (InteractiveObject)base.MyController;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnEnableStateChanged));
			}
			if (MyController != null)
			{
				UpdateView();
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_HIRELING_UPDATED, new EventHandler(OnHirelingUpdated));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_ENABLED_CHANGED, new EventHandler(OnEnableStateChanged));
			}
		}

		private void OnHirelingUpdated(Object sender, EventArgs args)
		{
			HirelingEventArgs hirelingEventArgs = (HirelingEventArgs)args;
			if (((NpcContainer)MyController).Contains(hirelingEventArgs.Npc))
			{
				UpdateView();
			}
		}

		private void UpdateView()
		{
			if (MyController != null)
			{
				NpcContainer npcContainer = (NpcContainer)MyController;
				SetVisible(npcContainer.IsEnabled && MyController.Enabled);
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
				transform.localPosition = localPosition;
			}
		}

		private void OnEnableStateChanged(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				SetVisible(MyController.Enabled);
			}
		}

		private void SetVisible(Boolean p_visible)
		{
			if (m_npcObjects != null)
			{
				foreach (GameObject gameObject in m_npcObjects)
				{
					if (gameObject != null)
					{
						gameObject.SetActive(p_visible);
					}
				}
			}
		}
	}
}
