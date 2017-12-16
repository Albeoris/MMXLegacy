using System;
using System.Text;
using System.Threading;
using Legacy.Core.Abilities;
using Legacy.Core.Buffs;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.HUD
{
	[AddComponentMenu("MM Legacy/MMGUI/MonsterBuffView")]
	public class MonsterBuffView : MonoBehaviour
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
		private GameObject m_buffIconFXOut;

		[SerializeField]
		private GameObject m_buffIconFXGreen;

		private static StringBuilder m_tempStringBuilder = new StringBuilder(10);

		private MonsterBuff m_monsterBuff;

		private MonsterAbilityBase m_ability;

		private Single m_enableTime;

		private Single m_disableTime = -1f;

		private Boolean m_isAbility;

		private Boolean m_labelEnabled;

	    public event EventHandler OnTooltipEvent;

		public UISprite Icon => m_icon;

	    public Boolean LabelEnabled => m_labelEnabled;

	    public MonsterBuff MonsterBuff => m_monsterBuff;

	    public MonsterAbilityBase Ability => m_ability;

	    public Boolean IsAbility
		{
			get => m_isAbility;
	        set => m_isAbility = value;
	    }

		public Boolean SetDestroyTime => m_disableTime > 0f;

	    public Single TimeTillDestroy => m_disableTime - Time.time;

	    public void Init(MonsterBuff p_buff, Boolean p_barIsVisible)
		{
			m_monsterBuff = p_buff;
			if (!m_isAbility && p_barIsVisible)
			{
				m_enableTime = Time.time + 0.8f;
				if (m_monsterBuff.IsDebuff)
				{
					if (m_buffIconFX != null)
					{
						GameObject gameObject = (GameObject)Instantiate(m_buffIconFX, m_icon.transform.position, new Quaternion(0f, 0f, 0f, 0f));
						if (gameObject != null)
						{
							gameObject.transform.parent = transform.parent;
							gameObject.transform.localScale = Vector3.one;
						}
					}
				}
				else if (m_buffIconFXGreen != null)
				{
					GameObject gameObject = (GameObject)Instantiate(m_buffIconFXGreen, m_icon.transform.position, new Quaternion(0f, 0f, 0f, 0f));
					if (gameObject != null)
					{
						gameObject.transform.parent = transform.parent;
						gameObject.transform.localScale = Vector3.one;
					}
				}
			}
			else
			{
				m_enableTime = Time.time;
			}
		}

		public void UpdateDurationLabel()
		{
			if (m_monsterBuff == null)
			{
				return;
			}
			if (m_monsterBuff.DurationMaxValue != -1 && ((!m_monsterBuff.IsExpired && m_monsterBuff.Duration > 0) || !m_monsterBuff.IsDebuff))
			{
				m_labelEnabled = true;
				m_label.enabled = true;
				Int32 num = m_monsterBuff.Duration;
				if (!m_monsterBuff.IsDebuff)
				{
					num = Mathf.Min(num + 1, m_monsterBuff.DurationMaxValue);
				}
				m_label.text = num.ToString();
			}
			else
			{
				m_labelEnabled = false;
				m_label.enabled = false;
			}
		}

		public void SetDisabled(Boolean p_isVisible)
		{
			m_disableTime = Time.time + 0.8f;
			NGUITools.SetActive(m_icon.gameObject, false);
			NGUITools.SetActive(m_iconFrame.gameObject, false);
			NGUITools.SetActive(m_label.gameObject, false);
			if (m_buffIconFXOut != null && p_isVisible)
			{
				GameObject gameObject = (GameObject)Instantiate(m_buffIconFXOut, m_icon.transform.position, new Quaternion(0f, 0f, 0f, 0f));
				if (gameObject != null)
				{
					gameObject.transform.parent = transform.parent;
					gameObject.transform.localScale = transform.lossyScale;
				}
				m_buffIconFXOut = null;
			}
			DisableTooltip();
		}

		public void UpdateBuff(MonsterBuff p_buff)
		{
			m_monsterBuff = p_buff;
			m_icon.spriteName = p_buff.Icon;
			m_label.enabled = false;
			if (p_buff.IsDebuff)
			{
				m_iconFrame.color = Color.red;
			}
			else
			{
				m_iconFrame.color = Color.green;
			}
		}

		public void UpdateAbility(MonsterAbilityBase p_ability)
		{
			m_ability = p_ability;
			m_icon.spriteName = p_ability.StaticData.Icon;
			m_label.enabled = false;
		}

		public Boolean IsWaitTimeDone()
		{
			return m_enableTime <= Time.time;
		}

		public Boolean ShouldBeDestroyed()
		{
			return m_disableTime <= Time.time;
		}

		public void DisableTooltip()
		{
			OnTooltip(false);
		}

		private void OnTooltip(Boolean show)
		{
			if (OnTooltipEvent != null)
			{
				EventArgs e = EventArgs.Empty;
				if (show)
				{
					m_tempStringBuilder.Length = 0;
					String text;
					String value;
					if (!m_isAbility)
					{
						text = LocaManager.GetText(m_monsterBuff.NameKey);
						value = InsertMagicValues(LocaManager.GetText(m_monsterBuff.NameKey + "_INFO"));
					}
					else
					{
						text = LocaManager.GetText(m_ability.StaticData.NameKey);
						value = m_ability.GetDescription();
					}
					m_tempStringBuilder.Append('[');
					m_tempStringBuilder.Append(NGUITools.EncodeColor(m_colorBuffName));
					m_tempStringBuilder.Append(']');
					m_tempStringBuilder.Append(text);
					m_tempStringBuilder.Append("[-]");
					m_tempStringBuilder.AppendLine();
					m_tempStringBuilder.Append('[');
					m_tempStringBuilder.Append(NGUITools.EncodeColor(m_colorBuffInfo));
					m_tempStringBuilder.Append(']');
					m_tempStringBuilder.Append(value);
					m_tempStringBuilder.Append("[-]");
					e = new StringEventArgs(m_tempStringBuilder.ToString());
				}
				OnTooltipEvent(this, e);
			}
		}

		private String InsertMagicValues(String p_text)
		{
			Single[] buffValues = m_monsterBuff.BuffValues;
			Object[] array = new Object[buffValues.Length];
			for (Int32 i = 0; i < buffValues.Length; i++)
			{
				Single buffValue = m_monsterBuff.GetBuffValue(i);
				if (m_monsterBuff.Type == EMonsterBuffType.HOUR_OF_JUSTICE)
				{
					array[i] = (buffValue * 100f).ToString();
				}
				else
				{
					array[i] = ((buffValue >= 1f) ? buffValue.ToString() : (buffValue * 100f).ToString());
				}
			}
			return String.Format(p_text, array);
		}
	}
}
