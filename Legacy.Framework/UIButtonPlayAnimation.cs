using System;
using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Play Animation")]
public class UIButtonPlayAnimation : MonoBehaviour
{
	public Animation target;

	public String clipName;

	public Trigger trigger;

	public Direction playDirection = Direction.Forward;

	public Boolean resetOnPlay;

	public Boolean clearSelection;

	public EnableCondition ifDisabledOnPlay;

	public DisableCondition disableWhenFinished;

	public GameObject eventReceiver;

	public String callWhenFinished;

	public ActiveAnimation.OnFinished onFinished;

	private Boolean mStarted;

	private Boolean mHighlighted;

	private void Start()
	{
		mStarted = true;
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

	private void Play(Boolean forward)
	{
		if (target == null)
		{
			target = GetComponentInChildren<Animation>();
		}
		if (target != null)
		{
			if (clearSelection && UICamera.selectedObject == gameObject)
			{
				UICamera.selectedObject = null;
			}
			Int32 num = -(Int32)playDirection;
			Direction direction = (Direction)((!forward) ? num : ((Int32)playDirection));
			ActiveAnimation activeAnimation = ActiveAnimation.Play(target, clipName, direction, ifDisabledOnPlay, disableWhenFinished);
			if (activeAnimation == null)
			{
				return;
			}
			if (resetOnPlay)
			{
				activeAnimation.Reset();
			}
			activeAnimation.onFinished = onFinished;
			if (eventReceiver != null && !String.IsNullOrEmpty(callWhenFinished))
			{
				activeAnimation.eventReceiver = eventReceiver;
				activeAnimation.callWhenFinished = callWhenFinished;
			}
			else
			{
				activeAnimation.eventReceiver = null;
			}
		}
	}
}
