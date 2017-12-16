using System;
using System.Collections;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneExclusiveDungeon : MonoBehaviour
	{
		[SerializeField]
		private Animation mCameraAnimation;

		[SerializeField]
		private String m_SoundKey;

		[SerializeField]
		private String[] m_animationQueue;

		private CutsceneView m_CutsceneView;

		private Boolean m_isFinishTriggered;

		private Boolean m_isFXPlayed;

		private ToggleByColliderTrigger m_skyObjectHandler;

		private CutsceneObjectMarker m_cutsceneObjects;

		private GameObject m_redinnObject;

		private void Awake()
		{
			enabled = false;
			m_skyObjectHandler = (ToggleByColliderTrigger)FindObjectOfType(typeof(ToggleByColliderTrigger));
			m_cutsceneObjects = (CutsceneObjectMarker)FindObjectOfType(typeof(CutsceneObjectMarker));
			foreach (SceneEventView_TwoObjects sceneEventView_TwoObjects in (SceneEventView_TwoObjects[])FindObjectsOfType(typeof(SceneEventView_TwoObjects)))
			{
				if (sceneEventView_TwoObjects != null && sceneEventView_TwoObjects.name == "RedHandInnVersions")
				{
					m_redinnObject = sceneEventView_TwoObjects.gameObject;
				}
			}
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start Exclusive Dungeon cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			m_isFinishTriggered = false;
			m_isFXPlayed = false;
			mCameraAnimation.Rewind();
			foreach (String animation in m_animationQueue)
			{
				mCameraAnimation.CrossFadeQueued(animation, 0.7f, QueueMode.CompleteOthers);
			}
			enabled = true;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = mCameraAnimation.transform;
			if (m_skyObjectHandler != null)
			{
				Invoke("DeactivateInn", 4f);
				m_skyObjectHandler.LowLODObject.SetActive(true);
				m_skyObjectHandler.HighLODObject.SetActive(false);
				m_skyObjectHandler.LowLODObject.transform.parent = mCameraAnimation.transform;
				m_skyObjectHandler.LowLODObject.transform.localPosition = Vector3.zero;
			}
			if (m_cutsceneObjects != null)
			{
				m_cutsceneObjects.ActivateObjects();
			}
			FXMainCamera.Instance.CurrentCamera.transform.localPosition = Vector3.zero;
			FXMainCamera.Instance.CurrentCamera.transform.localRotation = Quaternion.identity;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("stop Exclusive Dungeon cutscene!");
			enabled = false;
			mCameraAnimation.Stop();
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
			if (m_cutsceneObjects != null)
			{
				m_cutsceneObjects.DeactivateObjects();
			}
			if (m_skyObjectHandler != null)
			{
				m_skyObjectHandler.LowLODObject.transform.parent = null;
			}
		}

		private void Update()
		{
			String name = m_animationQueue[m_animationQueue.Length - 1];
			if (!m_isFinishTriggered && mCameraAnimation.IsPlaying(name))
			{
				m_isFinishTriggered = true;
				StartCoroutine(StopCutscene(mCameraAnimation[name].length));
			}
		}

		private IEnumerator StopCutscene(Single delay)
		{
			yield return new WaitForSeconds(delay);
			m_CutsceneView.MyController.StopCutscene();
			yield break;
		}

		private void DeactivateInn()
		{
			m_redinnObject.SetActive(false);
		}
	}
}
