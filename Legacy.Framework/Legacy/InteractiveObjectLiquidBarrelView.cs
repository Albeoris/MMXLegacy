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
	[AddComponentMenu("MM Legacy/Views/Liquid Barrel View")]
	public class InteractiveObjectLiquidBarrelView : BaseView
	{
		[SerializeField]
		private Transform m_objectRoot;

		[SerializeField]
		private GameObject m_liquidObject;

		[SerializeField]
		private GameObject m_closedObject;

		[SerializeField]
		private GameObject m_openObject;

		[SerializeField]
		private GameObject m_emptyObject;

		[SerializeField]
		private GameObject m_openEffect;

		[SerializeField]
		private GameObject m_drinkEffect;

		[SerializeField]
		private GameObject m_openParticleEffect;

		[SerializeField]
		private GameObject m_drinkParticleEffect;

		[SerializeField]
		private Single m_openDelay;

		[SerializeField]
		private Single m_drinkDelay;

		[SerializeField]
		private String m_openSound = "Gold_loot";

		[SerializeField]
		private String m_drinkSound = "Gold_loot";

		[SerializeField]
		private Single m_soundVolume = 1f;

		private GameObject m_currentObject;

		private GameObject m_currentLiquid;

		public void ObjectState(EInteractiveObjectState p_stateValue, Boolean p_skipAnimation)
		{
			switch (p_stateValue)
			{
			default:
				Invoke("Close", 0f);
				break;
			case EInteractiveObjectState.BARREL_OPEN:
				if (!p_skipAnimation)
				{
					AudioManager.Instance.RequestPlayAudioID(m_openSound, 1, -1f, transform, m_soundVolume, 0f, 0f, null);
					SpawnEffect(m_openEffect);
					SpawnEffect(m_openParticleEffect);
				}
				Invoke("Open", (!p_skipAnimation) ? m_openDelay : 0f);
				break;
			case EInteractiveObjectState.BARREL_EMPTY:
				if (!p_skipAnimation)
				{
					AudioManager.Instance.RequestPlayAudioID(m_drinkSound, 1, -1f, transform, m_soundVolume, 0f, 0f, null);
					SpawnEffect(m_drinkEffect);
					SpawnEffect(m_drinkParticleEffect);
				}
				Invoke("Drink", (!p_skipAnimation) ? m_drinkDelay : 0f);
				break;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			ObjectState(EInteractiveObjectState.BARREL_CLOSED, true);
		}

		protected new Barrel MyController => (Barrel)base.MyController;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.BARREL_STATE_CHANGE, new EventHandler(OnInteractiveObjectStateChanged));
			}
			if (MyController != null)
			{
				ObjectState(MyController.State, true);
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.BARREL_STATE_CHANGE, new EventHandler(OnInteractiveObjectStateChanged));
			}
		}

		private void OnInteractiveObjectStateChanged(Object p_sender, EventArgs p_args)
		{
			InteractiveObject interactiveObject = (InteractiveObject)p_sender;
			if (interactiveObject == MyController)
			{
				ObjectState(interactiveObject.State, false);
			}
		}

		private void Open()
		{
			SetGameObject(m_openObject, true);
		}

		private void Drink()
		{
			SetGameObject(m_emptyObject, false);
		}

		private void Close()
		{
			SetGameObject(m_closedObject, false);
		}

		private void SpawnEffect(GameObject p_effect)
		{
			if (p_effect != null)
			{
				GameObject gameObject = (GameObject)Instantiate(p_effect.gameObject, m_objectRoot.position, m_objectRoot.rotation);
				gameObject.transform.parent = transform;
			}
		}

		private void SetGameObject(GameObject obj, Boolean showLiquid)
		{
			if (m_currentObject != null && obj != null)
			{
				Destroy(m_currentObject);
			}
			if (obj != null)
			{
				m_currentObject = (GameObject)Instantiate(obj, m_objectRoot.position, m_objectRoot.rotation);
				m_currentObject.transform.parent = transform;
			}
			if (showLiquid && m_liquidObject != null)
			{
				m_currentLiquid = (GameObject)Instantiate(m_liquidObject, m_objectRoot.position, m_objectRoot.rotation);
				m_currentLiquid.transform.parent = transform;
			}
			else if (!showLiquid && m_currentLiquid != null)
			{
				Destroy(m_currentLiquid);
			}
		}
	}
}
