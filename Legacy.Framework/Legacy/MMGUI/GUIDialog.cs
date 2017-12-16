using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	public class GUIDialog : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_title;

		[SerializeField]
		private UILabel m_text;

		private GUIDialogParameters.Callback m_callback;

		public void SetTitle(String p_title)
		{
			m_title.text = p_title;
		}

		public void SetText(String p_text)
		{
			m_text.text = p_text;
		}

		public void SetCallback(GUIDialogParameters.Callback p_callback)
		{
			m_callback = p_callback;
		}

		private void OnButtonClickOK()
		{
			if (m_callback != null)
			{
				m_callback(GUIDialogParameters.EDialogAnswer.OK);
			}
			Destroy(gameObject);
		}

		private void OnButtonClickYES()
		{
			if (m_callback != null)
			{
				m_callback(GUIDialogParameters.EDialogAnswer.YES);
			}
			Destroy(gameObject);
		}

		private void OnButtonClickNO()
		{
			if (m_callback != null)
			{
				m_callback(GUIDialogParameters.EDialogAnswer.NO);
			}
			Destroy(gameObject);
		}
	}
}
