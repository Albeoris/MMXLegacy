using System;
using System.Collections;
using Legacy.Animations;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Map;
using Legacy.EffectEngine;
using UnityEngine;

namespace Legacy.Views.Cutscenes
{
	internal class CutsceneMarkusWolfEntrance : MonoBehaviour
	{
		[SerializeField]
		private SplineCameraMover m_CameraMover;

		[SerializeField]
		private Int32 m_MarkusWolfSpawnerID;

		[SerializeField]
		private Int32 m_MarkusWolfMoveToX;

		[SerializeField]
		private Int32 m_MarkusWolfMoveToY;

		[SerializeField]
		private EDirection m_MarkusWolfLooAt;

		[SerializeField]
		private Single m_TriggerMove;

		[SerializeField]
		private MovePoint[] m_MarkusMovePoints;

		[SerializeField]
		private GameObject m_MarkusWolfView;

		[SerializeField]
		private MoveEntity[] m_MoveEntities;

		private CutsceneView m_CutsceneView;

		private Boolean m_TriggeredMove;

		private Boolean m_TriggeredFinish;

		private Single m_Time;

		private void Awake()
		{
			enabled = false;
		}

		private void OnCutsceneStart(UnityEventArgs args)
		{
			Debug.Log("Start MarkusWolfEntrance cutscene!");
			m_CutsceneView = (CutsceneView)args.Sender;
			m_TriggeredFinish = false;
			m_CameraMover.enabled = true;
			m_CameraMover.Timer = 0f;
			enabled = true;
			m_Time = Time.time;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Cutscene;
			FXMainCamera.Instance.CurrentCamera.transform.parent = m_CameraMover.transform;
		}

		private void OnCutsceneStop(UnityEventArgs args)
		{
			Debug.Log("Stop MarkusWolfEntrance cutscene!");
			enabled = false;
			m_CameraMover.enabled = false;
			FXMainCamera.Instance.CameraModus = FXMainCamera.EModus.Normal;
			FXMainCamera.Instance.ResetCameraTransformation();
		}

		private void Update()
		{
			Transform transform = FXMainCamera.Instance.CurrentCamera.transform;
			transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 2f);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 2f);
			Single num = Time.time - m_Time;
			if (!m_TriggeredMove && num >= m_TriggerMove)
			{
				m_TriggeredMove = true;
				MoveEntities();
				StartCoroutine(FollowPath());
			}
			if (!m_TriggeredFinish && m_CameraMover.Timer >= 1f)
			{
				m_TriggeredFinish = true;
				m_CutsceneView.MyController.StopCutscene();
			}
		}

		private void MoveEntities()
		{
			if (m_MoveEntities != null)
			{
				foreach (MoveEntity moveEntity in m_MoveEntities)
				{
					Monster monster = LegacyLogic.Instance.WorldManager.FindObject(moveEntity.SpawnerID) as Monster;
					if (monster != null)
					{
						monster.Move(new Position(moveEntity.MoveToX, moveEntity.MoveToY), moveEntity.LookDirection, false);
					}
				}
			}
		}

		private IEnumerator FollowPath()
		{
			Int32 idx = 0;
			if (m_MarkusWolfView == null)
			{
				m_MarkusWolfView = ViewManager.Instance.FindView(m_MarkusWolfSpawnerID);
			}
			if (m_MarkusWolfView != null)
			{
				AnimatorControl animControl = m_MarkusWolfView.GetComponentInChildren<AnimatorControl>();
				animControl.AttackMagic(3);
				while (idx < m_MarkusMovePoints.Length)
				{
					Vector3 targetPos = transform.TransformPoint(m_MarkusMovePoints[idx].Position);
					Quaternion targetDir = Quaternion.Euler(m_MarkusMovePoints[idx].LookDirection);
					animControl.MoveTo(targetPos, targetDir);
					if (2f >= Vector3.Distance(targetPos, m_MarkusWolfView.transform.position))
					{
						idx++;
					}
					else
					{
						yield return null;
					}
				}
			}
			Monster entity = LegacyLogic.Instance.WorldManager.FindObject(m_MarkusWolfSpawnerID) as Monster;
			if (entity != null)
			{
				entity.Move(new Position(m_MarkusWolfMoveToX, m_MarkusWolfMoveToY), m_MarkusWolfLooAt, false);
			}
			yield break;
		}

		[Serializable]
		private class MoveEntity
		{
			public Int32 SpawnerID;

			public Int32 MoveToX;

			public Int32 MoveToY;

			public EDirection LookDirection;
		}

		[Serializable]
		private class MovePoint
		{
			public Vector3 Position;

			public Vector3 LookDirection;
		}
	}
}
