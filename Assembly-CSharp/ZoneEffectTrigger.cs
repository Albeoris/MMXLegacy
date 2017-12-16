using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneEffectTrigger : MonoBehaviour
{
	private const String TRIGGER_TAG = "Player";

	private Dictionary<Light, Single> m_LightIntensityDict;

	[SerializeField]
	private Int32 m_Priority;

	[SerializeField]
	private Single m_BlendTime = 0.5f;

	[SerializeField]
	private ZoneEffectSetting m_Setting;

	public Light[] ZoneOnlyLights;

	public Int32 Priority => m_Priority;

    public Single BlendTime => m_BlendTime;

    public ZoneEffectSetting Setting => m_Setting;

    private void Awake()
	{
		collider.isTrigger = true;
		if (ZoneOnlyLights != null && ZoneOnlyLights.Length > 0)
		{
			m_LightIntensityDict = new Dictionary<Light, Single>(ZoneOnlyLights.Length);
			foreach (Light light in ZoneOnlyLights)
			{
				if (light != null)
				{
					m_LightIntensityDict[light] = light.intensity;
					light.enabled = false;
				}
			}
		}
		if (m_Setting == null)
		{
			gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (ZoneEffectController.Instance != null)
		{
			ZoneEffectController.Instance.Deregister(this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player") || (other.attachedRigidbody != null && !other.attachedRigidbody.CompareTag("Player")))
		{
			return;
		}
		if (ZoneEffectController.Instance != null)
		{
			ZoneEffectController.Instance.Register(this, m_Setting, m_Priority, m_BlendTime);
		}
		if (m_LightIntensityDict != null && m_LightIntensityDict.Count > 0)
		{
			StartCoroutine(SwitchLights(true));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Player") || (other.attachedRigidbody != null && !other.attachedRigidbody.CompareTag("Player")))
		{
			return;
		}
		if (ZoneEffectController.Instance != null)
		{
			ZoneEffectController.Instance.Deregister(this);
		}
		if (m_LightIntensityDict != null && m_LightIntensityDict.Count > 0)
		{
			StartCoroutine(SwitchLights(false));
		}
	}

	private IEnumerator SwitchLights(Boolean p_enable)
	{
		Single lerpStart = (!p_enable) ? 1 : 0;
		Single lerpEnd = (!p_enable) ? 0 : 1;
		Single lerpBase = 0f;
		WaitForSeconds delay = new WaitForSeconds(0.025f);
		while (lerpBase <= 1f)
		{
			Single lerpValue = Mathf.Lerp(lerpStart, lerpEnd, lerpBase);
			foreach (KeyValuePair<Light, Single> light in m_LightIntensityDict)
			{
				light.Key.intensity = lerpValue * light.Value;
				light.Key.enabled = (light.Key.intensity > 0f);
			}
			yield return delay;
			lerpBase += 0.05f;
		}
		yield break;
	}
}
