using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	public abstract class SkySatellite : MonoBehaviour
	{
		private Transform m_CachedTransform;

		[SerializeField]
		protected Light m_LightSource;

		[SerializeField]
		protected Renderer m_RendererSource;

		public virtual Color color
		{
			get => (!(m_LightSource != null)) ? Color.clear : m_LightSource.color;
		    set
			{
				if (m_LightSource != null)
				{
					m_LightSource.color = value;
				}
			}
		}

		public virtual Single intensity
		{
			get => (!(m_LightSource != null)) ? 0f : m_LightSource.intensity;
		    set
			{
				if (m_LightSource != null)
				{
					m_LightSource.intensity = value;
				}
			}
		}

		public new virtual Renderer renderer => m_RendererSource;

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

		protected virtual void Awake()
		{
			transform = base.transform;
			if (m_LightSource == null)
			{
				m_LightSource = light;
			}
			if (m_RendererSource == null)
			{
				m_RendererSource = renderer;
			}
		}

		protected virtual void OnEnable()
		{
			if (m_LightSource != null)
			{
				m_LightSource.enabled = true;
			}
			if (m_RendererSource != null)
			{
				m_RendererSource.enabled = true;
			}
		}

		protected virtual void OnDisable()
		{
			if (m_LightSource != null)
			{
				m_LightSource.enabled = false;
			}
			if (m_RendererSource != null)
			{
				m_RendererSource.enabled = false;
			}
		}
	}
}
