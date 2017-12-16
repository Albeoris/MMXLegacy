using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class DetachFromParent : MonoBehaviour
	{
		[SerializeField]
		private Single DELAY;

		private Single m_detachTime;

		private void Start()
		{
			if (transform.parent == null)
			{
				Debug.LogError("DetachFromParent: parent is null!");
				Destroy(this);
				return;
			}
			if (DELAY == 0f)
			{
				transform.parent = null;
				Destroy(this);
			}
			else
			{
				m_detachTime = Time.time + DELAY;
			}
		}

		private void Update()
		{
			if (m_detachTime <= Time.time)
			{
				transform.parent = null;
				Destroy(this);
			}
		}
	}
}
