using System;
using System.Threading;
using UnityEngine;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDActionLogDragButton")]
	public class HUDActionLogDragButton : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_background;

		private IResizable m_parent;

		private Single m_dragYTotalDelta;

		private Boolean m_pressed;

		private Boolean m_sendEvent;

		public event EventHandler ButtonReleased;

		public Single DragYTotalDelta
		{
			get => m_dragYTotalDelta;
		    set => m_dragYTotalDelta = value;
		}

		public Boolean IsPressed => m_pressed;

	    public void Init(IResizable p_parent)
		{
			m_parent = p_parent;
			m_pressed = false;
			m_sendEvent = false;
		}

		public void FadeButton(Single p_alpha)
		{
			if (p_alpha == 0f)
			{
				NGUITools.SetActive(m_background.gameObject, false);
			}
			else
			{
				NGUITools.SetActive(m_background.gameObject, true);
			}
			m_background.alpha = p_alpha;
		}

		private void OnPress(Boolean p_isDown)
		{
			if (m_pressed && !p_isDown)
			{
				m_sendEvent = true;
			}
			m_pressed = p_isDown;
			if (p_isDown)
			{
				m_dragYTotalDelta = 0f;
			}
			if (m_sendEvent)
			{
				m_sendEvent = false;
				if (ButtonReleased != null)
				{
					ButtonReleased(this, EventArgs.Empty);
				}
			}
		}

		private void OnDrag(Vector2 p_delta)
		{
			m_dragYTotalDelta += p_delta.y;
			m_parent.OnSizeChanged();
		}
	}
}
