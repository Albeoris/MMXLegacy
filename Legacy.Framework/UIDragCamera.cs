using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Camera")]
[ExecuteInEditMode]
public class UIDragCamera : IgnoreTimeScale
{
	public UIDraggableCamera draggableCamera;

	[SerializeField]
	[HideInInspector]
	private Component target;

	private void Awake()
	{
		if (target != null)
		{
			if (draggableCamera == null)
			{
				draggableCamera = target.GetComponent<UIDraggableCamera>();
				if (draggableCamera == null)
				{
					draggableCamera = target.gameObject.AddComponent<UIDraggableCamera>();
				}
			}
			target = null;
		}
		else if (draggableCamera == null)
		{
			draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(gameObject);
		}
	}

	private void OnPress(Boolean isPressed)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
		{
			draggableCamera.Press(isPressed);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
		{
			draggableCamera.Drag(delta);
		}
	}

	private void OnScroll(Single delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
		{
			draggableCamera.Scroll(delta);
		}
	}
}
