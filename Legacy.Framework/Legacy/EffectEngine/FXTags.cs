using System;
using UnityEngine;

namespace Legacy.EffectEngine
{
	[AddComponentMenu("MM Legacy/Effects/FX Tags")]
	public class FXTags : MonoBehaviour
	{
		private GameObject[] m_DefaultCache = Arrays<GameObject>.Empty;

		[SerializeField]
		internal Tag[] m_Tags = Arrays<Tag>.Empty;

		public GameObject[] Find(String tag)
		{
			if (!String.IsNullOrEmpty(tag))
			{
				for (Int32 i = 0; i < m_Tags.Length; i++)
				{
					if (m_Tags[i].m_Tag == tag)
					{
						return m_Tags[i].m_GameObjects;
					}
				}
			}
			Debug.LogWarning("FXTags: Tag '" + tag + "' not defined!", gameObject);
			return m_DefaultCache;
		}

		public GameObject FindOne(String tag)
		{
			GameObject gameObject = Find(tag).RandomElement<GameObject>();
			return (!(gameObject == null)) ? gameObject : m_DefaultCache[0];
		}

		public Vector3 GetAveragePoint(String tag)
		{
			GameObject[] array = Find(tag);
			if (array.Length == 0)
			{
				return Vector3.zero;
			}
			Vector3 a = Vector3.zero;
			foreach (GameObject gameObject in array)
			{
				if (gameObject != null)
				{
					a += gameObject.transform.position;
				}
			}
			return a / array.Length;
		}

		private void Awake()
		{
			m_DefaultCache = new GameObject[]
			{
				gameObject
			};
		}

		[Serializable]
		internal class Tag
		{
			public String m_Tag = String.Empty;

			public GameObject[] m_GameObjects = Arrays<GameObject>.Empty;
		}
	}
}
