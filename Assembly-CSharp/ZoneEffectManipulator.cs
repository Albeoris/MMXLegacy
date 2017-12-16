using System;
using Legacy;
using UnityEngine;

public class ZoneEffectManipulator : MonoBehaviour
{
	[SerializeField]
	private Single m_vignetting_intensity = -1f;

	[SerializeField]
	private Single m_vignetting_blur = -1f;

	[SerializeField]
	private Single m_bloom_intensity = -1f;

	[SerializeField]
	private Single m_bloom_threshhold = -1f;

	[SerializeField]
	private Color m_bloom_threshholdColor = Color.clear;

	[SerializeField]
	private Int32 m_priority = 999;

	[SerializeField]
	private Single m_blendTime = 1f;

	[SerializeField]
	private Single m_delay;

	[SerializeField]
	private Single m_duration = 1f;

	private ZoneEffectController m_ctrl;

	private ZoneEffectSetting m_setting;

	private Single m_endTime;

	private Single m_applyTime;

	private void Start()
	{
		m_ctrl = ZoneEffectController.Instance;
		if (m_ctrl == null)
		{
			Debug.LogError("ZoneEffectManipulator: ZoneEffectController does not exist!");
			Destroy(this);
			return;
		}
		m_setting = Helper.Instantiate<ZoneEffectSetting>(m_ctrl.CurrentSetting);
		ZoneEffectSetting.ParameterSet[] array = new ZoneEffectSetting.ParameterSet[]
		{
			m_setting.Dawn,
			m_setting.Day,
			m_setting.Dusk,
			m_setting.Night
		};
		for (Int32 i = 0; i < array.Length; i++)
		{
			if (m_vignetting_intensity != -1f)
			{
				array[i].VignettingParameter.intensity = m_vignetting_intensity;
			}
			if (m_vignetting_blur != -1f)
			{
				array[i].VignettingParameter.blur = m_vignetting_blur;
			}
			if (m_bloom_intensity != -1f)
			{
				array[i].BloomParameter.bloomIntensity = m_bloom_intensity;
			}
			if (m_bloom_threshhold != -1f)
			{
				array[i].BloomParameter.bloomThreshhold = m_bloom_threshhold;
			}
			if (m_bloom_threshholdColor != Color.clear)
			{
				array[i].BloomParameter.bloomThreshholdColor = m_bloom_threshholdColor;
			}
		}
		if (m_delay == 0f)
		{
			Apply();
		}
		else
		{
			m_applyTime = Time.time + m_delay;
		}
	}

	private void Update()
	{
		if (m_ctrl != null)
		{
			if (m_applyTime > 0f)
			{
				if (m_applyTime <= Time.time)
				{
					Apply();
				}
			}
			else if (m_endTime <= Time.time)
			{
				m_applyTime = -2f;
				m_ctrl.Deregister(this);
				Destroy(this);
			}
		}
		else
		{
			Debug.LogError("ZoneEffectManipulator: ZoneEffectController is gone!");
			Destroy(this);
		}
	}

	private void OnDestroy()
	{
		if (m_applyTime == -1f)
		{
			m_ctrl.Deregister(this);
		}
	}

	private void Apply()
	{
		m_applyTime = -1f;
		m_ctrl.Register(this, m_setting, m_priority, m_blendTime);
		m_endTime = Time.time + m_duration;
	}
}
