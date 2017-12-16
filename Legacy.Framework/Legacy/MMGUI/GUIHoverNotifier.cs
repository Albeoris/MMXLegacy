using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	[AddComponentMenu("MM Legacy/GUI Misc/GUIHoverNotifier")]
	public class GUIHoverNotifier : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_notifiedObject;

		[SerializeField]
		private Boolean m_sendComponentMessage;

		private void OnHover(Boolean p_isHovered)
		{
			String methodName = (!m_sendComponentMessage) ? "OnHover" : "OnComponentHover";
			m_notifiedObject.SendMessage(methodName, p_isHovered, SendMessageOptions.DontRequireReceiver);
		}
	}
}
