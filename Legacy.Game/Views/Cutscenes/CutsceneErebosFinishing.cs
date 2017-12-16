using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.EffectEngine;
using Legacy.Game.IngameManagement;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneErebosFinishing : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_Root;

		[SerializeField]
		private Animation m_ErebosAnimation;

		[SerializeField]
		private GameObject m_ErebosCameraAnchor;

		[SerializeField]
		private Animation m_CannonCloseUpAnimation;

		[SerializeField]
		private GameObject m_CannonCloseUpCameraAnchor;

		[SerializeField]
		private Int32 m_ErebosMonsterSpawnID;

		private CutsceneView m_CutsceneView;

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start ErebosFinishing cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			m_Root.SetActive(true);
			StartCoroutine(PlayAnim());
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_ErebosCameraAnchor.transform;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("Stop ErebosFinishing cutscene!");
			m_Root.SetActive(false);
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
		}

		private IEnumerator PlayAnim()
		{
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_ErebosCameraAnchor.transform;
			FXMainCamera.Instance.CurrentCamera.transform.localPosition = Vector3.zero;
			FXMainCamera.Instance.CurrentCamera.transform.localRotation = Quaternion.identity;
			m_ErebosAnimation.Play("Summon 1");
			Single length = m_ErebosAnimation["Summon 1"].length - 3f;
			yield return new WaitForSeconds(length);
			m_ErebosAnimation.CrossFade("Victory", 1.5f, PlayMode.StopAll);
			yield return new WaitForSeconds(3.2f);
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_CannonCloseUpCameraAnchor.transform;
			FXMainCamera.Instance.CurrentCamera.transform.localPosition = Vector3.zero;
			FXMainCamera.Instance.CurrentCamera.transform.localRotation = Quaternion.identity;
			m_CannonCloseUpAnimation.Play("camera_anchor_Ending1_startCannon");
			yield return new WaitForSeconds(5f);
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_ErebosCameraAnchor.transform;
			FXMainCamera.Instance.CurrentCamera.transform.localPosition = Vector3.zero;
			FXMainCamera.Instance.CurrentCamera.transform.localRotation = Quaternion.identity;
			m_ErebosAnimation.Play("AttackCritical");
			length = m_ErebosAnimation["AttackCritical"].length - 1f;
			yield return new WaitForSeconds(length);
			m_ErebosAnimation.CrossFade("Turn90Right", 1f, PlayMode.StopAll);
			length = m_ErebosAnimation["Turn90Right"].length - 1f;
			yield return new WaitForSeconds(length);
			m_ErebosAnimation["Die"].speed = 0.5f;
			m_ErebosAnimation.CrossFade("Die", 1f, PlayMode.StopAll);
			yield return new WaitForSeconds(2f);
			Boolean addTokken = true;
			foreach (BaseObject obj in LegacyLogic.Instance.WorldManager.Objects)
			{
				Monster mob = obj as Monster;
				if (mob != null && mob.SpawnerID == m_ErebosMonsterSpawnID)
				{
					Debug.Log("kill erebos! " + m_ErebosMonsterSpawnID);
					mob.Die();
					mob.TriggerLateDieEffects();
					addTokken = false;
					break;
				}
			}
			if (addTokken)
			{
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddToken(699);
				Debug.Log("erebos not found " + m_ErebosMonsterSpawnID);
			}
			IngameController.Instance.EnableGuiCamera(true, false);
			IngameController.Instance.EnableGui(false, false);
			yield return new WaitForSeconds(5f);
			m_CutsceneView.MyController.StopCutscene();
			yield break;
		}
	}
}
