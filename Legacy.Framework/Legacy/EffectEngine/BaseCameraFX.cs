using System;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public abstract class BaseCameraFX : MonoBehaviour
	{
		private Transform m_CachedTransform;

		public new Transform transform
		{
			get
			{
				if (m_CachedTransform == null)
				{
					m_CachedTransform = base.transform;
				}
				return m_CachedTransform;
			}
			private set => m_CachedTransform = value;
		}

		public abstract void CancelEffect();

		protected virtual void Awake()
		{
			transform = base.transform;
		}
	}
}
