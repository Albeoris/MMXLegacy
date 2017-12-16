using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/DragMessage")]
	public class DragMessage : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_target;

		private void OnDrag(Vector2 p_delta)
		{
			if (DragDropManager.Instance.DraggedItem == null)
			{
				m_target.SendMessage("StartDrag", SendMessageOptions.DontRequireReceiver);
			}
		}

		protected virtual void OnPress(Boolean p_isDown)
		{
			if (UICamera.currentTouchID == -1 && !p_isDown)
			{
				m_target.SendMessage("EndDrag");
			}
		}
	}
}
