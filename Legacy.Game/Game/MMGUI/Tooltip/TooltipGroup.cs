using System;
using Legacy.Core.Configuration;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/TooltipGroup")]
	public class TooltipGroup : MonoBehaviour
	{
		[SerializeField]
		private UISlicedSprite m_background;

		[SerializeField]
		private UILabel m_label;

		[SerializeField]
		private Vector3 m_padding;

		private Single m_minHeight;

		private Boolean m_visible = true;

		private Align m_verticalAlign;

		private Int32 m_originalMaxWidth;

		public UILabel Label => m_label;

	    public Boolean IsVisible => m_visible;

	    public Vector3 Size => m_background.transform.localScale;

	    public Single MinHeight
		{
			get => m_minHeight;
	        set => m_minHeight = value;
	    }

		public Align VerticalAlign
		{
			get => m_verticalAlign;
		    set => m_verticalAlign = value;
		}

		public Single HorizontalScaleFactor
		{
			set => m_label.lineWidth = (Int32)Math.Round(m_originalMaxWidth * value);
		}

		private void Awake()
		{
			m_originalMaxWidth = m_label.lineWidth;
			m_background.alpha = ConfigManager.Instance.Options.TooltipOpacity;
		}

		public void SetVisible(Boolean p_visible)
		{
			if (p_visible != m_visible)
			{
				m_visible = p_visible;
				NGUITools.SetActiveSelf(gameObject, p_visible);
			}
		}

		public void Resize(Vector3 p_size)
		{
			m_background.transform.localScale = p_size;
		}

		public void UpdateText(String p_text)
		{
			m_label.text = p_text;
			m_label.text = p_text.Replace("  ", " ");
			Transform transform = m_label.transform;
			Vector3 localScale = transform.localScale;
			Vector3 vector = m_label.relativeSize;
			vector.x *= localScale.x;
			vector.y *= localScale.y;
			vector += 2f * m_padding;
			if (vector.y < m_minHeight)
			{
				vector.y = m_minHeight;
			}
			m_background.transform.localScale = vector;
			Vector3 localPosition = transform.localPosition;
			localPosition.y = ((m_verticalAlign != Align.TOP) ? (-Size.y * 0.5f) : (-m_padding.y));
			transform.localPosition = localPosition;
		}

		public void UpdatePositionY(Single p_posY)
		{
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.y = p_posY;
			gameObject.transform.localPosition = localPosition;
		}

		public enum Align
		{
			TOP,
			CENTER
		}
	}
}
