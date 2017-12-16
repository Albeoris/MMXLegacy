using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class SpawnPrefabOnMonsterEventListener : MonoBehaviour
	{
		private SpawnPrefabOnMonsterEvent m_script;

		public void SetSpawnPrefabOnMonsterEventScript(SpawnPrefabOnMonsterEvent p_script)
		{
			m_script = p_script;
		}

		public void OnReceivedAttacks(UnityEventArgs p_args)
		{
			m_script.OnReceivedAttacks(p_args);
		}

		public void OnBeginEffect(UnityEventArgs p_args)
		{
			m_script.OnBeginEffect(p_args);
		}

		public void OnEndEffect(UnityEventArgs p_args)
		{
			m_script.OnEndEffect(p_args);
		}
	}
}
