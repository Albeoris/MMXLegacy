using System;
using Legacy.Audio;
using Legacy.NpcInteraction;
using UnityEngine;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Audio/Ambient Indoor Sound Source")]
	public class AmbientIndoorSoundSource : AmbientSoundSourceBase
	{
		private IndoorSceneRoot m_SceneRoot;

		protected override Boolean IsSoundPlayable()
		{
			return base.IsSoundPlayable() && m_SceneRoot != null && m_SceneRoot.SceneCamera != null && m_SceneRoot.SceneCamera.enabled;
		}

		protected override void Update()
		{
			if (m_SceneRoot == null)
			{
				m_SceneRoot = transform.root.GetComponentInChildren<IndoorSceneRoot>();
			}
			else
			{
				base.Update();
			}
		}
	}
}
