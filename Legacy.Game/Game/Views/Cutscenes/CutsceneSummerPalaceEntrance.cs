using System;
using System.Collections;
using Legacy.Animations;
using Legacy.Core.Api;
using Legacy.Core.NpcInteraction;
using Legacy.EffectEngine;
using Legacy.Game.IngameManagement;
using Legacy.Views;
using Legacy.Views.Cutscenes;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Views.Cutscenes
{
	internal class CutsceneSummerPalaceEntrance : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_Root;

		[SerializeField]
		private Animation m_Animation;

		[SerializeField]
		private GameObject m_CameraAnchor;

		private CutsceneView m_CutsceneView;

		private void OnDestroy()
		{
			LegacyLogic.Instance.ConversationManager.CutsceneStateChanged -= ConversationManager_CutsceneStateChanged;
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start CutsceneSummerPalaceEntrance cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_CameraAnchor.transform;
			m_Root.SetActive(true);
			StartCoroutine(PlayBegin());
			LegacyLogic.Instance.ConversationManager.CutsceneStateChanged -= ConversationManager_CutsceneStateChanged;
			LegacyLogic.Instance.ConversationManager.CutsceneStateChanged += ConversationManager_CutsceneStateChanged;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("Stop CutsceneSummerPalaceEntrance cutscene!");
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
			m_Animation.Play("Cam_SPE_Start");
			Single length = m_Animation["Cam_SPE_Start"].length;
			yield return new WaitForSeconds(length);
			GameObject view = ViewManager.Instance.FindView(45);
			if (view != null)
			{
				AnimatorControl anim = view.GetComponentInChildren<AnimatorControl>();
				if (anim != null)
				{
					anim.EventSummon(anim.EventMaxValue);
				}
				else
				{
					Debug.LogError("not found sunrider anim view " + 45);
				}
			}
			else
			{
				Debug.LogError("not found sunrider " + 45);
			}
			IngameController.Instance.EnableGuiCamera(true, true);
			IngameController.Instance.EnableGui(true, true);
			yield break;
		}

		private IEnumerator PlayEnd()
		{
			Single animDelay = 1f;
			m_Animation.Play("Cam_SPE_Finish");
			Single length = m_Animation["Cam_SPE_Finish"].length;
			yield return new WaitForSeconds(animDelay * 0.5f);
			GameObject view = ViewManager.Instance.FindView(41);
			if (view != null)
			{
				AnimatorControl anim = view.GetComponentInChildren<AnimatorControl>();
				if (anim != null)
				{
					anim.IdleSpecial(1);
				}
			}
			yield return new WaitForSeconds(animDelay * 0.5f);
			view = ViewManager.Instance.FindView(42);
			if (view != null)
			{
				AnimatorControl anim2 = view.GetComponentInChildren<AnimatorControl>();
				if (anim2 != null)
				{
					anim2.IdleSpecial(1);
				}
			}
			yield return new WaitForSeconds(length - animDelay);
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
			m_Root.SetActive(false);
			m_CutsceneView.MyController.StopCutscene();
			yield break;
		}
	}
}
