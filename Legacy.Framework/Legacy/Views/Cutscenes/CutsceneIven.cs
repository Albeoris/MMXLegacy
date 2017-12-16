using System;
using System.Collections;
using Legacy.Animations;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneIven : MonoBehaviour
	{
		[SerializeField]
		private Int32 m_IvenSpawnerID;

		[SerializeField]
		private SplineCameraMover m_CameraMover;

		[SerializeField]
		private Single m_TiggerBark;

		[SerializeField]
		private Single m_TiggerTitleLabel;

		[SerializeField]
		private CutsceneSceenText m_TitleLabel;

		private Boolean m_barkTriggered;

		private CutsceneView m_CutsceneView;

		private Boolean m_finishTriggered;

		private Boolean m_titleTriggered;

		private void Awake()
		{
			enabled = false;
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start iven cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			m_finishTriggered = false;
			m_barkTriggered = false;
			m_CameraMover.enabled = true;
			m_CameraMover.Timer = 0f;
			enabled = true;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_CameraMover.transform;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("stop iven cutscene!");
			enabled = false;
			m_CameraMover.enabled = false;
			m_TitleLabel.Hide();
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
		}

		private void Update()
		{
			Transform transform = FXMainCamera.Instance.CurrentCamera.transform;
			transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 4f);
			if (!m_barkTriggered && m_CameraMover.Timer >= m_TiggerBark)
			{
				m_barkTriggered = true;
				GameObject gameObject = ViewManager.Instance.FindView(m_IvenSpawnerID);
				if (gameObject != null)
				{
					AnimHandler componentInChildren = gameObject.GetComponentInChildren<AnimHandler>();
					if (componentInChildren != null)
					{
						componentInChildren.Play("Command");
					}
					else
					{
						Debug.LogError("not found iven anim view " + m_IvenSpawnerID);
					}
				}
				else
				{
					Debug.LogError("not found iven " + m_IvenSpawnerID);
				}
			}
			if (!m_titleTriggered && m_CameraMover.Timer >= m_TiggerTitleLabel)
			{
				m_titleTriggered = true;
				m_TitleLabel.Show();
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
			yield break;
		}
	}
}
