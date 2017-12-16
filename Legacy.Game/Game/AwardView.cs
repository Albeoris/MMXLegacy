using System;
using Legacy.Core;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/AwardView")]
	public class AwardView : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_icon;

		private TokenStaticData m_data;

		private Boolean m_acquired;

		private String m_tt;

		private String m_tt_caption;

		public void Init(Party p_party, Int32 p_tokenID)
		{
			m_data = StaticDataHandler.GetStaticData<TokenStaticData>(EDataType.TOKEN, p_tokenID);
			m_acquired = (p_party.TokenHandler.GetTokens(p_tokenID) > 0);
			m_icon.spriteName = m_data.Icon;
			m_tt_caption = LocaManager.GetText(m_data.Name);
			if (m_acquired)
			{
				m_tt = p_party.TokenHandler.GetTokenDescription(p_tokenID);
				m_icon.color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				m_tt = LocaManager.GetText("TOKEN_NOT_ACQUIRED");
				m_icon.color = new Color(0.2f, 0.2f, 0.2f, 1f);
			}
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public void OnTooltip(Boolean isOver)
		{
			if (m_data == null)
			{
				return;
			}
			if (isOver)
			{
				Vector3 position = gameObject.transform.position;
				Vector3 p_offset = m_icon.gameObject.transform.localScale * 0.5f;
				TooltipManager.Instance.Show(this, m_tt_caption, m_tt, TextTooltip.ESize.BIG, position, p_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
