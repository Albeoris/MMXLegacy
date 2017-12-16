using System;
using UnityEngine;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/Prefab Anim Based Spawn View")]
	public class PrefabAnimBasedSpawnView : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_spawnObject;

		public void SpawnObject()
		{
			if (m_spawnObject != null)
			{
				GameObject gameObject = (GameObject)Instantiate(m_spawnObject, transform.position, transform.rotation);
				gameObject.transform.parent = transform;
			}
		}
	}
}
