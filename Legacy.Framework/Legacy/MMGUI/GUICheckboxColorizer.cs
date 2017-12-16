using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	[AddComponentMenu("MM Legacy/GUI Misc/GUICheckboxColorizer")]
	public class GUICheckboxColorizer : MonoBehaviour
	{
		[SerializeField]
		private UICheckbox m_checkbox;

		[SerializeField]
		private Color m_checkedColor = Color.white;

		private Color m_originalColor;

		private UILabel m_label;

		private void Awake()
		{
			if (m_checkbox == null)
			{
				return;
			}
			m_label = gameObject.GetComponent<UILabel>();
			if (m_label != null)
			{
				m_originalColor = m_label.color;
				UICheckbox checkbox = m_checkbox;
				checkbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(checkbox.onStateChange, new UICheckbox.OnStateChange(OnCheckboxStateChange));
				OnCheckboxStateChange(m_checkbox.isChecked);
			}
		}

		private void OnCheckboxStateChange(Boolean p_state)
		{
			if (m_label != null)
			{
				m_label.color = ((!p_state) ? m_originalColor : m_checkedColor);
			}
		}
	}
}
