using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Table")]
[ExecuteInEditMode]
public class UITable : MonoBehaviour
{
	public Int32 columns;

	public Direction direction;

	public Vector2 padding = Vector2.zero;

	public Boolean sorted;

	public Boolean hideInactive = true;

	public Boolean repositionNow;

	public Boolean keepWithinPanel;

	public OnReposition onReposition;

	private UIPanel mPanel;

	private UIDraggablePanel mDrag;

	private Boolean mStarted;

	private List<Transform> mChildren = new List<Transform>();

	public static Int32 SortByName(Transform a, Transform b)
	{
		return String.Compare(a.name, b.name);
	}

	public List<Transform> children
	{
		get
		{
			if (mChildren.Count == 0)
			{
				Transform transform = this.transform;
				mChildren.Clear();
				for (Int32 i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if (child && child.gameObject && (!hideInactive || NGUITools.GetActive(child.gameObject)))
					{
						mChildren.Add(child);
					}
				}
				if (sorted)
				{
					mChildren.Sort(new Comparison<Transform>(SortByName));
				}
			}
			return mChildren;
		}
	}

	private void RepositionVariableSize(List<Transform> children)
	{
		Single num = 0f;
		Single num2 = 0f;
		Int32 num3 = (columns <= 0) ? 1 : (children.Count / columns + 1);
		Int32 num4 = (columns <= 0) ? children.Count : columns;
		Bounds[,] array = new Bounds[num3, num4];
		Bounds[] array2 = new Bounds[num4];
		Bounds[] array3 = new Bounds[num3];
		Int32 num5 = 0;
		Int32 num6 = 0;
		Int32 i = 0;
		Int32 count = children.Count;
		while (i < count)
		{
			Transform transform = children[i];
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform);
			Vector3 localScale = transform.localScale;
			bounds.min = Vector3.Scale(bounds.min, localScale);
			bounds.max = Vector3.Scale(bounds.max, localScale);
			array[num6, num5] = bounds;
			array2[num5].Encapsulate(bounds);
			array3[num6].Encapsulate(bounds);
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
			}
			i++;
		}
		num5 = 0;
		num6 = 0;
		Int32 j = 0;
		Int32 count2 = children.Count;
		while (j < count2)
		{
			Transform transform2 = children[j];
			Bounds bounds2 = array[num6, num5];
			Bounds bounds3 = array2[num5];
			Bounds bounds4 = array3[num6];
			Vector3 localPosition = transform2.localPosition;
			localPosition.x = num + bounds2.extents.x - bounds2.center.x;
			localPosition.x += bounds2.min.x - bounds3.min.x + padding.x;
			if (direction == Direction.Down)
			{
				localPosition.y = -num2 - bounds2.extents.y - bounds2.center.y;
				localPosition.y += (bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y) * 0.5f - padding.y;
			}
			else
			{
				localPosition.y = num2 + bounds2.extents.y - bounds2.center.y;
				localPosition.y += (bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y) * 0.5f - padding.y;
			}
			num += bounds3.max.x - bounds3.min.x + padding.x * 2f;
			transform2.localPosition = localPosition;
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
				num = 0f;
				num2 += bounds4.size.y + padding.y * 2f;
			}
			j++;
		}
	}

	public void Reposition()
	{
		if (mStarted)
		{
			Transform transform = this.transform;
			mChildren.Clear();
			List<Transform> children = this.children;
			if (children.Count > 0)
			{
				RepositionVariableSize(children);
			}
			if (mDrag != null)
			{
				mDrag.UpdateScrollbars(true);
				mDrag.RestrictWithinBounds(true);
			}
			else if (mPanel != null)
			{
				mPanel.ConstrainTargetToBounds(transform, true);
			}
			if (onReposition != null)
			{
				onReposition();
			}
		}
		else
		{
			repositionNow = true;
		}
	}

	private void Start()
	{
		mStarted = true;
		if (keepWithinPanel)
		{
			mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
		}
		Reposition();
	}

	private void LateUpdate()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}

	public delegate void OnReposition();

	public enum Direction
	{
		Down,
		Up
	}
}
