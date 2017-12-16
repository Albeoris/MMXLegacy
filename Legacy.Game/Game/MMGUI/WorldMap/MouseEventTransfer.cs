using System;
using Legacy.MMGUI;
using UnityEngine;

namespace Legacy.Game.MMGUI.WorldMap
{
	internal class MouseEventTransfer : MonoBehaviour
	{
		[SerializeField]
		private UICameraCustom m_UICameraTarget;

		private void OnHover(Boolean isOver)
		{
			enabled = isOver;
		}

		private void Update()
		{
			Vector3 position = GUIMainCamera.Instance.camera.ScreenToWorldPoint(Input.mousePosition);
			position = transform.InverseTransformPoint(position);
			Vector3 localScale = transform.localScale;
			Vector2 mousePosition = new Vector2(position.x * localScale.x, position.y * localScale.y);
			mousePosition.x = localScale.x * 0.5f + mousePosition.x;
			mousePosition.y = localScale.y * 0.5f - mousePosition.y;
			mousePosition.y = localScale.y - mousePosition.y;
			m_UICameraTarget.ProcessEvents(mousePosition);
		}
	}
}
