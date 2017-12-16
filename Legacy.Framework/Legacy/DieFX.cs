using System;
using System.Collections;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	public class DieFX : MonoBehaviour
	{
		private const Single SINK_SPEED = 0.19f;

		private const Single TIME_DESTROY_OBJECT = 25f;

		private const Single TIME_START_SINKING = 10f;

		private const Single TIME_DESTROY_SKULLSFX = 30f;

		private const Single TIME_STOP_EMIT_SKULLFX = 7f;

		private const Single TIME_START_SKULLFX_BEFORE_SINK = 6f;

		private Single m_sinkStartTime;

		private Single m_startSkullFxTime;

		private Boolean m_isSinkStarted;

		private Object m_sender;

		private EventArgs m_eventArgs;

		private GameObject m_skullsFx;

		public void Init(Object p_sender, EventArgs p_args, Single p_dieAnimationDuration)
		{
			m_sender = p_sender;
			m_eventArgs = p_args;
			m_sinkStartTime = Time.time + p_dieAnimationDuration + 10f;
			m_startSkullFxTime = m_sinkStartTime - 6f;
			Destroy(gameObject, p_dieAnimationDuration + 10f + 25f);
			StartCoroutine(InvokeMonsterDiedEvent(p_dieAnimationDuration));
		}

		private IEnumerator InvokeMonsterDiedEvent(Single waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			DelayedEventManager.InvokeEvent(EDelayType.ON_FX_FINISH, EEventType.MONSTER_DIED, m_sender, m_eventArgs);
			yield break;
		}

		private void Update()
		{
			if (Time.time > m_sinkStartTime)
			{
				Vector3 position = transform.position;
				position.y -= Time.deltaTime * 0.19f;
				transform.position = position;
				if (!m_isSinkStarted)
				{
					m_isSinkStarted = true;
					AudioController.Play("CorpseDisapear", transform);
				}
			}
			if (m_skullsFx == null && Time.time > m_startSkullFxTime)
			{
				Vector3 vector = transform.position;
				FXTags componentInChildren = GetComponentInChildren<FXTags>();
				if (componentInChildren != null)
				{
					vector = componentInChildren.GetAveragePoint("HitSpot");
					if (vector == Vector3.zero)
					{
						vector = transform.position;
					}
				}
				UnityEngine.Object original = Resources.Load("FX/FX_DieSkulls");
				m_skullsFx = (GameObject)Instantiate(original, vector, Quaternion.identity);
				Destroy(m_skullsFx, 30f);
			}
			if (Time.time > m_sinkStartTime + 7f && m_skullsFx.GetComponent<ParticleEmitter>().emit)
			{
				m_skullsFx.GetComponent<ParticleEmitter>().emit = false;
			}
		}
	}
}
