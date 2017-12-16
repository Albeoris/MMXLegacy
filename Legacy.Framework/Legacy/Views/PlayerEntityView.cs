using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic.Interactions;
using Legacy.Core.Utilities.StateManagement;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Player Entity View")]
	public class PlayerEntityView : BaseView
	{
		private TimeStateMachine<EState> m_state;

		private Vector3 m_oldPosition;

		private Vector3 m_targetPosition;

		private Quaternion m_oldRotation;

		private Quaternion m_targetRotation;

		private Single m_DelayedMoveSoundTime = -1f;

		private Single m_stateOverrun;

		[SerializeField]
		private GameObject[] m_memberSlots = new GameObject[4];

		[SerializeField]
		private String m_foodstepSoundWater = "Move_Party";

		[SerializeField]
		private String m_foodstepSoundRough = "Move_Party";

		[SerializeField]
		private String m_foodstepSoundForest = "Move_Party";

		[SerializeField]
		private String m_foodstepSoundLava = "Move_Party";

		[SerializeField]
		private Single m_MoveTime = 0.2f;

		[SerializeField]
		private Single m_MoveTimeInFight = 0.7f;

		[SerializeField]
		private Single m_RotateTime = 0.3f;

		public void MoveTo(Vector3 localPosition)
		{
			m_oldPosition = transform.localPosition;
			m_targetPosition = localPosition;
			m_state.ChangeState(EState.MOVING, MoveTime());
			m_state.CurrentStateTime = m_stateOverrun;
			Vector3 localPosition2;
			Helper.Lerp(ref m_oldPosition, ref m_targetPosition, m_state.CurrentStateTimePer, out localPosition2);
			transform.localPosition = localPosition2;
			enabled = true;
		}

		public void RotateTo(EDirection direction)
		{
			m_oldRotation = transform.localRotation;
			m_targetRotation = Helper.GridDirectionToQuaternion(direction);
			m_state.ChangeState(EState.ROTATING, m_RotateTime);
			enabled = true;
		}

		public GameObject GetMemberGameObject(Int32 index)
		{
			if (index >= 0 && index < m_memberSlots.Length)
			{
				Int32 memberIndexByOrder = ((Party)MyController).GetMemberIndexByOrder(index);
				return m_memberSlots[memberIndexByOrder];
			}
			return null;
		}

		public void SetEntityPosition()
		{
			MovingEntity movingEntity = (MovingEntity)MyController;
			if (movingEntity != null)
			{
				transform.localPosition = Helper.SlotLocalPosition(movingEntity.Position, movingEntity.Height);
				transform.localRotation = Helper.GridDirectionToQuaternion(movingEntity.Direction);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			m_state = new TimeStateMachine<EState>();
			m_state.AddState(new TimeState<EState>(EState.IDLE));
			m_state.AddState(new TimeState<EState>(EState.MOVING));
			m_state.AddState(new TimeState<EState>(EState.ROTATING));
			m_state.ChangeState(EState.IDLE);
			enabled = false;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntityEvent));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ROTATE_ENTITY, new EventHandler(OnRotateEntityEvent));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnSetEntityPositionEvent));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnTeleport));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_TELEPORTER_USED, new EventHandler(OnPartyTeleporterUsedEvent));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.REBOUND_ENTITY, new EventHandler(OnReboundEntityEvent));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESTORED, new EventHandler(OnPartyRestoredEvent));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_REVIVED, new EventHandler(OnCharacterRevivedEvent));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_ADDED, new EventHandler(OnInventoryItemAdded));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.INVENTORY_ITEM_REMOVED, new EventHandler(OnInventoryItemRemoved));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntityEvent));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ROTATE_ENTITY, new EventHandler(OnRotateEntityEvent));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnSetEntityPositionEvent));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnTeleport));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_TELEPORTER_USED, new EventHandler(OnPartyTeleporterUsedEvent));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.REBOUND_ENTITY, new EventHandler(OnReboundEntityEvent));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESTORED, new EventHandler(OnPartyRestoredEvent));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_REVIVED, new EventHandler(OnCharacterRevivedEvent));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_ADDED, new EventHandler(OnInventoryItemAdded));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.INVENTORY_ITEM_REMOVED, new EventHandler(OnInventoryItemRemoved));
				SetEntityPosition();
			}
		}

		private void OnMoveEntityEvent(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = p_args as MoveEntityEventArgs;
			if (moveEntityEventArgs != null && p_sender == MyController)
			{
				Party party = (Party)MyController;
				MoveTo(Helper.SlotLocalPosition(moveEntityEventArgs.Position, party.Height));
				if (!party.IsPushed)
				{
					FXMainCamera.Instance.PlayWalkFX(MoveTime());
				}
				PlayMoveSound(false);
			}
		}

		private void OnRotateEntityEvent(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController)
			{
				MovingEntity movingEntity = (MovingEntity)MyController;
				RotateTo(movingEntity.Direction);
				PlayMoveSound(true);
			}
		}

		private void OnReboundEntityEvent(Object p_sender, EventArgs p_args)
		{
			ReboundEntityEventArgs reboundEntityEventArgs = p_args as ReboundEntityEventArgs;
			if (reboundEntityEventArgs != null && p_sender == MyController)
			{
				FXMainCamera.Instance.PlayReboundFX(new Vector3(reboundEntityEventArgs.ReboundDirection.X, 0f, reboundEntityEventArgs.ReboundDirection.Y));
				AudioController.Play("CannotMove");
			}
		}

		private void OnSetEntityPositionEvent(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController)
			{
				SetEntityPosition();
			}
		}

		private void OnTeleport(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController)
			{
				m_state.ChangeState(EState.IDLE);
				FXMainCamera.Instance.StopWalkFX();
				((MovingEntity)MyController).MovementDone.Trigger();
				enabled = false;
				MovingEntity movingEntity = (MovingEntity)baseObjectEventArgs.Object;
				transform.localPosition = Helper.SlotLocalPosition(baseObjectEventArgs.Position, movingEntity.Height);
				transform.localRotation = Helper.GridDirectionToQuaternion(movingEntity.Direction);
				LegacyLogic.Instance.CommandManager.ClearQueue();
			}
		}

		private void OnPartyTeleporterUsedEvent(Object p_sender, EventArgs p_args)
		{
			if (p_sender is TeleportInteraction && ((TeleportInteraction)p_sender).IsParentTeleporter)
			{
				AudioController.Play("Teleport");
				Helper.Instantiate<GameObject>(Helper.ResourcesLoad<GameObject>("FX/Teleport_Effect"), transform.position, transform.rotation);
			}
		}

		private void OnPartyRestoredEvent(Object p_sender, EventArgs p_argse)
		{
			AudioController.Play("PartyRestored");
		}

		private void OnCharacterRevivedEvent(Object p_sender, EventArgs p_args)
		{
			AudioController.Play("CharacterRevived");
		}

		private void OnInventoryItemAdded(Object p_sender, EventArgs p_args)
		{
			OnInventoryItemAddedOrRemoved("Equip", "Drop", p_sender);
		}

		private void OnInventoryItemRemoved(Object p_sender, EventArgs p_args)
		{
			OnInventoryItemAddedOrRemoved("Unequip", "Drag", p_sender);
		}

		private void OnInventoryItemAddedOrRemoved(String p_audioID, String p_audioIDtoStop, Object p_sender)
		{
			Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
			if (members != null)
			{
				for (Int32 i = 0; i < members.Length; i++)
				{
					if (members[i] != null && members[i].Equipment.Equipment == p_sender)
					{
						AudioController.Stop(p_audioIDtoStop, 0f);
						AudioController.Play(p_audioID, FXHelper.GetCharacterGO(members[i].Index).transform);
						return;
					}
				}
			}
		}

		private void Update()
		{
			Single stateOverrun = m_state.CurrentStateTime + Time.deltaTime - m_state.CurrentStateDuration;
			m_state.Update(Time.deltaTime);
			if (m_state.IsState(EState.MOVING))
			{
				Vector3 localPosition;
				Helper.Lerp(ref m_oldPosition, ref m_targetPosition, m_state.CurrentStateTimePer, out localPosition);
				transform.localPosition = localPosition;
				if (m_state.IsStateTimeout)
				{
					m_stateOverrun = stateOverrun;
					m_state.ChangeState(EState.IDLE);
					FXMainCamera.Instance.StopWalkFX();
					if (MyController != null)
					{
						((MovingEntity)MyController).MovementDone.Trigger();
					}
				}
			}
			else
			{
				m_stateOverrun = 0f;
				if (m_state.IsState(EState.ROTATING))
				{
					transform.localRotation = Quaternion.Lerp(m_oldRotation, m_targetRotation, m_state.CurrentStateTimePer);
					if (m_state.IsStateTimeout)
					{
						m_state.ChangeState(EState.IDLE);
						FXMainCamera.Instance.StopWalkFX();
						if (MyController != null)
						{
							((MovingEntity)MyController).RotationDone.Trigger();
						}
					}
				}
			}
			HandleDelayedSound();
		}

		private Single MoveTime()
		{
			Single result = m_MoveTime;
			if (((Party)MyController).HasAggro())
			{
				result = m_MoveTimeInFight;
			}
			return result;
		}

		private void PlayMoveSound(Boolean p_IsTurning)
		{
			AudioController.Play(GetFoodStepAudioID(), transform);
			if (!p_IsTurning)
			{
				m_DelayedMoveSoundTime = Time.time + MoveTime();
			}
			else
			{
				m_DelayedMoveSoundTime = -1f;
			}
		}

		private void HandleDelayedSound()
		{
			if (m_DelayedMoveSoundTime != -1f && m_DelayedMoveSoundTime <= Time.time)
			{
				m_DelayedMoveSoundTime = -1f;
				AudioController.Play(GetFoodStepAudioID(), transform);
			}
		}

		private String GetFoodStepAudioID()
		{
			MovingEntity movingEntity = (MovingEntity)MyController;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			GridSlot slot = grid.GetSlot(movingEntity.Position);
			String text = null;
			ETerrainType terrainType = slot.TerrainType;
			if ((terrainType & ETerrainType.WATER) > ETerrainType.NONE)
			{
				text = m_foodstepSoundWater;
			}
			else if ((terrainType & ETerrainType.ROUGH) > ETerrainType.NONE)
			{
				text = m_foodstepSoundRough;
			}
			else if ((terrainType & ETerrainType.FOREST) > ETerrainType.NONE)
			{
				text = m_foodstepSoundForest;
			}
			else if ((terrainType & ETerrainType.LAVA) > ETerrainType.NONE)
			{
				text = m_foodstepSoundLava;
			}
			if (String.IsNullOrEmpty(text))
			{
				text = "Move_Party";
			}
			return text;
		}

		private enum EState
		{
			IDLE,
			MOVING,
			ROTATING
		}
	}
}
