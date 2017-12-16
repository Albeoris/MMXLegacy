using System;
using System.Collections.Generic;

[Serializable]
public class BMGlyph
{
	public Int32 index;

	public Int32 x;

	public Int32 y;

	public Int32 width;

	public Int32 height;

	public Int32 offsetX;

	public Int32 offsetY;

	public Int32 advance;

	public Int32 channel;

	public List<Int32> kerning;

	public Int32 GetKerning(Int32 previousChar)
	{
		if (kerning != null)
		{
			Int32 i = 0;
			Int32 count = kerning.Count;
			while (i < count)
			{
				if (kerning[i] == previousChar)
				{
					return kerning[i + 1];
				}
				i += 2;
			}
		}
		return 0;
	}

	public void SetKerning(Int32 previousChar, Int32 amount)
	{
		if (kerning == null)
		{
			kerning = new List<Int32>();
		}
		for (Int32 i = 0; i < kerning.Count; i += 2)
		{
			if (kerning[i] == previousChar)
			{
				kerning[i + 1] = amount;
				return;
			}
		}
		kerning.Add(previousChar);
		kerning.Add(amount);
	}

	public void Trim(Int32 xMin, Int32 yMin, Int32 xMax, Int32 yMax)
	{
		Int32 num = x + width;
		Int32 num2 = y + height;
		if (x < xMin)
		{
			Int32 num3 = xMin - x;
			x += num3;
			width -= num3;
			offsetX += num3;
		}
		if (y < yMin)
		{
			Int32 num4 = yMin - y;
			y += num4;
			height -= num4;
			offsetY += num4;
		}
		if (num > xMax)
		{
			width -= num - xMax;
		}
		if (num2 > yMax)
		{
			height -= num2 - yMax;
		}
	}
}
