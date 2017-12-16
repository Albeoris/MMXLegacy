using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Map.Notes
{
	public class MapNotesController
	{
		private Dictionary<String, MapNoteCollection> m_MapNotes = new Dictionary<String, MapNoteCollection>();

		public event EventHandler<MapNoteEventArgs> AddedMapNote;

		public event EventHandler<MapNoteEventArgs> RemovedMapNote;

		public event EventHandler<MapNoteEventArgs> UpdatedMapNoteText;

		public MapNoteCollection FindMapNotes(String mapId)
		{
			MapNoteCollection result;
			m_MapNotes.TryGetValue(mapId, out result);
			return result;
		}

		public MapNoteCollection GetMapNotes(String mapId)
		{
			MapNoteCollection result;
			if (!m_MapNotes.TryGetValue(mapId, out result))
			{
				m_MapNotes.Add(mapId, result = new MapNoteCollection());
			}
			return result;
		}

		public MapNoteCollection GetMapNotes()
		{
			return GetMapNotes(LegacyLogic.Instance.MapLoader.GridFileName);
		}

		public MapNote GetMapNote(Position position)
		{
			String gridFileName = LegacyLogic.Instance.MapLoader.GridFileName;
			return GetMapNote(gridFileName, position);
		}

		public MapNote FindMapNote(String mapId, Position position)
		{
			MapNoteCollection mapNoteCollection = FindMapNotes(mapId);
			MapNote result;
			if (mapNoteCollection != null && mapNoteCollection.TryGetValue(position, out result))
			{
				return result;
			}
			return null;
		}

		public MapNote GetMapNote(String mapId, Position position)
		{
			MapNoteCollection mapNotes = GetMapNotes(mapId);
			MapNote mapNote;
			if (!mapNotes.TryGetValue(position, out mapNote))
			{
				mapNote = new MapNote(mapId, position, null);
				mapNotes.Add(position, mapNote);
				if (AddedMapNote != null)
				{
					AddedMapNote(this, new MapNoteEventArgs(mapId, mapNote));
				}
			}
			return mapNote;
		}

		public void SetMapNoteText(Position position, String text)
		{
			String gridFileName = LegacyLogic.Instance.MapLoader.GridFileName;
			SetMapNoteText(gridFileName, position, text);
		}

		public void SetMapNoteText(String mapId, Position position, String text)
		{
			MapNoteCollection mapNotes = GetMapNotes(mapId);
			MapNote mapNote;
			Boolean flag;
			if (!mapNotes.TryGetValue(position, out mapNote))
			{
				mapNote = new MapNote(mapId, position, text);
				mapNotes.Add(position, mapNote);
				flag = true;
			}
			else
			{
				flag = (mapNote.Note != text);
				mapNote.Note = text;
			}
			if (flag && UpdatedMapNoteText != null)
			{
				UpdatedMapNoteText(this, new MapNoteEventArgs(mapId, mapNote));
			}
		}

		public void RemoveMapNote(Position position)
		{
			String gridFileName = LegacyLogic.Instance.MapLoader.GridFileName;
			RemoveMapNote(gridFileName, position);
		}

		public void RemoveMapNote(String mapId, Position position)
		{
			MapNoteCollection mapNotes = GetMapNotes(mapId);
			MapNote mapNote;
			if (mapNotes.TryGetValue(position, out mapNote) && mapNotes.Remove(position) && RemovedMapNote != null)
			{
				RemovedMapNote(this, new MapNoteEventArgs(mapId, mapNote));
			}
		}

		public void Clear()
		{
			while (m_MapNotes.Count > 0)
			{
				Dictionary<String, MapNoteCollection>.Enumerator enumerator = m_MapNotes.GetEnumerator();
				if (enumerator.MoveNext())
				{
					KeyValuePair<String, MapNoteCollection> keyValuePair = enumerator.Current;
					String key = keyValuePair.Key;
					KeyValuePair<String, MapNoteCollection> keyValuePair2 = enumerator.Current;
					RemoveAllNotes(key, keyValuePair2.Value);
					Dictionary<String, MapNoteCollection> mapNotes = m_MapNotes;
					KeyValuePair<String, MapNoteCollection> keyValuePair3 = enumerator.Current;
					mapNotes.Remove(keyValuePair3.Key);
				}
			}
			m_MapNotes.Clear();
		}

		private void RemoveAllNotes(String mapId, MapNoteCollection collection)
		{
			MapNoteEventArgs mapNoteEventArgs = new MapNoteEventArgs(null, null);
			while (collection.Count > 0)
			{
				Dictionary<Position, MapNote>.Enumerator enumerator = collection.GetEnumerator();
				if (enumerator.MoveNext())
				{
					KeyValuePair<Position, MapNote> keyValuePair = enumerator.Current;
					collection.Remove(keyValuePair.Key);
					if (RemovedMapNote != null)
					{
						mapNoteEventArgs.MapID = mapId;
						MapNoteEventArgs mapNoteEventArgs2 = mapNoteEventArgs;
						KeyValuePair<Position, MapNote> keyValuePair2 = enumerator.Current;
						mapNoteEventArgs2.MapNote = keyValuePair2.Value;
						RemovedMapNote(this, mapNoteEventArgs);
					}
				}
			}
			collection.Clear();
		}

		private void Cleanup()
		{
			List<String> list = new List<String>();
			String mapFolder = LegacyLogic.Instance.MapLoader.MapFolder;
			foreach (KeyValuePair<String, MapNoteCollection> keyValuePair in m_MapNotes)
			{
				String path = Path.Combine(mapFolder, keyValuePair.Key);
				if (File.Exists(path))
				{
					keyValuePair.Value.Cleanup();
					if (keyValuePair.Value.Count == 0)
					{
						list.Add(keyValuePair.Key);
					}
				}
				else
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (String key in list)
			{
				m_MapNotes.Remove(key);
			}
		}

		internal void Load(SaveGameData mapdata)
		{
			Clear();
			if (mapdata == null)
			{
				return;
			}
			MapNoteEventArgs mapNoteEventArgs = new MapNoteEventArgs(null, null);
			Int32 num = mapdata.Get<Int32>("MapCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				String text = mapdata.Get<String>("Map" + i, null);
				SaveGameData saveGameData = mapdata.Get<SaveGameData>("Notes" + i, null);
				if (text != null && saveGameData != null)
				{
					MapNoteCollection mapNoteCollection = new MapNoteCollection();
					m_MapNotes.Add(text, mapNoteCollection);
					Int32 num2 = saveGameData.Get<Int32>("Count", 0);
					for (Int32 j = 0; j < num2; j++)
					{
						Int32 num3 = saveGameData.Get<Int32>("Nx" + j, -1);
						Int32 num4 = saveGameData.Get<Int32>("Ny" + j, -1);
						String text2 = saveGameData.Get<String>("Nt" + j, null);
						if (num3 != -1 && num4 != -1 && !String.IsNullOrEmpty(text2))
						{
							MapNote mapNote = new MapNote(text, new Position(num3, num4), text2);
							mapNoteCollection.Add(new Position(num3, num4), mapNote);
							if (AddedMapNote != null)
							{
								mapNoteEventArgs.MapID = text;
								mapNoteEventArgs.MapNote = mapNote;
								AddedMapNote(this, mapNoteEventArgs);
							}
						}
					}
				}
			}
		}

		internal void Save(SaveGameData mapdata)
		{
			Cleanup();
			if (m_MapNotes.Count == 0)
			{
				return;
			}
			mapdata.Set<Int32>("MapCount", m_MapNotes.Count);
			Int32 num = 0;
			foreach (KeyValuePair<String, MapNoteCollection> keyValuePair in m_MapNotes)
			{
				SaveGameData saveGameData = new SaveGameData(keyValuePair.Key);
				mapdata.Set<String>("Map" + num, keyValuePair.Key);
				mapdata.Set<SaveGameData>("Notes" + num, saveGameData);
				keyValuePair.Value.Cleanup();
				saveGameData.Set<Int32>("Count", keyValuePair.Value.Count);
				Int32 num2 = 0;
				foreach (MapNote mapNote in keyValuePair.Value.Values)
				{
					saveGameData.Set<Int32>("Nx" + num2, mapNote.Position.X);
					saveGameData.Set<Int32>("Ny" + num2, mapNote.Position.Y);
					saveGameData.Set<String>("Nt" + num2, mapNote.Note);
					num2++;
				}
				num++;
			}
		}

		public class MapNoteEventArgs : EventArgs
		{
			public MapNoteEventArgs(String mapId, MapNote mapNote)
			{
				MapID = mapId;
				MapNote = mapNote;
			}

			public String MapID { get; internal set; }

			public MapNote MapNote { get; internal set; }
		}
	}
}
