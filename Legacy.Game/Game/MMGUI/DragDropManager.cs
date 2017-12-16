using System;
using System.Threading;
using Legacy.Game.HUD;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/DragDropManager")]
	public class DragDropManager : MonoBehaviour
	{
		[SerializeField]
		private BaseDragObject m_draggedItem;

		[SerializeField]
		private UILabel m_itemCounter;

		[SerializeField]
		private UISprite m_sprite;

		[SerializeField]
		private UISprite m_actionSprite;

		[SerializeField]
		private UISprite m_scrollSprite;

		[SerializeField]
		private UISprite m_brokenSprite;

		[SerializeField]
		private UISprite m_itemBackground;

		[SerializeField]
		private Camera m_camera;

		[SerializeField]
		private CharacterHud m_characterHud;

		private ItemSlot m_hoveredSlot;

		private Boolean m_dragStopped;

		private static DragDropManager s_instance;

		public event EventHandler DragEvent;

		public event EventHandler DropEvent;

		public event EventHandler ShortcutRightclickEvent;

		public static DragDropManager Instance => s_instance;

	    public BaseDragObject DraggedItem => m_draggedItem;

	    private void Awake()
		{
			if (s_instance != null)
			{
				throw new InvalidOperationException("Instance already set!");
			}
			s_instance = this;
		}

		public void StartDrag(BaseDragObject p_dragObject)
		{
			m_draggedItem = p_dragObject;
			m_draggedItem.Sprite = m_sprite;
			m_draggedItem.ActionSprite = m_actionSprite;
			m_draggedItem.ItemCounter = m_itemCounter;
			m_draggedItem.ScrollSprite = m_scrollSprite;
			m_draggedItem.BrokenSprite = m_brokenSprite;
			m_draggedItem.ItemBackground = m_itemBackground;
			m_draggedItem.CharacterHud = m_characterHud;
			if (m_draggedItem != null)
			{
				m_draggedItem.Update();
				m_draggedItem.SetActive(true);
				Update();
				RaiseDragEvent();
			}
			AudioController.Play("Drag");
		}

		private void RaiseDragEvent()
		{
			if (DragEvent != null)
			{
				DragEvent(this, EventArgs.Empty);
			}
		}

		public void StopDrag()
		{
			m_dragStopped = true;
			AudioController.Play("Drop");
			NGUITools.SetActiveSelf(m_sprite.gameObject, false);
			NGUITools.SetActiveSelf(m_actionSprite.gameObject, false);
			NGUITools.SetActiveSelf(m_itemCounter.gameObject, false);
			NGUITools.SetActiveSelf(m_scrollSprite.gameObject, false);
			NGUITools.SetActiveSelf(m_brokenSprite.gameObject, false);
			NGUITools.SetActiveSelf(m_itemBackground.gameObject, false);
			NGUITools.SetActiveSelf(m_characterHud.gameObject, false);
		}

		public void DelayDragResolve()
		{
			m_dragStopped = false;
		}

		public void SetHoveredSlot(ItemSlot p_hoveredSlot)
		{
			m_hoveredSlot = p_hoveredSlot;
		}

		private void RaiseDropEvent()
		{
			if (DropEvent != null)
			{
				DropEvent(this, EventArgs.Empty);
			}
		}

		public void EndDragAction()
		{
			RaiseDropEvent();
			m_dragStopped = false;
			m_hoveredSlot = null;
			if (m_draggedItem != null)
			{
				m_draggedItem.SetActive(false);
				m_draggedItem = null;
			}
		}

		public void ShortcutRightClick(ItemSlot p_itemSlot)
		{
			if (ShortcutRightclickEvent != null)
			{
				ShortcutRightclickEvent(p_itemSlot, EventArgs.Empty);
			}
		}

		public void CancelDragAction()
		{
			if (m_draggedItem != null)
			{
				m_dragStopped = false;
				if (m_hoveredSlot != null)
				{
					m_hoveredSlot.OnDragHover(DragHoverEventArgs.Empty);
				}
				m_draggedItem.CancelDragAction();
				EndDragAction();
			}
		}

		private void Update()
		{
			if (m_dragStopped)
			{
				CancelDragAction();
			}
			if (m_draggedItem != null)
			{
				Vector3 mousePosition = Input.mousePosition;
				Transform transform = this.transform;
				if (m_camera != null)
				{
					mousePosition.x = Mathf.Clamp01(mousePosition.x / Screen.width);
					mousePosition.y = Mathf.Clamp01(mousePosition.y / Screen.height);
					transform.position = m_camera.ViewportToWorldPoint(mousePosition);
					if (m_camera.isOrthoGraphic)
					{
						transform.localPosition = NGUIMath.ApplyHalfPixelOffset(transform.localPosition, transform.localScale);
					}
				}
				else
				{
					mousePosition.x -= Screen.width * 0.5f;
					mousePosition.y -= Screen.height * 0.5f;
					transform.localPosition = NGUIMath.ApplyHalfPixelOffset(mousePosition, transform.localScale);
				}
			}
		}
	}
}
