using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.NpcInteraction;
using Legacy.EffectEngine;
using Legacy.Game.IngameManagement;
using Legacy.Views.Cutscenes;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Views.Cutscenes
{
	internal class CutsceneErebosEntrance : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_Root;

		[SerializeField]
		private Animation m_BeginAnimation;

		[SerializeField]
		private GameObject m_BeginCameraAnchor;

		[SerializeField]
		private Animation m_ErebosAnimation;

		[SerializeField]
		private GameObject m_ErebosCameraAnchor;

		private CutsceneView m_CutsceneView;

		private void OnDestroy()
		{
			LegacyLogic.Instance.ConversationManager.CutsceneStateChanged -= ConversationManager_CutsceneStateChanged;
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start ErebosEntrance cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_BeginCameraAnchor.transform;
			m_Root.SetActive(true);
			StartCoroutine(PlayBegin());
			LegacyLogic.Instance.ConversationManager.CutsceneStateChanged -= ConversationManager_CutsceneStateChanged;
			LegacyLogic.Instance.ConversationManager.CutsceneStateChanged += ConversationManager_CutsceneStateChanged;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("Stop ErebosEntrance cutscene!");
			m_Root.SetActive(false);
			StopAllCoroutines();
			LegacyLogic.Instance.ConversationManager.CutsceneStateChanged -= ConversationManager_CutsceneStateChanged;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
		}

		private void Update()
		{
			Transform transform = FXMainCamera.Instance.CurrentCamera.transform;
			transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 6f);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 4f);
		}

		private void ConversationManager_CutsceneStateChanged(Object sender, ChangedCutsceneStateEventArgs e)
		{
			if (m_CutsceneView != null && e.NewState == 1)
			{
				IngameController.Instance.EnableGuiCamera(false, false);
				IngameController.Instance.EnableGui(false, false);
				StopAllCoroutines();
				StartCoroutine(PlayEnd());
			}
		}

		private IEnumerator PlayBegin()
		{
			m_ErebosAnimation.gameObject.SetActive(false);
			m_BeginAnimation.Play("Cam_Entrance_Start");
			Single length = m_BeginAnimation["Cam_Entrance_Start"].length;
			yield return new WaitForSeconds(length);
			m_ErebosAnimation.gameObject.SetActive(true);
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_ErebosCameraAnchor.transform;
			FXMainCamera.Instance.CurrentCamera.transform.localPosition = Vector3.zero;
			FXMainCamera.Instance.CurrentCamera.transform.localRotation = Quaternion.identity;
			m_ErebosAnimation.Play("Summon");
			length = m_ErebosAnimation["Summon"].length - 1.5f;
			yield return new WaitForSeconds(length);
			IngameController.Instance.EnableGuiCamera(true, true);
			IngameController.Instance.EnableGui(true, true);
			m_ErebosAnimation.CrossFade("Idle", 1.5f, PlayMode.StopAll);
			yield break;
		}

		private IEnumerator PlayEnd()
		{
			if (m_ErebosAnimation.isPlaying)
			{
				m_ErebosAnimation.CrossFade("Vanish", 0.3f, PlayMode.StopAll);
			}
			else
			{
				m_ErebosAnimation.Play("Vanish", PlayMode.StopAll);
			}
			Single length = m_ErebosAnimation["Vanish"].length + 1f;
			yield return new WaitForSeconds(length);
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
			m_ErebosAnimation.gameObject.SetActive(false);
			m_Root.SetActive(false);
			m_CutsceneView.MyController.StopCutscene();
			yield break;
		}
	}
}
