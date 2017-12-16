using System;
using System.Collections;
using AnimationOrTween;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("NGUI/Internal/Active Animation")]
[RequireComponent(typeof(Animation))]
public class ActiveAnimation : IgnoreTimeScale
{
	public OnFinished onFinished;

	public GameObject eventReceiver;

	public String callWhenFinished;

	private Animation mAnim;

	private Direction mLastDirection;

	private Direction mDisableDirection;

	private Boolean mNotify;

	public Boolean isPlaying
	{
		get
		{
			if (mAnim == null)
			{
				return false;
			}
			IEnumerator enumerator = mAnim.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Object obj = enumerator.Current;
					AnimationState animationState = (AnimationState)obj;
					if (mAnim.IsPlaying(animationState.name))
					{
						if (mLastDirection == Direction.Forward)
						{
							if (animationState.time < animationState.length)
							{
								return true;
							}
						}
						else
						{
							if (mLastDirection != Direction.Reverse)
							{
								return true;
							}
							if (animationState.time > 0f)
							{
								return true;
							}
						}
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
			return false;
		}
	}

	public void Reset()
	{
		if (mAnim != null)
		{
			IEnumerator enumerator = mAnim.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Object obj = enumerator.Current;
					AnimationState animationState = (AnimationState)obj;
					if (mLastDirection == Direction.Reverse)
					{
						animationState.time = animationState.length;
					}
					else if (mLastDirection == Direction.Forward)
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
		}
	}

	private void Update()
	{
		Single num = UpdateRealTimeDelta();
		if (num == 0f)
		{
			return;
		}
		if (mAnim != null)
		{
			Boolean flag = false;
			IEnumerator enumerator = mAnim.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Object obj = enumerator.Current;
					AnimationState animationState = (AnimationState)obj;
					if (mAnim.IsPlaying(animationState.name))
					{
						Single num2 = animationState.speed * num;
						animationState.time += num2;
						if (num2 < 0f)
						{
							if (animationState.time > 0f)
							{
								flag = true;
							}
							else
							{
								animationState.time = 0f;
							}
						}
						else if (animationState.time < animationState.length)
						{
							flag = true;
						}
						else
						{
							animationState.time = animationState.length;
						}
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
			mAnim.Sample();
			if (flag)
			{
				return;
			}
			enabled = false;
			if (mNotify)
			{
				mNotify = false;
				if (onFinished != null)
				{
					onFinished(this);
				}
				if (eventReceiver != null && !String.IsNullOrEmpty(callWhenFinished))
				{
					eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
				}
				if (mDisableDirection != Direction.Toggle && mLastDirection == mDisableDirection)
				{
					NGUITools.SetActive(gameObject, false);
				}
			}
		}
		else
		{
			enabled = false;
		}
	}

	private void Play(String clipName, Direction playDirection)
	{
		if (mAnim != null)
		{
			enabled = true;
			mAnim.enabled = false;
			if (playDirection == Direction.Toggle)
			{
				playDirection = ((mLastDirection == Direction.Forward) ? Direction.Reverse : Direction.Forward);
			}
			Boolean flag = String.IsNullOrEmpty(clipName);
			if (flag)
			{
				if (!mAnim.isPlaying)
				{
					mAnim.Play();
				}
			}
			else if (!mAnim.IsPlaying(clipName))
			{
				mAnim.Play(clipName);
			}
			IEnumerator enumerator = mAnim.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Object obj = enumerator.Current;
					AnimationState animationState = (AnimationState)obj;
					if (String.IsNullOrEmpty(clipName) || animationState.name == clipName)
					{
						Single num = Mathf.Abs(animationState.speed);
						animationState.speed = num * (Single)playDirection;
						if (playDirection == Direction.Reverse && animationState.time == 0f)
						{
							animationState.time = animationState.length;
						}
						else if (playDirection == Direction.Forward && animationState.time == animationState.length)
						{
							animationState.time = 0f;
						}
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
			mLastDirection = playDirection;
			mNotify = true;
			mAnim.Sample();
		}
	}

	public static ActiveAnimation Play(Animation anim, String clipName, Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
	{
		if (!NGUITools.GetActive(anim.gameObject))
		{
			if (enableBeforePlay != EnableCondition.EnableThenPlay)
			{
				return null;
			}
			NGUITools.SetActive(anim.gameObject, true);
			UIPanel[] componentsInChildren = anim.gameObject.GetComponentsInChildren<UIPanel>();
			Int32 i = 0;
			Int32 num = componentsInChildren.Length;
			while (i < num)
			{
				componentsInChildren[i].Refresh();
				i++;
			}
		}
		ActiveAnimation activeAnimation = anim.GetComponent<ActiveAnimation>();
		if (activeAnimation == null)
		{
			activeAnimation = anim.gameObject.AddComponent<ActiveAnimation>();
		}
		activeAnimation.mAnim = anim;
		activeAnimation.mDisableDirection = (Direction)disableCondition;
		activeAnimation.eventReceiver = null;
		activeAnimation.callWhenFinished = null;
		activeAnimation.onFinished = null;
		activeAnimation.Play(clipName, playDirection);
		return activeAnimation;
	}

	public static ActiveAnimation Play(Animation anim, String clipName, Direction playDirection)
	{
		return Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}

	public static ActiveAnimation Play(Animation anim, Direction playDirection)
	{
		return Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
	}

	public delegate void OnFinished(ActiveAnimation anim);
}
