using System;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Audio/Ambient Sound Source")]
	public class AmbientSoundSource : AmbientSoundSourceBase
	{
		[SerializeField]
		private Single m_ActivateWithinRange = 100f;

		[SerializeField]
		private Single m_AudioSourceMinDist = 15f;

		[SerializeField]
		private Single m_AudioSourceMaxDist = 100f;

		private Transform m_MainCamera;

		public Single ActivateWithinRange
		{
			get => m_ActivateWithinRange;
		    set => m_ActivateWithinRange = value;
		}

		public Single AudioSourceMinDist
		{
			get => m_AudioSourceMinDist;
		    set => m_AudioSourceMinDist = value;
		}

		public Single AudioSourceMaxDist
		{
			get => m_AudioSourceMaxDist;
		    set => m_AudioSourceMaxDist = value;
		}

		protected override Boolean IsSoundPlayable()
		{
			return base.IsSoundPlayable() && m_MainCamera != null && (m_MainCamera.position - transform.position).sqrMagnitude < m_ActivateWithinRange * m_ActivateWithinRange;
		}

		protected override void OnNewAudioObject()
		{
			m_AudioObj.audio.minDistance = m_AudioSourceMinDist;
			m_AudioObj.audio.maxDistance = m_AudioSourceMaxDist;
		}

		protected override void Start()
		{
			base.Start();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnPartyPosChange));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnPartyPosChange));
		}

		protected override void Update()
		{
			if (m_MainCamera == null)
			{
				if (FXMainCamera.Instance != null)
				{
					m_MainCamera = FXMainCamera.Instance.DefaultCamera.transform;
				}
			}
			else
			{
				base.Update();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnPartyPosChange));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnPartyPosChange));
		}

		private void OnPartyPosChange(Object p_sender, EventArgs p_args)
		{
			if (p_args is BaseObjectEventArgs)
			{
				BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
				if (baseObjectEventArgs != null && baseObjectEventArgs.Object == LegacyLogic.Instance.WorldManager.Party)
				{
					Reactivate();
				}
			}
			else if (p_args is MoveEntityEventArgs)
			{
				MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
				if (moveEntityEventArgs != null && p_sender == LegacyLogic.Instance.WorldManager.Party)
				{
					Reactivate();
				}
			}
		}

		private void Reactivate()
		{
			m_ReactivateAt = Mathf.Min(m_ReactivateAt, Time.time + Random.Range(0f, 0.5f));
		}
	}
}
