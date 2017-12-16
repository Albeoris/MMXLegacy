using System;
using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox")]
public class UICheckbox : MonoBehaviour
{
	public static UICheckbox current;

	public UISprite checkSprite;

	public Animation checkAnimation;

	public Boolean instantTween;

	public Boolean startsChecked = true;

	public Transform radioButtonRoot;

	public Boolean optionCanBeNone;

	public GameObject eventReceiver;

	public String functionName = "OnActivate";

	public OnStateChange onStateChange;

	[SerializeField]
	[HideInInspector]
	private Boolean option;

	private Boolean mChecked = true;

	private Boolean mStarted;

	private Transform mTrans;

	public Boolean isChecked
	{
		get => mChecked;
	    set
		{
			if (radioButtonRoot == null || value || optionCanBeNone || !mStarted)
			{
				Set(value);
			}
		}
	}

	private void Awake()
	{
		mTrans = transform;
		if (checkSprite != null)
		{
			checkSprite.alpha = ((!startsChecked) ? 0f : 1f);
		}
		if (option)
		{
			option = false;
			if (radioButtonRoot == null)
			{
				radioButtonRoot = mTrans.parent;
			}
		}
	}

	private void Start()
	{
		if (eventReceiver == null)
		{
			eventReceiver = gameObject;
		}
		mChecked = !startsChecked;
		mStarted = true;
		Set(startsChecked);
	}

	private void OnClick()
	{
		if (enabled)
		{
			isChecked = !isChecked;
		}
	}

	private void Set(Boolean state)
	{
		if (!mStarted)
		{
			mChecked = state;
			startsChecked = state;
			if (checkSprite != null)
			{
				checkSprite.alpha = ((!state) ? 0f : 1f);
			}
		}
		else if (mChecked != state)
		{
			if (radioButtonRoot != null && state)
			{
				UICheckbox[] componentsInChildren = radioButtonRoot.GetComponentsInChildren<UICheckbox>(true);
				Int32 i = 0;
				Int32 num = componentsInChildren.Length;
				while (i < num)
				{
					UICheckbox uicheckbox = componentsInChildren[i];
					if (uicheckbox != this && uicheckbox.radioButtonRoot == radioButtonRoot)
					{
						uicheckbox.Set(false);
					}
					i++;
				}
			}
			mChecked = state;
			if (checkSprite != null)
			{
				if (instantTween)
				{
					checkSprite.alpha = ((!mChecked) ? 0f : 1f);
				}
				else
				{
					TweenAlpha.Begin(checkSprite.gameObject, 0.15f, (!mChecked) ? 0f : 1f);
				}
			}
			current = this;
			if (onStateChange != null)
			{
				onStateChange(mChecked);
			}
			if (eventReceiver != null && !String.IsNullOrEmpty(functionName))
			{
				eventReceiver.SendMessage(functionName, mChecked, SendMessageOptions.DontRequireReceiver);
			}
			current = null;
			if (checkAnimation != null)
			{
				ActiveAnimation.Play(checkAnimation, (!state) ? Direction.Reverse : Direction.Forward);
			}
		}
	}

	public delegate void OnStateChange(Boolean state);
}
