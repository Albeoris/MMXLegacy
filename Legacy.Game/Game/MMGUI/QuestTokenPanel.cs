using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/QuestTokenPanel")]
	public class QuestTokenPanel : MonoBehaviour
	{
		private const Int32 MAX_ENTRIES_PER_PAGE = 12;

		[SerializeField]
		private UIGrid m_grid;

		[SerializeField]
		private GameObject m_content;

		[SerializeField]
		private QuestTokenView[] m_tokenViews;

		[SerializeField]
		private UICheckbox m_checkbox;

		[SerializeField]
		private UILabel m_pagerLabel;

		[SerializeField]
		private UIButton m_nextButton;

		[SerializeField]
		private UIButton m_prevButton;

		private Int32 m_currentPage;

		private Int32 m_pageCount;

		private List<Int32> m_neededTokens = new List<Int32>();

		public void Init()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ANNOUNCE_NEEDED_TOKEN, new EventHandler(OnAnnounceNeededToken));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_REMOVED, new EventHandler(OnTokenRemoved));
			m_pageCount = 0;
			m_currentPage = 0;
		}

		public void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ANNOUNCE_NEEDED_TOKEN, new EventHandler(OnAnnounceNeededToken));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_REMOVED, new EventHandler(OnTokenRemoved));
		}

		private void OnAnnounceNeededToken(Object p_sender, EventArgs p_args)
		{
			if (p_args is TokenEventArgs)
			{
				TokenEventArgs tokenEventArgs = (TokenEventArgs)p_args;
				if (IsRelevantToken(tokenEventArgs.TokenID) && !m_neededTokens.Contains(tokenEventArgs.TokenID))
				{
					m_neededTokens.Add(tokenEventArgs.TokenID);
				}
			}
		}

		private void OnStartSceneLoad(Object p_sender, EventArgs p_args)
		{
			m_neededTokens.Clear();
		}

		public void Show()
		{
			NGUITools.SetActive(m_content.gameObject, true);
			NGUITools.SetActive(m_pagerLabel.gameObject, true);
			NGUITools.SetActive(m_nextButton.gameObject, true);
			NGUITools.SetActive(m_prevButton.gameObject, true);
			UICheckbox checkbox = m_checkbox;
			checkbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(checkbox.onStateChange, new UICheckbox.OnStateChange(OnCheckBoxChanged));
			UpdateData();
		}

		public void Hide()
		{
			UICheckbox checkbox = m_checkbox;
			checkbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(checkbox.onStateChange, new UICheckbox.OnStateChange(OnCheckBoxChanged));
			NGUITools.SetActive(m_content.gameObject, false);
			NGUITools.SetActive(m_pagerLabel.gameObject, false);
			NGUITools.SetActive(m_nextButton.gameObject, false);
			NGUITools.SetActive(m_prevButton.gameObject, false);
		}

		private void OnTokenRemoved(Object p_sender, EventArgs p_args)
		{
			UpdateData();
		}

		private void OnCheckBoxChanged(Boolean p_checked)
		{
			UpdateData();
		}

		private void UpdateData()
		{
			List<Int32> collectedTokens = LegacyLogic.Instance.WorldManager.Party.TokenHandler.CollectedTokens;
			List<Int32> list = new List<Int32>();
			for (Int32 i = collectedTokens.Count - 1; i >= 0; i--)
			{
				Int32 num = collectedTokens[i];
				TokenStaticData tokenData = TokenHandler.GetTokenData(num);
				if (tokenData.TokenVisible && IsRelevantToken(num) && (!m_checkbox.isChecked || m_neededTokens.Contains(num)))
				{
					list.Add(num);
				}
			}
			m_pageCount = (Int32)Math.Ceiling(list.Count / 12.0);
			NGUITools.SetActive(m_nextButton.gameObject, !AtEnd());
			NGUITools.SetActive(m_prevButton.gameObject, m_currentPage > 0);
			if (m_pageCount > 0)
			{
				m_pagerLabel.text = LocaManager.GetText("SPELLBOOK_PAGE", m_currentPage + 1, m_pageCount);
			}
			else
			{
				m_pagerLabel.text = String.Empty;
			}
			Int32 num2 = (12 <= list.Count) ? 12 : list.Count;
			Int32 num3 = 0;
			Int32 num4 = 0;
			Int32 num5 = m_currentPage * 12;
			Int32 num6 = (m_currentPage + 1) * 12 - 1;
			foreach (Int32 num7 in list)
			{
				if (num4 >= num5 && num4 <= num6)
				{
					TokenStaticData tokenData2 = TokenHandler.GetTokenData(num7);
					if (tokenData2.TokenVisible && IsRelevantToken(num7))
					{
						if (!m_checkbox.isChecked)
						{
							m_tokenViews[num3].SetTokenData(tokenData2, m_neededTokens.Contains(num7));
							num3++;
						}
						else
						{
							m_tokenViews[num3].SetTokenData(tokenData2, true);
							num3++;
						}
					}
				}
				num4++;
			}
			if (num3 < 12)
			{
				for (Int32 j = num3; j < 12; j++)
				{
					m_tokenViews[j].Hide();
				}
			}
			m_grid.Reposition();
		}

		private Boolean IsRelevantToken(Int32 p_id)
		{
			IEnumerator enumerator = Enum.GetValues(typeof(ETokenID)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Object obj = enumerator.Current;
					ETokenID etokenID = (ETokenID)obj;
					if (p_id == (Int32)etokenID)
					{
						return false;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return true;
		}

		public void PrevPage()
		{
			if (m_currentPage > 0)
			{
				m_currentPage--;
				UpdateData();
				return;
			}
		}

		public void NextPage()
		{
			if (!AtEnd())
			{
				m_currentPage++;
				UpdateData();
				return;
			}
		}

		public Boolean AtEnd()
		{
			return m_currentPage + 1 >= m_pageCount;
		}
	}
}
