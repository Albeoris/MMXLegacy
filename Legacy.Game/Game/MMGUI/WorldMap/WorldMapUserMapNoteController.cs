using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.Map.Notes;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.WorldMap
{
	internal class WorldMapUserMapNoteController : MonoBehaviour
	{
		private const String WORLDMAP_ID = "theworld.xml";

		private Dictionary<Position, WorldMapUserMapNoteView> m_ActiveNotes = new Dictionary<Position, WorldMapUserMapNoteView>();

		[SerializeField]
		private GameObject m_MapNotePrefab;

		private Position m_CurrentSelectedGridPosition;

		public void OnClick()
		{
			Vector3 position = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			position = transform.InverseTransformPoint(position) + new Vector3(6f, 6f, 0f);
			m_CurrentSelectedGridPosition = WorldMapController.GetWorldMapGridPosition(position);
			MapNote mapNote = LegacyLogic.Instance.WorldManager.MapNotesController.GetMapNote("theworld.xml", m_CurrentSelectedGridPosition);
			PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.MAP_NOTES, String.Empty, null, new PopupRequest.RequestCallback(OnRequestCallback));
			PopupRequest.Instance.InputAreaText = mapNote.Note;
		}

		public String GetUserMapNoteText(Vector3 mousePosition)
		{
			Vector3 position = UICamera.currentCamera.ScreenToWorldPoint(mousePosition);
			position = transform.InverseTransformPoint(position) + new Vector3(6f, 6f, 0f);
			Position worldMapGridPosition = WorldMapController.GetWorldMapGridPosition(position);
			MapNote mapNote = LegacyLogic.Instance.WorldManager.MapNotesController.FindMapNote("theworld.xml", worldMapGridPosition);
			return (mapNote == null) ? null : mapNote.Note;
		}

		private void Start()
		{
			OnFinishSceneLoad(null, null);
			LegacyLogic.Instance.WorldManager.MapNotesController.AddedMapNote += MapNotesController_AddedMapNote;
			LegacyLogic.Instance.WorldManager.MapNotesController.RemovedMapNote += MapNotesController_RemovedMapNote;
			LegacyLogic.Instance.WorldManager.MapNotesController.UpdatedMapNoteText += MapNotesController_UpdatedMapNoteText;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishSceneLoad));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.WorldManager.MapNotesController.AddedMapNote -= MapNotesController_AddedMapNote;
			LegacyLogic.Instance.WorldManager.MapNotesController.RemovedMapNote -= MapNotesController_RemovedMapNote;
			LegacyLogic.Instance.WorldManager.MapNotesController.UpdatedMapNoteText -= MapNotesController_UpdatedMapNoteText;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishSceneLoad));
		}

		private void OnRequestCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				if (String.IsNullOrEmpty(PopupRequest.Instance.InputAreaText))
				{
					LegacyLogic.Instance.WorldManager.MapNotesController.RemoveMapNote("theworld.xml", m_CurrentSelectedGridPosition);
				}
				else
				{
					LegacyLogic.Instance.WorldManager.MapNotesController.SetMapNoteText("theworld.xml", m_CurrentSelectedGridPosition, PopupRequest.Instance.InputAreaText);
				}
			}
			else if (p_result == PopupRequest.EResultType.DELETE)
			{
				LegacyLogic.Instance.WorldManager.MapNotesController.RemoveMapNote("theworld.xml", m_CurrentSelectedGridPosition);
			}
		}

		private void OnFinishSceneLoad(Object p_sender, EventArgs p_args)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Int32 width = grid.Width;
			Int32 height = grid.Height;
			ClearMapNoteViews();
			MapNoteCollection mapNotes = LegacyLogic.Instance.WorldManager.MapNotesController.GetMapNotes("theworld.xml");
			foreach (MapNote note in mapNotes.Values)
			{
				CreateMapNoteView(note);
			}
		}

		private void MapNotesController_AddedMapNote(Object sender, MapNotesController.MapNoteEventArgs e)
		{
			if (e.MapID != "theworld.xml")
			{
				return;
			}
			if (!m_ActiveNotes.ContainsKey(e.MapNote.Position) && !String.IsNullOrEmpty(e.MapNote.Note))
			{
				CreateMapNoteView(e.MapNote);
			}
		}

		private void MapNotesController_RemovedMapNote(Object sender, MapNotesController.MapNoteEventArgs e)
		{
			if (e.MapID != "theworld.xml")
			{
				return;
			}
			RemoveMapNoteView(e.MapNote);
		}

		private void MapNotesController_UpdatedMapNoteText(Object sender, MapNotesController.MapNoteEventArgs e)
		{
			if (e.MapID != "theworld.xml")
			{
				return;
			}
			if (!m_ActiveNotes.ContainsKey(e.MapNote.Position))
			{
				CreateMapNoteView(e.MapNote);
			}
		}

		private void CreateMapNoteView(MapNote note)
		{
			GameObject gameObject = Helper.Instantiate<GameObject>(m_MapNotePrefab);
			gameObject.transform.parent = transform;
			gameObject.transform.localPosition = WorldMapController.GetWorldMapPosition(note.Position);
			gameObject.GetComponent<UIWidget>().MakePixelPerfect();
			WorldMapUserMapNoteView component = gameObject.GetComponent<WorldMapUserMapNoteView>();
			component.MapNote = note;
			component.MouseClick += NodeView_MouseClick;
			m_ActiveNotes.Add(note.Position, component);
		}

		private void RemoveMapNoteView(MapNote note)
		{
			WorldMapUserMapNoteView worldMapUserMapNoteView;
			if (m_ActiveNotes.TryGetValue(note.Position, out worldMapUserMapNoteView))
			{
				worldMapUserMapNoteView.MouseClick -= NodeView_MouseClick;
				m_ActiveNotes.Remove(note.Position);
				Helper.Destroy<GameObject>(worldMapUserMapNoteView.gameObject);
			}
		}

		private void ClearMapNoteViews()
		{
			foreach (WorldMapUserMapNoteView worldMapUserMapNoteView in m_ActiveNotes.Values)
			{
				worldMapUserMapNoteView.MouseClick -= NodeView_MouseClick;
				Destroy(worldMapUserMapNoteView.gameObject);
			}
			m_ActiveNotes.Clear();
		}

		private void NodeView_MouseClick(Object sender, EventArgs e)
		{
			OnClick();
		}
	}
}
