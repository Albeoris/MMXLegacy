using System;
using UnityEngine;

public class DarkVision : MonoBehaviour
{
	[SerializeField]
	private Int32 m_Priority;

	[SerializeField]
	private Single m_BlendTime = 0.5f;

	[SerializeField]
	private ZoneEffectSetting m_Setting;

	private void Start()
	{
		ZoneEffectController.Instance.Register(this, m_Setting, m_Priority, m_BlendTime);
	}

	private void OnDestroy()
	{
		Debug.LogWarning("Destroy");
		ZoneEffectController.Instance.Deregister(this);
	}
}
