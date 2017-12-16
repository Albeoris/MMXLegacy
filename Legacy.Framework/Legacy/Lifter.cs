using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	public class Lifter : MonoBehaviour
	{
		[SerializeField]
		private String m_viewListenCommandName;

		[SerializeField]
		private Int32 m_MonsterID;

		[SerializeField]
		public Single m_Speed = 1f;

		[SerializeField]
		public Single m_RotationSpeed = 1f;

		[SerializeField]
		public Single m_DeltaPosition = 1f;

		[SerializeField]
		public Single m_Angle = 1f;

		[SerializeField]
		public Boolean m_RandomDelay;

		[SerializeField]
		public Single m_DelayMin;

		[SerializeField]
		public Single m_DelayMax = 5f;

		private Vector3 m_StartPos;

		private Vector3 m_MaxPos;

		private Vector3 m_MinPos;

		private Quaternion m_EndRotation;

		private Quaternion m_StartRotation;

		private Boolean m_RotationUp;

		private Boolean m_CurrentPosUp;

		private Boolean m_DestinationPosUp;

		private Boolean m_IsFirstTime = true;

		private Boolean m_CoroutineInProgess;

		private Single m_timer;

		private Single m_rottimer;

		private Boolean m_StopByTrigger;

		protected void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTERS_TURNED_AGGRO, new EventHandler(OnAggroTriggered));
		}

		private void OnAnimTriggered(Object p_sender, EventArgs p_args)
		{
			StringEventArgs stringEventArgs = p_args as StringEventArgs;
			if (stringEventArgs != null)
			{
				String[] array = stringEventArgs.text.Split(new Char[]
				{
					'_'
				});
				if (array.Length > 1 && array[0] == m_viewListenCommandName && array[1] == "Activate")
				{
					m_StopByTrigger = true;
				}
			}
		}

		private void OnAggroTriggered(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			Monster monster = (Monster)baseObjectEventArgs.Object;
			if (monster.StaticData.StaticID == m_MonsterID)
			{
				m_StopByTrigger = false;
			}
		}

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTERS_TURNED_AGGRO, new EventHandler(OnAggroTriggered));
			m_StartPos = transform.position;
			m_MaxPos = transform.position + new Vector3(0f, 1f, 0f) * (m_DeltaPosition / 2f);
			m_MinPos = transform.position - new Vector3(0f, 1f, 0f) * (m_DeltaPosition / 2f);
			m_StartRotation = transform.rotation;
		}

		private void Update()
		{
			if (m_StopByTrigger)
			{
				StopCoroutine("Updater");
				return;
			}
			if (!m_CoroutineInProgess)
			{
				StartCoroutine("Updater");
			}
		}

		private IEnumerator Updater()
		{
			if (m_IsFirstTime && m_RandomDelay)
			{
				m_CoroutineInProgess = true;
				m_IsFirstTime = false;
				yield return new WaitForSeconds(Random.Range(m_DelayMin, m_DelayMax));
				m_CoroutineInProgess = false;
			}
			LifterMovement();
			RoatatorSwap();
			yield break;
		}

		private void RoatatorSwap()
		{
			m_rottimer += Time.deltaTime * m_RotationSpeed;
			if (m_RotationUp)
			{
				m_EndRotation = Quaternion.AngleAxis(m_Angle, new Vector3(1f, 1f, 0f));
			}
			else
			{
				m_EndRotation = Quaternion.AngleAxis(m_Angle, new Vector3(-1f, -1f, 0f));
			}
			if (transform.rotation == m_EndRotation)
			{
				m_StartRotation = m_EndRotation;
				m_RotationUp = !m_RotationUp;
				m_rottimer = 0f;
			}
			transform.rotation = Quaternion.Lerp(m_StartRotation, m_EndRotation, m_rottimer);
		}

		private void LifterMovement()
		{
			m_timer += Time.deltaTime * m_Speed;
			if (!m_CurrentPosUp && !m_DestinationPosUp)
			{
				transform.position = Vector3.Lerp(m_StartPos, m_MinPos, m_timer);
				if (transform.position == m_MinPos)
				{
					m_timer = 0f;
					m_DestinationPosUp = true;
				}
			}
			else if (!m_CurrentPosUp && m_DestinationPosUp)
			{
				transform.position = Vector3.Lerp(m_MinPos, m_StartPos, m_timer);
				if (transform.position == m_StartPos)
				{
					m_timer = 0f;
					m_CurrentPosUp = true;
				}
			}
			else if (m_CurrentPosUp && m_DestinationPosUp)
			{
				transform.position = Vector3.Lerp(m_StartPos, m_MaxPos, m_timer);
				if (transform.position == m_MaxPos)
				{
					m_timer = 0f;
					m_DestinationPosUp = false;
				}
			}
			else if (m_CurrentPosUp && !m_DestinationPosUp)
			{
				transform.position = Vector3.Lerp(m_MaxPos, m_StartPos, m_timer);
				if (transform.position == m_StartPos)
				{
					m_timer = 0f;
					m_CurrentPosUp = false;
				}
			}
		}
	}
}
