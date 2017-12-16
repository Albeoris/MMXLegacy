using System;
using System.Collections.Generic;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.HUD;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.HUD
{
	[AddComponentMenu("MM Legacy/HUD/HUDPartyBuffs")]
	public class HUDPartyBuffs : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_buffViewPrefab;

		[SerializeField]
		private GameObject m_buffAnchor;

		private List<PartyBuffView> m_buffViews;

		private List<PartyBuff> m_partyBuffs;

		public void Init()
		{
			m_buffViews = new List<PartyBuffView>();
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		public void UpdateBuffs(List<PartyBuff> p_buffs)
		{
			m_partyBuffs = p_buffs;
			if (m_buffAnchor != null)
			{
				HelperBuffs.UpdatePartyBuffs(m_buffAnchor.gameObject, p_buffs, m_buffViews, m_buffViewPrefab, new EventHandler(OnTooltip));
			}
		}

		private void Update()
		{
			if (m_buffViews.Count > 0)
			{
				UpdateBuffs(m_partyBuffs);
			}
		}

		public void Clear()
		{
			if (m_buffViews != null)
			{
				for (Int32 i = 0; i < m_buffViews.Count; i++)
				{
					Destroy(m_buffViews[i].gameObject);
				}
				m_buffViews.Clear();
			}
		}

		private void OnTooltip(Boolean p_isOver)
		{
			OnTooltip(null, EventArgs.Empty);
		}

		private void OnTooltip(Object p_sender, EventArgs p_args)
		{
			PartyBuffView partyBuffView = p_sender as PartyBuffView;
			if (partyBuffView != null)
			{
				StringEventArgs stringEventArgs = p_args as StringEventArgs;
				if (stringEventArgs != null)
				{
					if (partyBuffView.PartyBuff != null)
					{
						if (stringEventArgs.caption != String.Empty)
						{
							TooltipManager.Instance.Show(this, stringEventArgs.caption, stringEventArgs.text, TextTooltip.ESize.BIG, partyBuffView.gameObject.transform.position, partyBuffView.Icon.transform.localScale * 0.5f);
						}
						else
						{
							TooltipManager.Instance.Show(this, stringEventArgs.text, partyBuffView.gameObject.transform.position, partyBuffView.Icon.transform.localScale * 0.5f);
						}
					}
				}
				else
				{
					TooltipManager.Instance.Hide(this);
				}
			}
		}
	}
}
