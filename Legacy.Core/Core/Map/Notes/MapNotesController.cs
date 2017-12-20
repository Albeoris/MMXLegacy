using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Map.Notes
{
	public class MapNotesController
	{
		private Dictionary<String, MapNoteCollection> m_MapNotes = new Dictionary<String, MapNoteCollection>();

		public event EventHandler<MapNoteEventArgs> AddedMapNote;
		public event EventHandler<MapNoteEventArgs> RemovedMapNote;
		public event EventHandler<MapNoteEventArgs> UpdatedMapNoteText;

        public MapNotesController(EventManager p_eventManager)
        {
            p_eventManager.Get<InitTrainingDialogArgs>().Event += OnTrainingDialog;
            p_eventManager.Get<InitServiceDialogArgs>().Event += OnServiceDialog;
            p_eventManager.Get<InitUniqueDialogArgs>().Event += OnUniqueDialog;
        }

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

	    private const String WhiteColor = "[FFFFFF]";
	    private const String CyanColor = "[00FFFF]";
        private const String YellowColor = "[FFFF00]";
	    private const String GreenColor = "[00FF00]";
	    private const String BlueColor = "[0000FF]";
	    private const String PurpleColor = "[FF00FF]";
	    private const String EndColor = "[-]";

	    private void OnServiceDialog(InitServiceDialogArgs args)
	    {
	        SetServiceNote(args.Caption, sb =>
	        {
	            sb.Append(WhiteColor);
	            sb.Append(args.Title);
	            sb.Append(EndColor);
	        });
	    }

	    private void OnTrainingDialog(InitTrainingDialogArgs args)
	    {
	        SetServiceNote(args.Caption, sb =>
	        {
	            switch (args.SkillRank)
	            {
	                case ETier.NONE:
	                case ETier.NOVICE:
	                    sb.Append(WhiteColor);
	                    break;
	                case ETier.EXPERT:
	                    sb.Append(GreenColor);
	                    break;
	                case ETier.MASTER:
	                    sb.Append(BlueColor);
	                    break;
	                case ETier.GRAND_MASTER:
	                    sb.Append(PurpleColor);
	                    break;
	            }
	            sb.Append(args.SkillName);
	            sb.Append(EndColor);
            });
        }

	    private void OnUniqueDialog(InitUniqueDialogArgs args)
	    {
	        SetServiceNote(args.Caption, sb => { });
	    }

        private void SetServiceNote(String caption, Action<StringBuilder> valueFactory)
	    {
            Grid grid = LegacyLogic.Instance.MapLoader.Grid;
            if (grid == null)
                return;

            if (!grid.GetPlayerPosition(out Position partyPosition))
                return;

            String note = GetMapNote(partyPosition).Note ?? String.Empty;
            if (note.Length > 0 && note[0] != '[')
                return;

            StringBuilder sb = new StringBuilder(64);
	        valueFactory(sb);

            // Single caption (Cyan)
            if (sb.Length == 0)
            {
                if (note.Contains(caption))
                    return;

                sb.Append(CyanColor);
                sb.Append(caption);
                sb.Append(EndColor);
                sb.Append(Environment.NewLine);
                sb.Append(note);
                SetMapNoteText(partyPosition, note);
                return;
            }
            
            String coloredValue = sb.ToString();
            if (note.Contains(coloredValue))
                return;

            sb.Length = 0;
            sb.Append(YellowColor);
            sb.Append(caption);
            sb.Append(EndColor);

            String coloredCaption = sb.ToString();

            // ReSharper disable once StringIndexOfIsCultureSpecific.1
            Int32 indexOfCaption = note.IndexOf(coloredCaption);
            if (indexOfCaption < 0)
            {
                sb.Append(' ');
                sb.Append(coloredValue);
                if (note.Length == 0)
                {
                    note = sb.ToString();
                }
                else
                {
                    Char lastCh = note[note.Length - 1];
                    if (lastCh == '\n' || lastCh == '\r')
                        note = note + sb;
                    else
                        note = note + Environment.NewLine + sb;
                }
            }
            else
            {
                indexOfCaption += coloredCaption.Length;
                indexOfCaption++; // Space

                if (indexOfCaption > note.Length)
                    indexOfCaption = note.Length;

                String left = note.Substring(startIndex: 0, length: indexOfCaption);
                if (left.Length == note.Length)
                {
                    if (left[left.Length - 1] == ' ')
                        note = note + coloredValue;
                    else
                        note = note + +' ' + sb;
                }
                else
                {
                    sb.Length = 0;
                    sb.Append(left);
                    if (sb[sb.Length - 1] != ' ')
                        sb.Append(' ');
                    sb.Append(coloredValue);
                    sb.Append(", ");
                    sb.Append(note.Substring(indexOfCaption));

                    note = sb.ToString();
                }
            }

            SetMapNoteText(partyPosition, note);
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
