using System;
using System.Collections;
using Legacy.Animations;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class CharacterReplacementFX : MonoBehaviour
	{
		private const Single CLEAN_UP_AFTER = 5f;

		public Single LightIntensityRnd = 2f;

		public Single LightIntensityRndSpeed = 5f;

		public Single BurnParticleSpread = 4f;

		public Single BurnParticleVRnd = 4f;

		public Single BurnTime = 0.15f;

		public Single BurnTimeRnd = 0.05f;

		public Single BurnDownDelay = 0.25f;

		public Single BurnDownSpeed = 5f;

		public Single FreezeAt = 1f;

		public Single FreezeReplaceRelTime = 0.5f;

		public Single FreezeDuration = 1f;

		public Int32 MagicAnimID = 1;

		public AnimatorControl Model;

		public GameObject[] ParticlePrefabs;

		public GameObject MovingPrefab;

		private Single m_startTime;

		private State m_state;

		private Boolean m_isModelReplaced;

		private Boolean m_isMovingEffectAdded;

		private Single m_burnDownMaxY = 0.1f;

		private void Start()
		{
			if (Model == null || ParticlePrefabs == null || MovingPrefab == null)
			{
				Debug.LogError("CharacterReplacementFX: model or particles prefabs not set!");
				enabled = false;
				return;
			}
			Model.AttackMagic(MagicAnimID);
			GoToState_BEFORE_FREEZE();
		}

		private void Update()
		{
			switch (m_state)
			{
			case State.BEFORE_FREEZE:
				HandleState_BEFORE_FREEZE();
				break;
			case State.FREEZED:
				HandleState_FREEZED();
				break;
			case State.AFTER_FREEZE:
				HandleState_AFTER_FREEZE();
				break;
			}
		}

		private void GoToState_BEFORE_FREEZE()
		{
			m_state = State.BEFORE_FREEZE;
			m_startTime = Time.time;
		}

		private void HandleState_BEFORE_FREEZE()
		{
			if (Time.time - m_startTime >= FreezeAt)
			{
				GoToState_FREEZED();
			}
		}

		private void GoToState_FREEZED()
		{
			m_state = State.FREEZED;
			m_startTime = Time.time;
			FreezeAnimator(Model);
			AddParticleFX(Model);
		}

		private void HandleState_FREEZED()
		{
			if (Time.time - m_startTime >= FreezeDuration)
			{
				GoToState_AFTER_FREEZE();
			}
			if (Time.time - m_startTime >= FreezeDuration * FreezeReplaceRelTime)
			{
				ReplaceModelsIfNotYetDone();
			}
			m_burnDownMaxY -= Time.deltaTime * BurnDownSpeed;
		}

		private void GoToState_AFTER_FREEZE()
		{
			m_state = State.AFTER_FREEZE;
			UnFreezeAnimator(Model);
			ReplaceModelsIfNotYetDone();
		}

		private void HandleState_AFTER_FREEZE()
		{
			m_burnDownMaxY -= Time.deltaTime * BurnDownSpeed;
		}

		private void FreezeAnimator(AnimatorControl p_model)
		{
			Animator component = p_model.GetComponent<Animator>();
			if (component != null)
			{
				component.speed = 0.2f;
			}
		}

		private void UnFreezeAnimator(AnimatorControl p_model)
		{
			Animator component = p_model.GetComponent<Animator>();
			if (component != null)
			{
				component.speed = 1f;
			}
		}

		private void ToggleSkinRenderers(AnimatorControl p_model)
		{
			SkinnedMeshRenderer[] componentsInChildren = p_model.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				skinnedMeshRenderer.enabled = !skinnedMeshRenderer.enabled;
			}
		}

		private void AddParticleFX(AnimatorControl p_model)
		{
			SkinnedMeshRenderer[] componentsInChildren = p_model.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer p_renderer in componentsInChildren)
			{
				ParticalizeRenderer(p_renderer);
			}
		}

		private void ParticalizeRenderer(SkinnedMeshRenderer p_renderer)
		{
			Mesh mesh = new Mesh();
			p_renderer.BakeMesh(mesh);
			Destroy(mesh, 5f);
			AddParticles(p_renderer.gameObject, mesh);
		}

		private void AddParticles(GameObject p_targetGO, Mesh p_pMesh)
		{
			foreach (GameObject p_original in ParticlePrefabs)
			{
				GameObject gameObject = Helper.Instantiate<GameObject>(p_original);
				Destroy(gameObject, 5f);
				gameObject.transform.position = p_targetGO.transform.position;
				gameObject.transform.rotation = p_targetGO.transform.rotation;
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponent<MeshFilter>().sharedMesh = p_pMesh;
				StartCoroutine(BurnDownParticles((ParticleEmitter)gameObject.GetComponent("MeshParticleEmitter")));
			}
		}

		private void ReplaceModelsIfNotYetDone()
		{
			if (!m_isModelReplaced)
			{
				m_isModelReplaced = true;
				ToggleSkinRenderers(Model);
			}
		}

		private void AddMovingEffect()
		{
			if (!m_isMovingEffectAdded)
			{
				m_isMovingEffectAdded = true;
				Vector3 position = Model.transform.position;
				position.y = m_burnDownMaxY;
				GameObject gameObject = Helper.Instantiate<GameObject>(MovingPrefab, position, Model.transform.rotation);
				Destroy(gameObject, 5f);
				StartCoroutine(MoveDown(gameObject));
			}
		}

		private IEnumerator MoveDown(GameObject p_object)
		{
			WaitForEndOfFrame wait = new WaitForEndOfFrame();
			yield return wait;
			Single lightIntensity = 0f;
			if (p_object.light != null)
			{
				lightIntensity = p_object.light.intensity;
			}
			while (m_burnDownMaxY > -0.5 && p_object != null)
			{
				if (p_object.light != null)
				{
					p_object.light.intensity = lightIntensity + LightIntensityRnd * Mathf.PerlinNoise(Time.time * LightIntensityRndSpeed, Time.time);
				}
				p_object.transform.position = new Vector3(p_object.transform.position.x, m_burnDownMaxY, p_object.transform.position.z);
				yield return wait;
			}
			ParticleSystem[] pSysArray = p_object.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem pSys in pSysArray)
			{
				pSys.Stop();
				pSys.transform.parent = null;
				Destroy(pSys.gameObject, pSys.startLifetime + 0.5f);
			}
			Destroy(p_object);
			yield break;
		}

		private IEnumerator BurnDownParticles(ParticleEmitter p_particleEmmiter)
		{
			WaitForEndOfFrame wait = new WaitForEndOfFrame();
			yield return wait;
			yield return new WaitForSeconds(BurnDownDelay);
			Particle[] particles = p_particleEmmiter.particles;
			for (Int32 i = 0; i < particles.Length; i++)
			{
				if (particles[i].position.y > m_burnDownMaxY)
				{
					m_burnDownMaxY = particles[i].position.y;
				}
			}
			AddMovingEffect();
			while (m_burnDownMaxY > -3f && p_particleEmmiter != null)
			{
				particles = p_particleEmmiter.particles;
				for (Int32 j = 0; j < particles.Length; j++)
				{
					if (particles[j].energy > BurnTime + BurnTimeRnd && particles[j].position.y > m_burnDownMaxY)
					{
						particles[j].energy = BurnTime + (UnityEngine.Random.value - 0.5f) * BurnTimeRnd;
						particles[j].velocity = (UnityEngine.Random.insideUnitSphere * BurnParticleVRnd + Vector3.down * (BurnDownSpeed * 0.95f - BurnParticleVRnd)) * (1f + BurnParticleSpread * UnityEngine.Random.value);
					}
				}
				p_particleEmmiter.particles = particles;
				yield return wait;
			}
			yield break;
		}

		private enum State
		{
			BEFORE_FREEZE,
			FREEZED,
			AFTER_FREEZE
		}
	}
}
