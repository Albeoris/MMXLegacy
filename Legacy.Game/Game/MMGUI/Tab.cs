using System;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/Tab")]
	public class Tab : MonoBehaviour
	{
		private const String ACTIVE_SPRITE = "WIN_tab_active_color";

		private const String INACTIVE_SPRITE = "WIN_tab_hidden_color";

		private const String ACTIVE_VERTICAL_SPRITE = "WIN_vertical_tab_active_color";

		private const String INACTIVE_VERTICAL_SPRITE = "WIN_vertical_tab_hidden_color";

		[SerializeField]
		private GameObject m_targetContent;

		[SerializeField]
		private String m_toolTip = String.Empty;

		[SerializeField]
		private Vector3 m_activeOffset;

		[SerializeField]
		private Vector3 m_hoverOffset;

		[SerializeField]
		private Vector3 m_disabledOffset;

		[SerializeField]
		private Single m_activeTweenDuration = 0.2f;

		[SerializeField]
		private Single m_hoverTweenDuration = 0.2f;

		[SerializeField]
		private UISprite m_background;

		[SerializeField]
		private Color m_hoverColor;

		[SerializeField]
		private Color m_activeColor;

		[SerializeField]
		private Color m_activeHoverColor;

		[SerializeField]
		private Color m_disabledColor;

		private Vector3 m_pos;

		private Boolean m_started;

		private Boolean m_active;

		private Boolean m_isHovered;

		private Boolean m_enabled = true;

		private Color m_normalColor;

		private BoxCollider m_collider;

		private Vector3 m_colliderSize;

		private Vector3 m_colliderHoveredSize;

		private String m_additionalTooltip = String.Empty;

		private TabController m_tabControl;

		public TabController TabControl
		{
			set => m_tabControl = value;
		}

		public GameObject TargetContent => m_targetContent;

	    public Boolean IsEnabled => m_enabled;

	    public Int32 TabID { get; set; }

		private void Awake()
		{
			if (!m_started)
			{
				m_started = true;
				m_pos = gameObject.transform.localPosition;
				m_normalColor = m_background.color;
				m_collider = GetComponent<BoxCollider>();
				m_colliderSize = m_collider.size;
				m_colliderHoveredSize = m_colliderSize;
				m_colliderHoveredSize.y = m_colliderHoveredSize.y + Math.Abs(m_hoverOffset.y) * 2f;
			}
		}

		private void OnDisable()
		{
			m_isHovered = false;
			m_collider.size = m_colliderSize;
			TooltipManager.Instance.Hide(this);
		}

		public void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (enabled)
			{
				if (m_active)
				{
					TweenColor.Begin(m_background.gameObject, m_hoverTweenDuration, (!p_isOver) ? m_activeColor : m_activeHoverColor);
				}
				else
				{
					TweenPosition.Begin(gameObject, m_activeTweenDuration, (!p_isOver) ? m_pos : (m_pos + m_hoverOffset));
					TweenColor.Begin(m_background.gameObject, m_hoverTweenDuration, (!p_isOver) ? m_normalColor : m_hoverColor);
					m_collider.size = ((!p_isOver) ? m_colliderSize : m_colliderHoveredSize);
				}
			}
		}

		public void SetEnabled(Boolean p_enabled)
		{
			if (m_enabled == p_enabled)
			{
				return;
			}
			m_enabled = p_enabled;
			m_collider.enabled = p_enabled;
			if (p_enabled)
			{
				gameObject.transform.localPosition = ((!m_active) ? m_pos : (m_pos + m_activeOffset));
				if (m_active)
				{
					TweenColor.Begin(m_background.gameObject, 0f, (!m_isHovered) ? m_activeColor : m_activeHoverColor);
				}
				else
				{
					TweenColor.Begin(m_background.gameObject, 0f, (!m_isHovered) ? m_normalColor : m_hoverColor);
				}
			}
			else
			{
				gameObject.transform.localPosition = m_pos + m_disabledOffset;
				TweenColor.Begin(m_background.gameObject, 0f, m_disabledColor);
				m_active = false;
			}
		}

		public void SetActive(Boolean p_active, Boolean p_skipAnimation)
		{
			if (!m_enabled)
			{
				if (m_targetContent != null)
				{
					NGUITools.SetActive(m_targetContent, false);
				}
				return;
			}
			m_active = p_active;
			Single duration = (!p_skipAnimation) ? m_activeTweenDuration : 0f;
			if (p_active)
			{
				TweenPosition tweenPosition = TweenPosition.Begin(gameObject, duration, m_pos + m_activeOffset);
				tweenPosition.method = UITweener.Method.EaseOut;
				tweenPosition.steeperCurves = true;
				TweenColor.Begin(m_background.gameObject, duration, (!m_isHovered) ? m_activeColor : m_activeHoverColor);
			}
			else
			{
				TweenPosition tweenPosition2 = TweenPosition.Begin(gameObject, duration, m_pos);
				tweenPosition2.method = UITweener.Method.EaseOut;
				tweenPosition2.steeperCurves = true;
				TweenColor.Begin(m_background.gameObject, duration, (!m_isHovered) ? m_normalColor : m_hoverColor);
			}
			if (m_targetContent != null)
			{
				NGUITools.SetActive(m_targetContent, p_active);
			}
		}

		public void OnTabClicked()
		{
			m_tabControl.OnTabClicked(TabID, false);
		}

		public void AddTextToTooltip(String p_additionalText)
		{
			m_additionalTooltip = p_additionalText;
		}

		private void OnTooltip(Boolean p_show)
		{
			if (p_show && m_toolTip != String.Empty)
			{
				String p_tooltipText = LocaManager.GetText(m_toolTip) + m_additionalTooltip;
				TooltipManager.Instance.Show(this, p_tooltipText, gameObject.transform.position, gameObject.transform.localScale * 0.5f);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
