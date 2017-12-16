using System;
using System.Collections;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneTaralethFlyby : MonoBehaviour
	{
		[SerializeField]
		private SplineCameraMover m_CameraMover;

		[SerializeField]
		private SplineMover m_DragonMover;

		[SerializeField]
		private Single m_TiggerEnableDragonMover;

		[SerializeField]
		private Single m_TiggerEnableCameraMover;

		private Boolean m_TiggeredEnableDragonMover;

		private Boolean m_TiggeredEnableCameraMover;

		private CutsceneView m_CutsceneView;

		private Boolean m_finishTriggered;

		private Single m_time;

		private void Awake()
		{
			enabled = false;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			m_DragonMover.gameObject.SetActive(false);
			m_CameraMover.gameObject.SetActive(false);
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start TaralethFlyby cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			enabled = true;
			m_time = Time.time;
			m_TiggeredEnableDragonMover = false;
			m_TiggeredEnableCameraMover = false;
			m_finishTriggered = false;
			m_DragonMover.enabled = true;
			m_DragonMover.PositionTimer.Time = 0f;
			m_DragonMover.gameObject.SetActive(false);
			m_CameraMover.enabled = true;
			m_CameraMover.PositionTimer.Time = 0f;
			m_CameraMover.gameObject.SetActive(false);
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("Stop TaralethFlyby cutscene!");
			enabled = false;
			m_CameraMover.enabled = false;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
		}

		private void Update()
		{
			Single num = Time.time - m_time;
			if (!m_TiggeredEnableDragonMover && num >= m_TiggerEnableDragonMover)
			{
				m_TiggeredEnableDragonMover = true;
				m_DragonMover.gameObject.SetActive(true);
			}
			if (!m_TiggeredEnableCameraMover && num >= m_TiggerEnableCameraMover)
			{
				m_TiggeredEnableCameraMover = true;
				m_CameraMover.gameObject.SetActive(true);
				FXMainCamera.Instance.CurrentCamera.transform.parent = m_CameraMover.transform;
			}
			if (m_TiggeredEnableCameraMover)
			{
				Transform transform = FXMainCamera.Instance.CurrentCamera.transform;
				transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
				transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 4f);
			}
			if (!m_finishTriggered && m_CameraMover.Timer >= 1f)
			{
				m_finishTriggered = true;
				StartCoroutine(StopCutscene(1f));
			}
		}

		private IEnumerator StopCutscene(Single delay)
		{
			yield return new WaitForSeconds(delay);
			m_CutsceneView.MyController.StopCutscene();
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
			m_DragonMover.gameObject.SetActive(false);
			m_CameraMover.gameObject.SetActive(false);
			yield break;
		}
	}
}
