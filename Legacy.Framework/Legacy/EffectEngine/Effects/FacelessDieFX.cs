using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class FacelessDieFX : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_pSysPrefab;

		[SerializeField]
		private GameObject m_pSysPrefab2;

		[SerializeField]
		private GameObject m_pSysPrefab3;

		public Single m_StartTime;

		private GameObject m_target;

		private Single m_TargetTime;

		private Boolean m_Done;

		private void SetMonster(UnityEventArgs p_args)
		{
			m_target = (GameObject)p_args.Sender;
		}

		private void Start()
		{
			m_TargetTime = Time.time + m_StartTime;
		}

		private void Update()
		{
			if (!m_Done && Time.time >= m_TargetTime)
			{
				SkinnedMeshRenderer[] componentsInChildren = m_target.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer p_renderer in componentsInChildren)
				{
					DieRenderer(p_renderer);
				}
				m_Done = true;
			}
		}

		private void DieRenderer(SkinnedMeshRenderer p_renderer)
		{
			Mesh mesh = new Mesh();
			p_renderer.BakeMesh(mesh);
			AddParticles(p_renderer.gameObject, mesh);
		}

		private void AddParticles(GameObject p_targetGO, Mesh p_pMesh)
		{
			GameObject gameObject = Helper.Instantiate<GameObject>(m_pSysPrefab);
			gameObject.transform.position = p_targetGO.transform.position;
			gameObject.transform.rotation = p_targetGO.transform.rotation;
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponent<MeshFilter>().sharedMesh = p_pMesh;
			GameObject gameObject2 = Helper.Instantiate<GameObject>(m_pSysPrefab2);
			gameObject2.transform.position = p_targetGO.transform.position;
			gameObject2.transform.rotation = p_targetGO.transform.rotation;
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.GetComponent<MeshFilter>().sharedMesh = p_pMesh;
			GameObject gameObject3 = Helper.Instantiate<GameObject>(m_pSysPrefab3);
			gameObject3.transform.position = p_targetGO.transform.position;
			gameObject3.transform.rotation = p_targetGO.transform.rotation;
			gameObject3.transform.localScale = Vector3.one;
			gameObject3.GetComponent<MeshFilter>().sharedMesh = p_pMesh;
		}
	}
}
