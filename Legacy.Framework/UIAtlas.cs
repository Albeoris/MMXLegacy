using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Atlas")]
public class UIAtlas : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private Material material;

	[SerializeField]
	[HideInInspector]
	private List<Sprite> sprites = new List<Sprite>();

	[HideInInspector]
	[SerializeField]
	private Coordinates mCoordinates;

	[SerializeField]
	[HideInInspector]
	private Single mPixelSize = 1f;

	[SerializeField]
	[HideInInspector]
	private UIAtlas mReplacement;

	private Int32 mPMA = -1;

	public Material spriteMaterial
	{
		get => (!(mReplacement != null)) ? material : mReplacement.spriteMaterial;
	    set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteMaterial = value;
			}
			else if (material == null)
			{
				mPMA = 0;
				material = value;
			}
			else
			{
				MarkAsDirty();
				mPMA = -1;
				material = value;
				MarkAsDirty();
			}
		}
	}

	public Boolean premultipliedAlpha
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.premultipliedAlpha;
			}
			if (mPMA == -1)
			{
				Material spriteMaterial = this.spriteMaterial;
				mPMA = ((!(spriteMaterial != null) || !(spriteMaterial.shader != null) || !spriteMaterial.shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return mPMA == 1;
		}
	}

	public List<Sprite> spriteList
	{
		get => (!(mReplacement != null)) ? sprites : mReplacement.spriteList;
	    set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteList = value;
			}
			else
			{
				sprites = value;
			}
		}
	}

	public Texture texture => (!(mReplacement != null)) ? ((!(material != null)) ? null : material.mainTexture) : mReplacement.texture;

    public Coordinates coordinates
	{
		get => (!(mReplacement != null)) ? mCoordinates : mReplacement.coordinates;
        set
		{
			if (mReplacement != null)
			{
				mReplacement.coordinates = value;
			}
			else if (mCoordinates != value)
			{
				if (material == null || material.mainTexture == null)
				{
					Debug.LogError("Can't switch coordinates until the atlas material has a valid texture");
					return;
				}
				mCoordinates = value;
				Texture mainTexture = material.mainTexture;
				Int32 i = 0;
				Int32 count = sprites.Count;
				while (i < count)
				{
					Sprite sprite = sprites[i];
					if (mCoordinates == Coordinates.TexCoords)
					{
						sprite.outer = NGUIMath.ConvertToTexCoords(sprite.outer, mainTexture.width, mainTexture.height);
						sprite.inner = NGUIMath.ConvertToTexCoords(sprite.inner, mainTexture.width, mainTexture.height);
					}
					else
					{
						sprite.outer = NGUIMath.ConvertToPixels(sprite.outer, mainTexture.width, mainTexture.height, true);
						sprite.inner = NGUIMath.ConvertToPixels(sprite.inner, mainTexture.width, mainTexture.height, true);
					}
					i++;
				}
			}
		}
	}

	public Single pixelSize
	{
		get => (!(mReplacement != null)) ? mPixelSize : mReplacement.pixelSize;
	    set
		{
			if (mReplacement != null)
			{
				mReplacement.pixelSize = value;
			}
			else
			{
				Single num = Mathf.Clamp(value, 0.25f, 4f);
				if (mPixelSize != num)
				{
					mPixelSize = num;
					MarkAsDirty();
				}
			}
		}
	}

	public UIAtlas replacement
	{
		get => mReplacement;
	    set
		{
			UIAtlas uiatlas = value;
			if (uiatlas == this)
			{
				uiatlas = null;
			}
			if (mReplacement != uiatlas)
			{
				if (uiatlas != null && uiatlas.replacement == this)
				{
					uiatlas.replacement = null;
				}
				if (mReplacement != null)
				{
					MarkAsDirty();
				}
				mReplacement = uiatlas;
				MarkAsDirty();
			}
		}
	}

	public Sprite GetSprite(String name)
	{
		if (mReplacement != null)
		{
			return mReplacement.GetSprite(name);
		}
		if (!String.IsNullOrEmpty(name))
		{
			Int32 i = 0;
			Int32 count = sprites.Count;
			while (i < count)
			{
				Sprite sprite = sprites[i];
				if (!String.IsNullOrEmpty(sprite.name) && name == sprite.name)
				{
					return sprite;
				}
				i++;
			}
		}
		return null;
	}

	private static Int32 CompareString(String a, String b)
	{
		return a.CompareTo(b);
	}

	public BetterList<String> GetListOfSprites()
	{
		if (mReplacement != null)
		{
			return mReplacement.GetListOfSprites();
		}
		BetterList<String> betterList = new BetterList<String>();
		Int32 i = 0;
		Int32 count = sprites.Count;
		while (i < count)
		{
			Sprite sprite = sprites[i];
			if (sprite != null && !String.IsNullOrEmpty(sprite.name))
			{
				betterList.Add(sprite.name);
			}
			i++;
		}
		return betterList;
	}

	public BetterList<String> GetListOfSprites(String match)
	{
		if (mReplacement != null)
		{
			return mReplacement.GetListOfSprites(match);
		}
		if (String.IsNullOrEmpty(match))
		{
			return GetListOfSprites();
		}
		BetterList<String> betterList = new BetterList<String>();
		Int32 i = 0;
		Int32 count = sprites.Count;
		while (i < count)
		{
			Sprite sprite = sprites[i];
			if (sprite != null && !String.IsNullOrEmpty(sprite.name) && String.Equals(match, sprite.name, StringComparison.OrdinalIgnoreCase))
			{
				betterList.Add(sprite.name);
				return betterList;
			}
			i++;
		}
		String[] array = match.Split(new Char[]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		for (Int32 j = 0; j < array.Length; j++)
		{
			array[j] = array[j].ToLower();
		}
		Int32 k = 0;
		Int32 count2 = sprites.Count;
		while (k < count2)
		{
			Sprite sprite2 = sprites[k];
			if (sprite2 != null && !String.IsNullOrEmpty(sprite2.name))
			{
				String text = sprite2.name.ToLower();
				Int32 num = 0;
				for (Int32 l = 0; l < array.Length; l++)
				{
					if (text.Contains(array[l]))
					{
						num++;
					}
				}
				if (num == array.Length)
				{
					betterList.Add(sprite2.name);
				}
			}
			k++;
		}
		return betterList;
	}

	private Boolean References(UIAtlas atlas)
	{
		return !(atlas == null) && (atlas == this || (mReplacement != null && mReplacement.References(atlas)));
	}

	public static Boolean CheckIfRelated(UIAtlas a, UIAtlas b)
	{
		return !(a == null) && !(b == null) && (a == b || a.References(b) || b.References(a));
	}

	public void MarkAsDirty()
	{
		if (mReplacement != null)
		{
			mReplacement.MarkAsDirty();
		}
		UISprite[] array = NGUITools.FindActive<UISprite>();
		Int32 i = 0;
		Int32 num = array.Length;
		while (i < num)
		{
			UISprite uisprite = array[i];
			if (CheckIfRelated(this, uisprite.atlas))
			{
				UIAtlas atlas = uisprite.atlas;
				uisprite.atlas = null;
				uisprite.atlas = atlas;
			}
			i++;
		}
		UIFont[] array2 = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
		Int32 j = 0;
		Int32 num2 = array2.Length;
		while (j < num2)
		{
			UIFont uifont = array2[j];
			if (CheckIfRelated(this, uifont.atlas))
			{
				UIAtlas atlas2 = uifont.atlas;
				uifont.atlas = null;
				uifont.atlas = atlas2;
			}
			j++;
		}
		UILabel[] array3 = NGUITools.FindActive<UILabel>();
		Int32 k = 0;
		Int32 num3 = array3.Length;
		while (k < num3)
		{
			UILabel uilabel = array3[k];
			if (uilabel.font != null && CheckIfRelated(this, uilabel.font.atlas))
			{
				UIFont font = uilabel.font;
				uilabel.font = null;
				uilabel.font = font;
			}
			k++;
		}
	}

	[Serializable]
	public class Sprite
	{
		public String name = "Unity Bug";

		public Rect outer = new Rect(0f, 0f, 1f, 1f);

		public Rect inner = new Rect(0f, 0f, 1f, 1f);

		public Boolean rotated;

		public Single paddingLeft;

		public Single paddingRight;

		public Single paddingTop;

		public Single paddingBottom;

		public Boolean hasPadding => paddingLeft != 0f || paddingRight != 0f || paddingTop != 0f || paddingBottom != 0f;
	}

	public enum Coordinates
	{
		Pixels,
		TexCoords
	}
}
