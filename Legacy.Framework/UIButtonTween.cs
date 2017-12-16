using System;
using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Tween")]
public class UIButtonTween : MonoBehaviour
{
	public GameObject tweenTarget;

	public Int32 tweenGroup;

	public Trigger trigger;

	public Direction playDirection = Direction.Forward;

	public Boolean resetOnPlay;

	public EnableCondition ifDisabledOnPlay;

	public DisableCondition disableWhenFinished;

	public Boolean includeChildren;

	public GameObject eventReceiver;

	public String callWhenFinished;

	public UITweener.OnFinished onFinished;

	private UITweener[] mTweens;

	private Boolean mStarted;

	private Boolean mHighlighted;

	private void Start()
	{
		mStarted = true;
		if (tweenTarget == null)
		{
			tweenTarget = gameObject;
		}
	}

	private void OnEnable()
	{
		if (mStarted && mHighlighted)
		{
			OnHover(UICamera.IsHighlighted(gameObject));
		}
	}

	private void OnHover(Boolean isOver)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnHover || (trigger == Trigger.OnHoverTrue && isOver) || (trigger == Trigger.OnHoverFalse && !isOver))
			{
				Play(isOver);
			}
			mHighlighted = isOver;
		}
	}

	private void OnPress(Boolean isPressed)
	{
		if (enabled && (trigger == Trigger.OnPress || (trigger == Trigger.OnPressTrue && isPressed) || (trigger == Trigger.OnPressFalse && !isPressed)))
		{
			Play(isPressed);
		}
	}

	private void OnClick()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			Play(true);
		}
	}

	private void OnDoubleClick()
	{
		if (enabled && trigger == Trigger.OnDoubleClick)
		{
			Play(true);
		}
	}

	private void OnSelect(Boolean isSelected)
	{
		if (enabled && (trigger == Trigger.OnSelect || (trigger == Trigger.OnSelectTrue && isSelected) || (trigger == Trigger.OnSelectFalse && !isSelected)))
		{
			Play(true);
		}
	}

	private void OnActivate(Boolean isActive)
	{
		if (enabled && (trigger == Trigger.OnActivate || (trigger == Trigger.OnActivateTrue && isActive) || (trigger == Trigger.OnActivateFalse && !isActive)))
		{
			Play(isActive);
		}
	}

	private void Update()
	{
		if (disableWhenFinished != DisableCondition.DoNotDisable && mTweens != null)
		{
			Boolean flag = true;
			Boolean flag2 = true;
			Int32 i = 0;
			Int32 num = mTweens.Length;
			while (i < num)
			{
				UITweener uitweener = mTweens[i];
				if (uitweener.tweenGroup == tweenGroup)
				{
					if (uitweener.enabled)
					{
						flag = false;
						break;
					}
					if (uitweener.direction != (Direction)disableWhenFinished)
					{
						flag2 = false;
					}
				}
				i++;
			}
			if (flag)
			{
				if (flag2)
				{
					NGUITools.SetActive(tweenTarget, false);
				}
				mTweens = null;
			}
		}
	}

	public void Play(Boolean forward)
	{
		GameObject gameObject = (!(tweenTarget == null)) ? tweenTarget : this.gameObject;
		if (!NGUITools.GetActive(gameObject))
		{
			if (ifDisabledOnPlay != EnableCondition.EnableThenPlay)
			{
				return;
			}
			NGUITools.SetActive(gameObject, true);
		}
		mTweens = ((!includeChildren) ? gameObject.GetComponents<UITweener>() : gameObject.GetComponentsInChildren<UITweener>());
		if (mTweens.Length == 0)
		{
			if (disableWhenFinished != DisableCondition.DoNotDisable)
			{
				NGUITools.SetActive(tweenTarget, false);
			}
		}
		else
		{
			Boolean flag = false;
			if (playDirection == Direction.Reverse)
			{
				forward = !forward;
			}
			Int32 i = 0;
			Int32 num = mTweens.Length;
			while (i < num)
			{
				UITweener uitweener = mTweens[i];
				if (uitweener.tweenGroup == tweenGroup)
				{
					if (!flag && !NGUITools.GetActive(gameObject))
					{
						flag = true;
						NGUITools.SetActive(gameObject, true);
					}
					if (playDirection == Direction.Toggle)
					{
						uitweener.Toggle();
					}
					else
					{
						uitweener.Play(forward);
					}
					if (resetOnPlay)
					{
						uitweener.Reset();
					}
					uitweener.onFinished = onFinished;
					if (eventReceiver != null && !String.IsNullOrEmpty(callWhenFinished))
					{
						uitweener.eventReceiver = eventReceiver;
						uitweener.callWhenFinished = callWhenFinished;
					}
				}
				i++;
			}
		}
	}
}
