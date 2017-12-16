using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ByteReader
{
	private Byte[] mBuffer;

	private Int32 mOffset;

	public ByteReader(Byte[] bytes)
	{
		mBuffer = bytes;
	}

	public ByteReader(TextAsset asset)
	{
		mBuffer = asset.bytes;
	}

	public Boolean canRead => mBuffer != null && mOffset < mBuffer.Length;

    private static String ReadLine(Byte[] buffer, Int32 start, Int32 count)
	{
		return Encoding.UTF8.GetString(buffer, start, count);
	}

	public String ReadLine()
	{
		Int32 num = mBuffer.Length;
		while (mOffset < num && mBuffer[mOffset] < 32)
		{
			mOffset++;
		}
		Int32 i = mOffset;
		if (i < num)
		{
			while (i < num)
			{
				Int32 num2 = mBuffer[i++];
				if (num2 == 10 || num2 == 13)
				{
					String result = ReadLine(mBuffer, mOffset, i - mOffset - 1);
					mOffset = i;
					return result;
				}
			}
			i++;
		    String result2 = ReadLine(mBuffer, mOffset, i - mOffset - 1);
		    mOffset = i;
		    return result2;
        }
		mOffset = num;
		return null;
	}

	public Dictionary<String, String> ReadDictionary()
	{
		Dictionary<String, String> dictionary = new Dictionary<String, String>();
		Char[] separator = new Char[]
		{
			'='
		};
		while (canRead)
		{
			String text = ReadLine();
			if (text == null)
			{
				break;
			}
			if (!text.StartsWith("//"))
			{
				String[] array = text.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 2)
				{
					String key = array[0].Trim();
					String value = array[1].Trim().Replace("\\n", "\n");
					dictionary[key] = value;
				}
			}
		}
		return dictionary;
	}
}
