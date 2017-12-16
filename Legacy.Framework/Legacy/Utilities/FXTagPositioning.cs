using System;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/FX TagPositioning")]
	public class FXTagPositioning : MonoBehaviour
	{
		public Boolean m_AttackToTransform;

		public String m_Tag;

		private void OnBeginEffect(UnityEventArgs<FXArgs> e)
		{
			FXTags component = e.EventArgs.Target.GetComponent<FXTags>();
			Transform transform;
			if (component == null)
			{
				Debug.LogError("FXTags not found at model " + e.EventArgs.Target, e.EventArgs.Target);
				transform = e.EventArgs.TargetTransform;
			}
			else
			{
				transform = component.FindOne(m_Tag).transform;
			}
			this.transform.position = transform.position;
			if (m_AttackToTransform)
			{
				this.transform.parent = transform;
			}
		}
	}
}
