using System;
using Legacy.EffectEngine.Effects;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/FX StopAutoDestructParticleSystem")]
	public class FXStopAutoDestructParticleSystem : AutoDestructParticleSystem
	{
		private void OnEndEffect(UnityEventArgs<FXArgs> e)
		{
			for (Int32 i = 0; i < m_ParticleSystems.Length; i++)
			{
				if (m_ParticleSystems[i] != null)
				{
					m_ParticleSystems[i].Stop();
				}
			}
		}
	}
}
