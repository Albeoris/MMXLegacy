using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Forward Events")]
public class UIForwardEvents : MonoBehaviour
{
	public GameObject target;

	public Boolean onHover;

	public Boolean onPress;

	public Boolean onClick;

	public Boolean onDoubleClick;

	public Boolean onSelect;

	public Boolean onDrag;

	public Boolean onDrop;

	public Boolean onInput;

	public Boolean onSubmit;

	public Boolean onScroll;

	private void OnHover(Boolean isOver)
	{
		if (onHover && target != null)
		{
			target.SendMessage("OnHover", isOver, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnPress(Boolean pressed)
	{
		if (onPress && target != null)
		{
			target.SendMessage("OnPress", pressed, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnClick()
	{
		if (onClick && target != null)
		{
			target.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnDoubleClick()
	{
		if (onDoubleClick && target != null)
		{
			target.SendMessage("OnDoubleClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnSelect(Boolean selected)
	{
		if (onSelect && target != null)
		{
			target.SendMessage("OnSelect", selected, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (onDrag && target != null)
		{
			target.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnDrop(GameObject go)
	{
		if (onDrop && target != null)
		{
			target.SendMessage("OnDrop", go, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnInput(String text)
	{
		if (onInput && target != null)
		{
			target.SendMessage("OnInput", text, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnSubmit()
	{
		if (onSubmit && target != null)
		{
			target.SendMessage("OnSubmit", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnScroll(Single delta)
	{
		if (onScroll && target != null)
		{
			target.SendMessage("OnScroll", delta, SendMessageOptions.DontRequireReceiver);
		}
	}
}
