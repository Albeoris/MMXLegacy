using System;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour
{
	public Object parameter;

	public VoidDelegate onSubmit;

	public VoidDelegate onClick;

	public VoidDelegate onDoubleClick;

	public BoolDelegate onHover;

	public BoolDelegate onPress;

	public BoolDelegate onSelect;

	public FloatDelegate onScroll;

	public VectorDelegate onDrag;

	public ObjectDelegate onDrop;

	public StringDelegate onInput;

	public KeyCodeDelegate onKey;

	private void OnSubmit()
	{
		if (onSubmit != null)
		{
			onSubmit(gameObject);
		}
	}

	private void OnClick()
	{
		if (onClick != null)
		{
			onClick(gameObject);
		}
	}

	private void OnDoubleClick()
	{
		if (onDoubleClick != null)
		{
			onDoubleClick(gameObject);
		}
	}

	private void OnHover(Boolean isOver)
	{
		if (onHover != null)
		{
			onHover(gameObject, isOver);
		}
	}

	private void OnPress(Boolean isPressed)
	{
		if (onPress != null)
		{
			onPress(gameObject, isPressed);
		}
	}

	private void OnSelect(Boolean selected)
	{
		if (onSelect != null)
		{
			onSelect(gameObject, selected);
		}
	}

	private void OnScroll(Single delta)
	{
		if (onScroll != null)
		{
			onScroll(gameObject, delta);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (onDrag != null)
		{
			onDrag(gameObject, delta);
		}
	}

	private void OnDrop(GameObject go)
	{
		if (onDrop != null)
		{
			onDrop(gameObject, go);
		}
	}

	private void OnInput(String text)
	{
		if (onInput != null)
		{
			onInput(gameObject, text);
		}
	}

	private void OnKey(KeyCode key)
	{
		if (onKey != null)
		{
			onKey(gameObject, key);
		}
	}

	public static UIEventListener Get(GameObject go)
	{
		UIEventListener uieventListener = go.GetComponent<UIEventListener>();
		if (uieventListener == null)
		{
			uieventListener = go.AddComponent<UIEventListener>();
		}
		return uieventListener;
	}

	public delegate void VoidDelegate(GameObject go);

	public delegate void BoolDelegate(GameObject go, Boolean state);

	public delegate void FloatDelegate(GameObject go, Single delta);

	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	public delegate void StringDelegate(GameObject go, String text);

	public delegate void ObjectDelegate(GameObject go, GameObject draggedObject);

	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);
}
