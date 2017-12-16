using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : MonoBehaviour
{
	public Arrangement arrangement;

	public Int32 maxPerLine;

	public Single cellWidth = 200f;

	public Single cellHeight = 200f;

	public Boolean repositionNow;

	public Boolean sorted;

	public Boolean hideInactive = true;

	private Boolean mStarted;

	private void Start()
	{
		mStarted = true;
		Reposition();
	}

	private void Update()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}

	public static Int32 SortByName(Transform a, Transform b)
	{
		return String.Compare(a.name, b.name);
	}

	public void Reposition()
	{
		if (!mStarted)
		{
			repositionNow = true;
			return;
		}
		Transform transform = this.transform;
		Int32 num = 0;
		Int32 num2 = 0;
		if (sorted)
		{
			List<Transform> list = new List<Transform>();
			for (Int32 i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child && (!hideInactive || NGUITools.GetActive(child.gameObject)))
				{
					list.Add(child);
				}
			}
			list.Sort(new Comparison<Transform>(SortByName));
			Int32 j = 0;
			Int32 count = list.Count;
			while (j < count)
			{
				Transform transform2 = list[j];
				if (NGUITools.GetActive(transform2.gameObject) || !hideInactive)
				{
					Single z = transform2.localPosition.z;
					transform2.localPosition = ((arrangement != Arrangement.Horizontal) ? new Vector3(cellWidth * num2, -cellHeight * num, z) : new Vector3(cellWidth * num, -cellHeight * num2, z));
					if (++num >= maxPerLine && maxPerLine > 0)
					{
						num = 0;
						num2++;
					}
				}
				j++;
			}
		}
		else
		{
			for (Int32 k = 0; k < transform.childCount; k++)
			{
				Transform child2 = transform.GetChild(k);
				if (NGUITools.GetActive(child2.gameObject) || !hideInactive)
				{
					Single z2 = child2.localPosition.z;
					child2.localPosition = ((arrangement != Arrangement.Horizontal) ? new Vector3(cellWidth * num2, -cellHeight * num, z2) : new Vector3(cellWidth * num, -cellHeight * num2, z2));
					if (++num >= maxPerLine && maxPerLine > 0)
					{
						num = 0;
						num2++;
					}
				}
			}
		}
		UIDraggablePanel uidraggablePanel = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
		if (uidraggablePanel != null)
		{
			uidraggablePanel.UpdateScrollbars(true);
		}
	}

	public enum Arrangement
	{
		Horizontal,
		Vertical
	}
}
