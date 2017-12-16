using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class MonsterEventListener : MonoBehaviour
	{
		private EventListener m_script;

		public void SetEventListener(EventListener p_script)
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

		public interface EventListener
		{
			void OnReceivedAttacks(UnityEventArgs p_args);

			void OnBeginEffect(UnityEventArgs p_args);

			void OnEndEffect(UnityEventArgs p_args);
		}
	}
}
