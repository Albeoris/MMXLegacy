using System;
using System.Collections;
using UnityEngine;

namespace CutsceneTools
{
	internal class BlackDragonLandAnimation : MonoBehaviour
	{
		[SerializeField]
		private Animation m_Animation;

		[SerializeField]
		private Single m_Delay;

		[SerializeField]
		private GameObject m_LandFX;

		[SerializeField]
		private String m_IdleName;

		[SerializeField]
		private String m_LandName;

		private void Awake()
		{
			m_Animation = animation;
			m_Animation.Stop();
			ParticleSystem[] componentsInChildren = m_LandFX.GetComponentsInChildren<ParticleSystem>();
			if (componentsInChildren.Length > 0)
			{
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Stop(false);
				}
			}
		}

		private IEnumerator Start()
		{
			m_Animation[m_LandName].enabled = true;
			m_Animation[m_LandName].weight = 1f;
			m_Animation.Sample();
			m_Animation[m_LandName].enabled = false;
			m_Animation.Sample();
			yield return new WaitForSeconds(m_Delay);
			m_Animation.Play(m_LandName);
			ParticleSystem[] system = m_LandFX.GetComponentsInChildren<ParticleSystem>();
			if (system.Length > 0)
			{
				foreach (ParticleSystem item in system)
				{
					item.Play(false);
				}
			}
			yield return new WaitForSeconds(m_Animation[m_LandName].length - 0.5f);
			m_Animation.CrossFade(m_IdleName, 0.5f);
			yield break;
		}
	}
}
