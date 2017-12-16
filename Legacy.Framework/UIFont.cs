using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("NGUI/UI/Font")]
[ExecuteInEditMode]
public class UIFont : MonoBehaviour
{
	private const Int32 STANDARD_HEIGHT = 100;

	[SerializeField]
	[HideInInspector]
	private Material mMat;

	[HideInInspector]
	[SerializeField]
	private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	[SerializeField]
	[HideInInspector]
	private BMFont mFont = new BMFont();

	[SerializeField]
	[HideInInspector]
	private Int32 mSpacingX;

	[SerializeField]
	[HideInInspector]
	private Int32 mSpacingY;

	[SerializeField]
	[HideInInspector]
	private UIAtlas mAtlas;

	[HideInInspector]
	[SerializeField]
	private UIFont mReplacement;

	[SerializeField]
	[HideInInspector]
	private Single mPixelSize = 1f;

	[SerializeField]
	[HideInInspector]
	private List<BMSymbol> mSymbols = new List<BMSymbol>();

	[HideInInspector]
	[SerializeField]
	private Font mDynamicFont;

	[SerializeField]
	[HideInInspector]
	private Int32 mDynamicFontSize = 16;

	[SerializeField]
	[HideInInspector]
	private FontStyle mDynamicFontStyle;

	[SerializeField]
	[HideInInspector]
	private Single mDynamicFontOffset;

	private UIAtlas.Sprite mSprite;

	private Int32 mPMA = -1;

	private Boolean mSpriteSet;

	private List<Color32> mColors = new List<Color32>();

	private static CharacterInfo mChar;

	private Font.FontTextureRebuildCallback m_FontChangedCallback;

	public UIFont()
	{
		m_FontChangedCallback = new Font.FontTextureRebuildCallback(OnFontChanged);
	}

	public BMFont bmFont => (!(mReplacement != null)) ? mFont : mReplacement.bmFont;

    public Int32 texWidth => (!(mReplacement != null)) ? ((mFont == null) ? 1 : mFont.texWidth) : mReplacement.texWidth;

    public Int32 texHeight => (!(mReplacement != null)) ? ((mFont == null) ? 1 : mFont.texHeight) : mReplacement.texHeight;

    public Boolean hasSymbols => (!(mReplacement != null)) ? (mSymbols.Count != 0) : mReplacement.hasSymbols;

    public List<BMSymbol> symbols => (!(mReplacement != null)) ? mSymbols : mReplacement.symbols;

    public UIAtlas atlas
	{
		get => (!(mReplacement != null)) ? mAtlas : mReplacement.atlas;
        set
		{
			if (mReplacement != null)
			{
				mReplacement.atlas = value;
			}
			else if (mAtlas != value)
			{
				if (value == null)
				{
					if (mAtlas != null)
					{
						mMat = mAtlas.spriteMaterial;
					}
					if (sprite != null)
					{
						mUVRect = uvRect;
					}
				}
				mPMA = -1;
				mAtlas = value;
				MarkAsDirty();
			}
		}
	}

	public Material material
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.material;
			}
			if (mAtlas != null)
			{
				return mAtlas.spriteMaterial;
			}
			if (mMat != null)
			{
				if (mDynamicFont != null && mMat != mDynamicFont.material)
				{
					mMat.mainTexture = mDynamicFont.material.mainTexture;
				}
				return mMat;
			}
			if (mDynamicFont != null)
			{
				return mDynamicFont.material;
			}
			return null;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.material = value;
			}
			else if (mMat != value)
			{
				mPMA = -1;
				mMat = value;
				MarkAsDirty();
			}
		}
	}

	public Single pixelSize
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.pixelSize;
			}
			if (mAtlas != null)
			{
				return mAtlas.pixelSize;
			}
			return mPixelSize;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.pixelSize = value;
			}
			else if (mAtlas != null)
			{
				mAtlas.pixelSize = value;
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

	public Boolean premultipliedAlpha
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.premultipliedAlpha;
			}
			if (mAtlas != null)
			{
				return mAtlas.premultipliedAlpha;
			}
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((!(material != null) || !(material.shader != null) || !material.shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return mPMA == 1;
		}
	}

	public Texture2D texture
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.texture;
			}
			Material material = this.material;
			return (!(material != null)) ? null : (material.mainTexture as Texture2D);
		}
	}

	public Rect uvRect
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.uvRect;
			}
			if (mAtlas != null && mSprite == null && sprite != null)
			{
				Texture texture = mAtlas.texture;
				if (texture != null)
				{
					mUVRect = mSprite.outer;
					if (mAtlas.coordinates == UIAtlas.Coordinates.Pixels)
					{
						mUVRect = NGUIMath.ConvertToTexCoords(mUVRect, texture.width, texture.height);
					}
					if (mSprite.hasPadding)
					{
						Rect rect = mUVRect;
						mUVRect.xMin = rect.xMin - mSprite.paddingLeft * rect.width;
						mUVRect.yMin = rect.yMin - mSprite.paddingBottom * rect.height;
						mUVRect.xMax = rect.xMax + mSprite.paddingRight * rect.width;
						mUVRect.yMax = rect.yMax + mSprite.paddingTop * rect.height;
					}
					if (mSprite.hasPadding)
					{
						Trim();
					}
				}
			}
			return mUVRect;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.uvRect = value;
			}
			else if (sprite == null && mUVRect != value)
			{
				mUVRect = value;
				MarkAsDirty();
			}
		}
	}

	public String spriteName
	{
		get => (!(mReplacement != null)) ? mFont.spriteName : mReplacement.spriteName;
	    set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteName = value;
			}
			else if (mFont.spriteName != value)
			{
				mFont.spriteName = value;
				MarkAsDirty();
			}
		}
	}

	public Int32 horizontalSpacing
	{
		get => (!(mReplacement != null)) ? mSpacingX : mReplacement.horizontalSpacing;
	    set
		{
			if (mReplacement != null)
			{
				mReplacement.horizontalSpacing = value;
			}
			else if (mSpacingX != value)
			{
				mSpacingX = value;
				MarkAsDirty();
			}
		}
	}

	public Int32 verticalSpacing
	{
		get => (!(mReplacement != null)) ? mSpacingY : mReplacement.verticalSpacing;
	    set
		{
			if (mReplacement != null)
			{
				mReplacement.verticalSpacing = value;
			}
			else if (mSpacingY != value)
			{
				mSpacingY = value;
				MarkAsDirty();
			}
		}
	}

	public Boolean isValid => mDynamicFont != null || mFont.isValid;

    public Int32 size => (!(mReplacement != null)) ? ((!isDynamic) ? mFont.charSize : mDynamicFontSize) : mReplacement.size;

    public UIAtlas.Sprite sprite
	{
		get
		{
			if (mReplacement != null)
			{
				return mReplacement.sprite;
			}
			if (!mSpriteSet)
			{
				mSprite = null;
			}
			if (mSprite == null)
			{
				if (mAtlas != null && !String.IsNullOrEmpty(mFont.spriteName))
				{
					mSprite = mAtlas.GetSprite(mFont.spriteName);
					if (mSprite == null)
					{
						mSprite = mAtlas.GetSprite(name);
					}
					mSpriteSet = true;
					if (mSprite == null)
					{
						mFont.spriteName = null;
					}
				}
				Int32 i = 0;
				Int32 count = mSymbols.Count;
				while (i < count)
				{
					symbols[i].MarkAsDirty();
					i++;
				}
			}
			return mSprite;
		}
	}

	public UIFont replacement
	{
		get => mReplacement;
	    set
		{
			UIFont uifont = value;
			if (uifont == this)
			{
				uifont = null;
			}
			if (mReplacement != uifont)
			{
				if (uifont != null && uifont.replacement == this)
				{
					uifont.replacement = null;
				}
				if (mReplacement != null)
				{
					MarkAsDirty();
				}
				mReplacement = uifont;
				MarkAsDirty();
			}
		}
	}

	public Boolean isDynamic => mDynamicFont != null;

    public Font dynamicFont
	{
		get => (!(mReplacement != null)) ? mDynamicFont : mReplacement.dynamicFont;
        set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFont = value;
			}
			else if (mDynamicFont != value)
			{
				if (mDynamicFont != null)
				{
					material = null;
				}
				mDynamicFont = value;
				MarkAsDirty();
			}
		}
	}

	public Int32 dynamicFontSize
	{
		get => (!(mReplacement != null)) ? mDynamicFontSize : mReplacement.dynamicFontSize;
	    set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFontSize = value;
			}
			else
			{
				value = Mathf.Clamp(value, 4, 128);
				if (mDynamicFontSize != value)
				{
					mDynamicFontSize = value;
					MarkAsDirty();
				}
			}
		}
	}

	public FontStyle dynamicFontStyle
	{
		get => (!(mReplacement != null)) ? mDynamicFontStyle : mReplacement.dynamicFontStyle;
	    set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFontStyle = value;
			}
			else if (mDynamicFontStyle != value)
			{
				mDynamicFontStyle = value;
				MarkAsDirty();
			}
		}
	}

	private void Trim()
	{
		Texture texture = mAtlas.texture;
		if (texture != null && mSprite != null)
		{
			Rect rect = NGUIMath.ConvertToPixels(mUVRect, this.texture.width, this.texture.height, true);
			Rect rect2 = (mAtlas.coordinates != UIAtlas.Coordinates.TexCoords) ? mSprite.outer : NGUIMath.ConvertToPixels(mSprite.outer, texture.width, texture.height, true);
			Int32 xMin = Mathf.RoundToInt(rect2.xMin - rect.xMin);
			Int32 yMin = Mathf.RoundToInt(rect2.yMin - rect.yMin);
			Int32 xMax = Mathf.RoundToInt(rect2.xMax - rect.xMin);
			Int32 yMax = Mathf.RoundToInt(rect2.yMax - rect.yMin);
			mFont.Trim(xMin, yMin, xMax, yMax);
		}
	}

	private Boolean References(UIFont font)
	{
		return !(font == null) && (font == this || (mReplacement != null && mReplacement.References(font)));
	}

	public static Boolean CheckIfRelated(UIFont a, UIFont b)
	{
		return !(a == null) && !(b == null) && ((a.isDynamic && b.isDynamic && a.dynamicFont.fontNames[0] == b.dynamicFont.fontNames[0]) || a == b || a.References(b) || b.References(a));
	}

	private Texture dynamicTexture
	{
		get
		{
			if (mReplacement)
			{
				return mReplacement.dynamicTexture;
			}
			if (isDynamic)
			{
				return mDynamicFont.material.mainTexture;
			}
			return null;
		}
	}

	public void MarkAsDirty()
	{
		if (mReplacement != null)
		{
			mReplacement.MarkAsDirty();
		}
		RecalculateDynamicOffset();
		mSprite = null;
		UILabel[] array = NGUITools.FindActive<UILabel>();
		Int32 i = 0;
		Int32 num = array.Length;
		while (i < num)
		{
			UILabel uilabel = array[i];
			if (uilabel.enabled && NGUITools.GetActive(uilabel.gameObject) && CheckIfRelated(this, uilabel.font))
			{
				UIFont font = uilabel.font;
				uilabel.font = null;
				uilabel.font = font;
			}
			i++;
		}
		Int32 j = 0;
		Int32 count = mSymbols.Count;
		while (j < count)
		{
			symbols[j].MarkAsDirty();
			j++;
		}
	}

	public Boolean RecalculateDynamicOffset()
	{
		if (mDynamicFont != null)
		{
			mDynamicFont.RequestCharactersInTexture("j", mDynamicFontSize, mDynamicFontStyle);
			CharacterInfo characterInfo;
			mDynamicFont.GetCharacterInfo('j', out characterInfo, mDynamicFontSize, mDynamicFontStyle);
			Single num = mDynamicFontSize + characterInfo.vert.yMax;
			if (!Object.Equals(mDynamicFontOffset, num))
			{
				mDynamicFontOffset = num;
				return true;
			}
		}
		return false;
	}

	public Vector2 CalculatePrintedSize(String text, Boolean encoding, SymbolStyle symbolStyle)
	{
		return CalculatePrintedSize(text, encoding, symbolStyle, 100);
	}

	public Vector2 CalculatePrintedSize(String text, Boolean encoding, SymbolStyle symbolStyle, Int32 limbicBlankHeight)
	{
		if (mReplacement != null)
		{
			return mReplacement.CalculatePrintedSize(text, encoding, symbolStyle, limbicBlankHeight);
		}
		Vector2 zero = Vector2.zero;
		Boolean isDynamic = this.isDynamic;
		if (isDynamic || (mFont != null && mFont.isValid && !String.IsNullOrEmpty(text)))
		{
			if (encoding)
			{
				text = NGUITools.StripSymbols(text);
			}
			if (isDynamic)
			{
				mDynamicFont.textureRebuildCallback = m_FontChangedCallback;
				mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
				mDynamicFont.textureRebuildCallback = null;
			}
			Int32 length = text.Length;
			Int32 num = 0;
			Int32 num2 = 0;
			Int32 num3 = 0;
			Int32 num4 = 0;
			Int32 size = this.size;
			Int32 num5 = size + mSpacingY;
			Boolean flag = encoding && symbolStyle != SymbolStyle.None && hasSymbols;
			Int32 num6 = 0;
			for (Int32 i = 0; i < length; i++)
			{
				Char c = text[i];
				if (c == '\n')
				{
					if (num2 > num)
					{
						num = num2;
					}
					num2 = 0;
					num6++;
					if (num6 == 2)
					{
						num6 = 0;
						num3 += Mathf.RoundToInt(num5 * (Single)limbicBlankHeight / 100f);
					}
					else
					{
						num3 += num5;
					}
					num4 = 0;
				}
				else
				{
					num6 = 0;
					if (c < ' ')
					{
						num4 = 0;
					}
					else if (!isDynamic)
					{
						BMSymbol bmsymbol = (!flag) ? null : MatchSymbol(text, i, length);
						if (bmsymbol == null)
						{
							BMGlyph glyph = mFont.GetGlyph(c);
							if (glyph != null)
							{
								num2 += mSpacingX + ((num4 == 0) ? glyph.advance : (glyph.advance + glyph.GetKerning(num4)));
								num4 = c;
							}
						}
						else
						{
							num2 += mSpacingX + bmsymbol.width;
							i += bmsymbol.length - 1;
							num4 = 0;
						}
					}
					else if (mDynamicFont.GetCharacterInfo(c, out mChar, mDynamicFontSize, mDynamicFontStyle))
					{
						num2 += (Int32)(mSpacingX + mChar.width);
					}
				}
			}
			Single num7 = (size <= 0) ? 1f : (1f / size);
			zero.x = num7 * ((num2 <= num) ? num : num2);
			zero.y = num7 * (num3 + num5);
		}
		return zero;
	}

	private static void EndLine(ref StringBuilder s)
	{
		Int32 num = s.Length - 1;
		if (num > 0 && s[num] == ' ')
		{
			s[num] = '\n';
		}
		else
		{
			s.Append('\n');
		}
	}

	public String GetEndOfLineThatFits(String text, Single maxWidth, Boolean encoding, SymbolStyle symbolStyle)
	{
		if (mReplacement != null)
		{
			return mReplacement.GetEndOfLineThatFits(text, maxWidth, encoding, symbolStyle);
		}
		Int32 num = Mathf.RoundToInt(maxWidth * size);
		if (num < 1)
		{
			return text;
		}
		Int32 length = text.Length;
		Int32 num2 = num;
		BMGlyph bmglyph = null;
		Int32 num3 = length;
		Boolean flag = encoding && symbolStyle != SymbolStyle.None && hasSymbols;
		Boolean isDynamic = this.isDynamic;
		if (isDynamic)
		{
			mDynamicFont.textureRebuildCallback = m_FontChangedCallback;
			mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
			mDynamicFont.textureRebuildCallback = null;
		}
		while (num3 > 0 && num2 > 0)
		{
			Char c = text[--num3];
			BMSymbol bmsymbol = (!flag) ? null : MatchSymbol(text, num3, length);
			Int32 num4 = mSpacingX;
			if (!isDynamic)
			{
				if (bmsymbol != null)
				{
					num4 += bmsymbol.advance;
				}
				else
				{
					BMGlyph glyph = mFont.GetGlyph(c);
					if (glyph == null)
					{
						bmglyph = null;
						continue;
					}
					num4 += glyph.advance + ((bmglyph != null) ? bmglyph.GetKerning(c) : 0);
					bmglyph = glyph;
				}
			}
			else if (mDynamicFont.GetCharacterInfo(c, out mChar, mDynamicFontSize, mDynamicFontStyle))
			{
				num4 += (Int32)mChar.width;
			}
			num2 -= num4;
		}
		if (num2 < 0)
		{
			num3++;
		}
		return text.Substring(num3, length - num3);
	}

	public String WrapText(String text, Single maxWidth, Int32 maxLineCount, Boolean encoding, SymbolStyle symbolStyle)
	{
		if (mReplacement != null)
		{
			return mReplacement.WrapText(text, maxWidth, maxLineCount, encoding, symbolStyle);
		}
		Int32 num = Mathf.RoundToInt(maxWidth * size);
		if (num < 1)
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		Int32 length = text.Length;
		Int32 num2 = num;
		Int32 num3 = 0;
		Int32 num4 = 0;
		Int32 i = 0;
		Boolean flag = true;
		Boolean flag2 = maxLineCount != 1;
		Int32 num5 = 1;
		Boolean flag3 = encoding && symbolStyle != SymbolStyle.None && hasSymbols;
		Boolean isDynamic = this.isDynamic;
		if (isDynamic)
		{
			mDynamicFont.textureRebuildCallback = m_FontChangedCallback;
			mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
			mDynamicFont.textureRebuildCallback = null;
		}
		while (i < length)
		{
			Char c = text[i];
			if (c == '\n')
			{
				if (!flag2 || num5 == maxLineCount)
				{
					break;
				}
				num2 = num;
				if (num4 < i)
				{
					stringBuilder.Append(text.Substring(num4, i - num4 + 1));
				}
				else
				{
					stringBuilder.Append(c);
				}
				flag = true;
				num5++;
				num4 = i + 1;
				num3 = 0;
			}
			else
			{
				if (c == ' ' && num3 != 32 && num4 < i)
				{
					stringBuilder.Append(text.Substring(num4, i - num4 + 1));
					flag = false;
					num4 = i + 1;
					num3 = c;
				}
				if (encoding && c == '[' && i + 2 < length)
				{
					if (text[i + 1] == '-' && text[i + 2] == ']')
					{
						i += 2;
						goto IL_3E7;
					}
					if (i + 7 < length && text[i + 7] == ']' && NGUITools.EncodeColor(NGUITools.ParseColor(text, i + 1)) == text.Substring(i + 1, 6).ToUpper())
					{
						i += 7;
						goto IL_3E7;
					}
				}
				BMSymbol bmsymbol = (!flag3) ? null : MatchSymbol(text, i, length);
				Int32 num6 = mSpacingX;
				if (!isDynamic)
				{
					if (bmsymbol != null)
					{
						num6 += bmsymbol.advance;
					}
					else
					{
						BMGlyph bmglyph = (bmsymbol != null) ? null : mFont.GetGlyph(c);
						if (bmglyph == null)
						{
							goto IL_3E7;
						}
						num6 += ((num3 == 0) ? bmglyph.advance : (bmglyph.advance + bmglyph.GetKerning(num3)));
					}
				}
				else if (mDynamicFont.GetCharacterInfo(c, out mChar, mDynamicFontSize, mDynamicFontStyle))
				{
					num6 += Mathf.RoundToInt(mChar.width);
				}
				num2 -= num6;
				if (num2 < 0)
				{
					if (flag || !flag2 || num5 == maxLineCount)
					{
						stringBuilder.Append(text.Substring(num4, Mathf.Max(0, i - num4)));
						if (!flag2 || num5 == maxLineCount)
						{
							num4 = i;
							break;
						}
						EndLine(ref stringBuilder);
						flag = true;
						num5++;
						if (c == ' ')
						{
							num4 = i + 1;
							num2 = num;
						}
						else
						{
							num4 = i;
							num2 = num - num6;
						}
						num3 = 0;
					}
					else
					{
						while (num4 < length && text[num4] == ' ')
						{
							num4++;
						}
						flag = true;
						num2 = num;
						i = num4 - 1;
						num3 = 0;
						if (!flag2 || num5 == maxLineCount)
						{
							break;
						}
						num5++;
						EndLine(ref stringBuilder);
						goto IL_3E7;
					}
				}
				else
				{
					num3 = c;
				}
				if (!isDynamic && bmsymbol != null)
				{
					i += bmsymbol.length - 1;
					num3 = 0;
				}
			}
			IL_3E7:
			i++;
		}
		if (num4 < i)
		{
			stringBuilder.Append(text.Substring(num4, i - num4));
		}
		return stringBuilder.ToString();
	}

	public String WrapText(String text, Single maxWidth, Int32 maxLineCount, Boolean encoding)
	{
		return WrapText(text, maxWidth, maxLineCount, encoding, SymbolStyle.None);
	}

	public String WrapText(String text, Single maxWidth, Int32 maxLineCount)
	{
		return WrapText(text, maxWidth, maxLineCount, false, SymbolStyle.None);
	}

	private void Align(BetterList<Vector3> verts, Int32 indexOffset, Alignment alignment, Int32 x, Int32 lineWidth)
	{
		if (alignment != Alignment.Left)
		{
			Int32 size = this.size;
			if (size > 0)
			{
				Single num;
				if (alignment == Alignment.Right)
				{
					num = Mathf.RoundToInt(lineWidth - x);
					if (num < 0f)
					{
						num = 0f;
					}
					num /= this.size;
				}
				else
				{
					num = Mathf.RoundToInt((lineWidth - x) * 0.5f);
					if (num < 0f)
					{
						num = 0f;
					}
					num /= this.size;
					if ((lineWidth & 1) == 1)
					{
						num += 0.5f / size;
					}
				}
				for (Int32 i = indexOffset; i < verts.size; i++)
				{
					Vector3 vector = verts.buffer[i];
					vector.x += num;
					verts.buffer[i] = vector;
				}
			}
		}
	}

	private void OnFontChanged()
	{
		MarkAsDirty();
	}

	public void Print(String text, Color32 color, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, Boolean encoding, SymbolStyle symbolStyle, Alignment alignment, Int32 lineWidth, Boolean premultiply)
	{
		Print(text, color, verts, uvs, cols, encoding, symbolStyle, alignment, lineWidth, 100, premultiply);
	}

	public void Print(String text, Color32 color, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, Boolean encoding, SymbolStyle symbolStyle, Alignment alignment, Int32 lineWidth, Int32 limbicblankHeight, Boolean premultiply)
	{
		if (mReplacement != null)
		{
			mReplacement.Print(text, color, verts, uvs, cols, encoding, symbolStyle, alignment, lineWidth, limbicblankHeight, premultiply);
		}
		else if (text != null)
		{
			if (!isValid)
			{
				Debug.LogError("Attempting to print using an invalid font!");
				return;
			}
			Boolean isDynamic = this.isDynamic;
			if (isDynamic)
			{
				mDynamicFont.textureRebuildCallback = m_FontChangedCallback;
				mDynamicFont.RequestCharactersInTexture(text, mDynamicFontSize, mDynamicFontStyle);
				mDynamicFont.textureRebuildCallback = null;
			}
			mColors.Clear();
			mColors.Add(color);
			Int32 size = this.size;
			Vector2 vector = (size <= 0) ? Vector2.one : new Vector2(1f / size, 1f / size);
			Int32 size2 = verts.size;
			Int32 num = 0;
			Int32 num2 = 0;
			Int32 num3 = 0;
			Int32 num4 = 0;
			Int32 num5 = size + mSpacingY;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector2 zero3 = Vector2.zero;
			Vector2 zero4 = Vector2.zero;
			Single num6 = this.uvRect.width / mFont.texWidth;
			Single num7 = mUVRect.height / mFont.texHeight;
			Int32 length = text.Length;
			Boolean flag = encoding && symbolStyle != SymbolStyle.None && hasSymbols && sprite != null;
			verts.Reserve(length * 4);
			uvs.Reserve(length * 4);
			cols.Reserve(length * 4);
			Int32 num8 = 0;
			for (Int32 i = 0; i < length; i++)
			{
				Char c = text[i];
				if (c == '\n')
				{
					if (num2 > num)
					{
						num = num2;
					}
					if (alignment != Alignment.Left)
					{
						Align(verts, size2, alignment, num2, lineWidth);
						size2 = verts.size;
					}
					num2 = 0;
					num8++;
					if (num8 == 2)
					{
						num8 = 0;
						num3 += Mathf.RoundToInt(num5 * (Single)limbicblankHeight / 100f);
					}
					else
					{
						num3 += num5;
					}
					num4 = 0;
				}
				else
				{
					num8 = 0;
					if (c < ' ')
					{
						num4 = 0;
					}
					else
					{
						if (encoding && c == '[')
						{
							Int32 num9 = NGUITools.ParseSymbol(text, i, mColors, premultiply);
							if (num9 > 0)
							{
								Byte a = color.a;
								color = mColors[mColors.Count - 1];
								color.a = a;
								i += num9 - 1;
								goto IL_9C3;
							}
						}
						if (!isDynamic)
						{
							BMSymbol bmsymbol = (!flag) ? null : MatchSymbol(text, i, length);
							if (bmsymbol == null)
							{
								BMGlyph glyph = mFont.GetGlyph(c);
								if (glyph == null)
								{
									goto IL_9C3;
								}
								if (num4 != 0)
								{
									num2 += glyph.GetKerning(num4);
								}
								if (c == ' ')
								{
									num2 += mSpacingX + glyph.advance;
									num4 = c;
									goto IL_9C3;
								}
								zero.x = vector.x * (num2 + glyph.offsetX);
								zero.y = -vector.y * (num3 + glyph.offsetY);
								zero2.x = zero.x + vector.x * glyph.width;
								zero2.y = zero.y - vector.y * glyph.height;
								zero3.x = mUVRect.xMin + num6 * glyph.x;
								zero3.y = mUVRect.yMax - num7 * glyph.y;
								zero4.x = zero3.x + num6 * glyph.width;
								zero4.y = zero3.y - num7 * glyph.height;
								num2 += mSpacingX + glyph.advance;
								num4 = c;
								if (glyph.channel == 0 || glyph.channel == 15)
								{
									for (Int32 j = 0; j < 4; j++)
									{
										cols.Add(color);
									}
								}
								else
								{
									Color color2 = color;
									color2 *= 0.49f;
									switch (glyph.channel)
									{
									case 1:
										color2.b += 0.51f;
										break;
									case 2:
										color2.g += 0.51f;
										break;
									case 4:
										color2.r += 0.51f;
										break;
									case 8:
										color2.a += 0.51f;
										break;
									}
									for (Int32 k = 0; k < 4; k++)
									{
										cols.Add(color2);
									}
								}
							}
							else
							{
								zero.x = vector.x * (num2 + bmsymbol.offsetX);
								zero.y = -vector.y * (num3 + bmsymbol.offsetY);
								zero2.x = zero.x + vector.x * bmsymbol.width;
								zero2.y = zero.y - vector.y * bmsymbol.height;
								Rect uvRect = bmsymbol.uvRect;
								zero3.x = uvRect.xMin;
								zero3.y = uvRect.yMax;
								zero4.x = uvRect.xMax;
								zero4.y = uvRect.yMin;
								num2 += mSpacingX + bmsymbol.advance;
								i += bmsymbol.length - 1;
								num4 = 0;
								if (symbolStyle == SymbolStyle.Colored)
								{
									for (Int32 l = 0; l < 4; l++)
									{
										cols.Add(color);
									}
								}
								else
								{
									Color32 item = Color.white;
									item.a = color.a;
									for (Int32 m = 0; m < 4; m++)
									{
										cols.Add(item);
									}
								}
							}
							verts.Add(new Vector3(zero2.x, zero.y));
							verts.Add(new Vector3(zero2.x, zero2.y));
							verts.Add(new Vector3(zero.x, zero2.y));
							verts.Add(new Vector3(zero.x, zero.y));
							uvs.Add(new Vector2(zero4.x, zero3.y));
							uvs.Add(new Vector2(zero4.x, zero4.y));
							uvs.Add(new Vector2(zero3.x, zero4.y));
							uvs.Add(new Vector2(zero3.x, zero3.y));
						}
						else if (mDynamicFont.GetCharacterInfo(c, out mChar, mDynamicFontSize, mDynamicFontStyle))
						{
							zero.x = vector.x * (num2 + mChar.vert.xMin);
							zero.y = -vector.y * (num3 - mChar.vert.yMax + mDynamicFontOffset);
							zero2.x = zero.x + vector.x * mChar.vert.width;
							zero2.y = zero.y - vector.y * mChar.vert.height;
							zero3.x = mChar.uv.xMin;
							zero3.y = mChar.uv.yMin;
							zero4.x = mChar.uv.xMax;
							zero4.y = mChar.uv.yMax;
							num2 += mSpacingX + (Int32)mChar.width;
							for (Int32 n = 0; n < 4; n++)
							{
								cols.Add(color);
							}
							if (mChar.flipped)
							{
								uvs.Add(new Vector2(zero3.x, zero4.y));
								uvs.Add(new Vector2(zero3.x, zero3.y));
								uvs.Add(new Vector2(zero4.x, zero3.y));
								uvs.Add(new Vector2(zero4.x, zero4.y));
							}
							else
							{
								uvs.Add(new Vector2(zero4.x, zero3.y));
								uvs.Add(new Vector2(zero3.x, zero3.y));
								uvs.Add(new Vector2(zero3.x, zero4.y));
								uvs.Add(new Vector2(zero4.x, zero4.y));
							}
							verts.Add(new Vector3(zero2.x, zero.y));
							verts.Add(new Vector3(zero.x, zero.y));
							verts.Add(new Vector3(zero.x, zero2.y));
							verts.Add(new Vector3(zero2.x, zero2.y));
						}
					}
				}
				IL_9C3:;
			}
			if (alignment != Alignment.Left && size2 < verts.size)
			{
				Align(verts, size2, alignment, num2, lineWidth);
				size2 = verts.size;
			}
		}
	}

	private BMSymbol GetSymbol(String sequence, Boolean createIfMissing)
	{
		Int32 i = 0;
		Int32 count = mSymbols.Count;
		while (i < count)
		{
			BMSymbol bmsymbol = mSymbols[i];
			if (bmsymbol.sequence == sequence)
			{
				return bmsymbol;
			}
			i++;
		}
		if (createIfMissing)
		{
			BMSymbol bmsymbol2 = new BMSymbol();
			bmsymbol2.sequence = sequence;
			mSymbols.Add(bmsymbol2);
			return bmsymbol2;
		}
		return null;
	}

	private BMSymbol MatchSymbol(String text, Int32 offset, Int32 textLength)
	{
		Int32 count = mSymbols.Count;
		if (count == 0)
		{
			return null;
		}
		textLength -= offset;
		for (Int32 i = 0; i < count; i++)
		{
			BMSymbol bmsymbol = mSymbols[i];
			Int32 length = bmsymbol.length;
			if (length != 0 && textLength >= length)
			{
				Boolean flag = true;
				for (Int32 j = 0; j < length; j++)
				{
					if (text[offset + j] != bmsymbol.sequence[j])
					{
						flag = false;
						break;
					}
				}
				if (flag && bmsymbol.Validate(atlas))
				{
					return bmsymbol;
				}
			}
		}
		return null;
	}

	public void AddSymbol(String sequence, String spriteName)
	{
		BMSymbol symbol = GetSymbol(sequence, true);
		symbol.spriteName = spriteName;
		MarkAsDirty();
	}

	public void RemoveSymbol(String sequence)
	{
		BMSymbol symbol = GetSymbol(sequence, false);
		if (symbol != null)
		{
			symbols.Remove(symbol);
		}
		MarkAsDirty();
	}

	public void RenameSymbol(String before, String after)
	{
		BMSymbol symbol = GetSymbol(before, false);
		if (symbol != null)
		{
			symbol.sequence = after;
		}
		MarkAsDirty();
	}

	public Boolean UsesSprite(String s)
	{
		if (!String.IsNullOrEmpty(s))
		{
			if (s.Equals(spriteName))
			{
				return true;
			}
			Int32 i = 0;
			Int32 count = symbols.Count;
			while (i < count)
			{
				BMSymbol bmsymbol = symbols[i];
				if (s.Equals(bmsymbol.spriteName))
				{
					return true;
				}
				i++;
			}
		}
		return false;
	}

	public enum Alignment
	{
		Left,
		Center,
		Right
	}

	public enum SymbolStyle
	{
		None,
		Uncolored,
		Colored
	}
}
