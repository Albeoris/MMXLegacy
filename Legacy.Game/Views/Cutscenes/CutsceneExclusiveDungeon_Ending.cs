using System;
using System.Collections;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneExclusiveDungeon_Ending : MonoBehaviour
	{
		private CutsceneView m_CutsceneView;

		private ToggleByColliderTrigger m_skyObjectHandler;

		[SerializeField]
		private GameObject m_Root;

		[SerializeField]
		private Animation m_MalbethAnimation;

		[SerializeField]
		private GameObject m_CameraAnchor;

		[SerializeField]
		private Animation m_CameraAnchorAnimation;

		[SerializeField]
		private Int32 m_MalbethSpawnerID;

		private void Awake()
		{
			enabled = false;
			transform.position = Vector3.zero;
			m_skyObjectHandler = (ToggleByColliderTrigger)FindObjectOfType(typeof(ToggleByColliderTrigger));
			m_Root.SetActive(false);
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start Exclusive Dungeon Ending cutscene!");
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			m_CutsceneView = (CutsceneView)args.Sender;
			StartCoroutine(PlayAnim());
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("Stop Exclusive Dungeon Ending cutscene!");
			enabled = false;
			m_Root.SetActive(false);
			StopAllCoroutines();
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
			if (m_skyObjectHandler != null)
			{
				m_skyObjectHandler.LowLODObject.transform.parent = null;
			}
		}

		private IEnumerator PlayAnim()
		{
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = null;
			GameObject view = ViewManager.Instance.FindView(m_MalbethSpawnerID);
			if (view != null)
			{
				view.transform.position = new Vector3(1000f, 2000f, 1000f);
			}
			view = ViewManager.Instance.FindView(126);
			if (view != null)
			{
				view.transform.position = new Vector3(1000f, 2000f, 1000f);
			}
			enabled = true;
			m_Root.SetActive(true);
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_CameraAnchor.transform;
			FXMainCamera.Instance.CurrentCamera.transform.localPosition = Vector3.zero;
			FXMainCamera.Instance.CurrentCamera.transform.localRotation = Quaternion.identity;
			if (m_skyObjectHandler != null)
			{
				m_skyObjectHandler.LowLODObject.SetActive(true);
				m_skyObjectHandler.LowLODObject.transform.parent = m_CameraAnchor.transform;
				m_skyObjectHandler.LowLODObject.transform.localPosition = Vector3.zero;
				m_skyObjectHandler.LowLODObject.transform.localRotation = Quaternion.identity;
			}
			m_MalbethAnimation.Play("Ability2");
			m_CameraAnchorAnimation.Play("ExclusiveDungeonEnding_Camera");
			Single length = m_CameraAnchorAnimation["ExclusiveDungeonEnding_Camera"].length;
			yield return new WaitForSeconds(length);
			m_CutsceneView.MyController.StopCutscene();
			yield break;
		}
	}
}
