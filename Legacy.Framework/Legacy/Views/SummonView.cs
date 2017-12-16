using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class SummonView : BaseView
	{
		private ISummonMovementTiming m_movementTiming;

		public void SetMovementTiming(ISummonMovementTiming p_timing)
		{
			if (m_movementTiming != null)
			{
				Debug.LogError("SummonView: ISummonMovementTiming was set multiple times!");
			}
			m_movementTiming = p_timing;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.SUMMON_CAST_SPELL, new EventHandler(OnEntityCastSpell));
				DelayedEventManager.UnregisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnEntityMovement));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DESTROY_BASEOBJECT, new EventHandler(OnEntityDestroyed));
			}
			if (MyController != null)
			{
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.SUMMON_CAST_SPELL, new EventHandler(OnEntityCastSpell));
				DelayedEventManager.RegisterEvent(EDelayType.ON_FRAME_END, EEventType.MOVE_ENTITY, new EventHandler(OnEntityMovement));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DESTROY_BASEOBJECT, new EventHandler(OnEntityDestroyed));
				MovingEntity movingEntity = (MovingEntity)MyController;
				SetPosition(movingEntity);
				if (movingEntity is Summon)
				{
					((Summon)movingEntity).ViewIsDone.Trigger();
				}
			}
		}

		protected virtual void SetPosition(MovingEntity entity)
		{
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(entity.Position);
			transform.localPosition = Helper.SlotLocalPosition(slot.Position, slot.Height);
			transform.localRotation = Helper.GridDirectionToQuaternion(entity.Direction);
		}

		private void OnEntityCastSpell(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				MovingEntity movingEntity = (MovingEntity)p_sender;
				movingEntity.AttackingDone.Trigger();
				Summon summon = (Summon)p_sender;
				if (summon != null)
				{
					summon.FlushActionLog();
				}
			}
		}

		private void OnEntityMovement(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				MovingEntity entity = (MovingEntity)p_sender;
				SetPosition(entity);
				if (m_movementTiming == null)
				{
					entity.MovementDone.Trigger();
				}
				else
				{
					m_movementTiming.OnMoveEntity(p_sender, p_args, delegate
					{
						entity.MovementDone.Trigger();
					});
				}
			}
		}

		private void OnEntityDestroyed(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			if (baseObjectEventArgs.Object == MyController)
			{
				Destroy(gameObject);
			}
		}
	}
}
