using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SelectionList")]
	public class SelectionList : MonoBehaviour, IScrollingListener
	{
		[SerializeField]
		private GameObject m_dataItemPrefab;

		private List<SelectionListItem> m_dataList = new List<SelectionListItem>();

		[SerializeField]
		private UIScrollBar m_scrollbar;

		[SerializeField]
		private GameObject m_listHolder;

		[SerializeField]
		private ScrollingHelper m_helper;

		private SelectionListItem m_selectedItem;

		void IScrollingListener.OnScroll(Single p_delta)
		{
			OnScroll(p_delta);
		}

		public String SelectedItem => m_selectedItem.Value;

	    private void Awake()
		{
			UIScrollBar scrollbar = m_scrollbar;
			scrollbar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(scrollbar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollbarChanged));
			m_helper.Init(this);
		}

		private void OnDestroy()
		{
			UIScrollBar scrollbar = m_scrollbar;
			scrollbar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Remove(scrollbar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollbarChanged));
		}

		private void OnScrollbarChanged(UIScrollBar sb)
		{
			Single num = 24f * (m_dataList.Count * m_scrollbar.scrollValue);
			Single val = 24 * m_dataList.Count - 96;
			num = Math.Min(num, val);
			m_listHolder.transform.localPosition = new Vector3(m_listHolder.transform.localPosition.x, num, m_listHolder.transform.localPosition.z);
		}

		private void OnScroll(Single delta)
		{
			Debug.Log(delta);
			if (delta < 0f)
			{
				OnScrollDownButtonClicked(null);
			}
			else
			{
				OnScrollUpButtonClicked(null);
			}
		}

		public void OnScrollUpButtonClicked(GameObject p_sender)
		{
			Int32 num = (Int32)Math.Round(m_dataList.Count * m_scrollbar.scrollValue);
			num -= 4;
			num = Math.Max(num, 0);
			m_scrollbar.scrollValue = num / (Single)m_dataList.Count;
		}

		public void OnScrollDownButtonClicked(GameObject p_sender)
		{
			Int32 num = (Int32)Math.Round(m_dataList.Count * m_scrollbar.scrollValue);
			num += 4;
			num = Math.Min(num, m_dataList.Count);
			m_scrollbar.scrollValue = num / (Single)m_dataList.Count;
		}

		public void AddItemWithoutReposition(String p_data)
		{
			GameObject gameObject = NGUITools.AddChild(m_listHolder, m_dataItemPrefab);
			gameObject.GetComponent<UILabel>().text = p_data;
			m_dataList.Add(gameObject.GetComponent<SelectionListItem>());
			gameObject.GetComponent<UIButtonMessage>().target = this.gameObject;
			UpdateScrollbar();
			if (m_dataList.Count == 1)
			{
				m_dataList[0].Selected = true;
				m_selectedItem = m_dataList[0];
			}
		}

		public void AddItem(String p_data)
		{
			GameObject gameObject = NGUITools.AddChild(m_listHolder, m_dataItemPrefab);
			gameObject.GetComponent<UILabel>().text = p_data;
			m_dataList.Add(gameObject.GetComponent<SelectionListItem>());
			gameObject.GetComponent<UIButtonMessage>().target = this.gameObject;
			ReposItems();
			UpdateScrollbar();
			if (m_dataList.Count == 1)
			{
				m_dataList[0].Selected = true;
				m_selectedItem = m_dataList[0];
			}
		}

		public void SelectItem(GameObject p_sender)
		{
			if (m_selectedItem != null)
			{
				m_selectedItem.Selected = false;
			}
			m_selectedItem = p_sender.GetComponent<SelectionListItem>();
			m_selectedItem.Selected = true;
		}

		public void ReposItems()
		{
			for (Int32 i = 0; i < m_dataList.Count; i++)
			{
				m_dataList[i].transform.localPosition = new Vector3(5f, -5 - 24 * i, -15f);
				m_dataList[i].transform.localScale = new Vector3(18f, 18f, 1f);
			}
		}

		private void UpdateScrollbar()
		{
			Int32 num = 24 * m_dataList.Count;
			Int32 num2 = 96;
			if (num != 0f)
			{
				m_scrollbar.barSize = num2 / num;
			}
			else
			{
				m_scrollbar.barSize = 1f;
			}
			m_scrollbar.scrollValue = 0f;
		}

		internal void Clear()
		{
			for (Int32 i = 0; i < m_dataList.Count; i++)
			{
				Destroy(m_dataList[i].gameObject);
			}
			m_dataList.Clear();
		}
	}
}
