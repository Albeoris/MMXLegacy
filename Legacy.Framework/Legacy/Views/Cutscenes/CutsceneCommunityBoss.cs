using System;
using System.Collections;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneCommunityBoss : MonoBehaviour
	{
		[SerializeField]
		private Int32 m_MonsterSpawnerID;

		[SerializeField]
		private SplineCameraMover m_CameraMover;

		[SerializeField]
		private String m_SoundKey;

		private CutsceneView m_CutsceneView;

		private Boolean m_isFinishTriggered;

		private Boolean m_isFXPlayed;

		private void Awake()
		{
			enabled = false;
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start community boss cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			m_isFinishTriggered = false;
			m_isFXPlayed = false;
			m_CameraMover.enabled = true;
			m_CameraMover.Timer = 0f;
			enabled = true;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_CameraMover.transform;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("stop community boss cutscene!");
			enabled = false;
			m_CameraMover.enabled = false;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
		}

		private void Update()
		{
			Transform transform = FXMainCamera.Instance.CurrentCamera.transform;
			transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 4f);
			if (!m_isFXPlayed)
			{
				m_isFXPlayed = true;
				Monster monster = (Monster)LegacyLogic.Instance.WorldManager.FindObject(m_MonsterSpawnerID);
				if (monster != null)
				{
					monster.IsAggro = true;
				}
				else
				{
					Debug.LogError("not found community boss monster object " + m_MonsterSpawnerID);
				}
				GameObject gameObject = ViewManager.Instance.FindView(m_MonsterSpawnerID);
				if (gameObject != null)
				{
					if (!String.IsNullOrEmpty(m_SoundKey))
					{
						AudioManager.Instance.RequestPlayAudioID(m_SoundKey, 0, -1f, gameObject.transform, 1f, 0f, 0f, null);
					}
					CommunityDungeonBossRevealView componentInChildren = gameObject.GetComponentInChildren<CommunityDungeonBossRevealView>();
					if (componentInChildren != null)
					{
						componentInChildren.RunFX();
					}
					else
					{
						Debug.LogError("not found community boss fx view " + m_MonsterSpawnerID);
					}
				}
				else
				{
					Debug.LogError("not found community boss " + m_MonsterSpawnerID);
				}
			}
			if (!m_isFinishTriggered && m_CameraMover.Timer >= 1f)
			{
				m_isFinishTriggered = true;
				StartCoroutine(StopCutscene(1f));
			}
		}

		private IEnumerator StopCutscene(Single delay)
		{
			yield return new WaitForSeconds(delay);
			m_CutsceneView.MyController.StopCutscene();
			yield break;
		}
	}
}
