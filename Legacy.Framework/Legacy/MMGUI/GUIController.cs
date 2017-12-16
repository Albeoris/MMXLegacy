using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	public class GUIController : MonoBehaviour
	{
		[SerializeField]
		private UIAnchor m_anchor;

		private GUIDialogController m_dialogCtrl = new GUIDialogController();

		private void OnShowDialog(GUIDialogParameters p_dialogParams)
		{
			m_dialogCtrl.OnShowDialog(p_dialogParams, m_anchor);
		}
	}
}
