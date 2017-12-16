using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Legacy.Core.Api;
using Legacy.Utilities;

namespace Legacy.Core.SaveGameManagement
{
	public class SaveGame
	{
		private Dictionary<String, SaveGameData> m_saveGameData;

		private SaveGameMeta m_metaData;

		public SaveGame()
		{
			m_saveGameData = new Dictionary<String, SaveGameData>();
			m_metaData = new SaveGameMeta(DateTime.Now, default(TimeSpan), 0, ESaveGameType.NORMAL);
		}

		public SaveGame(SaveGameMeta p_meta)
		{
			m_saveGameData = new Dictionary<String, SaveGameData>();
			m_metaData = p_meta;
		}

		public Int32 Count => m_saveGameData.Count;

	    public SaveGameData this[String p_key]
		{
			get
			{
				SaveGameData result;
				m_saveGameData.TryGetValue(p_key, out result);
				return result;
			}
		}

		public void Clear()
		{
			m_saveGameData.Clear();
		}

		public Boolean Add(SaveGameData p_data)
		{
			if (!m_saveGameData.ContainsKey(p_data.ID))
			{
				m_saveGameData.Add(p_data.ID, p_data);
				return true;
			}
			return false;
		}

		public void Write(BinaryWriter p_writer)
		{
			WriteMetaData(p_writer);
			p_writer.Write(m_saveGameData.Count);
			foreach (KeyValuePair<String, SaveGameData> keyValuePair in m_saveGameData)
			{
				p_writer.Write(keyValuePair.Key);
				keyValuePair.Value.Write(p_writer);
			}
		}

		public void Read(BinaryReader p_reader)
		{
			ReadMetaData(p_reader);
			Int32 num = p_reader.ReadInt32();
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				SaveGameData saveGameData = new SaveGameData(text);
				saveGameData.Read(p_reader);
				m_saveGameData.Add(text, saveGameData);
			}
		}

		private void WriteMetaData(BinaryWriter p_writer)
		{
			p_writer.Write(m_metaData.Time.ToString("yyyy-MM-dd HH:mm"));
			p_writer.Write(m_metaData.PlayTime.Ticks);
			p_writer.Write(LegacyLogic.Instance.WorldManager.HighestSaveGameNumber);
			p_writer.Write((Int32)LegacyLogic.Instance.WorldManager.CurrentSaveGameType);
		}

		public static SaveGameData CreateDataFromArray<T>(String p_id, ICollection<T> p_array)
		{
			SaveGameData saveGameData = new SaveGameData(p_id);
			Int32 num = 0;
			foreach (T p_value in p_array)
			{
				saveGameData.Set<T>("data_" + num, p_value);
				num++;
			}
			return saveGameData;
		}

		public static List<T> CreateArrayFromData<T>(SaveGameData p_data)
		{
			if (p_data != null)
			{
				List<T> list = new List<T>(p_data.Count);
				for (Int32 i = 0; i < p_data.Count; i++)
				{
					list.Add(p_data.Get<T>("data_" + i, default(T)));
				}
				return list;
			}
			return null;
		}

		public static SaveGameMeta ReadMetaData(BinaryReader p_reader)
		{
			return ReadMetaData(p_reader, false);
		}

		public static SaveGameMeta ReadMetaData(BinaryReader p_reader, Boolean p_getDifficulty)
		{
			String text = p_reader.ReadString();
			Int64 value = p_reader.ReadInt64();
			Int32 p_saveNumber = p_reader.ReadInt32();
			Int32 p_type = p_reader.ReadInt32();
			DateTime p_time;
			if (!DateTime.TryParseExact(text, "yyyy-MM-dd HH:mm", null, DateTimeStyles.None, out p_time))
			{
				LegacyLogger.Log("Cannot parse Time: " + text);
				return default(SaveGameMeta);
			}
			SaveGameMeta result;
			if (p_getDifficulty)
			{
				result = new SaveGameMeta(p_time, TimeSpan.FromTicks(value), p_saveNumber, (ESaveGameType)p_type, GetDifficulty(p_reader));
			}
			else
			{
				result = new SaveGameMeta(p_time, TimeSpan.FromTicks(value), p_saveNumber, (ESaveGameType)p_type);
			}
			result.Loaded = true;
			return result;
		}

		private static EDifficulty GetDifficulty(BinaryReader p_reader)
		{
			p_reader.ReadInt32();
			String text = p_reader.ReadString();
			if (text != "MainData")
			{
				LegacyLogger.Log("Not MainData: " + text);
				return EDifficulty.NORMAL;
			}
			SaveGameData saveGameData = new SaveGameData(text);
			saveGameData.Read(p_reader);
			return saveGameData.Get<EDifficulty>("Difficulty", EDifficulty.NORMAL);
		}
	}
}
