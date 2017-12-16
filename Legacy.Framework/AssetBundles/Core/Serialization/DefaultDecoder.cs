using System;
using System.IO;
using System.Text;

namespace AssetBundles.Core.Serialization
{
	public class DefaultDecoder : Decoder
	{
		public DefaultDecoder() : base(new FourCC('A', 'D', 'B', '0'))
		{
		}

		public override AssetBundleDatabase Decode(Stream stream)
		{
			AssetBundleDatabase result;
			using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8))
			{
				UInt16 num = binaryReader.ReadUInt16();
				Int32 num2 = 0;
				AssetBundleData[] array = new AssetBundleData[num];
				for (Int32 i = 0; i < array.Length; i++)
				{
					array[i] = new AssetBundleData
					{
						Name = binaryReader.ReadString(),
						Path = binaryReader.ReadString(),
						Version = binaryReader.ReadInt32(),
						Size = binaryReader.ReadInt32(),
						CrcValue = binaryReader.ReadUInt32()
					};
				}
				for (Int32 j = 0; j < array.Length; j++)
				{
					num = binaryReader.ReadUInt16();
					AssetBundleData[] array2 = new AssetBundleData[num];
					for (Int32 k = 0; k < array2.Length; k++)
					{
						UInt16 num3 = binaryReader.ReadUInt16();
						array2[k] = array[num3];
					}
					array[j].BundleDependency = array2;
					num = binaryReader.ReadUInt16();
					String[] array3 = new String[num];
					for (Int32 l = 0; l < array3.Length; l++)
					{
						num2++;
						String text = binaryReader.ReadString();
						array3[l] = text;
					}
					array[j].Assets = array3;
				}
				result = new AssetBundleDatabase(array, num2);
			}
			return result;
		}
	}
}
