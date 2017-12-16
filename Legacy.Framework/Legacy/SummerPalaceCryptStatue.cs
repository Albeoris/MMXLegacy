using System;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/SummerPalaceCrypt Statue")]
	public class SummerPalaceCryptStatue : BaseView
	{
		[SerializeField]
		private String m_activateSound = "Gold_loot";

		[SerializeField]
		private String m_finalActivateSound = "Gold_loot";

		[SerializeField]
		private GameObject m_StatueObject;

		[SerializeField]
		private Transform m_InitialPosition;

		[SerializeField]
		private Transform m_TargetPosition;

		[SerializeField]
		private Light m_targetLight;

		[SerializeField]
		private ParticleSystem m_targetEffect;

		[SerializeField]
		private Single m_soundVolume = 1f;

		[SerializeField]
		private Single m_animTime = 1f;

		[SerializeField]
		private AnimationCurve m_lerpCurvePos;

		[SerializeField]
		private AnimationCurve m_lerpCurveRot;

		private Single mTimer;

		protected override void Awake()
		{
			base.Awake();
		}

		protected new InteractiveObject MyController => (InteractiveObject)base.MyController;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
				ObjectState(MyController.State, true);
			}
		}

		private void OnInteractiveObjectStateChanged(Object p_sender, EventArgs p_args)
		{
			InteractiveObject interactiveObject;
			if (p_sender is InteractiveObject)
			{
				interactiveObject = (InteractiveObject)p_sender;
			}
			else
			{
				BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
				interactiveObject = (InteractiveObject)baseObjectEventArgs.Object;
			}
			if (interactiveObject == MyController)
			{
				ObjectState(interactiveObject.State, false);
			}
		}

		public void ObjectState(EInteractiveObjectState p_state, Boolean p_setup)
		{
			StopAllCoroutines();
			if (p_state == EInteractiveObjectState.ON)
			{
				if (p_setup)
				{
					if (m_StatueObject != null && m_InitialPosition != null && m_TargetPosition != null)
					{
						m_StatueObject.transform.position = m_TargetPosition.position;
						m_StatueObject.transform.localRotation = m_TargetPosition.localRotation;
					}
					if (m_targetLight != null)
					{
						m_targetLight.enabled = true;
					}
					if (m_targetEffect != null)
					{
						m_targetEffect.Play(true);
					}
				}
				else
				{
					enabled = true;
					mTimer = 0f;
					AudioManager.Instance.RequestPlayAudioID(m_activateSound, 0, -1f, transform, m_soundVolume, 0f, 0f, null);
				}
			}
			else
			{
				InitializeStartSetup();
				AudioController.Stop(m_activateSound);
			}
		}

		private void Update()
		{
			mTimer += Time.deltaTime / m_animTime;
			if (mTimer >= 1f)
			{
				enabled = false;
				mTimer = 1f;
				if (m_targetLight != null)
				{
					m_targetLight.enabled = true;
				}
				if (m_targetEffect != null)
				{
					m_targetEffect.Play(true);
				}
			}
			if (m_StatueObject != null && m_InitialPosition != null && m_TargetPosition != null)
			{
				m_StatueObject.transform.position = Vector3.Lerp(m_InitialPosition.position, m_TargetPosition.position, m_lerpCurvePos.Evaluate(mTimer));
				m_StatueObject.transform.localRotation = Quaternion.Lerp(m_InitialPosition.localRotation, m_TargetPosition.localRotation, m_lerpCurveRot.Evaluate(mTimer));
			}
		}

		private void InitializeStartSetup()
		{
			if (m_StatueObject != null && m_InitialPosition != null && m_TargetPosition != null)
			{
				m_StatueObject.transform.position = m_InitialPosition.position;
				m_StatueObject.transform.localRotation = m_InitialPosition.localRotation;
			}
			if (m_targetLight != null)
			{
				m_targetLight.enabled = false;
			}
			if (m_targetEffect != null)
			{
				m_targetEffect.Stop(true);
			}
			mTimer = 0f;
		}
	}
}
