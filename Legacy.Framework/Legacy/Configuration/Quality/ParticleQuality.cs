using System;
using UnityEngine;

namespace Legacy.Configuration.Quality
{
	[RequireComponent(typeof(ParticleSystem))]
	[AddComponentMenu("MM Legacy/Quality/ParticleQuality")]
	public class ParticleQuality : QualityConfigurationBase
	{
		protected ParticleSystem[] m_ParticleSystems;

		public QualityFloatValue SquaredDistanceToCamera = new QualityFloatValue();

		private Boolean m_Visible = true;

		public override void OnQualityConfigutationChanged()
		{
			SquaredDistanceToCamera.SetQualityValue((Int32)GraphicsConfigManager.Settings.EffectQuality);
		}

		public void Play()
		{
			if (m_ParticleSystems != null)
			{
				for (Int32 i = 0; i < m_ParticleSystems.Length; i++)
				{
					if (m_ParticleSystems[i] != null)
					{
						m_ParticleSystems[i].Play(false);
					}
				}
			}
		}

		public void Stop()
		{
			if (m_ParticleSystems != null)
			{
				for (Int32 i = 0; i < m_ParticleSystems.Length; i++)
				{
					if (m_ParticleSystems[i] != null)
					{
						m_ParticleSystems[i].Stop(false);
					}
				}
			}
		}

		protected override void Awake()
		{
			m_ParticleSystems = GetComponentsInChildren<ParticleSystem>(true);
			if (m_ParticleSystems.Length == 0)
			{
				Destroy(this);
			}
			base.Awake();
		}

		private void Update()
		{
			Camera main = Camera.main;
			if (main != null)
			{
				if (Vector3.Magnitude(transform.position - main.transform.position) > SquaredDistanceToCamera.GetQualityValue())
				{
					if (m_Visible)
					{
						Stop();
						m_Visible = false;
					}
				}
				else if (!m_Visible)
				{
					Play();
					m_Visible = true;
				}
			}
		}
	}
}
