using System;
using UnityEngine;

public class DragHoverEventArgs : EventArgs
{
	public new static readonly DragHoverEventArgs Empty = new DragHoverEventArgs(null, false);

	public DragHoverEventArgs(GameObject p_dragger, Boolean p_isHovered)
	{
		Dragger = p_dragger;
		IsHovered = p_isHovered;
	}

	public GameObject Dragger { get; private set; }

	public Boolean IsHovered { get; private set; }
}
