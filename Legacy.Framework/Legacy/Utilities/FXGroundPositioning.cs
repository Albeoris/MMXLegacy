using System;
using Legacy.EffectEngine.Effects;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/FX GroundPositioning")]
	public class FXGroundPositioning : MonoBehaviour
	{
		[SerializeField]
		private Single m_selfDestoryOnBeginEffect = 5f;

		[SerializeField]
		private Boolean m_attachToTarget = true;

		[SerializeField]
		private Vector3 m_offset;

		private void OnBeginEffect(UnityEventArgs<FXArgs> e)
		{
			if (m_attachToTarget)
			{
				transform.parent = e.EventArgs.TargetTransform;
			}
			transform.position = e.EventArgs.TargetTransform.position + m_offset;
			if (m_selfDestoryOnBeginEffect != -1f)
			{
				Destroy(gameObject, Mathf.Max(m_selfDestoryOnBeginEffect, 0f));
			}
		}
	}
}
