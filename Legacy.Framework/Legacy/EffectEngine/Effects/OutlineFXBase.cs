using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	[Serializable]
	public abstract class OutlineFXBase : MonoBehaviour
	{
		public abstract Boolean IsDestroyed { get; }

		public abstract Boolean IsOutlineShown { get; }

		public abstract Color OutlineColor { get; }

		public abstract void SetGlobalIntensity(Single intensity);

		public abstract void ShowOutline(Boolean doHighlight, Color color);

		public abstract void HideOutline();

		public abstract void Destroy();
	}
}
