using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy.World
{
	public class SpiritBeaconController : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_MarkerPrefab;

		private GameObject m_MarkerInstance;

		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnUpdateSpiritBeacon));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SPIRIT_BEACON_UPDATE, new EventHandler(OnUpdateSpiritBeacon));
		}

		private void Destroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnUpdateSpiritBeacon));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SPIRIT_BEACON_UPDATE, new EventHandler(OnUpdateSpiritBeacon));
		}

		private void OnUpdateSpiritBeacon(Object sender, EventArgs args)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			if (grid != null && grid.Type == EMapType.OUTDOOR && LegacyLogic.Instance.WorldManager.SpiritBeaconController.Existent)
			{
				SpiritBeaconPosition spiritBeacon = LegacyLogic.Instance.WorldManager.SpiritBeaconController.SpiritBeacon;
				Vector3 vector = Helper.SlotLocalPosition(spiritBeacon.Position, grid.GetSlot(spiritBeacon.Position).Height);
				ParticleSystem componentInChildren;
				if (m_MarkerInstance != null)
				{
					if (!(m_MarkerInstance.transform.localPosition != vector))
					{
						return;
					}
					componentInChildren = m_MarkerInstance.GetComponentInChildren<ParticleSystem>();
					if (componentInChildren != null)
					{
						componentInChildren.Stop(true);
					}
					UnityEngine.Object.Destroy(m_MarkerInstance, 10f);
				}
				Transform gridOrigin = ViewManager.Instance.GridOrigin;
				m_MarkerInstance = Helper.Instantiate<GameObject>(m_MarkerPrefab, gridOrigin.position + vector);
				m_MarkerInstance.transform.parent = gridOrigin;
				m_MarkerInstance.transform.localPosition = vector;
				componentInChildren = m_MarkerInstance.GetComponentInChildren<ParticleSystem>();
				if (componentInChildren != null)
				{
					componentInChildren.Play(true);
				}
			}
		}
	}
}
