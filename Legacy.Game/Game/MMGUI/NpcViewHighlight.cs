using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/NpcViewHighlight")]
	public class NpcViewHighlight : MonoBehaviour
	{
		[SerializeField]
		private UISlicedSprite m_sprite;

		private void OnEnter(GameObject p_sender)
		{
			NGUITools.SetActive(m_sprite.gameObject, true);
		}

		private void OnLeave(GameObject p_sender)
		{
			NGUITools.SetActive(m_sprite.gameObject, false);
		}
	}
}
