using System;
using System.Text;
using System.Threading;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using UnityEngine;

namespace Legacy.HUD
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyBuffView")]
	public class PartyBuffView : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private UISprite m_iconFrame;

		[SerializeField]
		private UILabel m_label;

		[SerializeField]
		private Color m_colorBuffName = new Color(0f, 0f, 0f);

		[SerializeField]
		private Color m_colorBuffInfo = new Color(0f, 0f, 0f);

		[SerializeField]
		private GameObject m_buffIconFX;

		[SerializeField]
		private GameObject m_buffIconFXDisappear;

		private PartyBuff m_partyBuff;

		private String m_colorBuffNameHex;

		private String m_colorBuffInfoHex;

		private StringBuilder m_tempStringBuilder;

		private Single m_enableTime;

		private Single m_disableTime = -1f;

	    public event EventHandler OnTooltipEvent;

		public UISprite Icon => m_icon;

	    public PartyBuff PartyBuff => m_partyBuff;

	    public void Init()
		{
			m_colorBuffNameHex = "[" + NGUITools.EncodeColor(m_colorBuffName) + "]";
			m_colorBuffInfoHex = "[" + NGUITools.EncodeColor(m_colorBuffInfo) + "]";
			m_tempStringBuilder = new StringBuilder(10);
			m_enableTime = Time.time + 0.8f;
			if (m_buffIconFX != null)
			{
				GameObject gameObject = (GameObject)Instantiate(m_buffIconFX, m_icon.transform.position, new Quaternion(0f, 0f, 0f, 0f));
				if (gameObject != null)
				{
					gameObject.transform.parent = transform.parent;
					gameObject.transform.localScale = Vector3.one;
				}
				m_buffIconFX = null;
			}
		}

		public Boolean SetDestroyTime => m_disableTime > 0f;

	    public void SetDisabled()
		{
			m_disableTime = Time.time + 0.8f;
			NGUITools.SetActive(m_icon.gameObject, false);
			NGUITools.SetActive(m_iconFrame.gameObject, false);
			NGUITools.SetActive(m_label.gameObject, false);
			if (m_buffIconFXDisappear != null)
			{
				GameObject gameObject = (GameObject)Instantiate(m_buffIconFXDisappear, m_icon.transform.position, new Quaternion(0f, 0f, 0f, 0f));
				if (gameObject != null)
				{
					gameObject.transform.parent = transform.parent;
					gameObject.transform.localScale = transform.lossyScale;
				}
				m_buffIconFXDisappear = null;
			}
		}

		public void UpdateBuff(PartyBuff p_buff)
		{
			m_partyBuff = p_buff;
			m_icon.spriteName = p_buff.StaticData.Icon;
		}

		public Boolean IsWaitTimeDone()
		{
			return m_enableTime <= Time.time;
		}

		public void UpdateDurationLabel()
		{
			if (m_partyBuff == null)
			{
				return;
			}
			if (!m_partyBuff.IsExpired())
			{
				m_label.enabled = true;
				if (m_partyBuff.DurationIsMinutes)
				{
					MMTime mmtime = m_partyBuff.ExpireTime - LegacyLogic.Instance.GameTime.Time;
					m_label.text = LocaManager.GetText("TIMESTRING", mmtime.Hours.ToString("D2"), mmtime.Minutes.ToString("D2"));
				}
				else if (m_partyBuff.Infinite)
				{
					m_label.enabled = false;
				}
				else
				{
					m_label.text = m_partyBuff.ExpireTimeTurns.ToString();
				}
			}
			else
			{
				OnTooltip(false);
			}
		}

		public Boolean ShouldBeDestroyed()
		{
			return m_disableTime <= Time.time;
		}

		private void OnClick()
		{
			LegacyLogic.Instance.WorldManager.Party.Buffs.RequestBuffCancel(m_partyBuff);
		}

		private void OnTooltip(Boolean show)
		{
			if (OnTooltipEvent != null)
			{
				m_tempStringBuilder.Length = 0;
				EventArgs e;
				if (!show || SetDestroyTime)
				{
					e = EventArgs.Empty;
				}
				else
				{
					m_tempStringBuilder.Append(m_colorBuffNameHex);
					m_tempStringBuilder.Append(LocaManager.GetText(m_partyBuff.StaticData.Name));
					m_tempStringBuilder.Append("[-]");
					String p_caption = m_tempStringBuilder.ToString();
					m_tempStringBuilder.Length = 0;
					m_tempStringBuilder.Append(m_colorBuffInfoHex);
					m_tempStringBuilder.Append(m_partyBuff.Description);
					m_tempStringBuilder.Append("[-]");
					e = new StringEventArgs(m_tempStringBuilder.ToString(), p_caption);
				}
				OnTooltipEvent(this, e);
			}
		}
	}
}
