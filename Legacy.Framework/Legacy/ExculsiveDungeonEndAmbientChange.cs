using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	public class ExculsiveDungeonEndAmbientChange : MonoBehaviour
	{
		public BoxCollider VisibleAreaBox;

		public Boolean CurrentVisibleState;

		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnChangedPlayerTeleport));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnChangedPlayerTeleport));
		}

		private void OnChangedPlayerPosition(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object is Party)
			{
				OnPositionChange(baseObjectEventArgs.Position, true);
			}
		}

		private void OnChangedPlayerMoved(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
			if (moveEntityEventArgs != null && p_sender is Party)
			{
				OnPositionChange(moveEntityEventArgs.Position, true);
			}
		}

		private void OnChangedPlayerTeleport(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			if (baseObjectEventArgs != null && p_sender is Party)
			{
				OnPositionChange(baseObjectEventArgs.Position, false);
			}
		}

		private void OnPositionChange(Position p_playerPos, Boolean p_fastfade)
		{
			Vector3 point = Helper.SlotLocalPosition(p_playerPos, LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_playerPos).Height) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
			Boolean flag = VisibleAreaBox.bounds.Contains(point);
			if (flag && !CurrentVisibleState)
			{
				CurrentVisibleState = flag;
				StartCoroutine(FadeToBlack(p_fastfade));
			}
		}

		private IEnumerator FadeToBlack(Boolean p_fastfade)
		{
			Color fogColor = RenderSettings.fogColor;
			Color ambientColor = RenderSettings.ambientLight;
			if (p_fastfade)
			{
				RenderSettings.fogColor = fogColor * 0f;
				RenderSettings.ambientLight = ambientColor * 0f;
			}
			else
			{
				for (Single lerpValue = 1f; lerpValue > 0f; lerpValue -= Time.deltaTime / 8f)
				{
					RenderSettings.fogColor = fogColor * lerpValue;
					RenderSettings.ambientLight = ambientColor * lerpValue;
					yield return new WaitForEndOfFrame();
				}
			}
			yield break;
		}
	}
}
