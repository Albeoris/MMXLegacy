using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/StretchAndScale")]
public class UIStretchedAndScaled : MonoBehaviour
{
	private Rect m_rect;

	private UIRoot m_root;

	public Camera m_uiCamera;

	public Style style;

	public Vector2 relativeSize = Vector2.one;

	public Boolean ScaleRelative = true;

	[SerializeField]
	private Vector2 m_realSize = Vector2.zero;

	private void Start()
	{
		if (m_uiCamera == null)
		{
			m_uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
		}
		m_root = NGUITools.FindInParents<UIRoot>(gameObject);
		m_realSize = transform.localScale;
	}

	private void Update()
	{
		if (style != Style.None)
		{
			Single num = 1f;
			if (!(m_uiCamera != null))
			{
				return;
			}
			m_rect = m_uiCamera.pixelRect;
			if (m_root != null)
			{
				num = m_root.pixelSizeAdjustment;
			}
			Single num2 = m_rect.width;
			Single num3 = m_rect.height;
			if (num != 1f && num3 > 1f)
			{
				Single num4 = m_root.activeHeight / num3;
				num2 *= num4;
				num3 *= num4;
			}
			Vector3 localScale = transform.localScale;
			if (style == Style.Both)
			{
				localScale.x = relativeSize.x * num2;
				localScale.y = relativeSize.y * num3;
			}
			if (style == Style.Horizontal)
			{
				localScale.x = relativeSize.x * num2;
				if (ScaleRelative && transform.localScale.x != localScale.x)
				{
					localScale.y = m_realSize.y * relativeSize.x;
				}
			}
			if (style == Style.Vertical)
			{
				localScale.y = relativeSize.y * num3;
				if (ScaleRelative && transform.localScale.y != localScale.y)
				{
					localScale.x = m_realSize.x * relativeSize.y;
				}
			}
			if (transform.localScale != localScale)
			{
				transform.localScale = localScale;
			}
		}
	}

	public enum Style
	{
		None,
		Horizontal,
		Vertical,
		Both
	}
}
