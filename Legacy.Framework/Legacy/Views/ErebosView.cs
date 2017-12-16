using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Utilities.StateManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	public class ErebosView : LevelEntityView
	{
		[SerializeField]
		private String m_vanishClipName;

		[SerializeField]
		private String m_appearClipName;

		private TimeStateMachine<EState> m_State;

		private Int32 m_vanishHash;

		private Int32 m_appearHash;

		private Boolean m_doLateVanish;

		protected override void Awake()
		{
			m_State = new TimeStateMachine<EState>();
			m_State.AddState(new TimeState<EState>(EState.IDLE));
			m_State.AddState(new TimeState<EState>(EState.VANISH, 3f));
			m_State.AddState(new TimeState<EState>(EState.APPEAR, 3f));
			m_State.StateChangedMethod += State_StateChangedMethod;
			m_State.ChangeState(EState.IDLE);
			base.Awake();
			m_vanishHash = Animator.StringToHash(m_vanishClipName);
			m_appearHash = Animator.StringToHash(m_appearClipName);
		}

		private void State_StateChangedMethod(EState p_fromState, EState p_toState)
		{
			if (p_fromState == EState.APPEAR && p_toState == EState.IDLE)
			{
				((Monster)MyController).AppearAnimationDone.Trigger();
			}
			if (p_fromState == EState.VANISH && p_toState == EState.IDLE)
			{
				((Monster)MyController).VanishAnimationDone.Trigger();
			}
		}

		protected override void OnSetEntityPositionEvent(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (p_args != null && baseObjectEventArgs.Object == MyController)
			{
				if (baseObjectEventArgs.Animation == "APPEARING")
				{
					PlaceMonster();
					m_animatorControl.EventSummon(1);
					m_State.ChangeState(EState.APPEAR);
				}
				else if (baseObjectEventArgs.Animation == "VANISHING")
				{
					if (m_State.CurrentState.Id == EState.APPEAR)
					{
						m_doLateVanish = true;
					}
					else
					{
						m_animatorControl.EventSummon(3);
						m_State.ChangeState(EState.VANISH);
					}
				}
			}
		}

		protected override void Update()
		{
			base.Update();
			m_State.Update(Time.deltaTime);
			switch (m_State.CurrentStateID)
			{
			case EState.IDLE:
				if (m_doLateVanish)
				{
					m_animatorControl.EventSummon(3);
					m_State.ChangeState(EState.VANISH);
					m_doLateVanish = false;
				}
				break;
			case EState.VANISH:
				if (m_animator.GetCurrentAnimatorStateInfo(0).nameHash == m_vanishHash)
				{
					if (m_animator.IsInTransition(0))
					{
						((Monster)MyController).VanishAnimationDone.Trigger();
						PlaceMonster();
						((Monster)MyController).AiHandler.Update();
						m_State.ChangeState(EState.IDLE);
					}
				}
				else if (m_State.IsStateTimeout)
				{
					((Monster)MyController).VanishAnimationDone.Trigger();
					PlaceMonster();
					((Monster)MyController).AiHandler.Update();
					m_State.ChangeState(EState.IDLE);
				}
				break;
			case EState.APPEAR:
				if (m_animator.GetCurrentAnimatorStateInfo(0).nameHash == m_appearHash)
				{
					if (m_animator.IsInTransition(0))
					{
						((Monster)MyController).AppearAnimationDone.Trigger();
						((Monster)MyController).AiHandler.Update();
						m_State.ChangeState(EState.IDLE);
					}
				}
				else if (m_State.IsStateTimeout)
				{
					((Monster)MyController).AppearAnimationDone.Trigger();
					((Monster)MyController).AiHandler.Update();
					m_State.ChangeState(EState.IDLE);
				}
				break;
			}
		}

		private void PlaceMonster()
		{
			MovingEntity myController = MyController;
			Vector3 vectorPosition = GetVectorPosition(myController.Position, myController.Size, myController.Height);
			Quaternion quaternion = Helper.GridDirectionToQuaternion(myController.Direction);
			transform.localPosition = vectorPosition;
			transform.localRotation = quaternion;
			m_animatorControl.TargetPosition = ViewManager.Instance.GridOrigin.TransformPoint(vectorPosition);
			m_animatorControl.TargetRotation = quaternion;
		}

		private enum EState
		{
			IDLE,
			VANISH,
			APPEAR
		}
	}
}
