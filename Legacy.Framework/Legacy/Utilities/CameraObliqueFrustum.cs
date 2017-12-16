using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Camera Oblique Frustum")]
	[RequireComponent(typeof(Camera))]
	public class CameraObliqueFrustum : MonoBehaviour
	{
		private Single m_oldHorizontalOblique;

		private Single m_oldVerticalOblique;

		private Single m_oldScreenWidth;

		private Single m_oldScreenHeight;

		[SerializeField]
		private Single m_horizontalOblique;

		[SerializeField]
		private Single m_verticalOblique;

		public Single HorizontalObliqu
		{
			get => m_horizontalOblique;
		    set
			{
				if (m_horizontalOblique != value)
				{
					m_horizontalOblique = value;
					m_oldHorizontalOblique = value;
					SetObliqueness(m_horizontalOblique, m_verticalOblique);
				}
			}
		}

		public Single VerticalOblique
		{
			get => m_verticalOblique;
		    set
			{
				if (m_verticalOblique != value)
				{
					m_verticalOblique = value;
					m_oldVerticalOblique = value;
					SetObliqueness(m_horizontalOblique, m_verticalOblique);
				}
			}
		}

		private void Awake()
		{
			ResetAndSetObliqueness();
			m_oldScreenWidth = Screen.width;
			m_oldScreenHeight = Screen.height;
		}

		private void OnDestroy()
		{
			if (camera != null)
			{
				camera.ResetProjectionMatrix();
			}
		}

		private void Update()
		{
			if (m_oldVerticalOblique != m_verticalOblique || m_oldHorizontalOblique != m_horizontalOblique)
			{
				m_oldHorizontalOblique = m_horizontalOblique;
				m_oldVerticalOblique = m_verticalOblique;
				SetObliqueness(m_horizontalOblique, m_verticalOblique);
			}
			if (Screen.width != m_oldScreenWidth || Screen.height != m_oldScreenHeight)
			{
				ResetAndSetObliqueness();
				m_oldScreenWidth = Screen.width;
				m_oldScreenHeight = Screen.height;
			}
		}

		private void ResetAndSetObliqueness()
		{
			camera.ResetProjectionMatrix();
			SetObliqueness(m_horizontalOblique, m_verticalOblique);
		}

		private void SetObliqueness(Single p_horizontalOblique, Single p_verticalOblique)
		{
			Matrix4x4 projectionMatrix = camera.projectionMatrix;
			projectionMatrix[0, 2] = p_horizontalOblique;
			projectionMatrix[1, 2] = p_verticalOblique;
			camera.projectionMatrix = projectionMatrix;
		}
	}
}
