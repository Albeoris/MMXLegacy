using System;
using System.Collections;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneFortLaegaireGuard : MonoBehaviour
	{
		private CutsceneView m_CutsceneView;

		private Boolean m_finishTriggered;

		private void Awake()
		{
			enabled = false;
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			m_CutsceneView = (CutsceneView)args.Sender;
			enabled = true;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = transform;
			animation.Play();
			FXMainCamera.Instance.CurrentCamera.transform.localPosition = Vector3.zero;
			FXMainCamera.Instance.CurrentCamera.transform.localRotation = Quaternion.identity;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			enabled = false;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
		}

		private void Update()
		{
			if (!m_finishTriggered && animation.isPlaying)
			{
				m_finishTriggered = true;
				StartCoroutine(StopCutscene(animation.clip.length + 1f));
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
