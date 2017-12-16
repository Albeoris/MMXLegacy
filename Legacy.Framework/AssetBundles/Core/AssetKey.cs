using System;

namespace AssetBundles.Core
{
	internal struct AssetKey : IEquatable<AssetKey>
	{
		public String Name;

		public Type Type;

		public AssetKey(String name, Type type)
		{
			Name = name;
			Type = type;
		}

		public override Boolean Equals(Object obj)
		{
			if (obj != null && obj is AssetKey)
			{
				AssetKey assetKey = (AssetKey)obj;
				return Equals(ref assetKey);
			}
			return false;
		}

		public Boolean Equals(ref AssetKey other)
		{
			return Type == other.Type && String.Equals(Name, other.Name, StringComparison.InvariantCulture);
		}

		public Boolean Equals(AssetKey other)
		{
			return Equals(ref other);
		}

		public override Int32 GetHashCode()
		{
			return Name.GetHashCode() ^ Type.GetHashCode();
		}

		public override String ToString()
		{
			return String.Format("[{0}] '{1}'", Type.ToString(), Name);
		}
	}
}
