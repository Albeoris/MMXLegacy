using System;
using UnityEngine;

namespace Legacy.Animations
{
	[AddComponentMenu("MM Legacy/Animation/Animation EventHandler")]
	public class AnimEventHandler : BaseEventHandler
	{
		[SerializeField]
		private AnimHandler m_animHandler;

		private void Awake()
		{
			if (m_animHandler == null)
			{
				m_animHandler = this.GetComponent<AnimHandler>(true);
			}
		}

		protected override void Update()
		{
			if (m_animHandler != null && m_animHandler.IsPlaying(EAnimType.IDLE))
			{
				base.Update();
			}
		}
	}
}
