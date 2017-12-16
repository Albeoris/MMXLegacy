using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Utility/MoveToPlayerPosition")]
	public class MoveToPlayerPosition : MonoBehaviour
	{
		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnChangedPlayerPosition));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnChangedPlayerPosition));
		}

		private void OnChangedPlayerPosition(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object is Party)
			{
				OnPositionChange(baseObjectEventArgs.Position);
			}
		}

		private void OnChangedPlayerMoved(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = p_args as MoveEntityEventArgs;
			if (moveEntityEventArgs != null && p_sender is Party)
			{
				OnPositionChange(moveEntityEventArgs.Position);
			}
		}

		private void OnPositionChange(Position p_playerPos)
		{
			if (transform.parent == null)
			{
				Vector3 position = Helper.SlotLocalPosition(p_playerPos, LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_playerPos).Height) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
				transform.position = position;
			}
		}
	}
}
