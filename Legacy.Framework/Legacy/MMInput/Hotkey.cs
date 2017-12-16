using System;
using UnityEngine;

namespace Legacy.MMInput
{
	public struct Hotkey : IEquatable<Hotkey>
	{
		public readonly EHotkeyType Type;

		public KeyCode Key1;

		public KeyCode Key2;

		public KeyCode Key3;

		public KeyCode AltKey1;

		public KeyCode AltKey2;

		public KeyCode AltKey3;

		public Byte KeyCount;

		public Byte AltKeyCount;

		public Hotkey(EHotkeyType type, KeyCode key1)
		{
			this = new Hotkey(type, key1, KeyCode.None, KeyCode.None);
		}

		public Hotkey(EHotkeyType type, KeyCode key1, KeyCode key2)
		{
			this = new Hotkey(type, key1, key2, KeyCode.None);
		}

		public Hotkey(EHotkeyType type, KeyCode key1, KeyCode key2, KeyCode key3)
		{
			Type = type;
			Key1 = key1;
			Key2 = key2;
			Key3 = key3;
			AltKey1 = KeyCode.None;
			AltKey2 = KeyCode.None;
			AltKey3 = KeyCode.None;
			AltKeyCount = 0;
			KeyCount = 0;
			if (key1 != KeyCode.None)
			{
				KeyCount += 1;
			}
			if (key2 != KeyCode.None)
			{
				KeyCount += 1;
			}
			if (key3 != KeyCode.None)
			{
				KeyCount += 1;
			}
		}

		public Boolean IsMouse => Key1 >= KeyCode.Mouse0 && Key1 <= KeyCode.Mouse6;

	    public Boolean IsPressed => IsPrimaryPressed || IsAlternativePressed;

	    private Boolean IsPrimaryPressed
		{
			get
			{
				if ((Key3 | Key2) == KeyCode.None)
				{
					return Input.GetKey(Key1);
				}
				if (Key3 == KeyCode.None)
				{
					return Input.GetKey(Key1) && Input.GetKey(Key2);
				}
				return Input.GetKey(Key1) & Input.GetKey(Key2) & Input.GetKey(Key3);
			}
		}

		public Boolean IsAlternativePressed
		{
			get
			{
				if ((AltKey3 | AltKey2) == KeyCode.None)
				{
					return Input.GetKey(AltKey1);
				}
				if (AltKey3 == KeyCode.None)
				{
					return Input.GetKey(AltKey1) && Input.GetKey(AltKey2);
				}
				return Input.GetKey(AltKey1) & Input.GetKey(AltKey2) & Input.GetKey(AltKey3);
			}
		}

		public Boolean IsDown => (IsPrimaryDown && IsAlternativeDown) || (IsPrimaryDown && !IsAlternativePressed) || (IsAlternativeDown && !IsPrimaryPressed);

	    public Boolean IsPrimaryDown
		{
			get
			{
				if ((Key3 | Key2) == KeyCode.None)
				{
					return Input.GetKeyDown(Key1);
				}
				if (Key3 == KeyCode.None)
				{
					return Input.GetKey(Key1) && Input.GetKeyDown(Key2);
				}
				return Input.GetKey(Key1) && Input.GetKey(Key2) && Input.GetKeyDown(Key3);
			}
		}

		public Boolean IsAlternativeDown
		{
			get
			{
				if ((AltKey3 | AltKey2) == KeyCode.None)
				{
					return Input.GetKeyDown(AltKey1);
				}
				if (AltKey3 == KeyCode.None)
				{
					return Input.GetKey(AltKey1) && Input.GetKeyDown(AltKey2);
				}
				return Input.GetKey(AltKey1) && Input.GetKey(AltKey2) && Input.GetKeyDown(AltKey3);
			}
		}

		public Boolean IsUp => (IsAlternativeUp && !IsPrimaryPressed) || (IsPrimaryUp && !IsAlternativePressed);

	    public Boolean IsPrimaryUp
		{
			get
			{
				if ((Key3 | Key2) == KeyCode.None)
				{
					return Input.GetKeyUp(Key1);
				}
				if (Key3 == KeyCode.None)
				{
					return Input.GetKey(Key1) && Input.GetKeyUp(Key2);
				}
				return Input.GetKey(Key1) && Input.GetKey(Key2) && Input.GetKeyUp(Key3);
			}
		}

		public Boolean IsAlternativeUp
		{
			get
			{
				if ((AltKey3 | AltKey2) == KeyCode.None)
				{
					return Input.GetKeyUp(AltKey1);
				}
				if (AltKey3 == KeyCode.None)
				{
					return Input.GetKey(AltKey1) && Input.GetKeyUp(AltKey2);
				}
				return Input.GetKey(AltKey1) && Input.GetKey(AltKey2) && Input.GetKeyUp(AltKey3);
			}
		}

		public void SetAlternativeHotkeys(KeyCode key1)
		{
			SetAlternativeHotkeys(key1, KeyCode.None, KeyCode.None);
		}

		public void SetAlternativeHotkeys(KeyCode key1, KeyCode key2)
		{
			SetAlternativeHotkeys(key1, key2, KeyCode.None);
		}

		public void SetAlternativeHotkeys(KeyCode key1, KeyCode key2, KeyCode key3)
		{
			AltKey1 = key1;
			AltKey2 = key2;
			AltKey3 = key3;
			AltKeyCount = 0;
			if (key1 != KeyCode.None)
			{
				AltKeyCount += 1;
			}
			if (key2 != KeyCode.None)
			{
				AltKeyCount += 1;
			}
			if (key3 != KeyCode.None)
			{
				AltKeyCount += 1;
			}
		}

		public override String ToString()
		{
			String text;
			switch (KeyCount)
			{
			case 1:
				text = String.Format("{0}", Key1);
				break;
			case 2:
				text = String.Format("{0}+{1}", Key1, Key2);
				break;
			case 3:
				text = String.Format("{0}+{1}+{2}", Key1, Key2, Key3);
				break;
			default:
				text = KeyCode.None.ToString();
				break;
			}
			switch (AltKeyCount)
			{
			case 1:
				text += String.Format(",{0}", AltKey1);
				break;
			case 2:
				text += String.Format(",{0}+{1}", AltKey1, AltKey2);
				break;
			case 3:
				text += String.Format(",{0}+{1}+{2}", AltKey1, AltKey2, AltKey3);
				break;
			}
			return text;
		}

		public static Boolean TryParse(EHotkeyType p_type, String p_value, out Hotkey p_hotkey)
		{
			Boolean result = false;
			p_hotkey = new Hotkey(p_type, KeyCode.None);
			String[] array = p_value.Split(new Char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 0)
			{
				String[] array2 = array[0].Split(new Char[]
				{
					'+'
				}, StringSplitOptions.RemoveEmptyEntries);
				try
				{
					if (array2.Length == 1)
					{
						p_hotkey = new Hotkey(p_type, (KeyCode)Enum.Parse(typeof(KeyCode), array2[0], true));
						result = true;
					}
					if (array2.Length == 2)
					{
						KeyCode key = (KeyCode)Enum.Parse(typeof(KeyCode), array2[0], true);
						KeyCode key2 = (KeyCode)Enum.Parse(typeof(KeyCode), array2[1], true);
						p_hotkey = new Hotkey(p_type, key, key2);
						result = true;
					}
					if (array2.Length == 3)
					{
						KeyCode key3 = (KeyCode)Enum.Parse(typeof(KeyCode), array2[0], true);
						KeyCode key4 = (KeyCode)Enum.Parse(typeof(KeyCode), array2[1], true);
						KeyCode key5 = (KeyCode)Enum.Parse(typeof(KeyCode), array2[2], true);
						p_hotkey = new Hotkey(p_type, key3, key4, key5);
						result = true;
					}
				}
				catch
				{
				}
			}
			if (array.Length > 1 && array.Length > 0)
			{
				String[] array3 = array[1].Split(new Char[]
				{
					'+'
				}, StringSplitOptions.RemoveEmptyEntries);
				try
				{
					if (array3.Length == 1)
					{
						p_hotkey.SetAlternativeHotkeys((KeyCode)Enum.Parse(typeof(KeyCode), array3[0], true));
						result = true;
					}
					if (array3.Length == 2)
					{
						KeyCode key6 = (KeyCode)Enum.Parse(typeof(KeyCode), array3[0], true);
						KeyCode key7 = (KeyCode)Enum.Parse(typeof(KeyCode), array3[1], true);
						p_hotkey.SetAlternativeHotkeys(key6, key7);
						result = true;
					}
					if (array3.Length == 3)
					{
						KeyCode key8 = (KeyCode)Enum.Parse(typeof(KeyCode), array3[0], true);
						KeyCode key9 = (KeyCode)Enum.Parse(typeof(KeyCode), array3[1], true);
						KeyCode key10 = (KeyCode)Enum.Parse(typeof(KeyCode), array3[2], true);
						p_hotkey.SetAlternativeHotkeys(key8, key9, key10);
						result = true;
					}
				}
				catch
				{
				}
			}
			return result;
		}

		public Boolean Equals(Hotkey other)
		{
			return Type == other.Type && Key1 == other.Key1 && Key2 == other.Key2 && Key3 == other.Key3 && AltKey1 == other.AltKey1 && AltKey2 == other.AltKey2 && AltKey3 == other.AltKey3;
		}
	}
}
