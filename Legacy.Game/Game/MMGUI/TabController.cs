using System;
using System.Threading;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/TabController")]
	public class TabController : MonoBehaviour
	{
		[SerializeField]
		private Tab[] m_tabs;

		private Int32 m_currentTabIndex;

		private Boolean m_firstTabCall = true;

		public event EventHandler TabIndexChanged;

		public Tab[] Tabs => m_tabs;

	    public Int32 CurrentTabIndex
		{
			get => m_currentTabIndex;
	        set => m_currentTabIndex = value;
	    }

		public void Awake()
		{
			InitTabs();
		}

		private void InitTabs()
		{
			for (Int32 i = 0; i < m_tabs.Length; i++)
			{
				Tab t = m_tabs[i];
				InitTab(i, t);
			}
		}

		private void InitTab(Int32 index, Tab t)
		{
			t.TabControl = this;
			t.TabID = index;
		}

		public void OnTabClicked(Int32 p_tabID, Boolean p_skipAnimation)
		{
			if (m_currentTabIndex == p_tabID && !m_firstTabCall)
			{
				return;
			}
			m_firstTabCall = false;
			SelectTab(p_tabID, p_skipAnimation);
		}

		public void SelectTab(Int32 p_tabID, Boolean p_skipAnimation)
		{
			for (Int32 i = 0; i < m_tabs.Length; i++)
			{
				Tab tab = m_tabs[i];
				if (tab.TabID == p_tabID)
				{
					m_currentTabIndex = i;
					tab.SetActive(true, p_skipAnimation);
					SendOnAfterEnable(tab.TargetContent);
				}
				else
				{
					tab.SetActive(false, p_skipAnimation);
				}
			}
			if (TabIndexChanged != null)
			{
				TabIndexChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void SendOnAfterEnable(GameObject p_receiver)
		{
			if (p_receiver != null)
			{
				p_receiver.BroadcastMessage("OnAfterEnable", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
