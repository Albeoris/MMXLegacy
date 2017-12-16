using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BMFont
{
	[HideInInspector]
	[SerializeField]
	private Int32 mSize;

	[HideInInspector]
	[SerializeField]
	private Int32 mBase;

	[SerializeField]
	[HideInInspector]
	private Int32 mWidth;

	[SerializeField]
	[HideInInspector]
	private Int32 mHeight;

	[SerializeField]
	[HideInInspector]
	private String mSpriteName;

	[SerializeField]
	[HideInInspector]
	private List<BMGlyph> mSaved = new List<BMGlyph>();

	private Dictionary<Int32, BMGlyph> mDict = new Dictionary<Int32, BMGlyph>();

	public Boolean isValid => mSaved.Count > 0;

    public Int32 charSize
	{
		get => mSize;
        set => mSize = value;
    }

	public Int32 baseOffset
	{
		get => mBase;
	    set => mBase = value;
	}

	public Int32 texWidth
	{
		get => mWidth;
	    set => mWidth = value;
	}

	public Int32 texHeight
	{
		get => mHeight;
	    set => mHeight = value;
	}

	public Int32 glyphCount => (!isValid) ? 0 : mSaved.Count;

    public String spriteName
	{
		get => mSpriteName;
	    set => mSpriteName = value;
	}

	public BMGlyph GetGlyph(Int32 index, Boolean createIfMissing)
	{
		BMGlyph bmglyph = null;
		if (mDict.Count == 0)
		{
			Int32 i = 0;
			Int32 count = mSaved.Count;
			while (i < count)
			{
				BMGlyph bmglyph2 = mSaved[i];
				mDict.Add(bmglyph2.index, bmglyph2);
				i++;
			}
		}
		if (!mDict.TryGetValue(index, out bmglyph) && createIfMissing)
		{
			bmglyph = new BMGlyph();
			bmglyph.index = index;
			mSaved.Add(bmglyph);
			mDict.Add(index, bmglyph);
		}
		return bmglyph;
	}

	public BMGlyph GetGlyph(Int32 index)
	{
		return GetGlyph(index, false);
	}

	public void Clear()
	{
		mDict.Clear();
		mSaved.Clear();
	}

	public void Trim(Int32 xMin, Int32 yMin, Int32 xMax, Int32 yMax)
	{
		if (isValid)
		{
			Int32 i = 0;
			Int32 count = mSaved.Count;
			while (i < count)
			{
				BMGlyph bmglyph = mSaved[i];
				if (bmglyph != null)
				{
					bmglyph.Trim(xMin, yMin, xMax, yMax);
				}
				i++;
			}
		}
	}
}
