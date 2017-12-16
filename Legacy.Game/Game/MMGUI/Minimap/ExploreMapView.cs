using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Pathfinding;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/ExploreMapView")]
	public class ExploreMapView : MonoBehaviour
	{
		private Color32[] m_TextureBuffer;

		private Color32[] m_AnimationBuffer;

		private Texture2D m_ExploreTexture;

		private Single m_UpdateTime;

		[SerializeField]
		private UITexture m_UITexture;

		private Boolean m_StopUpdate;

		private static Quaternion m_ViewRotation = Quaternion.AngleAxis(45f, Vector3.forward);

		private void Start()
		{
			enabled = false;
			transform.localPosition = new Vector3(-12f, -12f, transform.localPosition.z);
			m_ExploreTexture = new Texture2D(1, 1, TextureFormat.Alpha8, false);
			m_ExploreTexture.filterMode = FilterMode.Point;
			m_ExploreTexture.Apply(false);
			m_ExploreTexture.hideFlags = HideFlags.DontSave;
			m_UITexture.mainTexture = m_ExploreTexture;
			if (!LegacyLogic.Instance.MapLoader.IsLoading)
			{
				RebuildExploreMap();
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnPartyMoved));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnPartyMoved));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DOOR_STATE_CHANGED, new EventHandler(OnDoorStateChanged));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnPartyMoved));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnPartyMoved));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DOOR_STATE_CHANGED, new EventHandler(OnDoorStateChanged));
			Helper.DestroyImmediate<Texture2D>(ref m_ExploreTexture);
		}

		private void Update()
		{
			if ((m_UpdateTime += Time.deltaTime) < 0.04f)
			{
				return;
			}
			m_UpdateTime = 0f;
			Boolean flag = false;
			Int32 num = 0;
			while (num < m_TextureBuffer.Length && num < m_AnimationBuffer.Length)
			{
				flag |= (m_TextureBuffer[num].a != m_AnimationBuffer[num].a);
				m_TextureBuffer[num] = Color32.Lerp(m_TextureBuffer[num], m_AnimationBuffer[num], 0.1f);
				num++;
			}
			m_ExploreTexture.SetPixels32(m_TextureBuffer);
			m_ExploreTexture.Apply(false);
			enabled = flag;
		}

		private void OnStartSceneLoad(Object p_sender, EventArgs p_args)
		{
			m_StopUpdate = true;
		}

		private void OnFinishSceneLoad(Object p_sender, EventArgs p_args)
		{
			m_StopUpdate = false;
			RebuildExploreMap();
		}

		private void OnPartyMoved(Object p_sender, EventArgs p_args)
		{
			if (m_StopUpdate)
			{
				return;
			}
			Party party = p_sender as Party;
			if (party == null)
			{
				return;
			}
			UpdateExploreView(party, false);
		}

		private void OnDoorStateChanged(Object p_sender, EventArgs p_args)
		{
			if (m_StopUpdate)
			{
				return;
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			UpdateExploreView(party, true);
		}

		private void UpdateExploreView(Party party, Boolean force)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Int32 num = Mathf.NextPowerOfTwo(grid.Width);
			Int32 bufferHeight = Mathf.NextPowerOfTwo(grid.Height);
			Position position = party.Position;
			Int32 num2 = position.Y * num + position.X;
			if (m_AnimationBuffer != null && (force || m_AnimationBuffer[num2].a > 0))
			{
				Int32 range = party.ExploreRange + 1;
				ExploreView(grid, m_AnimationBuffer, num, bufferHeight, party.Position, range);
				enabled = true;
			}
		}

		private void RebuildExploreMap()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Int32 num = Mathf.NextPowerOfTwo(grid.Width);
			Int32 num2 = Mathf.NextPowerOfTwo(grid.Height);
			if (m_TextureBuffer == null || m_TextureBuffer.Length != num * num2)
			{
				m_TextureBuffer = new Color32[num * num2];
			}
			if (m_AnimationBuffer == null || m_AnimationBuffer.Length != num * num2)
			{
				m_AnimationBuffer = new Color32[num * num2];
			}
			Int32 num3 = 0;
			while (num3 < m_TextureBuffer.Length && num3 < m_AnimationBuffer.Length)
			{
				m_TextureBuffer[num3] = (m_AnimationBuffer[num3] = new Color32(0, 0, 0, Byte.MaxValue));
				num3++;
			}
			m_ExploreTexture.Resize(num, num2, TextureFormat.Alpha8, false);
			m_ExploreTexture.SetPixels32(m_TextureBuffer);
			m_ExploreTexture.Apply(false);
			transform.localScale = new Vector3(num * 24, num2 * 24, 1f);
			Vector3 localPosition = transform.localPosition;
			localPosition.x = -12f;
			localPosition.y = -12f;
			transform.localPosition = localPosition;
			Int32 range = LegacyLogic.Instance.WorldManager.Party.ExploreRange + 1;
			foreach (GridSlot gridSlot in grid.SlotIterator())
			{
				if (gridSlot.VisitedByParty)
				{
					ExploreView(grid, m_AnimationBuffer, num, num2, gridSlot.Position, range);
				}
			}
			enabled = true;
		}

		private static void ExploreView(Grid grid, Color32[] buffer, Int32 bufferWidth, Int32 bufferHeight, Position viewPosition, Int32 range)
		{
			Position position = viewPosition - new Position(range, range);
			Position position2 = viewPosition + new Position(range, range);
			position.X = Mathf.Clamp(position.X, 0, grid.Width - 1);
			position.Y = Mathf.Clamp(position.Y, 0, grid.Height - 1);
			position2.X = Mathf.Clamp(position2.X, 0, grid.Width - 1);
			position2.Y = Mathf.Clamp(position2.Y, 0, grid.Height - 1);
			OrientedBoundingBox orientedBoundingBox = default(OrientedBoundingBox);
			orientedBoundingBox.Extents = new Vector3(range - 0.5f, range - 0.5f, range - 0.5f);
			orientedBoundingBox.Transformation = Matrix4x4.TRS(new Vector3(viewPosition.X, viewPosition.Y, 0f), m_ViewRotation, Vector3.one);
			Vector3 point = default(Vector3);
			Position position3;
			position3.Y = position.Y;
			while (position3.Y <= position2.Y)
			{
				position3.X = position.X;
				while (position3.X <= position2.X)
				{
					Byte val = Byte.MaxValue;
					Int32 num = position3.Y * bufferWidth + position3.X;
					if (position3 == viewPosition)
					{
						val = 0;
					}
					else
					{
						point.x = position3.X;
						point.y = position3.Y;
						if (orientedBoundingBox.Contains(point))
						{
							GridSlot slot = grid.GetSlot(position3);
							if (slot != null && (slot.TerrainType & ETerrainType.BLOCKED) != ETerrainType.BLOCKED && InMoveRange(grid, viewPosition, position3, range))
							{
								val = 178;
							}
						}
					}
					buffer[num].a = Math.Min(buffer[num].a, val);
					position3.X++;
				}
				position3.Y++;
			}
		}

		private static Boolean InMoveRange(Grid grid, Position start, Position target, Int32 range)
		{
			GridSlot slot = grid.GetSlot(start);
			GridSlot slot2 = grid.GetSlot(target);
			Int32 num = AStarHelper<GridSlot>.Calculate(slot, slot2, range, null, false, null);
			return num > 0;
		}
	}
}
