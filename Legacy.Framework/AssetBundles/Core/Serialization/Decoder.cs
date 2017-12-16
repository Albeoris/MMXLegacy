using System;
using System.Diagnostics;
using System.IO;

namespace AssetBundles.Core.Serialization
{
	[DebuggerDisplay("{Head} Deserializer")]
	public abstract class Decoder
	{
		public static readonly Decoder Default = new DefaultDecoder();

		public readonly FourCC Head;

		public Decoder(FourCC headCode)
		{
			Head = headCode;
		}

		public abstract AssetBundleDatabase Decode(Stream stream);
	}
}
