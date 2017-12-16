using System;
using System.Collections;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Animations
{
	public class AnimRotationHandler : MonoBehaviour
	{
		private MonsterEntityView m_MovinEntityView;

		private AnimHandler m_AnimationHandler;

		private AnimEventHandler m_AnimationEventHandler;

		private void Awake()
		{
			m_MovinEntityView = this.GetComponent< MonsterEntityView>(true);
			m_AnimationHandler = this.GetComponent<AnimHandler>(true);
			m_AnimationEventHandler = this.GetComponent< AnimEventHandler>(true);
			m_AnimationHandler.PlayingAnimation += AnimationHandlerOnPlay;
		}

		private void OnDestroy()
		{
			m_AnimationHandler.PlayingAnimation -= AnimationHandlerOnPlay;
		}

		private void AnimationHandlerOnPlay(Object p_sender, AnimPlayEventArgs p_args)
		{
			if (p_args.Type == EAnimType.TURN_LEFT || p_args.Type == EAnimType.TURN_RIGHT)
			{
				m_AnimationEventHandler.RegisterAnimationCallback(EAnimEventType.TURN_DONE, delegate
				{
					transform.localRotation = m_MovinEntityView.TargetRotation;
					IEnumerator enumerator = animation.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Object obj = enumerator.Current;
							AnimationState animationState = (AnimationState)obj;
							if (animationState.enabled)
							{
								animationState.time = 0f;
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					animation.Sample();
					m_AnimationHandler.Stop();
					m_AnimationHandler.Play(EAnimType.IDLE);
				});
			}
		}
	}
}
