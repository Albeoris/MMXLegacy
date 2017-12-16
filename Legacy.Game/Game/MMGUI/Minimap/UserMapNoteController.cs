using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.Map.Notes;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	internal class UserMapNoteController : MonoBehaviour
	{
		private Dictionary<Position, UserMapNoteView> m_ActiveNotes = new Dictionary<Position, UserMapNoteView>();

		private static UserMapNoteController m_Instance;

		private Position m_CurrentSelectedGridPosition;

		[SerializeField]
		private GameObject m_MapNotePrefab;

		[SerializeField]
		private Transform m_MouseClickPlane;

		[SerializeField]
		private UICameraCustom m_TargetCamera;

		private void Awake()
		{
			m_Instance = this;
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

		private void OnFinishSceneLoad(Object p_sender, EventArgs p_args)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Int32 width = grid.Width;
			Int32 height = grid.Height;
			m_MouseClickPlane.localScale = new Vector3(width * 24, height * 24, 1f);
			Vector3 localPosition = m_MouseClickPlane.localPosition;
			localPosition.x = -12f;
			localPosition.y = -12f;
			m_MouseClickPlane.localPosition = localPosition;
			ClearMapNoteViews();
			MapNoteCollection mapNotes = LegacyLogic.Instance.WorldManager.MapNotesController.GetMapNotes();
			foreach (MapNote note in mapNotes.Values)
			{
				CreateMapNoteView(note);
			}
		}

		private void OnMouseClick()
		{
			if (UICameraCustom.current != m_TargetCamera)
			{
				return;
			}
			Vector3 position = UICameraCustom.currentCamera.ScreenToWorldPoint(UICameraCustom.currentMousePosition);
			position = transform.InverseTransformPoint(position) + new Vector3(12f, 12f, 0f);
			m_CurrentSelectedGridPosition = new Position(Mathf.FloorToInt(position.x / 24f), Mathf.FloorToInt(position.y / 24f));
			MapNote mapNote = LegacyLogic.Instance.WorldManager.MapNotesController.GetMapNote(m_CurrentSelectedGridPosition);
			PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.MAP_NOTES, String.Empty, null, new PopupRequest.RequestCallback(OnRequestCallback));
			PopupRequest.Instance.InputAreaText = mapNote.Note;
		}

		private void OnRequestCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				if (String.IsNullOrEmpty(PopupRequest.Instance.InputAreaText))
				{
					LegacyLogic.Instance.WorldManager.MapNotesController.RemoveMapNote(m_CurrentSelectedGridPosition);
				}
				else
				{
					LegacyLogic.Instance.WorldManager.MapNotesController.SetMapNoteText(m_CurrentSelectedGridPosition, PopupRequest.Instance.InputAreaText);
				}
			}
			else if (p_result == PopupRequest.EResultType.DELETE)
			{
				LegacyLogic.Instance.WorldManager.MapNotesController.RemoveMapNote(m_CurrentSelectedGridPosition);
			}
		}

		private void MapNotesController_AddedMapNote(Object sender, MapNotesController.MapNoteEventArgs e)
		{
			if (e.MapID != LegacyLogic.Instance.MapLoader.GridFileName)
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
			if (e.MapID != LegacyLogic.Instance.MapLoader.GridFileName)
			{
				return;
			}
			RemoveMapNoteView(e.MapNote);
		}

		private void MapNotesController_UpdatedMapNoteText(Object sender, MapNotesController.MapNoteEventArgs e)
		{
			if (e.MapID != LegacyLogic.Instance.MapLoader.GridFileName)
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
			gameObject.transform.localPosition = new Vector3(note.Position.X * 24, note.Position.Y * 24, 0f);
			gameObject.GetComponent<UIWidget>().MakePixelPerfect();
			UserMapNoteView component = gameObject.GetComponent<UserMapNoteView>();
			component.MapNote = note;
			component.MouseClick += NodeView_MouseClick;
			m_ActiveNotes.Add(note.Position, component);
		}

		private void RemoveMapNoteView(MapNote note)
		{
			UserMapNoteView userMapNoteView;
			if (m_ActiveNotes.TryGetValue(note.Position, out userMapNoteView))
			{
				userMapNoteView.MouseClick -= NodeView_MouseClick;
				m_ActiveNotes.Remove(note.Position);
				Helper.Destroy<GameObject>(userMapNoteView.gameObject);
			}
		}

		private void ClearMapNoteViews()
		{
			foreach (UserMapNoteView userMapNoteView in m_ActiveNotes.Values)
			{
				userMapNoteView.MouseClick -= NodeView_MouseClick;
				Destroy(userMapNoteView.gameObject);
			}
			m_ActiveNotes.Clear();
		}

		private void NodeView_MouseClick(Object sender, EventArgs e)
		{
			OnMouseClick();
		}

		internal static void ProcessMouseClick()
		{
			if (m_Instance != null)
			{
				m_Instance.OnMouseClick();
			}
		}
	}
}
