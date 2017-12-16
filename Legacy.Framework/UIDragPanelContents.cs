using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Panel Contents")]
[ExecuteInEditMode]
public class UIDragPanelContents : MonoBehaviour
{
	public UIDraggablePanel draggablePanel;

	[SerializeField]
	[HideInInspector]
	private UIPanel panel;

	private void Awake()
	{
		if (panel != null)
		{
			if (draggablePanel == null)
			{
				draggablePanel = panel.GetComponent<UIDraggablePanel>();
				if (draggablePanel == null)
				{
					draggablePanel = panel.gameObject.AddComponent<UIDraggablePanel>();
				}
			}
			panel = null;
		}
	}

	private void Start()
	{
		if (draggablePanel == null)
		{
			draggablePanel = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
		}
	}

	private void OnPress(Boolean pressed)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggablePanel != null)
		{
			draggablePanel.Press(pressed);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggablePanel != null)
		{
			draggablePanel.Drag();
		}
	}

	private void OnScroll(Single delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggablePanel != null)
		{
			draggablePanel.Scroll(delta);
		}
	}
}
