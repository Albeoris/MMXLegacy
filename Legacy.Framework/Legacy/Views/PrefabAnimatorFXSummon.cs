using System;
using Legacy.Animations;
using Legacy.Core.Entities;
using UnityEngine;

namespace Legacy.Views
{
	public class PrefabAnimatorFXSummon : BaseView
	{
		[SerializeField]
		private GameObject[] m_effect;

		[SerializeField]
		private Vector3[] m_spawnEffectOffset;

		private Animator m_Animator;

		private AnimatorControl m_AnimatorControl;

		private Int32 m_EventHash;

		private Boolean m_InSummon;

		public Boolean m_SummonEffects = true;

		protected override void Awake()
		{
			base.Awake();
			m_EventHash = Animator.StringToHash("EVENT");
			m_AnimatorControl = GetComponentInChildren<AnimatorControl>();
			m_Animator = GetComponentInChildren<Animator>();
		}

		private void Start()
		{
			SpawnObject();
		}

		public void SpawnObject()
		{
			Monster monster = (Monster)MyController;
			Int32 spawnAnim = monster.SpawnAnim;
			if (spawnAnim > 0)
			{
				Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.enabled = false;
				}
				m_AnimatorControl.EventSummon(spawnAnim);
				if (m_SummonEffects)
				{
					GameObject gameObject = m_effect[spawnAnim - 1];
					Vector3 b = Matrix4x4.TRS(Vector3.zero, transform.rotation, transform.localScale).MultiplyVector(m_spawnEffectOffset[spawnAnim - 1]);
					transform.localPosition = transform.localPosition + b;
					gameObject = Helper.Instantiate<GameObject>(gameObject, transform.position);
					gameObject.transform.rotation = transform.rotation;
					if (gameObject.particleSystem != null)
					{
						gameObject.particleSystem.Play();
					}
				}
				else
				{
					Debug.Log("No Effect attached");
				}
			}
		}

		private void Update()
		{
			if (!m_InSummon)
			{
				Int32 tagHash = m_Animator.GetCurrentAnimatorStateInfo(0).tagHash;
				tagHash = m_Animator.GetCurrentAnimatorStateInfo(0).tagHash;
				if (tagHash == m_EventHash)
				{
					Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
					foreach (Renderer renderer in componentsInChildren)
					{
						renderer.enabled = true;
					}
					m_InSummon = true;
				}
			}
		}
	}
}
