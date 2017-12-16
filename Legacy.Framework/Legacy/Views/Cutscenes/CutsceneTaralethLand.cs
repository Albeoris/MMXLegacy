using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneTaralethLand : MonoBehaviour
	{
		[SerializeField]
		private SplineCameraMover m_CameraMover;

		[SerializeField]
		private Single m_TriggerTaralethLandAnimation;

		[SerializeField]
		private Single m_TriggerTaralethSwtich;

		[SerializeField]
		private Animation m_TaralethAnimation;

		[SerializeField]
		private GameObject m_TaralethLandFX;

		[SerializeField]
		private Int32 m_TaralethSpawnID;

		[SerializeField]
		private SlotPosi m_TaralethSlotPosi;

		[SerializeField]
		private SlotPosi m_PartySlotPosi;

		private CutsceneView m_CutsceneView;

		private Boolean m_TriggeredTaralethLandAnimation;

		private Boolean m_TriggeredTaralethSwtich;

		private Single m_time;

		private void Awake()
		{
			enabled = false;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			m_TaralethAnimation.gameObject.SetActive(false);
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start TaralethLand cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			enabled = true;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			m_time = Time.time;
			m_CameraMover.enabled = true;
			m_CameraMover.Timer = 0f;
			m_TriggeredTaralethLandAnimation = false;
			m_TriggeredTaralethSwtich = false;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_CameraMover.transform;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("Stop TaralethLand cutscene!");
			enabled = false;
			m_CameraMover.enabled = false;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
		}

		private void Update()
		{
			Transform transform = FXMainCamera.Instance.CurrentCamera.transform;
			transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 2f);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 2f);
			Single num = Time.time - m_time;
			if (!m_TriggeredTaralethLandAnimation && num >= m_TriggerTaralethLandAnimation)
			{
				m_TriggeredTaralethLandAnimation = true;
				m_TaralethAnimation.gameObject.SetActive(true);
				StartCoroutine(PlayLandAnimation());
			}
			if (!m_TriggeredTaralethSwtich && num >= m_TriggerTaralethSwtich)
			{
				m_TriggeredTaralethSwtich = true;
				m_TaralethAnimation.gameObject.SetActive(false);
				foreach (BaseObject baseObject in LegacyLogic.Instance.WorldManager.Objects)
				{
					Monster monster = baseObject as Monster;
					if (monster != null && monster.SpawnerID == m_TaralethSpawnID)
					{
						monster.Move(new Position(m_TaralethSlotPosi.X, m_TaralethSlotPosi.Y), true);
						monster.Rotate(m_TaralethSlotPosi.LookDir, true);
						break;
					}
				}
				Party party = LegacyLogic.Instance.WorldManager.Party;
				party.Move(new Position(m_PartySlotPosi.X, m_PartySlotPosi.Y), false);
				party.Rotate(m_PartySlotPosi.LookDir, true);
				StartCoroutine(StopCutscene(1f));
			}
		}

		private IEnumerator StopCutscene(Single delay)
		{
			yield return new WaitForSeconds(delay);
			m_CutsceneView.MyController.StopCutscene();
			yield break;
		}

		private IEnumerator PlayLandAnimation()
		{
			Animation anim = m_TaralethAnimation;
			anim["FlyEnd_2"].enabled = true;
			anim["FlyEnd_2"].weight = 1f;
			anim.Sample();
			anim["FlyEnd_2"].enabled = false;
			anim.Sample();
			anim.Play("FlyEnd_2");
			ParticleSystem[] system = m_TaralethLandFX.GetComponentsInChildren<ParticleSystem>();
			if (system.Length > 0)
			{
				foreach (ParticleSystem item in system)
				{
					item.Play(false);
				}
			}
			yield return new WaitForSeconds(anim["FlyEnd_2"].length - 0.5f);
			anim.CrossFade("Ability2", 0.5f);
			yield break;
		}

		[Serializable]
		private class SlotPosi
		{
			public Int32 X;

			public Int32 Y;

			public EDirection LookDir;
		}
	}
}
