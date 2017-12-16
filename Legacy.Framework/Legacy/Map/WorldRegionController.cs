using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Map
{
	[AddComponentMenu("MM Legacy/Map/Regions/WorldRegionController")]
	public class WorldRegionController : MonoBehaviour
	{
		public RegionEntry[] RegionEntries;

		public WorldRegionCorridor[] RegionTransitions;

		public Int32 MaxStepsUntilUpdate = 10;

		public Single MinUpdateTime = 1f;

		public Single MaxUpdateTime = 5f;

		private Int32 mSteps;

		private Position mLastPlayerPos;

		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_TELEPORTER_USED, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(Init));
		}

		private void Init(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			if (baseObjectEventArgs.Object is Party)
			{
				mLastPlayerPos = baseObjectEventArgs.Position;
				OnPositionChange();
			}
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_TELEPORTER_USED, new EventHandler(OnChangedPlayerPosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(Init));
		}

		private void OnChangedPlayerPosition(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object is Party)
			{
				mLastPlayerPos = baseObjectEventArgs.Position;
				if (IsInvoking("OnPositionChange"))
				{
					CancelInvoke("OnPositionChange");
				}
				OnPositionChange();
			}
		}

		private void OnChangedPlayerMoved(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = p_args as MoveEntityEventArgs;
			if (moveEntityEventArgs != null && p_sender is Party)
			{
				mLastPlayerPos = moveEntityEventArgs.Position;
				if (mSteps < MaxStepsUntilUpdate)
				{
					if (IsInvoking("OnPositionChange"))
					{
						CancelInvoke("OnPositionChange");
					}
					Invoke("OnPositionChange", Random.Range(MinUpdateTime, MaxUpdateTime));
					mSteps++;
				}
				else
				{
					if (IsInvoking("OnPositionChange"))
					{
						CancelInvoke("OnPositionChange");
					}
					OnPositionChange();
				}
			}
		}

		private void UpdateSharedObjects()
		{
			foreach (RegionEntry regionEntry in RegionEntries)
			{
				regionEntry.RegionTrigger.SetSharedObjects(false);
			}
			foreach (RegionEntry regionEntry2 in RegionEntries)
			{
				if (regionEntry2.CurrentState == RegionState.Active)
				{
					regionEntry2.RegionTrigger.SetSharedObjects(true);
				}
			}
		}

		private void OnPositionChange()
		{
			mSteps = 0;
			Vector3 point = Helper.SlotLocalPosition(mLastPlayerPos, LegacyLogic.Instance.MapLoader.Grid.GetSlot(mLastPlayerPos).Height) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
			Boolean flag = false;
			foreach (WorldRegionCorridor worldRegionCorridor in RegionTransitions)
			{
				if (worldRegionCorridor.TransitionCollider.bounds.Contains(point))
				{
					foreach (RegionEntry regionEntry in RegionEntries)
					{
						if (regionEntry.RegionTrigger == worldRegionCorridor.RegionA || regionEntry.RegionTrigger == worldRegionCorridor.RegionB)
						{
							regionEntry.CurrentState = RegionState.Active;
							regionEntry.RegionTrigger.LoadRegion();
							flag = true;
						}
						else
						{
							regionEntry.CurrentState = RegionState.Passive;
							regionEntry.RegionTrigger.UnloadRegion();
						}
					}
				}
			}
			if (flag)
			{
				UpdateSharedObjects();
				return;
			}
			RegionEntry regionEntry2 = null;
			Int32 num = 0;
			foreach (RegionEntry regionEntry3 in RegionEntries)
			{
				foreach (BoxCollider boxCollider in regionEntry3.RegionTrigger.RegionAreas)
				{
					if (boxCollider.bounds.Contains(point))
					{
						regionEntry2 = regionEntry3;
						num++;
					}
				}
			}
			if (num == 1)
			{
				foreach (RegionEntry regionEntry4 in RegionEntries)
				{
					if (regionEntry4 == regionEntry2)
					{
						regionEntry4.CurrentState = RegionState.Active;
						regionEntry4.RegionTrigger.LoadRegion();
					}
					else if (regionEntry4.CurrentState == RegionState.Active || regionEntry4.CurrentState == RegionState.OnHold)
					{
						regionEntry4.CurrentState = RegionState.Passive;
						regionEntry4.RegionTrigger.UnloadRegion();
					}
				}
				UpdateSharedObjects();
			}
		}
	}
}
