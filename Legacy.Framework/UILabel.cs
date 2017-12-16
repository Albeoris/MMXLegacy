using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Label")]
public class UILabel : UIWidget
{
	[HideInInspector]
	[SerializeField]
	private UIFont mFont;

	[HideInInspector]
	[SerializeField]
	private String mText = String.Empty;

	[SerializeField]
	[HideInInspector]
	private Int32 mMaxLineWidth;

	[HideInInspector]
	[SerializeField]
	private Boolean mEncoding = true;

	[HideInInspector]
	[SerializeField]
	private Int32 mMaxLineCount;

	[HideInInspector]
	[SerializeField]
	private Boolean mPassword;

	[SerializeField]
	[HideInInspector]
	private Boolean mShowLastChar;

	[HideInInspector]
	[SerializeField]
	private Effect mEffectStyle;

	[HideInInspector]
	[SerializeField]
	private Color mEffectColor = Color.black;

	[SerializeField]
	[HideInInspector]
	private UIFont.SymbolStyle mSymbols = UIFont.SymbolStyle.Uncolored;

	[SerializeField]
	[HideInInspector]
	private Vector2 mEffectDistance = Vector2.one;

	[SerializeField]
	[HideInInspector]
	private Boolean mShrinkToFit;

	[SerializeField]
	[HideInInspector]
	private Int32 m_limbicBlankHeight = 100;

	[SerializeField]
	[HideInInspector]
	private Single mLineWidth;

	[HideInInspector]
	[SerializeField]
	private Boolean mMultiline = true;

	private Boolean mShouldBeProcessed = true;

	private String mProcessedText;

	private Vector3 mLastScale = Vector3.one;

	private String mLastText = String.Empty;

	private Int32 mLastWidth;

	private Boolean mLastEncoding = true;

	private Int32 mLastCount;

	private Boolean mLastPass;

	private Boolean mLastShow;

	private Effect mLastEffect;

	private Vector2 mSize = Vector2.zero;

	private Boolean mPremultiply;

	private Boolean hasChanged
	{
		get => mShouldBeProcessed || mLastText != text || mLastWidth != mMaxLineWidth || mLastEncoding != mEncoding || mLastCount != mMaxLineCount || mLastPass != mPassword || mLastShow != mShowLastChar || mLastEffect != mEffectStyle;
	    set
		{
			if (value)
			{
				mChanged = true;
				mShouldBeProcessed = true;
			}
			else
			{
				mShouldBeProcessed = false;
				mLastText = text;
				mLastWidth = mMaxLineWidth;
				mLastEncoding = mEncoding;
				mLastCount = mMaxLineCount;
				mLastPass = mPassword;
				mLastShow = mShowLastChar;
				mLastEffect = mEffectStyle;
			}
		}
	}

	public UIFont font
	{
		get => mFont;
	    set
		{
			if (mFont != value)
			{
				mFont = value;
				material = ((!(mFont != null)) ? null : mFont.material);
				mChanged = true;
				hasChanged = true;
				MarkAsChanged();
			}
		}
	}

	public String text
	{
		get => mText;
	    set
		{
			if (String.IsNullOrEmpty(value))
			{
				if (!String.IsNullOrEmpty(mText))
				{
					mText = String.Empty;
				}
				hasChanged = true;
			}
			else if (mText != value)
			{
				mText = value;
				hasChanged = true;
				if (shrinkToFit)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public Boolean supportEncoding
	{
		get => mEncoding;
	    set
		{
			if (mEncoding != value)
			{
				mEncoding = value;
				hasChanged = true;
				if (value)
				{
					mPassword = false;
				}
			}
		}
	}

	public UIFont.SymbolStyle symbolStyle
	{
		get => mSymbols;
	    set
		{
			if (mSymbols != value)
			{
				mSymbols = value;
				hasChanged = true;
			}
		}
	}

	public Int32 lineWidth
	{
		get => mMaxLineWidth;
	    set
		{
			if (mMaxLineWidth != value)
			{
				mMaxLineWidth = value;
				hasChanged = true;
				if (shrinkToFit)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public Boolean multiLine
	{
		get => mMaxLineCount != 1;
	    set
		{
			if (mMaxLineCount != 1 != value)
			{
				mMaxLineCount = ((!value) ? 1 : 0);
				hasChanged = true;
				if (value)
				{
					mPassword = false;
				}
			}
		}
	}

	public Int32 maxLineCount
	{
		get => mMaxLineCount;
	    set
		{
			if (mMaxLineCount != value)
			{
				mMaxLineCount = Mathf.Max(value, 0);
				hasChanged = true;
				if (value == 1)
				{
					mPassword = false;
				}
			}
		}
	}

	public Boolean password
	{
		get => mPassword;
	    set
		{
			if (mPassword != value)
			{
				if (value)
				{
					mMaxLineCount = 1;
					mEncoding = false;
				}
				mPassword = value;
				hasChanged = true;
			}
		}
	}

	public Boolean showLastPasswordChar
	{
		get => mShowLastChar;
	    set
		{
			if (mShowLastChar != value)
			{
				mShowLastChar = value;
				hasChanged = true;
			}
		}
	}

	public Effect effectStyle
	{
		get => mEffectStyle;
	    set
		{
			if (mEffectStyle != value)
			{
				mEffectStyle = value;
				hasChanged = true;
			}
		}
	}

	public Color effectColor
	{
		get => mEffectColor;
	    set
		{
			if (!mEffectColor.Equals(value))
			{
				mEffectColor = value;
				if (mEffectStyle != Effect.None)
				{
					hasChanged = true;
				}
			}
		}
	}

	public Vector2 effectDistance
	{
		get => mEffectDistance;
	    set
		{
			if (mEffectDistance != value)
			{
				mEffectDistance = value;
				hasChanged = true;
			}
		}
	}

	public Boolean shrinkToFit
	{
		get => mShrinkToFit;
	    set
		{
			if (mShrinkToFit != value)
			{
				mShrinkToFit = value;
				hasChanged = true;
			}
		}
	}

	public Int32 limbicBlankHeight
	{
		get => m_limbicBlankHeight;
	    set
		{
			if (m_limbicBlankHeight != value)
			{
				m_limbicBlankHeight = value;
				hasChanged = true;
			}
		}
	}

	public String processedText
	{
		get
		{
			if (mLastScale != cachedTransform.localScale)
			{
				mLastScale = cachedTransform.localScale;
				mShouldBeProcessed = true;
			}
			if (hasChanged)
			{
				ProcessText();
			}
			return mProcessedText;
		}
	}

	public override Material material
	{
		get
		{
			Material material = base.material;
			if (material == null)
			{
				material = ((!(mFont != null)) ? null : mFont.material);
				this.material = material;
			}
			return material;
		}
	}

	public override Vector2 relativeSize
	{
		get
		{
			if (mFont == null)
			{
				return Vector3.one;
			}
			if (hasChanged)
			{
				ProcessText();
			}
			return mSize;
		}
	}

	protected override void OnStart()
	{
		if (mLineWidth > 0f)
		{
			mMaxLineWidth = Mathf.RoundToInt(mLineWidth);
			mLineWidth = 0f;
		}
		if (!mMultiline)
		{
			mMaxLineCount = 1;
			mMultiline = true;
		}
		mPremultiply = (font != null && font.material != null && font.material.shader.name.Contains("Premultiplied"));
	}

	public override void MarkAsChanged()
	{
		hasChanged = true;
		base.MarkAsChanged();
	}

	private void ProcessText()
	{
		mChanged = true;
		hasChanged = false;
		mLastText = mText;
		Single num = Mathf.Abs(cachedTransform.localScale.x);
		Single num2 = mFont.size * mMaxLineCount;
		if (num > 0f)
		{
			String text = mText.Replace("\\n", "\n");
			do
			{
				if (mPassword)
				{
					mProcessedText = String.Empty;
					if (mShowLastChar)
					{
						Int32 i = 0;
						Int32 num3 = text.Length - 1;
						while (i < num3)
						{
							mProcessedText += "*";
							i++;
						}
						if (text.Length > 0)
						{
							mProcessedText += text[text.Length - 1];
						}
					}
					else
					{
						Int32 j = 0;
						Int32 length = text.Length;
						while (j < length)
						{
							mProcessedText += "*";
							j++;
						}
					}
					mProcessedText = mFont.WrapText(mProcessedText, mMaxLineWidth / num, mMaxLineCount, false, UIFont.SymbolStyle.None);
				}
				else if (mMaxLineWidth > 0)
				{
					mProcessedText = mFont.WrapText(text, mMaxLineWidth / num, (!mShrinkToFit) ? mMaxLineCount : 0, mEncoding, mSymbols);
				}
				else if (!mShrinkToFit && mMaxLineCount > 0)
				{
					mProcessedText = mFont.WrapText(text, 100000f, mMaxLineCount, mEncoding, mSymbols);
				}
				else
				{
					mProcessedText = text;
				}
				mSize = (String.IsNullOrEmpty(mProcessedText) ? Vector2.one : mFont.CalculatePrintedSize(mProcessedText, mEncoding, mSymbols, m_limbicBlankHeight));
				if (!mShrinkToFit)
				{
					goto IL_2EB;
				}
				if (mMaxLineCount <= 0 || mSize.y * num <= num2)
				{
					break;
				}
				num = Mathf.Round(num - 1f);
			}
			while (num > 1f);
			if (mMaxLineWidth > 0)
			{
				Single num4 = mMaxLineWidth / num;
				Single a = (mSize.x * num <= num4) ? num : (num4 / mSize.x * num);
				num = Mathf.Min(a, num);
			}
			num = Mathf.Round(num);
			cachedTransform.localScale = new Vector3(num, num, 1f);
			IL_2EB:
			mSize.x = Mathf.Max(mSize.x, (num <= 0f) ? 1f : (lineWidth / num));
		}
		else
		{
			mSize.x = 1f;
			num = mFont.size;
			cachedTransform.localScale = new Vector3(0.01f, 0.01f, 1f);
			mProcessedText = String.Empty;
		}
		mSize.y = Mathf.Max(mSize.y, 1f);
	}

	public override void MakePixelPerfect()
	{
		if (mFont != null)
		{
			Single pixelSize = font.pixelSize;
			Vector3 localScale = cachedTransform.localScale;
			localScale.x = mFont.size * pixelSize;
			localScale.y = localScale.x;
			localScale.z = 1f;
			Vector3 localPosition = cachedTransform.localPosition;
			localPosition.x = Mathf.CeilToInt(localPosition.x / pixelSize * 4f) >> 2;
			localPosition.y = Mathf.CeilToInt(localPosition.y / pixelSize * 4f) >> 2;
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			localPosition.x *= pixelSize;
			localPosition.y *= pixelSize;
			cachedTransform.localPosition = localPosition;
			cachedTransform.localScale = localScale;
			if (shrinkToFit)
			{
				ProcessText();
			}
		}
		else
		{
			base.MakePixelPerfect();
		}
	}

	private void ApplyShadow(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, Int32 start, Int32 end, Single x, Single y)
	{
		Color color = mEffectColor;
		color.a *= alpha * mPanel.alpha;
		Color32 color2 = (!font.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color);
		verts.AddRanged(verts.buffer, start, end - start);
		uvs.AddRanged(uvs.buffer, start, end - start);
		cols.AddRanged(cols.buffer, start, end - start);
		for (Int32 i = start; i < end; i++)
		{
			Vector3 vector = verts.buffer[i];
			vector.x += x;
			vector.y += y;
			verts.buffer[i] = vector;
			cols.buffer[i] = color2;
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mFont == null)
		{
			return;
		}
		Pivot pivot = this.pivot;
		Int32 num = verts.size;
		Color c = color;
		c.a *= mPanel.alpha;
		if (font.premultipliedAlpha)
		{
			c = NGUITools.ApplyPMA(c);
		}
		if (pivot == Pivot.Left || pivot == Pivot.TopLeft || pivot == Pivot.BottomLeft)
		{
			mFont.Print(processedText, c, verts, uvs, cols, mEncoding, mSymbols, UIFont.Alignment.Left, 0, m_limbicBlankHeight, mPremultiply);
		}
		else if (pivot == Pivot.Right || pivot == Pivot.TopRight || pivot == Pivot.BottomRight)
		{
			mFont.Print(processedText, c, verts, uvs, cols, mEncoding, mSymbols, UIFont.Alignment.Right, Mathf.RoundToInt(relativeSize.x * mFont.size), m_limbicBlankHeight, mPremultiply);
		}
		else
		{
			mFont.Print(processedText, c, verts, uvs, cols, mEncoding, mSymbols, UIFont.Alignment.Center, Mathf.RoundToInt(relativeSize.x * mFont.size), m_limbicBlankHeight, mPremultiply);
		}
		if (effectStyle != Effect.None)
		{
			Int32 size = verts.size;
			Single num2 = 1f / mFont.size;
			Single num3 = num2 * mEffectDistance.x;
			Single num4 = num2 * mEffectDistance.y;
			if (effectStyle == Effect.Outline)
			{
				verts.Reserve((size - num) * 8);
				uvs.Reserve((size - num) * 8);
				cols.Reserve((size - num) * 8);
			}
			else
			{
				verts.Reserve(size - num);
				uvs.Reserve(size - num);
				cols.Reserve(size - num);
			}
			ApplyShadow(verts, uvs, cols, num, size, num3, -num4);
			if (effectStyle == Effect.Outline)
			{
				num = size;
				size = verts.size;
				ApplyShadow(verts, uvs, cols, num, size, -num3, num4);
				num = size;
				size = verts.size;
				ApplyShadow(verts, uvs, cols, num, size, -num3, -num4);
				num = size;
				size = verts.size;
				ApplyShadow(verts, uvs, cols, num, size, num3, num4);
				num = size;
				size = verts.size;
				ApplyShadow(verts, uvs, cols, num, size, -num3, 0f);
				num = size;
				size = verts.size;
				ApplyShadow(verts, uvs, cols, num, size, num3, 0f);
				num = size;
				size = verts.size;
				ApplyShadow(verts, uvs, cols, num, size, 0f, -num4);
				num = size;
				size = verts.size;
				ApplyShadow(verts, uvs, cols, num, size, 0f, num4);
			}
		}
	}

	public enum Effect
	{
		None,
		Shadow,
		Outline
	}
}
