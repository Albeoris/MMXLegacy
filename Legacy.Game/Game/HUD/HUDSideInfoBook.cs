using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.StaticData;
using UnityEngine;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDSideInfoBook")]
	public class HUDSideInfoBook : HUDSideInfoBase
	{
		[SerializeField]
		private String m_bookIconName = String.Empty;

		private Int32 m_loreBookId;

		private String m_loreBookTitle;

		private ELoreBookCategories m_category;

		public Int32 BookId => m_loreBookId;

	    public String BookTitle => m_loreBookTitle;

	    public ELoreBookCategories Category => m_category;

	    public void Init(LoreBookStaticData p_data)
		{
			m_loreBookId = p_data.StaticID;
			m_loreBookTitle = LocaManager.GetText(p_data.TitleKey);
			m_category = p_data.Category;
			Color yellow = Color.yellow;
			base.Init(m_loreBookTitle, m_bookIconName, 0, yellow);
		}

		protected override Boolean IsTooltipNeeded()
		{
			return false;
		}

		protected override void ShowTooltip()
		{
		}

		protected override void HideTooltip()
		{
		}

		protected override void OnClick()
		{
			if (m_loreBookId > 0)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.OPEN_LOREBOOK, EventArgs.Empty);
			}
		}
	}
}
