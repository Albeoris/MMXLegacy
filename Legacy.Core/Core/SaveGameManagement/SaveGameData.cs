using System;
using System.Collections.Generic;
using System.IO;

namespace Legacy.Core.SaveGameManagement
{
	public class SaveGameData
	{
		private String m_id;

		private Boolean m_sealed;

		private Dictionary<String, TypeCode> m_mapping = new Dictionary<String, TypeCode>();

		private Dictionary<String, Boolean> m_bools;

		private Dictionary<String, Byte> m_bytes;

		private Dictionary<String, Int16> m_shorts;

		private Dictionary<String, UInt16> m_ushorts;

		private Dictionary<String, Int32> m_ints;

		private Dictionary<String, UInt32> m_uints;

		private Dictionary<String, Int64> m_longs;

		private Dictionary<String, UInt64> m_ulongs;

		private Dictionary<String, Single> m_floats;

		private Dictionary<String, Double> m_doubles;

		private Dictionary<String, String> m_strings;

		private Dictionary<String, SaveGameData> m_dataObjects;

		public SaveGameData(String p_id)
		{
			m_id = p_id;
		}

		public String ID => m_id;

	    public Int32 Count => m_mapping.Count;

	    public T Get<T>(String p_key, T defaultValue = default(T))
		{
			T result = defaultValue;
			TypeCode typeCode;
			if (m_mapping.TryGetValue(p_key, out typeCode))
			{
				switch (typeCode)
				{
				case TypeCode.Object:
					if (typeof(T).IsAssignableFrom(typeof(SaveGameData)))
					{
						result = ((m_dataObjects == null || !m_dataObjects.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_dataObjects[p_key])));
						goto IL_3A8;
					}
					break;
				case TypeCode.Boolean:
					result = ((m_bools == null || !m_bools.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_bools[p_key])));
					goto IL_3A8;
				case TypeCode.Byte:
					result = ((m_bytes == null || !m_bytes.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_bytes[p_key])));
					goto IL_3A8;
				case TypeCode.Int16:
					result = ((m_shorts == null || !m_shorts.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_shorts[p_key])));
					goto IL_3A8;
				case TypeCode.UInt16:
					result = ((m_ushorts == null || !m_ushorts.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_ushorts[p_key])));
					goto IL_3A8;
				case TypeCode.Int32:
					result = ((m_ints == null || !m_ints.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_ints[p_key])));
					goto IL_3A8;
				case TypeCode.UInt32:
					result = ((m_uints == null || !m_uints.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_uints[p_key])));
					goto IL_3A8;
				case TypeCode.Int64:
					result = ((m_longs == null || !m_longs.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_longs[p_key])));
					goto IL_3A8;
				case TypeCode.UInt64:
					result = ((m_ulongs == null || !m_ulongs.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_ulongs[p_key])));
					goto IL_3A8;
				case TypeCode.Single:
					result = ((m_floats == null || !m_floats.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_floats[p_key])));
					goto IL_3A8;
				case TypeCode.Double:
					result = ((m_doubles == null || !m_doubles.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_doubles[p_key])));
					goto IL_3A8;
				case TypeCode.String:
					result = ((m_strings == null || !m_strings.ContainsKey(p_key)) ? defaultValue : ((T)((Object)m_strings[p_key])));
					goto IL_3A8;
				}
				throw new NotSupportedException(String.Concat(new String[]
				{
					"The Type: ",
					p_key,
					":",
					typeCode.ToString(),
					" is not supported"
				}));
				IL_3A8:;
			}
			return result;
		}

		public void Set<T>(String p_key, T p_value)
		{
			if (m_sealed)
			{
				throw new NotSupportedException("the SaveGameData is Sealed (loaded from SaveGame)");
			}
			TypeCode typeCode = Type.GetTypeCode(typeof(T));
			if (m_mapping.ContainsKey(p_key))
			{
				throw new ArgumentException("the given Key is already Used '" + p_key + "'");
			}
			switch (typeCode)
			{
			case TypeCode.Object:
				if (p_value is SaveGameData)
				{
					if (m_dataObjects == null)
					{
						m_dataObjects = new Dictionary<String, SaveGameData>();
					}
					m_dataObjects.Add(p_key, (SaveGameData)((Object)p_value));
					goto IL_31C;
				}
				break;
			case TypeCode.Boolean:
				if (m_bools == null)
				{
					m_bools = new Dictionary<String, Boolean>();
				}
				m_bools.Add(p_key, (Boolean)((Object)p_value));
				goto IL_31C;
			case TypeCode.Byte:
				if (m_bytes == null)
				{
					m_bytes = new Dictionary<String, Byte>();
				}
				m_bytes.Add(p_key, (Byte)((Object)p_value));
				goto IL_31C;
			case TypeCode.Int16:
				if (m_shorts == null)
				{
					m_shorts = new Dictionary<String, Int16>();
				}
				m_shorts.Add(p_key, (Int16)((Object)p_value));
				goto IL_31C;
			case TypeCode.UInt16:
				if (m_ushorts == null)
				{
					m_ushorts = new Dictionary<String, UInt16>();
				}
				m_ushorts.Add(p_key, (UInt16)((Object)p_value));
				goto IL_31C;
			case TypeCode.Int32:
				if (m_ints == null)
				{
					m_ints = new Dictionary<String, Int32>();
				}
				m_ints.Add(p_key, (Int32)((Object)p_value));
				goto IL_31C;
			case TypeCode.UInt32:
				if (m_uints == null)
				{
					m_uints = new Dictionary<String, UInt32>();
				}
				m_uints.Add(p_key, (UInt32)((Object)p_value));
				goto IL_31C;
			case TypeCode.Int64:
				if (m_longs == null)
				{
					m_longs = new Dictionary<String, Int64>();
				}
				m_longs.Add(p_key, (Int64)((Object)p_value));
				goto IL_31C;
			case TypeCode.UInt64:
				if (m_ulongs == null)
				{
					m_ulongs = new Dictionary<String, UInt64>();
				}
				m_ulongs.Add(p_key, (UInt64)((Object)p_value));
				goto IL_31C;
			case TypeCode.Single:
				if (m_floats == null)
				{
					m_floats = new Dictionary<String, Single>();
				}
				m_floats.Add(p_key, (Single)((Object)p_value));
				goto IL_31C;
			case TypeCode.Double:
				if (m_doubles == null)
				{
					m_doubles = new Dictionary<String, Double>();
				}
				m_doubles.Add(p_key, (Double)((Object)p_value));
				goto IL_31C;
			case TypeCode.String:
				if (m_strings == null)
				{
					m_strings = new Dictionary<String, String>();
				}
				m_strings.Add(p_key, (String)((Object)p_value));
				goto IL_31C;
			}
			throw new NotSupportedException("The given objects type is not supported");
			IL_31C:
			m_mapping.Add(p_key, typeCode);
		}

		public void Write(BinaryWriter p_writer)
		{
			if (m_bools != null)
			{
				p_writer.Write(m_bools.Count);
				foreach (KeyValuePair<String, Boolean> keyValuePair in m_bools)
				{
					p_writer.Write(keyValuePair.Key);
					p_writer.Write(keyValuePair.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_bytes != null)
			{
				p_writer.Write(m_bytes.Count);
				foreach (KeyValuePair<String, Byte> keyValuePair2 in m_bytes)
				{
					p_writer.Write(keyValuePair2.Key);
					p_writer.Write(keyValuePair2.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_shorts != null)
			{
				p_writer.Write(m_shorts.Count);
				foreach (KeyValuePair<String, Int16> keyValuePair3 in m_shorts)
				{
					p_writer.Write(keyValuePair3.Key);
					p_writer.Write(keyValuePair3.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_ushorts != null)
			{
				p_writer.Write(m_ushorts.Count);
				foreach (KeyValuePair<String, UInt16> keyValuePair4 in m_ushorts)
				{
					p_writer.Write(keyValuePair4.Key);
					p_writer.Write(keyValuePair4.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_ints != null)
			{
				p_writer.Write(m_ints.Count);
				foreach (KeyValuePair<String, Int32> keyValuePair5 in m_ints)
				{
					p_writer.Write(keyValuePair5.Key);
					p_writer.Write(keyValuePair5.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_uints != null)
			{
				p_writer.Write(m_uints.Count);
				foreach (KeyValuePair<String, UInt32> keyValuePair6 in m_uints)
				{
					p_writer.Write(keyValuePair6.Key);
					p_writer.Write(keyValuePair6.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_longs != null)
			{
				p_writer.Write(m_longs.Count);
				foreach (KeyValuePair<String, Int64> keyValuePair7 in m_longs)
				{
					p_writer.Write(keyValuePair7.Key);
					p_writer.Write(keyValuePair7.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_ulongs != null)
			{
				p_writer.Write(m_ulongs.Count);
				foreach (KeyValuePair<String, UInt64> keyValuePair8 in m_ulongs)
				{
					p_writer.Write(keyValuePair8.Key);
					p_writer.Write(keyValuePair8.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_floats != null)
			{
				p_writer.Write(m_floats.Count);
				foreach (KeyValuePair<String, Single> keyValuePair9 in m_floats)
				{
					p_writer.Write(keyValuePair9.Key);
					p_writer.Write(keyValuePair9.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_doubles != null)
			{
				p_writer.Write(m_doubles.Count);
				foreach (KeyValuePair<String, Double> keyValuePair10 in m_doubles)
				{
					p_writer.Write(keyValuePair10.Key);
					p_writer.Write(keyValuePair10.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_strings != null)
			{
				p_writer.Write(m_strings.Count);
				foreach (KeyValuePair<String, String> keyValuePair11 in m_strings)
				{
					p_writer.Write(keyValuePair11.Key);
					p_writer.Write(keyValuePair11.Value);
				}
			}
			else
			{
				p_writer.Write(0);
			}
			if (m_dataObjects != null)
			{
				p_writer.Write(m_dataObjects.Count);
				foreach (KeyValuePair<String, SaveGameData> keyValuePair12 in m_dataObjects)
				{
					p_writer.Write(keyValuePair12.Key);
					keyValuePair12.Value.Write(p_writer);
				}
			}
			else
			{
				p_writer.Write(0);
			}
		}

		public void Read(BinaryReader p_reader)
		{
			m_sealed = true;
			Int32 num = p_reader.ReadInt32();
			if (num > 0 && m_bools == null)
			{
				m_bools = new Dictionary<String, Boolean>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_bools.Add(text, p_reader.ReadBoolean());
				m_mapping.Add(text, TypeCode.Boolean);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_bytes == null)
			{
				m_bytes = new Dictionary<String, Byte>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_bytes.Add(text, p_reader.ReadByte());
				m_mapping.Add(text, TypeCode.Byte);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_shorts == null)
			{
				m_shorts = new Dictionary<String, Int16>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_shorts.Add(text, p_reader.ReadInt16());
				m_mapping.Add(text, TypeCode.Int16);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_ushorts == null)
			{
				m_ushorts = new Dictionary<String, UInt16>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_ushorts.Add(text, p_reader.ReadUInt16());
				m_mapping.Add(text, TypeCode.UInt16);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_ints == null)
			{
				m_ints = new Dictionary<String, Int32>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_ints.Add(text, p_reader.ReadInt32());
				m_mapping.Add(text, TypeCode.Int32);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_uints == null)
			{
				m_uints = new Dictionary<String, UInt32>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_uints.Add(text, p_reader.ReadUInt32());
				m_mapping.Add(text, TypeCode.UInt32);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_longs == null)
			{
				m_longs = new Dictionary<String, Int64>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_longs.Add(text, p_reader.ReadInt64());
				m_mapping.Add(text, TypeCode.Int64);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_ulongs == null)
			{
				m_ulongs = new Dictionary<String, UInt64>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_ulongs.Add(text, p_reader.ReadUInt64());
				m_mapping.Add(text, TypeCode.UInt64);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_floats == null)
			{
				m_floats = new Dictionary<String, Single>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_floats.Add(text, p_reader.ReadSingle());
				m_mapping.Add(text, TypeCode.Single);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_doubles == null)
			{
				m_doubles = new Dictionary<String, Double>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_doubles.Add(text, p_reader.ReadDouble());
				m_mapping.Add(text, TypeCode.Double);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_strings == null)
			{
				m_strings = new Dictionary<String, String>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				m_strings.Add(text, p_reader.ReadString());
				m_mapping.Add(text, TypeCode.String);
			}
			num = p_reader.ReadInt32();
			if (num > 0 && m_dataObjects == null)
			{
				m_dataObjects = new Dictionary<String, SaveGameData>(num);
			}
			for (Int32 i = 0; i < num; i++)
			{
				String text = p_reader.ReadString();
				SaveGameData saveGameData = new SaveGameData(text);
				saveGameData.Read(p_reader);
				m_dataObjects.Add(text, saveGameData);
				m_mapping.Add(text, TypeCode.Object);
			}
		}
	}
}
