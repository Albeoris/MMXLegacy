using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/FX DisableGameObject")]
	public class FXDisableGameObject : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] Objects;

		public void OnEndEffect()
		{
			for (Int32 i = 0; i < Objects.Length; i++)
			{
				Objects[i].SetActive(false);
			}
		}
	}
}
