using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetBundles.Core.Serialization
{
	public class DatabaseDeserializer
	{
		private Dictionary<FourCC, Decoder> mDecoders;

		public DatabaseDeserializer()
		{
			mDecoders = new Dictionary<FourCC, Decoder>(1);
			RegisterDecoder(Decoder.Default);
		}

		public void RegisterDecoder(Decoder decoder)
		{
			mDecoders.Add(decoder.Head, decoder);
		}

		public AssetBundleDatabase Deserialize(String filePath)
		{
			AssetBundleDatabase result;
			using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				result = DecodeStream(fileStream);
			}
			return result;
		}

		public AssetBundleDatabase Deserialize(TextAsset data)
		{
			AssetBundleDatabase result;
			using (MemoryStream memoryStream = new MemoryStream(data.bytes))
			{
				result = DecodeStream(memoryStream);
			}
			return result;
		}

		private AssetBundleDatabase DecodeStream(Stream stream)
		{
			FourCC fourCC = default(FourCC);
			fourCC.Deserialize(stream);
			Decoder decoder;
			if (mDecoders.TryGetValue(fourCC, out decoder))
			{
				return decoder.Decode(stream);
			}
			throw new InvalidDataException(String.Format("Invalid data '{0}' in '{1}'", fourCC.ToString(), fourCC));
		}
	}
}
