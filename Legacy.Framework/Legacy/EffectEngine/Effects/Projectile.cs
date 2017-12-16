using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class Projectile : FXBase
	{
		public GameObject m_Prefab;

		public String m_StartSound;

		public String m_FlightLoopSound;

		public String m_HitSound;

		public Single m_Speed = 1f;

		public Vector2 m_CurveHorizontal;

		public Vector2 m_CurveVertical;

		public String m_Tag;

		public Vector3 m_TargetOffset;

		public Vector3 m_Scale = Vector3.one;

		public Boolean m_SpawnAndForget;

		public Boolean m_AttachOnCollision;

		public Single m_DestroyDelay;

		private Transform m_Instance;

		private Single m_TimeLength;

		private Single m_FinishMoveTime;

		private Vector3 m_Start;

		private Vector3 m_StartTangent;

		private Vector3 m_EndTangent;

		private AudioObject m_LoopSound;

		public override void Begin(Single p_lifetime, FXArgs p_args)
		{
			base.Begin(p_lifetime, p_args);
			Transform transform;
			if (!String.IsNullOrEmpty(m_Tag))
			{
				FXTags component = p_args.Origin.GetComponent< FXTags>(true);
				if (component == null)
				{
					Debug.Log("Effect require FXTags component on " + p_args.Origin, p_args.Origin);
					ForceFinish();
					return;
				}
				transform = component.FindOne(m_Tag).transform;
			}
			else
			{
				transform = p_args.BeginPointTransform;
			}
			m_Start = transform.position;
			m_StartTangent = p_args.EndPointTransform.position - m_Start;
			Single magnitude = m_StartTangent.magnitude;
			m_StartTangent.Normalize();
			m_StartTangent = Quaternion.LookRotation(m_StartTangent) * Quaternion.Euler(-Random.Range(m_CurveVertical.x, m_CurveVertical.y), Random.Range(m_CurveHorizontal.x, m_CurveHorizontal.y), 0f) * new Vector3(0f, 0f, magnitude);
			m_TimeLength = magnitude / Math.Max(m_Speed, 0.0001f);
			m_FinishMoveTime = Time.time + m_TimeLength;
			GameObject gameObject = Helper.Instantiate<GameObject>(m_Prefab, m_Start);
			m_Instance = gameObject.transform;
			m_Instance.localScale = m_Scale;
			if (!String.IsNullOrEmpty(m_StartSound))
			{
				AudioController.Play(m_StartSound, m_Start, null);
			}
			if (!String.IsNullOrEmpty(m_FlightLoopSound))
			{
				m_LoopSound = AudioController.Play(m_FlightLoopSound, m_Instance);
			}
			Update();
			SendBroadcastBeginEffect<UnityEventArgs<FXArgs>>(gameObject, new UnityEventArgs<FXArgs>(this, p_args));
		}

		public override void Update()
		{
			Transform endPointTransform = EffectArguments.EndPointTransform;
			Single num = 1f;
			if (Time.time < m_FinishMoveTime)
			{
				num = Mathf.Clamp01(1f - (m_FinishMoveTime - Time.time) / m_TimeLength);
			}
			Vector3 vector = endPointTransform.position + m_TargetOffset;
			Vector3 vector2;
			Helper.Hermite(ref m_Start, ref m_StartTangent, ref vector, ref m_EndTangent, num, out vector2);
			if (num != 0f)
			{
				m_Instance.LookAt(vector2);
			}
			else
			{
				m_Instance.LookAt(vector2 + m_StartTangent);
			}
			m_Instance.position = vector2;
			if (Helper.Equals(ref vector2, ref vector, 0.005f))
			{
				if (m_AttachOnCollision)
				{
					m_Instance.parent = endPointTransform;
				}
				if (!String.IsNullOrEmpty(m_HitSound))
				{
					AudioController.Play(m_HitSound, endPointTransform.position, null);
				}
				StopLoopSound();
				ForceFinish();
			}
		}

		public override void Destroy()
		{
			StopLoopSound();
			if (m_Instance != null)
			{
				SendBroadcastEndEffect<UnityEventArgs<FXArgs>>(m_Instance.gameObject, new UnityEventArgs<FXArgs>(this, EffectArguments));
				if (!m_SpawnAndForget)
				{
					Helper.DestroyGO<Transform>(ref m_Instance, Math.Max(0f, m_DestroyDelay));
				}
				m_Instance = null;
			}
		}

		private void StopLoopSound()
		{
			if (m_LoopSound != null && m_LoopSound.IsPlaying())
			{
				m_LoopSound.Stop();
			}
			m_LoopSound = null;
		}
	}
}
