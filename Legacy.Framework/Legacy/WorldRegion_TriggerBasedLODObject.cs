using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Map/WorldRegion_TriggerBasedLODObject")]
	public class WorldRegion_TriggerBasedLODObject : MonoBehaviour
	{
		public BoxCollider[] VisibleAreaBoxes;

		public GameObject LowLODObject;

		public GameObject HighLODObject;

		private Boolean m_currentVisibleState;

		private void Awake()
		{
			if (LowLODObject == null && HighLODObject == null)
			{
				enabled = false;
				return;
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
			if (LowLODObject != null)
			{
				LowLODObject.SetActive(!m_currentVisibleState);
			}
			if (HighLODObject != null)
			{
				HighLODObject.SetActive(m_currentVisibleState);
			}
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
		}

		private void OnChangedPlayerPosition(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object is Party)
			{
				OnPositionChange(baseObjectEventArgs.Position);
			}
		}

		private void OnChangedPlayerMoved(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
			if (moveEntityEventArgs != null && p_sender is Party)
			{
				OnPositionChange(moveEntityEventArgs.Position);
			}
		}

		private void OnPositionChange(Position p_playerPos)
		{
			Vector3 point = Helper.SlotLocalPosition(p_playerPos, LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_playerPos).Height) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
			Boolean flag = false;
			foreach (BoxCollider boxCollider in VisibleAreaBoxes)
			{
				if (boxCollider.bounds.Contains(point))
				{
					flag = true;
					break;
				}
			}
			if (flag != m_currentVisibleState)
			{
				m_currentVisibleState = flag;
				if (LowLODObject != null)
				{
					LowLODObject.SetActive(!m_currentVisibleState);
				}
				if (HighLODObject != null)
				{
					HighLODObject.SetActive(m_currentVisibleState);
				}
			}
		}
	}
}
