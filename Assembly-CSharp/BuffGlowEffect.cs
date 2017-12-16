using System;
using Legacy.EffectEngine.Effects;
using UnityEngine;

public class BuffGlowEffect : MonoBehaviour
{
	public String material = String.Empty;

	private MonsterBuffGlowFX m_MonsterBuffGlow;

	private GameObject target;

	private void Start()
	{
		target = gameObject;
		getTarget();
	}

	private void getTarget()
	{
		if (target.transform.parent.gameObject != transform.root.gameObject)
		{
			target = target.transform.parent.gameObject;
			getTarget();
		}
		else
		{
			m_MonsterBuffGlow = target.AddComponent<MonsterBuffGlowFX>();
			m_MonsterBuffGlow.Materialpath = "FXMaterials/" + material;
			enabled = true;
			m_MonsterBuffGlow.ShowOutline();
		}
	}

	private void OnDestroy()
	{
		if (m_MonsterBuffGlow != null)
		{
			m_MonsterBuffGlow.HideOutline();
			Destroy(m_MonsterBuffGlow);
		}
	}
}
