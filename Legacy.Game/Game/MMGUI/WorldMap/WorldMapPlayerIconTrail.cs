using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.WorldMap
{
	[AddComponentMenu("MM Legacy/MMGUI/WorldMap player icon trail")]
	internal class WorldMapPlayerIconTrail : MonoBehaviour
	{
		private const Int32 TRAIL_LENGTH = 16;

		[SerializeField]
		private GameObject m_SpritePrefab;

		private Int32 m_MoveIndex;

		private UISprite[] m_TrailInstances;

		private void Awake()
		{
			if (m_TrailInstances == null)
			{
				m_TrailInstances = new UISprite[16];
				for (Int32 i = 0; i < m_TrailInstances.Length; i++)
				{
					UISprite component = NGUITools.AddChild(gameObject, m_SpritePrefab.gameObject).GetComponent<UISprite>();
					component.gameObject.SetActive(false);
					component.alpha = 0f;
					component.MakePixelPerfect();
					m_TrailInstances[i] = component;
				}
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnEntityChangePosition));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnEntityChangePosition));
		}

		private void OnStartSceneLoad(Object sender, EventArgs e)
		{
			foreach (UISprite uisprite in m_TrailInstances)
			{
				uisprite.gameObject.SetActive(false);
				uisprite.alpha = 0f;
			}
			m_MoveIndex = 0;
		}

		private void OnEntityChangePosition(Object sender, EventArgs e)
		{
			if (e is BaseObjectEventArgs)
			{
				sender = ((BaseObjectEventArgs)e).Object;
			}
			if (sender is Party)
			{
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				if (grid.Type != EMapType.OUTDOOR)
				{
					return;
				}
				Vector3 worldMapPosition = WorldMapController.GetWorldMapPosition(((Party)sender).Position);
				UISprite uisprite = m_TrailInstances[m_MoveIndex];
				uisprite.gameObject.SetActive(true);
				uisprite.transform.localPosition = worldMapPosition;
				m_MoveIndex++;
				if (m_MoveIndex >= m_TrailInstances.Length)
				{
					m_MoveIndex = 0;
				}
				Int32 num = m_TrailInstances.Length;
				for (Int32 i = 0; i < num; i++)
				{
					Int32 num2 = i + m_MoveIndex;
					if (num2 >= num)
					{
						num2 -= num;
					}
					TweenAlpha.Begin(m_TrailInstances[num2].gameObject, 1f, 1f * (i / (num - 1f)));
				}
			}
		}
	}
}
