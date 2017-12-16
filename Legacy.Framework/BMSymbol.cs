using System;
using UnityEngine;

[Serializable]
public class BMSymbol
{
	public String sequence;

	public String spriteName;

	private UIAtlas.Sprite mSprite;

	private Boolean mIsValid;

	private Int32 mLength;

	private Int32 mOffsetX;

	private Int32 mOffsetY;

	private Int32 mWidth;

	private Int32 mHeight;

	private Int32 mAdvance;

	private Rect mUV;

	public Int32 length
	{
		get
		{
			if (mLength == 0)
			{
				mLength = sequence.Length;
			}
			return mLength;
		}
	}

	public Int32 offsetX => mOffsetX;

    public Int32 offsetY => mOffsetY;

    public Int32 width => mWidth;

    public Int32 height => mHeight;

    public Int32 advance => mAdvance;

    public Rect uvRect => mUV;

    public void MarkAsDirty()
	{
		mIsValid = false;
	}

	public Boolean Validate(UIAtlas atlas)
	{
		if (atlas == null)
		{
			return false;
		}
		if (!mIsValid)
		{
			if (String.IsNullOrEmpty(spriteName))
			{
				return false;
			}
			mSprite = ((!(atlas != null)) ? null : atlas.GetSprite(spriteName));
			if (mSprite != null)
			{
				Texture texture = atlas.texture;
				if (texture == null)
				{
					mSprite = null;
				}
				else
				{
					Rect rect = mSprite.outer;
					mUV = rect;
					if (atlas.coordinates == UIAtlas.Coordinates.Pixels)
					{
						mUV = NGUIMath.ConvertToTexCoords(mUV, texture.width, texture.height);
					}
					else
					{
						rect = NGUIMath.ConvertToPixels(rect, texture.width, texture.height, true);
					}
					mOffsetX = Mathf.RoundToInt(mSprite.paddingLeft * rect.width);
					mOffsetY = Mathf.RoundToInt(mSprite.paddingTop * rect.width);
					mWidth = Mathf.RoundToInt(rect.width);
					mHeight = Mathf.RoundToInt(rect.height);
					mAdvance = Mathf.RoundToInt(rect.width + (mSprite.paddingRight + mSprite.paddingLeft) * rect.width);
					mIsValid = true;
				}
			}
		}
		return mSprite != null;
	}
}
