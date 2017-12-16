using System;
using System.IO;

namespace AssetBundles.Core.Serialization
{
	public struct FourCC : IEquatable<FourCC>
	{
		public Int32 RawValue;

		public FourCC(Int32 rawValue)
		{
			RawValue = rawValue;
		}

		public FourCC(Char a, Char b, Char c, Char d)
		{
			RawValue = a;
			RawValue |= b << 8;
			RawValue |= c << 16;
			RawValue |= d << 24;
		}

		public void Serialize(Stream stream)
		{
			stream.WriteByte((Byte)RawValue);
			stream.WriteByte((Byte)(RawValue >> 8));
			stream.WriteByte((Byte)(RawValue >> 16));
			stream.WriteByte((Byte)(RawValue >> 24));
		}

		public void Deserialize(Stream stream)
		{
			RawValue = stream.ReadByte();
			RawValue |= stream.ReadByte() << 8;
			RawValue |= stream.ReadByte() << 16;
			RawValue |= stream.ReadByte() << 24;
		}

		public override Boolean Equals(Object obj)
		{
			return obj != null && obj is FourCC && Equals((FourCC)obj);
		}

		public Boolean Equals(FourCC other)
		{
			return RawValue == other.RawValue;
		}

		public override Int32 GetHashCode()
		{
			return RawValue.GetHashCode();
		}

		public override String ToString()
		{
			return String.Format("Code: {0}{1}{2}{3}", new Object[]
			{
				(Char)((Byte)RawValue),
				(Char)((Byte)(RawValue >> 8)),
				(Char)((Byte)(RawValue >> 16)),
				(Char)((Byte)(RawValue >> 24))
			});
		}

		public static Boolean operator !=(FourCC a, FourCC b)
		{
			return a.RawValue != b.RawValue;
		}

		public static Boolean operator ==(FourCC a, FourCC b)
		{
			return a.RawValue == b.RawValue;
		}
	}
}
