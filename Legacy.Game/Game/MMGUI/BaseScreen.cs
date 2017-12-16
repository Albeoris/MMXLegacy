using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/BaseScreen")]
	public class BaseScreen : MonoBehaviour
	{
		protected BilateralScreen m_bilaterialScreen;

		protected TabController m_tabController;

		private Boolean m_isopen;

		public BilateralScreen BilateralScreen
		{
			get => m_bilaterialScreen;
		    set => m_bilaterialScreen = value;
		}

		public Boolean IsOpen => m_isopen;

	    public virtual void ToggleOpenClose()
		{
			m_isopen = !m_isopen;
			SetElementsActiveState(m_isopen);
		}

		public virtual void Close()
		{
			m_isopen = false;
			SetElementsActiveState(m_isopen);
		}

		public virtual void Open(Int32 p_tabID)
		{
			m_isopen = true;
			SetElementsActiveState(m_isopen, p_tabID);
		}

		protected virtual void SetElementsActiveState(Boolean p_enabled)
		{
			NGUITools.SetActiveSelf(gameObject, p_enabled);
			if (p_enabled)
			{
				OpenDefaultTab();
			}
			else
			{
				DragDropManager.Instance.CancelDragAction();
			}
		}

		protected virtual void SetElementsActiveState(Boolean p_enabled, Int32 p_tabID)
		{
			NGUITools.SetActiveSelf(gameObject, p_enabled);
			if (p_enabled)
			{
				OpenTab(p_tabID);
			}
			else
			{
				DragDropManager.Instance.CancelDragAction();
			}
		}

		protected virtual void OpenDefaultTab()
		{
			if (m_tabController == null)
			{
				m_tabController = gameObject.GetComponentInChildren<TabController>();
			}
			m_tabController.OnTabClicked(0, true);
		}

		protected virtual void OpenTab(Int32 p_tabIndex)
		{
			if (m_tabController == null)
			{
				m_tabController = gameObject.GetComponentInChildren<TabController>();
			}
			m_tabController.OnTabClicked(p_tabIndex, false);
		}
	}
}
