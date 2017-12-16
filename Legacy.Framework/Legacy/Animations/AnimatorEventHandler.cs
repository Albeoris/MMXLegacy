using System;
using UnityEngine;

namespace Legacy.Animations
{
	[AddComponentMenu("MM Legacy/Animation/Animator EventHandler")]
	public class AnimatorEventHandler : BaseEventHandler
	{
		[SerializeField]
		private AnimatorControl m_animatorControl;

		private void Awake()
		{
			if (m_animatorControl == null)
			{
				m_animatorControl = this.GetComponent<AnimatorControl>(true);
			}
		}

		protected override void Update()
		{
			if (m_animatorControl != null && m_animatorControl.IsIdle)
			{
				base.Update();
			}
		}
	}
}
