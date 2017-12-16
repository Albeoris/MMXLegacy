using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Animations;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class IcePrisonFX : FXBase
	{
		private const Single DELAY = 0.5f;

		private const Single ICE_EXTRUDE_PROBABILITY = 0.04f;

		private const Single ICE_EXTRUDE_MAX_AMOUNT = 0.2f;

		private const Int32 ICE_PERFORMANCE = 350000;

		[SerializeField]
		private GameObject m_spawner;

		[SerializeField]
		private GameObject m_meshParticleSys;

		[SerializeField]
		private GameObject m_meshParticleSysFadeIn;

		[SerializeField]
		private GameObject m_meshParticleSysFadeOut;

		private List<SkinnedMeshRenderer> m_smrRenderes = new List<SkinnedMeshRenderer>();

		private List<MeshRenderer> m_meshRenderes = new List<MeshRenderer>();

		private List<GameObject> m_smrFakeGOs = new List<GameObject>();

		private List<GameObject> m_meshFakeGOs = new List<GameObject>();

		private List<Mesh> m_meshInstances = new List<Mesh>();

		private GameObject m_target;

		private Single m_freezeTime = -1f;

		public override Boolean IsFinished => false;

	    public override void Begin(Single p_lifetime, FXArgs p_args)
		{
			base.Begin(p_lifetime, p_args);
			m_freezeTime = Time.time + 0.5f;
			m_target = p_args.Target;
			if (m_spawner != null)
			{
				m_spawner = Helper.Instantiate<GameObject>(m_spawner, m_target.transform.position);
			}
		}

		public override void Update()
		{
			base.Update();
			if (m_freezeTime != -1f && m_freezeTime < Time.time && m_target != null)
			{
				m_freezeTime = -1f;
				m_smrRenderes.AddRange(m_target.GetComponentsInChildren<SkinnedMeshRenderer>());
				for (Int32 i = m_smrRenderes.Count - 1; i >= 0; i--)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = m_smrRenderes[i];
					if (skinnedMeshRenderer.enabled)
					{
						FreezeRenderer(skinnedMeshRenderer);
						skinnedMeshRenderer.enabled = false;
					}
					else
					{
						m_smrRenderes.RemoveAt(i);
					}
				}
				m_meshRenderes.AddRange(m_target.GetComponentsInChildren<MeshRenderer>());
				for (Int32 j = m_meshRenderes.Count - 1; j >= 0; j--)
				{
					MeshRenderer meshRenderer = m_meshRenderes[j];
					if (meshRenderer.enabled)
					{
						FreezeRenderer(meshRenderer);
						meshRenderer.enabled = false;
					}
					else
					{
						m_meshRenderes.RemoveAt(j);
					}
				}
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			UnmuteSound();
			if (m_spawner != null)
			{
				UnityEngine.Object.Destroy(m_spawner, 5f);
				Animation componentInChildren = m_spawner.GetComponentInChildren<Animation>();
				componentInChildren.Play("_disappearPrison");
				ParticleSystem[] componentsInChildren = m_spawner.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Stop();
				}
			}
			foreach (GameObject gameObject in m_smrFakeGOs)
			{
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject.GetComponent<MeshFilter>().sharedMesh);
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			m_smrFakeGOs.Clear();
			foreach (GameObject gameObject2 in m_meshFakeGOs)
			{
				if (gameObject2 != null)
				{
					UnityEngine.Object.Destroy(gameObject2);
				}
			}
			m_meshFakeGOs.Clear();
			foreach (Mesh mesh in m_meshInstances)
			{
				if (mesh != null)
				{
					UnityEngine.Object.Destroy(mesh);
				}
			}
			m_meshInstances.Clear();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in m_smrRenderes)
			{
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
			foreach (MeshRenderer meshRenderer in m_meshRenderes)
			{
				if (meshRenderer != null)
				{
					meshRenderer.enabled = true;
				}
			}
			if (m_target != null)
			{
				GameObject gameObject3 = Helper.Instantiate<GameObject>(m_meshParticleSysFadeOut);
				gameObject3.transform.position = m_target.transform.position;
				m_target = null;
			}
		}

		private void FreezeRenderer(SkinnedMeshRenderer p_renderer)
		{
			MuteSound();
			GameObject gameObject = CreateIceGO(p_renderer, m_smrFakeGOs, true);
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			p_renderer.BakeMesh(component.sharedMesh);
			CreateFrozenGO(p_renderer, m_smrFakeGOs, component.sharedMesh);
			DelayedEventManagerWorker delayedEventManagerWorker = gameObject.AddComponent<DelayedEventManagerWorker>();
			delayedEventManagerWorker.StartCoroutine(FixIceNormals(component));
			AddParticles(gameObject, component, m_meshParticleSysFadeIn);
			AddParticles(gameObject, component, m_meshParticleSys);
		}

		private void FreezeRenderer(MeshRenderer p_renderer)
		{
			MuteSound();
			Mesh sharedMesh = p_renderer.GetComponent<MeshFilter>().sharedMesh;
			GameObject gameObject = CreateIceGO(p_renderer, m_meshFakeGOs, sharedMesh.isReadable);
			gameObject.transform.localScale = p_renderer.transform.lossyScale;
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			component.sharedMesh = sharedMesh;
			GameObject gameObject2 = CreateFrozenGO(p_renderer, m_meshFakeGOs, component.sharedMesh);
			gameObject2.transform.localScale = p_renderer.transform.lossyScale;
			DelayedEventManagerWorker delayedEventManagerWorker = gameObject.AddComponent<DelayedEventManagerWorker>();
			delayedEventManagerWorker.StartCoroutine(FixIceNormals(component));
			AddParticles(gameObject, component, m_meshParticleSysFadeIn);
			AddParticles(gameObject, component, m_meshParticleSys);
		}

		private static GameObject CreateIceGO(Renderer p_renderer, List<GameObject> p_referenceList, Boolean p_isMeshReadable)
		{
			GameObject gameObject = new GameObject(p_renderer.name + "_ice");
			p_referenceList.Add(gameObject);
			gameObject.transform.position = p_renderer.transform.position;
			gameObject.transform.rotation = p_renderer.transform.rotation;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = new Mesh();
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			Material[] array = new Material[p_renderer.sharedMaterials.Length];
			Material material;
			if (p_isMeshReadable)
			{
				material = Helper.ResourcesLoad<Material>("FXMaterials/IcePrisonFrozenMaterial");
			}
			else
			{
				material = Helper.ResourcesLoad<Material>("FXMaterials/IcePrisonFrozenMaterialNotReadableMesh");
			}
			for (Int32 i = 0; i < array.Length; i++)
			{
				array[i] = material;
			}
			meshRenderer.materials = array;
			return gameObject;
		}

		private static GameObject AddParticles(GameObject p_iceGO, MeshFilter p_iceMeshFilter, GameObject p_pSysPrefab)
		{
			GameObject gameObject = Helper.Instantiate<GameObject>(p_pSysPrefab);
			p_iceGO.transform.AddChildAlignOrigin(gameObject.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponent<MeshFilter>().sharedMesh = p_iceMeshFilter.sharedMesh;
			return gameObject;
		}

		private static GameObject CreateFrozenGO(Renderer p_renderer, List<GameObject> p_referenceList, Mesh p_mesh)
		{
			GameObject gameObject = new GameObject(p_renderer.name + "_frozen");
			p_referenceList.Add(gameObject);
			gameObject.transform.position = p_renderer.transform.position;
			gameObject.transform.rotation = p_renderer.transform.rotation;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = p_mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterials = p_renderer.sharedMaterials;
			return gameObject;
		}

		private IEnumerator FixIceNormals(MeshFilter p_filter)
		{
			Int32 waitCount = 350000 / p_filter.sharedMesh.vertexCount;
			if (waitCount > 35 && p_filter.sharedMesh.isReadable)
			{
				Vector3[] verts = p_filter.mesh.vertices;
				Vector3[] normals = p_filter.mesh.normals;
				m_meshInstances.Add(p_filter.mesh);
				Vector3 extrudeOffset = Vector3.zero;
				Int32 waitAtVert = waitCount;
				for (Int32 i = 0; i < verts.Length; i++)
				{
					if (i >= waitAtVert)
					{
						waitAtVert += waitCount;
						yield return new WaitForEndOfFrame();
						if (p_filter == null || p_filter.gameObject == null)
						{
							break;
						}
					}
					Boolean isExtruded = Random.Value <= 0.04f;
					if (isExtruded)
					{
						extrudeOffset = normals[i] * Random.Value * 0.2f;
						verts[i] += extrudeOffset;
					}
					for (Int32 j = 0; j < verts.Length; j++)
					{
						if (i != j && verts[i] == verts[j])
						{
							normals[i] = (normals[i] + normals[j]) / 2f;
							normals[j] = (normals[i] + normals[j]) / 2f;
							if (isExtruded)
							{
								verts[j] += extrudeOffset;
							}
						}
					}
				}
				if (p_filter != null && p_filter.gameObject != null)
				{
					p_filter.mesh.normals = normals;
					p_filter.mesh.vertices = verts;
				}
			}
			yield break;
		}

		private void MuteSound()
		{
			if (m_target != null)
			{
				AnimatorSoundEffects componentInChildren = m_target.GetComponentInChildren<AnimatorSoundEffects>();
				if (componentInChildren != null)
				{
					componentInChildren.IsMuted = true;
				}
				else
				{
					Debug.LogError("IcePrisonFX: MuteSound: AnimatorSoundEffects not found!");
				}
			}
		}

		private void UnmuteSound()
		{
			if (m_target != null)
			{
				AnimatorSoundEffects componentInChildren = m_target.GetComponentInChildren<AnimatorSoundEffects>();
				if (componentInChildren != null)
				{
					componentInChildren.IsMuted = false;
				}
				else
				{
					Debug.LogError("IcePrisonFX: UnmuteSound: AnimatorSoundEffects not found!");
				}
			}
		}
	}
}
