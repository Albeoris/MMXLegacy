using System;
using System.Collections;
using Legacy.Animations;
using Legacy.Audio;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views
{
	public class SpecialDieView : MonsterDieView
	{
		private static Single ANIM_TIME = 3.8f;

		private Single m_StartTime;

		private Single m_Targettime;

		private Boolean m_DieStart;

		[SerializeField]
		private GameObject m_effect;

		[SerializeField]
		private Single destroytime = 0.2f;

		[SerializeField]
		private Single m_AnimDuration;

		[SerializeField]
		private String deathanimname = "Ability";

		[SerializeField]
		private Boolean mamushi = true;

		[SerializeField]
		private Vector3 m_spawnEffectOffset;

		[SerializeField]
		private AnimHandler m_AnimHandler;

		[SerializeField]
		private Animation m_lightOffAnim_1;

		[SerializeField]
		private Animation m_lightOffAnim_2;

		private void Start()
		{
			if (mamushi)
			{
				DelayedEventManager.SetFixedDelay(EEventType.CHARACTER_XP_GAIN, ANIM_TIME);
			}
			else
			{
				ANIM_TIME = m_AnimDuration;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (mamushi)
			{
				DelayedEventManager.ResetFixedDelayToDefault(EEventType.CHARACTER_XP_GAIN);
			}
		}

		protected override void OnDie()
		{
			m_StartTime = Time.time;
			m_Targettime = m_StartTime + ANIM_TIME - destroytime;
			m_DieStart = true;
			Destroy(this.gameObject, ANIM_TIME);
			if (m_lightOffAnim_1 != null && m_lightOffAnim_2 != null)
			{
				m_lightOffAnim_1.Play();
				m_lightOffAnim_2.Play();
			}
			m_AnimHandler.Play(deathanimname);
			GameObject gameObject = Helper.Instantiate<GameObject>(m_effect);
			gameObject.transform.position = transform.position + m_spawnEffectOffset;
			if (mamushi)
			{
				gameObject.particleSystem.Play();
				AudioManager.Instance.RequestPlayAudioID("mamushidie", 0, -1f, transform.position, null, 1f, 0f, 0f, null);
				DelayedEventManagerWorker delayedEventManagerWorker = new GameObject("Mamushi Script Worker").AddComponent<DelayedEventManagerWorker>();
				delayedEventManagerWorker.transform.position = transform.position;
				DontDestroyOnLoad(delayedEventManagerWorker);
				delayedEventManagerWorker.StartCoroutine(PlayLateShakeFX(delayedEventManagerWorker));
			}
		}

		private void Update()
		{
			if (m_DieStart && Time.time > m_Targettime)
			{
				Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.enabled = false;
				}
				m_DieStart = false;
			}
		}

		private static IEnumerator PlayLateShakeFX(DelayedEventManagerWorker p_Worker)
		{
			yield return new WaitForSeconds(ANIM_TIME - 0.9f);
			Vector3 vec = new Vector3(0f, 0.1f, 0f);
			AudioController.Play("LightningBoltImpact", p_Worker.transform.position, null);
			FXMainCamera.Instance.PlayShakeFX(0.3f, vec);
			yield return new WaitForSeconds(2.2f);
			Single jingleTime = -1f;
			AudioObject jingle = AudioController.Play("BossDefeatedJingle", p_Worker.transform.position, null);
			if (jingle != null)
			{
				jingleTime = jingle.clipLength;
			}
			GameMessageEventArgs gameMessageArgs = new GameMessageEventArgs("GAME_MESSAGE_MAMUSHI_DEFEATED", jingleTime - 2.4f);
			DelayedEventManager.InvokeEvent(EDelayType.ON_FRAME_END, EEventType.GAME_MESSAGE, null, gameMessageArgs);
			DelayedEventManager.InvokeEvent(EDelayType.ON_FIXED_DELAY, EEventType.GAME_MESSAGE, null, gameMessageArgs);
			Destroy(p_Worker);
			yield break;
		}
	}
}
