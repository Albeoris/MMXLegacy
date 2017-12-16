using System;
using Legacy.EffectEngine.Effects;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class AlignPrefabToTargetPosition : MonoBehaviour
	{
		[SerializeField]
		private Vector3 m_worldOffset = Vector3.zero;

		[SerializeField]
		private Boolean m_ignoreEndEffectEvent;

		private FXArgs m_Args;

		private Boolean m_BeginEffect;

		private Boolean m_FXplayed = true;

		private void OnBeginEffect(UnityEventArgs<FXArgs> p_args)
		{
			m_Args = p_args.EventArgs;
			m_BeginEffect = true;
			m_FXplayed = false;
		}

		private void OnEndEffect(UnityEventArgs<FXArgs> p_args)
		{
			if (!m_ignoreEndEffectEvent)
			{
				m_BeginEffect = false;
				if (particleSystem != null)
				{
					particleSystem.Stop();
				}
			}
		}

		private void Rotate()
		{
			Vector3 a = m_Args.Target.transform.position + m_worldOffset;
			Vector3 position = transform.position;
			Vector3 forward = a - position;
			Quaternion rotation = Quaternion.LookRotation(forward);
			transform.rotation = rotation;
			if (!m_FXplayed)
			{
				if (particleSystem != null)
				{
					particleSystem.Play();
				}
				m_FXplayed = true;
			}
		}

		private void Update()
		{
			if (m_BeginEffect)
			{
				Rotate();
			}
		}
	}
}
