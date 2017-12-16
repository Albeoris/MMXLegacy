using System;
using System.Collections;
using Legacy.Views;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class CelestialArmorMonsterBuffFX : MonoBehaviour, MonsterEventListener.EventListener
	{
		[SerializeField]
		private GameObject m_hitParticeFX;

		private WaitForSeconds WAIT1 = new WaitForSeconds(0.15f);

		private WaitForSeconds WAIT2 = new WaitForSeconds(0.1f);

		private WaitForSeconds WAIT3 = new WaitForSeconds(0.05f);

		private MonsterEventListener m_listener;

		private MonsterBuffGlowFX m_MonsterBuffGlow;

		private GameObject target;

		public String material = String.Empty;

		private GameObject[] m_hitSpots;

		private void Start()
		{
			target = gameObject;
			getTarget();
			StartCoroutine(HitAnim());
			CombatViewBase component = GetComponent<CombatViewBase>();
			Transform parent = transform.parent;
			while (parent != null && (component = parent.GetComponent<CombatViewBase>()) == null)
			{
				parent = parent.parent;
			}
			if (component != null)
			{
				m_listener = component.gameObject.AddComponent<MonsterEventListener>();
				m_listener.SetEventListener(this);
				FXTags component2 = component.GetComponent<FXTags>();
				if (component2 != null)
				{
					m_hitSpots = component2.Find("HitSpot");
					if (m_hitSpots == null || m_hitSpots.Length == 0)
					{
						m_hitSpots = new GameObject[]
						{
							gameObject
						};
					}
				}
			}
			else
			{
				Debug.LogError("CelestialArmorMonsterBuffFX: CombatViewBase not found!");
			}
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
			if (m_listener != null)
			{
				Destroy(m_listener);
			}
			m_MonsterBuffGlow.HideOutline();
			Destroy(m_MonsterBuffGlow);
		}

		public void OnReceivedAttacks(UnityEventArgs p_args)
		{
			if (m_MonsterBuffGlow != null)
			{
				if (m_hitParticeFX != null)
				{
					foreach (GameObject gameObject in m_hitSpots)
					{
						Helper.Instantiate<GameObject>(m_hitParticeFX, gameObject.transform.position, gameObject.transform.rotation).transform.parent = gameObject.transform;
					}
				}
				StopAllCoroutines();
				StartCoroutine(HitAnim());
			}
		}

		public void OnBeginEffect(UnityEventArgs p_args)
		{
		}

		public void OnEndEffect(UnityEventArgs p_args)
		{
		}

		private IEnumerator HitAnim()
		{
			for (Int32 i = 0; i < 8; i++)
			{
				if (m_MonsterBuffGlow != null)
				{
					Renderer[] renders = m_MonsterBuffGlow.GetRenderers();
					if (renders != null)
					{
						foreach (Renderer r in renders)
						{
							if (r != null)
							{
								r.sharedMaterials[r.sharedMaterials.Length - 1].SetFloat("_AllPower", 7f / (2f * (i + 1)));
							}
						}
						if (i == 0)
						{
							yield return WAIT1;
						}
						else if (i == 1)
						{
							yield return WAIT2;
						}
						else
						{
							yield return WAIT3;
						}
					}
				}
			}
			yield break;
		}
	}
}
