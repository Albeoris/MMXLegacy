using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Message")]
public class UIButtonMessage : MonoBehaviour
{
	public GameObject target;

	public String functionName;

	public Trigger trigger;

	public Boolean includeChildren;

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
			if ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut))
			{
				Send();
			}
			mHighlighted = isOver;
		}
	}

	private void OnPress(Boolean isPressed)
	{
		if (enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
		{
			Send();
		}
	}

	private void OnClick()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			Send();
		}
	}

	private void OnDoubleClick()
	{
		if (enabled && trigger == Trigger.OnDoubleClick)
		{
			Send();
		}
	}

	private void Send()
	{
		if (String.IsNullOrEmpty(functionName))
		{
			return;
		}
		if (target == null)
		{
			target = gameObject;
		}
		if (includeChildren)
		{
			Transform[] componentsInChildren = target.GetComponentsInChildren<Transform>();
			Int32 i = 0;
			Int32 num = componentsInChildren.Length;
			while (i < num)
			{
				Transform transform = componentsInChildren[i];
				transform.gameObject.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
				i++;
			}
		}
		else
		{
			target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick
	}
}
