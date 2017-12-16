using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Game.MMGUI;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDLootEntry")]
	public class HUDLootEntry : HUDSideInfoBase
	{
		private BaseItem m_item;

		public void Init(BaseItem item, Int32 gold)
		{
			m_item = item;
			String icon = null;
			if (m_item is Scroll)
			{
				icon = "ITM_consumable_scroll";
			}
			else if (m_item != null)
			{
				icon = m_item.Icon;
			}
			base.Init((m_item == null) ? null : m_item.Name, icon, gold, (m_item == null) ? Color.green : ItemTooltip.GetItemColor(m_item));
			if (m_item is Scroll)
			{
				NGUITools.SetActiveSelf(m_scrollIcon.gameObject, true);
				m_scrollIcon.spriteName = m_item.Icon;
			}
			else if (m_item != null)
			{
				ItemSlot.UpdateItemBackground(m_itemIconBackground, m_item);
			}
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		protected override Boolean IsTooltipNeeded()
		{
			return m_item != null;
		}

		protected override void ShowTooltip()
		{
			TooltipManager.Instance.Show(this, m_item, null, gameObject.transform.position, m_backgroundFill.gameObject.transform.localScale * 0.5f);
		}

		protected override void HideTooltip()
		{
			TooltipManager.Instance.Hide(this);
		}

		protected override void OnClick()
		{
			if (m_item != null)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(m_item, EEventType.INVENTORY_SELECTED, EventArgs.Empty);
			}
		}
	}
}
