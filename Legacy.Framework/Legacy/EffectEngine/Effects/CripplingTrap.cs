using System;
using System.Collections;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class CripplingTrap : MonoBehaviour
	{
		[SerializeField]
		private Single m_startAnimDelay = 0.5f;

		[SerializeField]
		private Single m_startAnimTime = 0.5f;

		[SerializeField]
		private Single m_lookAnimDuration = 1.5f;

		[SerializeField]
		private Renderer m_renderer;

		[SerializeField]
		private Animation m_anim;

		[SerializeField]
		private Transform m_lookAt;

		private DetachAndFollowParent m_parent;

		private Boolean m_isDone;

		private IEnumerator Start()
		{
			m_parent = GetComponent<DetachAndFollowParent>();
			if (m_anim == null || m_lookAt == null || m_renderer == null || m_parent == null)
			{
				Debug.LogError("CripplingTrap: animation or look at transform or renderer or DetachAndFollowParent not set!");
				Destroy(this);
			}
			else
			{
				AudioController.Play("CripplingTrapSpawn", transform);
				m_renderer.enabled = false;
				InteractiveObjectCamera.Instance.ActivateInteractiveObjectLook(m_lookAt);
				yield return new WaitForSeconds(m_startAnimDelay);
				m_renderer.enabled = true;
				m_anim.Play("Start");
				m_anim["Start"].time = m_startAnimTime;
				yield return new WaitForSeconds(m_lookAnimDuration);
				InteractiveObjectCamera.Instance.CancelEffect();
				m_isDone = true;
			}
			yield break;
		}

		private void Update()
		{
			if (m_parent != null && m_parent.IsDestroyed)
			{
				m_anim.Play("Activate");
				AudioController.Play("CripplingTrapDespawn", transform);
				m_parent = null;
			}
		}

		private void OnDestroy()
		{
			if (!m_isDone)
			{
				InteractiveObjectCamera.Instance.CancelEffect();
				StopAllCoroutines();
				m_isDone = true;
			}
		}
	}
}
