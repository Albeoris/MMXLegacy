using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	public class GUITabView : MonoBehaviour
	{
		public GameObject m_tabButtonPrefab;

		public GUITabParameters[] m_tabs;

		public Int32 m_currTabIndex;

		public GameObject m_currTab;

		public void Start()
		{
			ShowTab(m_currTabIndex);
			for (Int32 i = 0; i < m_tabs.Length; i++)
			{
				GameObject gameObject = NGUITools.AddChild(this.gameObject, m_tabButtonPrefab);
				gameObject.transform.localPosition = gameObject.transform.localPosition + Vector3.right * i * 200f;
				UIButtonMessage uibuttonMessage = gameObject.AddComponent<UIButtonMessage>();
				uibuttonMessage.target = this.gameObject;
				uibuttonMessage.functionName = "OnTabButtonClick";
			}
		}

		private void ShowTab(Int32 p_tabIndex)
		{
			if (m_currTab != null)
			{
				if (m_tabs[m_currTabIndex].m_tabName == m_tabs[p_tabIndex].m_tabName)
				{
					return;
				}
				Destroy(m_currTab);
			}
			m_currTab = NGUITools.AddChild(gameObject, m_tabs[p_tabIndex].m_prefab);
			m_currTabIndex = p_tabIndex;
		}

		private void OnTabButtonClick()
		{
			Int32 num = m_currTabIndex + 1;
			if (num >= m_tabs.Length)
			{
				num = 0;
			}
			ShowTab(num);
		}

		[Serializable]
		public class GUITabParameters
		{
			public GameObject m_prefab;

			public String m_tabName;
		}
	}
}
