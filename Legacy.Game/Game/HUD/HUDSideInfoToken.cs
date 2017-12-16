using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDSideInfoToken")]
	public class HUDSideInfoToken : HUDSideInfoBase
	{
		[SerializeField]
		private Color m_textColor = Color.white;

		private TokenStaticData m_staticData;

		private Boolean m_isTokenUsable;

		public override Boolean IsAlignedToBottom => true;

	    public void Init(TokenStaticData p_staticData, Boolean p_isTokenUsable)
		{
			m_staticData = p_staticData;
			m_isTokenUsable = p_isTokenUsable;
			base.Init(LocaManager.GetText(m_staticData.Name), m_staticData.Icon, 0, m_textColor);
			if (m_isTokenUsable)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_REMOVED, new EventHandler(OnTokenRemoved));
			}
		}

		public override void UpdateEntry()
		{
			if (m_isTokenUsable && m_insertTime + LOOT_SHOW_TIME - 0.01 < Time.time)
			{
				m_insertTime = Time.time - LOOT_SHOW_TIME + 0.01f;
			}
			base.UpdateEntry();
		}

		public void HideToken()
		{
			m_isTokenUsable = false;
		}

		protected override Boolean IsTooltipNeeded()
		{
			return m_staticData != null;
		}

		protected override void ShowTooltip()
		{
			if (m_staticData.Description != String.Empty)
			{
				TooltipManager.Instance.Show(this, LocaManager.GetText(m_staticData.Name) + "\n\n" + LocaManager.GetText(m_staticData.Description), gameObject.transform.position, m_backgroundFill.gameObject.transform.localScale * 0.5f);
			}
			else
			{
				TooltipManager.Instance.Show(this, LocaManager.GetText(m_staticData.Name), gameObject.transform.position, m_backgroundFill.gameObject.transform.localScale * 0.5f);
			}
		}

		protected override void HideTooltip()
		{
			TooltipManager.Instance.Hide(this);
		}

		protected override void OnClick()
		{
			if (m_staticData != null)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(m_staticData, EEventType.QUESTLOG_SELECTED, EventArgs.Empty);
			}
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_REMOVED, new EventHandler(OnTokenRemoved));
		}

		private void OnTokenRemoved(Object p_sender, EventArgs p_args)
		{
			if (p_args is TokenEventArgs)
			{
				TokenEventArgs tokenEventArgs = (TokenEventArgs)p_args;
				if (m_staticData.StaticID == tokenEventArgs.TokenID)
				{
					m_isTokenUsable = false;
					m_insertTime = Time.time - LOOT_SHOW_TIME;
				}
			}
		}
	}
}
