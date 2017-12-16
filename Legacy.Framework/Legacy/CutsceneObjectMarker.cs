using System;
using UnityEngine;

namespace Legacy
{
	public class CutsceneObjectMarker : MonoBehaviour
	{
		public GameObject[] CutsceneObjects;

		public void ActivateObjects()
		{
			foreach (GameObject gameObject in CutsceneObjects)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
			}
		}

		public void DeactivateObjects()
		{
			foreach (GameObject gameObject in CutsceneObjects)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
		}
	}
}
