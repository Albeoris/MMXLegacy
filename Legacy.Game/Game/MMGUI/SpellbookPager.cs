using System;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SpellbookPager")]
	public class SpellbookPager : MonoBehaviour
	{
		[SerializeField]
		private UIButton m_prevButton;

		[SerializeField]
		private UIButton m_nextButton;

		[SerializeField]
		private UILabel m_pageLabel;

		public void UpdatePager(Int32 p_page, Int32 p_pageCount)
		{
			if (p_pageCount <= 1)
			{
				m_pageLabel.text = String.Empty;
			}
			else
			{
				m_pageLabel.text = LocaManager.GetText("SPELLBOOK_PAGE", p_page + 1, p_pageCount);
			}
			NGUITools.SetActive(m_prevButton.gameObject, p_page > 0);
			NGUITools.SetActive(m_nextButton.gameObject, p_page < p_pageCount - 1);
		}
	}
}
