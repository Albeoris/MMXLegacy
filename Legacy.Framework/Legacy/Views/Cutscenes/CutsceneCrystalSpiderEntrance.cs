using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Map;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneCrystalSpiderEntrance : MonoBehaviour
	{
		[SerializeField]
		private Animation m_CrystalSpiderAnimation;

		[SerializeField]
		private GameObject m_CameraAnchor;

		[SerializeField]
		private Int32 m_CrystalSpiderSpawnerID;

		[SerializeField]
		private Int32 m_CrystalSpiderPosX;

		[SerializeField]
		private Int32 m_CrystalSpiderPosY;

		[SerializeField]
		private EDirection m_CrystalSpiderDir;

		private CutsceneView m_CutsceneView;

		private void Awake()
		{
			enabled = false;
			m_CrystalSpiderAnimation.gameObject.SetActive(false);
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start Crystal Spider cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			enabled = true;
			m_CrystalSpiderAnimation.gameObject.SetActive(true);
			StartCoroutine(PlayAnim());
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_CameraAnchor.transform;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("Stop Crystal Spider cutscene!");
			enabled = false;
			m_CrystalSpiderAnimation.gameObject.SetActive(false);
			StopAllCoroutines();
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
		}

		private void Update()
		{
			Transform transform = FXMainCamera.Instance.CurrentCamera.transform;
			transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime);
		}

		private IEnumerator PlayAnim()
		{
			m_CrystalSpiderAnimation.Play("entrance");
			Single length = m_CrystalSpiderAnimation["entrance"].length - 1.5f;
			yield return new WaitForSeconds(length);
			m_CrystalSpiderAnimation.CrossFade("grunt", 1.5f, PlayMode.StopAll);
			length = m_CrystalSpiderAnimation["grunt"].length - 1.5f;
			yield return new WaitForSeconds(length);
			m_CrystalSpiderAnimation.CrossFade("combatidle_0", 1.5f, PlayMode.StopAll);
			length = m_CrystalSpiderAnimation["combatidle_0"].length - 0.5f;
			yield return new WaitForSeconds(length);
			Monster mob = LegacyLogic.Instance.WorldManager.FindObject(m_CrystalSpiderSpawnerID) as Monster;
			if (mob != null)
			{
				mob.Move(new Position(m_CrystalSpiderPosX, m_CrystalSpiderPosY), m_CrystalSpiderDir, true);
			}
			m_CutsceneView.MyController.StopCutscene();
			yield break;
		}
	}
}
