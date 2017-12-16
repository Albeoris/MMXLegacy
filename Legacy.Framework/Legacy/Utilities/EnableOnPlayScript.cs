using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/EnableOnPlayScript")]
	public class EnableOnPlayScript : MonoBehaviour
	{
		public GameObject[] EnableTargets;

		public GameObject[] DisableTargets;

		private void Start()
		{
			foreach (GameObject gameObject in EnableTargets)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
			}
			foreach (GameObject gameObject2 in DisableTargets)
			{
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
		}
	}
}
