using System;
using Legacy.Core.Mods;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ModEntry")]
	public class ModEntry : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_label;

		[SerializeField]
		private Color m_selectedColor;

		[SerializeField]
		private GameObject m_background;

		private ModMenu m_controller;

		private ModController.ModInfo m_modInfo;

		public ModController.ModInfo ModInfo => m_modInfo;

	    public void Init(ModMenu p_controller, ModController.ModInfo p_modInfo)
		{
			m_controller = p_controller;
			m_modInfo = p_modInfo;
			m_label.text = p_modInfo.Name;
		}

		public void CleanUp()
		{
			m_controller = null;
		}

		public void Select()
		{
			GetComponent<UIButtonColor>().enabled = false;
			m_label.color = m_selectedColor;
		}

		public void Unselect()
		{
			m_label.color = Color.black;
			GetComponent<UIButtonColor>().enabled = true;
		}

		public void OnEntryClick(GameObject p_sender)
		{
			m_controller.ClickedEntry(this);
		}

		public void OnEnter(GameObject p_sender)
		{
			NGUITools.SetActive(m_background, true);
		}

		public void OnLeave(GameObject p_sender)
		{
			NGUITools.SetActive(m_background, false);
		}
	}
}
