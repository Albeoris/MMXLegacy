using System;
using Legacy.Views;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class SpawnPrefabOnMonsterEvent : MonoBehaviour
	{
		[SerializeField]
		private GameObject SpawnOnReceivedAttacks;

		[SerializeField]
		private GameObject SpawnOnBeginEffect;

		[SerializeField]
		private GameObject SpawnOnEndEffect;

		private SpawnPrefabOnMonsterEventListener m_listener;

		public void OnReceivedAttacks(UnityEventArgs p_args)
		{
			Spawn(SpawnOnReceivedAttacks);
		}

		public void OnBeginEffect(UnityEventArgs p_args)
		{
			Spawn(SpawnOnBeginEffect);
		}

		public void OnEndEffect(UnityEventArgs p_args)
		{
			Spawn(SpawnOnEndEffect);
		}

		private void Spawn(GameObject p_prefab)
		{
			if (p_prefab != null)
			{
				GameObject gameObject = Helper.Instantiate<GameObject>(p_prefab, transform.position, transform.rotation);
				transform.AddChildAlignOrigin(gameObject.transform);
			}
		}

		private void Start()
		{
			CombatViewBase component = GetComponent<CombatViewBase>();
			Transform parent = transform.parent;
			while (parent != null && (component = parent.GetComponent<CombatViewBase>()) == null)
			{
				parent = parent.parent;
			}
			if (component != null)
			{
				m_listener = component.gameObject.AddComponent<SpawnPrefabOnMonsterEventListener>();
				m_listener.SetSpawnPrefabOnMonsterEventScript(this);
			}
			else
			{
				Debug.LogError("SpawnPrefabOnMonsterEvent: CombatViewBase not found!");
			}
		}

		private void OnDestroy()
		{
			if (m_listener != null)
			{
				Destroy(m_listener);
			}
		}
	}
}
