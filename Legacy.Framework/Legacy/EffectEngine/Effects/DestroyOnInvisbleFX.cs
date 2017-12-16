using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class DestroyOnInvisbleFX : MonoBehaviour
	{
		private void OnBecameInvisible()
		{
			if (gameObject.activeInHierarchy)
			{
				Destroy(gameObject);
			}
		}
	}
}
