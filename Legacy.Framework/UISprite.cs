using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite")]
[ExecuteInEditMode]
public class UISprite : UIWidget
{
	[SerializeField]
	[HideInInspector]
	private UIAtlas mAtlas;

	[HideInInspector]
	[SerializeField]
	private String mSpriteName;

	[HideInInspector]
	[SerializeField]
	private Boolean mFillCenter = true;

	[SerializeField]
	[HideInInspector]
	private Type mType;

	[SerializeField]
	[HideInInspector]
	private FillDirection mFillDirection = FillDirection.Radial360;

	[HideInInspector]
	[SerializeField]
	private Single mFillAmount = 1f;

	[SerializeField]
	[HideInInspector]
	private Boolean mInvert;

	protected UIAtlas.Sprite mSprite;

	protected Rect mInner;

	protected Rect mInnerUV;

	protected Rect mOuter;

	protected Rect mOuterUV;

	protected Vector3 mScale = Vector3.one;

	private Boolean mSpriteSet;

	private static Vector2[] v = new Vector2[4];

	private static Vector2[] xy = new Vector2[4];

	private static Vector2[] uv = new Vector2[4];

	private static Vector2[] oxy = new Vector2[4];

	private static Vector2[] ouv = new Vector2[4];

	private static Single[] matrix = new Single[]
	{
		0.5f,
		1f,
		0f,
		0.5f,
		0.5f,
		1f,
		0.5f,
		1f,
		0f,
		0.5f,
		0.5f,
		1f,
		0f,
		0.5f,
		0f,
		0.5f
	};

	public virtual Type type
	{
		get => mType;
	    set
		{
			if (mType != value)
			{
				mType = value;
				MarkAsChanged();
			}
		}
	}

	public UIAtlas atlas
	{
		get => mAtlas;
	    set
		{
			if (mAtlas != value)
			{
				mAtlas = value;
				mSpriteSet = false;
				mSprite = null;
				material = ((!(mAtlas != null)) ? null : mAtlas.spriteMaterial);
				if (String.IsNullOrEmpty(mSpriteName) && mAtlas != null && mAtlas.spriteList.Count > 0)
				{
					SetAtlasSprite(mAtlas.spriteList[0]);
					mSpriteName = mSprite.name;
				}
				if (!String.IsNullOrEmpty(mSpriteName))
				{
					String spriteName = mSpriteName;
					mSpriteName = String.Empty;
					this.spriteName = spriteName;
					mChanged = true;
					UpdateUVs(true);
				}
			}
		}
	}

	public String spriteName
	{
		get => mSpriteName;
	    set
		{
			if (String.IsNullOrEmpty(value))
			{
				if (String.IsNullOrEmpty(mSpriteName))
				{
					return;
				}
				mSpriteName = String.Empty;
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
			}
			else if (mSpriteName != value)
			{
				mSpriteName = value;
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
				if (isValid)
				{
					UpdateUVs(true);
				}
			}
		}
	}

	public Boolean isValid => GetAtlasSprite() != null;

    public override Material material
	{
		get
		{
			Material material = base.material;
			if (material == null)
			{
				material = ((!(mAtlas != null)) ? null : mAtlas.spriteMaterial);
				mSprite = null;
				this.material = material;
				if (material != null)
				{
					UpdateUVs(true);
				}
			}
			return material;
		}
	}

	public Rect innerUV
	{
		get
		{
			UpdateUVs(false);
			return mInnerUV;
		}
	}

	public Rect outerUV
	{
		get
		{
			UpdateUVs(false);
			return mOuterUV;
		}
	}

	public Boolean fillCenter
	{
		get => mFillCenter;
	    set
		{
			if (mFillCenter != value)
			{
				mFillCenter = value;
				MarkAsChanged();
			}
		}
	}

	public FillDirection fillDirection
	{
		get => mFillDirection;
	    set
		{
			if (mFillDirection != value)
			{
				mFillDirection = value;
				mChanged = true;
			}
		}
	}

	public Single fillAmount
	{
		get => mFillAmount;
	    set
		{
			Single num = Mathf.Clamp01(value);
			if (mFillAmount != num)
			{
				mFillAmount = num;
				mChanged = true;
			}
		}
	}

	public Boolean invert
	{
		get => mInvert;
	    set
		{
			if (mInvert != value)
			{
				mInvert = value;
				mChanged = true;
			}
		}
	}

	public override Vector4 relativePadding
	{
		get
		{
			if (isValid && type == Type.Simple)
			{
				return new Vector4(mSprite.paddingLeft, mSprite.paddingTop, mSprite.paddingRight, mSprite.paddingBottom);
			}
			return base.relativePadding;
		}
	}

	public override Vector4 border
	{
		get
		{
			if (type != Type.Sliced)
			{
				return base.border;
			}
			UIAtlas.Sprite atlasSprite = GetAtlasSprite();
			if (atlasSprite == null)
			{
				return Vector2.zero;
			}
			Rect rect = atlasSprite.outer;
			Rect rect2 = atlasSprite.inner;
			Texture mainTexture = this.mainTexture;
			if (atlas.coordinates == UIAtlas.Coordinates.TexCoords && mainTexture != null)
			{
				rect = NGUIMath.ConvertToPixels(rect, mainTexture.width, mainTexture.height, true);
				rect2 = NGUIMath.ConvertToPixels(rect2, mainTexture.width, mainTexture.height, true);
			}
			return new Vector4(rect2.xMin - rect.xMin, rect2.yMin - rect.yMin, rect.xMax - rect2.xMax, rect.yMax - rect2.yMax) * atlas.pixelSize;
		}
	}

	public override Boolean pixelPerfectAfterResize => type == Type.Sliced;

    public UIAtlas.Sprite GetAtlasSprite()
	{
		if (!mSpriteSet)
		{
			mSprite = null;
		}
		if (mSprite == null && mAtlas != null)
		{
			if (!String.IsNullOrEmpty(mSpriteName))
			{
				UIAtlas.Sprite sprite = mAtlas.GetSprite(mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				SetAtlasSprite(sprite);
			}
			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UIAtlas.Sprite sprite2 = mAtlas.spriteList[0];
				if (sprite2 == null)
				{
					return null;
				}
				SetAtlasSprite(sprite2);
				if (mSprite == null)
				{
					Debug.LogError(mAtlas.name + " seems to have a null sprite!");
					return null;
				}
				mSpriteName = mSprite.name;
			}
			if (mSprite != null)
			{
				material = mAtlas.spriteMaterial;
				UpdateUVs(true);
			}
		}
		return mSprite;
	}

	protected void SetAtlasSprite(UIAtlas.Sprite sp)
	{
		mChanged = true;
		mSpriteSet = true;
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = ((mSprite == null) ? String.Empty : mSprite.name);
			mSprite = sp;
		}
	}

	public virtual void UpdateUVs(Boolean force)
	{
		if ((type == Type.Sliced || type == Type.Tiled) && cachedTransform.localScale != mScale)
		{
			mScale = cachedTransform.localScale;
			mChanged = true;
		}
		if (isValid && force)
		{
			Texture mainTexture = this.mainTexture;
			if (mainTexture != null)
			{
				mInner = mSprite.inner;
				mOuter = mSprite.outer;
				mInnerUV = mInner;
				mOuterUV = mOuter;
				if (atlas.coordinates == UIAtlas.Coordinates.Pixels)
				{
					mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, mainTexture.width, mainTexture.height);
					mInnerUV = NGUIMath.ConvertToTexCoords(mInnerUV, mainTexture.width, mainTexture.height);
				}
			}
		}
	}

	public override void MakePixelPerfect()
	{
		if (!isValid)
		{
			return;
		}
		UpdateUVs(false);
		Type type = this.type;
		if (type == Type.Sliced)
		{
			Vector3 localPosition = cachedTransform.localPosition;
			localPosition.x = Mathf.RoundToInt(localPosition.x);
			localPosition.y = Mathf.RoundToInt(localPosition.y);
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			cachedTransform.localPosition = localPosition;
			Vector3 localScale = cachedTransform.localScale;
			localScale.x = Mathf.RoundToInt(localScale.x * 0.5f) << 1;
			localScale.y = Mathf.RoundToInt(localScale.y * 0.5f) << 1;
			localScale.z = 1f;
			cachedTransform.localScale = localScale;
		}
		else if (type == Type.Tiled)
		{
			Vector3 localPosition2 = cachedTransform.localPosition;
			localPosition2.x = Mathf.RoundToInt(localPosition2.x);
			localPosition2.y = Mathf.RoundToInt(localPosition2.y);
			localPosition2.z = Mathf.RoundToInt(localPosition2.z);
			cachedTransform.localPosition = localPosition2;
			Vector3 localScale2 = cachedTransform.localScale;
			localScale2.x = Mathf.RoundToInt(localScale2.x);
			localScale2.y = Mathf.RoundToInt(localScale2.y);
			localScale2.z = 1f;
			cachedTransform.localScale = localScale2;
		}
		else
		{
			Texture mainTexture = this.mainTexture;
			Vector3 localScale3 = cachedTransform.localScale;
			if (mainTexture != null)
			{
				Rect rect = NGUIMath.ConvertToPixels(outerUV, mainTexture.width, mainTexture.height, true);
				Single pixelSize = atlas.pixelSize;
				localScale3.x = Mathf.RoundToInt(rect.width * pixelSize) * Mathf.Sign(localScale3.x);
				localScale3.y = Mathf.RoundToInt(rect.height * pixelSize) * Mathf.Sign(localScale3.y);
				localScale3.z = 1f;
				cachedTransform.localScale = localScale3;
			}
			Int32 num = Mathf.RoundToInt(Mathf.Abs(localScale3.x) * (1f + mSprite.paddingLeft + mSprite.paddingRight));
			Int32 num2 = Mathf.RoundToInt(Mathf.Abs(localScale3.y) * (1f + mSprite.paddingTop + mSprite.paddingBottom));
			Vector3 localPosition3 = cachedTransform.localPosition;
			localPosition3.x = Mathf.CeilToInt(localPosition3.x * 4f) >> 2;
			localPosition3.y = Mathf.CeilToInt(localPosition3.y * 4f) >> 2;
			localPosition3.z = Mathf.RoundToInt(localPosition3.z);
			if (num % 2 == 1 && (pivot == Pivot.Top || pivot == Pivot.Center || pivot == Pivot.Bottom))
			{
				localPosition3.x += 0.5f;
			}
			if (num2 % 2 == 1 && (pivot == Pivot.Left || pivot == Pivot.Center || pivot == Pivot.Right))
			{
				localPosition3.y += 0.5f;
			}
			cachedTransform.localPosition = localPosition3;
		}
	}

	protected override void OnStart()
	{
		if (mAtlas != null)
		{
			UpdateUVs(true);
		}
	}

	public override void Update()
	{
		base.Update();
		if (mChanged || !mSpriteSet)
		{
			mSpriteSet = true;
			mSprite = null;
			mChanged = true;
			UpdateUVs(true);
		}
		else
		{
			UpdateUVs(false);
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		switch (type)
		{
		case Type.Simple:
			SimpleFill(verts, uvs, cols);
			break;
		case Type.Sliced:
			SlicedFill(verts, uvs, cols);
			break;
		case Type.Tiled:
			TiledFill(verts, uvs, cols);
			break;
		case Type.Filled:
			FilledFill(verts, uvs, cols);
			break;
		}
	}

	protected void SimpleFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector2 item = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		Vector2 item2 = new Vector2(mOuterUV.xMax, mOuterUV.yMax);
		verts.Reserve(4);
		uvs.Reserve(4);
		cols.Reserve(4);
		verts.Add(new Vector3(1f, 0f, 0f));
		verts.Add(new Vector3(1f, -1f, 0f));
		verts.Add(new Vector3(0f, -1f, 0f));
		verts.Add(new Vector3(0f, 0f, 0f));
		uvs.Add(item2);
		uvs.Add(new Vector2(item2.x, item.y));
		uvs.Add(item);
		uvs.Add(new Vector2(item.x, item2.y));
		Color color = this.color;
		color.a *= mPanel.alpha;
		Color32 item3 = (!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color);
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
	}

	protected void SlicedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mOuterUV == mInnerUV)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}
		Texture mainTexture = this.mainTexture;
		v[0] = Vector2.zero;
		v[1] = Vector2.zero;
		v[2] = new Vector2(1f, -1f);
		v[3] = new Vector2(1f, -1f);
		if (mainTexture != null)
		{
			Single pixelSize = atlas.pixelSize;
			Single num = (mInnerUV.xMin - mOuterUV.xMin) * pixelSize;
			Single num2 = (mOuterUV.xMax - mInnerUV.xMax) * pixelSize;
			Single num3 = (mInnerUV.yMax - mOuterUV.yMax) * pixelSize;
			Single num4 = (mOuterUV.yMin - mInnerUV.yMin) * pixelSize;
			Vector3 localScale = cachedTransform.localScale;
			localScale.x = Mathf.Max(0f, localScale.x);
			localScale.y = Mathf.Max(0f, localScale.y);
			Vector2 vector = new Vector2(localScale.x / mainTexture.width, localScale.y / mainTexture.height);
			Vector2 vector2 = new Vector2(num / vector.x, num3 / vector.y);
			Vector2 vector3 = new Vector2(num2 / vector.x, num4 / vector.y);
			Pivot pivot = this.pivot;
			if (pivot == Pivot.Right || pivot == Pivot.TopRight || pivot == Pivot.BottomRight)
			{
				v[0].x = Mathf.Min(0f, 1f - (vector3.x + vector2.x));
				v[1].x = v[0].x + vector2.x;
				v[2].x = v[0].x + Mathf.Max(vector2.x, 1f - vector3.x);
				v[3].x = v[0].x + Mathf.Max(vector2.x + vector3.x, 1f);
			}
			else
			{
				v[1].x = vector2.x;
				v[2].x = Mathf.Max(vector2.x, 1f - vector3.x);
				v[3].x = Mathf.Max(vector2.x + vector3.x, 1f);
			}
			if (pivot == Pivot.Bottom || pivot == Pivot.BottomLeft || pivot == Pivot.BottomRight)
			{
				v[0].y = Mathf.Max(0f, -1f - (vector3.y + vector2.y));
				v[1].y = v[0].y + vector2.y;
				v[2].y = v[0].y + Mathf.Min(vector2.y, -1f - vector3.y);
				v[3].y = v[0].y + Mathf.Min(vector2.y + vector3.y, -1f);
			}
			else
			{
				v[1].y = vector2.y;
				v[2].y = Mathf.Min(vector2.y, -1f - vector3.y);
				v[3].y = Mathf.Min(vector2.y + vector3.y, -1f);
			}
			uv[0] = new Vector2(mOuterUV.xMin, mOuterUV.yMax);
			uv[1] = new Vector2(mInnerUV.xMin, mInnerUV.yMax);
			uv[2] = new Vector2(mInnerUV.xMax, mInnerUV.yMin);
			uv[3] = new Vector2(mOuterUV.xMax, mOuterUV.yMin);
		}
		else
		{
			for (Int32 i = 0; i < 4; i++)
			{
				uv[i] = Vector2.zero;
			}
		}
		Color color = this.color;
		color.a *= mPanel.alpha;
		Color32 item = (!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color);
		if (mFillCenter)
		{
			verts.Reserve(36);
			uvs.Reserve(36);
			cols.Reserve(36);
		}
		else
		{
			verts.Reserve(16);
			uvs.Reserve(16);
			cols.Reserve(16);
		}
		for (Int32 j = 0; j < 3; j++)
		{
			Int32 num5 = j + 1;
			for (Int32 k = 0; k < 3; k++)
			{
				if (mFillCenter || j != 1 || k != 1)
				{
					Int32 num6 = k + 1;
					verts.Add(new Vector3(v[num5].x, v[k].y, 0f));
					verts.Add(new Vector3(v[num5].x, v[num6].y, 0f));
					verts.Add(new Vector3(v[j].x, v[num6].y, 0f));
					verts.Add(new Vector3(v[j].x, v[k].y, 0f));
					uvs.Add(new Vector2(uv[num5].x, uv[k].y));
					uvs.Add(new Vector2(uv[num5].x, uv[num6].y));
					uvs.Add(new Vector2(uv[j].x, uv[num6].y));
					uvs.Add(new Vector2(uv[j].x, uv[k].y));
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
				}
			}
		}
	}

	protected Boolean AdjustRadial(Vector2[] xy, Vector2[] uv, Single fill, Boolean invert)
	{
		if (fill < 0.001f)
		{
			return false;
		}
		if (!invert && fill > 0.999f)
		{
			return true;
		}
		Single num = Mathf.Clamp01(fill);
		if (!invert)
		{
			num = 1f - num;
		}
		num *= 1.57079637f;
		Single num2 = Mathf.Sin(num);
		Single num3 = Mathf.Cos(num);
		if (num2 > num3)
		{
			num3 *= 1f / num2;
			num2 = 1f;
			if (!invert)
			{
				xy[0].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
				xy[3].y = xy[0].y;
				uv[0].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
				uv[3].y = uv[0].y;
			}
		}
		else if (num3 > num2)
		{
			num2 *= 1f / num3;
			num3 = 1f;
			if (invert)
			{
				xy[0].x = Mathf.Lerp(xy[2].x, xy[0].x, num2);
				xy[1].x = xy[0].x;
				uv[0].x = Mathf.Lerp(uv[2].x, uv[0].x, num2);
				uv[1].x = uv[0].x;
			}
		}
		else
		{
			num2 = 1f;
			num3 = 1f;
		}
		if (invert)
		{
			xy[1].y = Mathf.Lerp(xy[2].y, xy[0].y, num3);
			uv[1].y = Mathf.Lerp(uv[2].y, uv[0].y, num3);
		}
		else
		{
			xy[3].x = Mathf.Lerp(xy[2].x, xy[0].x, num2);
			uv[3].x = Mathf.Lerp(uv[2].x, uv[0].x, num2);
		}
		return true;
	}

	protected void Rotate(Vector2[] v, Int32 offset)
	{
		for (Int32 i = 0; i < offset; i++)
		{
			Vector2 vector = new Vector2(v[3].x, v[3].y);
			v[3].x = v[2].y;
			v[3].y = v[2].x;
			v[2].x = v[1].y;
			v[2].y = v[1].x;
			v[1].x = v[0].y;
			v[1].y = v[0].x;
			v[0].x = vector.y;
			v[0].y = vector.x;
		}
	}

	protected void FilledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Single x = 0f;
		Single y = 0f;
		Single num = 1f;
		Single num2 = -1f;
		Single num3 = mOuterUV.xMin;
		Single num4 = mOuterUV.yMin;
		Single num5 = mOuterUV.xMax;
		Single num6 = mOuterUV.yMax;
		if (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical)
		{
			Single num7 = (num5 - num3) * mFillAmount;
			Single num8 = (num6 - num4) * mFillAmount;
			if (fillDirection == FillDirection.Horizontal)
			{
				if (mInvert)
				{
					x = 1f - mFillAmount;
					num3 = num5 - num7;
				}
				else
				{
					num *= mFillAmount;
					num5 = num3 + num7;
				}
			}
			else if (fillDirection == FillDirection.Vertical)
			{
				if (mInvert)
				{
					num2 *= mFillAmount;
					num4 = num6 - num8;
				}
				else
				{
					y = -(1f - mFillAmount);
					num6 = num4 + num8;
				}
			}
		}
		xy[0] = new Vector2(num, y);
		xy[1] = new Vector2(num, num2);
		xy[2] = new Vector2(x, num2);
		xy[3] = new Vector2(x, y);
		uv[0] = new Vector2(num5, num6);
		uv[1] = new Vector2(num5, num4);
		uv[2] = new Vector2(num3, num4);
		uv[3] = new Vector2(num3, num6);
		Color color = this.color;
		color.a *= mPanel.alpha;
		Color32 item = (!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color);
		if (fillDirection == FillDirection.Radial90)
		{
			if (!AdjustRadial(xy, uv, mFillAmount, mInvert))
			{
				return;
			}
		}
		else
		{
			if (fillDirection == FillDirection.Radial180)
			{
				for (Int32 i = 0; i < 2; i++)
				{
					oxy[0] = new Vector2(0f, 0f);
					oxy[1] = new Vector2(0f, 1f);
					oxy[2] = new Vector2(1f, 1f);
					oxy[3] = new Vector2(1f, 0f);
					ouv[0] = new Vector2(0f, 0f);
					ouv[1] = new Vector2(0f, 1f);
					ouv[2] = new Vector2(1f, 1f);
					ouv[3] = new Vector2(1f, 0f);
					if (mInvert)
					{
						if (i > 0)
						{
							Rotate(oxy, i);
							Rotate(ouv, i);
						}
					}
					else if (i < 1)
					{
						Rotate(oxy, 1 - i);
						Rotate(ouv, 1 - i);
					}
					Single num9;
					Single num10;
					if (i == 1)
					{
						num9 = ((!mInvert) ? 1f : 0.5f);
						num10 = ((!mInvert) ? 0.5f : 1f);
					}
					else
					{
						num9 = ((!mInvert) ? 0.5f : 1f);
						num10 = ((!mInvert) ? 1f : 0.5f);
					}
					oxy[1].y = Mathf.Lerp(num9, num10, oxy[1].y);
					oxy[2].y = Mathf.Lerp(num9, num10, oxy[2].y);
					ouv[1].y = Mathf.Lerp(num9, num10, ouv[1].y);
					ouv[2].y = Mathf.Lerp(num9, num10, ouv[2].y);
					Single fill = mFillAmount * 2f - i;
					Boolean flag = i % 2 == 1;
					if (AdjustRadial(oxy, ouv, fill, !flag))
					{
						if (mInvert)
						{
							flag = !flag;
						}
						verts.Reserve(4);
						uvs.Reserve(4);
						cols.Reserve(4);
						if (flag)
						{
							for (Int32 j = 0; j < 4; j++)
							{
								num9 = Mathf.Lerp(xy[0].x, xy[2].x, oxy[j].x);
								num10 = Mathf.Lerp(xy[0].y, xy[2].y, oxy[j].y);
								Single x2 = Mathf.Lerp(uv[0].x, uv[2].x, ouv[j].x);
								Single y2 = Mathf.Lerp(uv[0].y, uv[2].y, ouv[j].y);
								verts.Add(new Vector3(num9, num10, 0f));
								uvs.Add(new Vector2(x2, y2));
								cols.Add(item);
							}
						}
						else
						{
							for (Int32 k = 3; k > -1; k--)
							{
								num9 = Mathf.Lerp(xy[0].x, xy[2].x, oxy[k].x);
								num10 = Mathf.Lerp(xy[0].y, xy[2].y, oxy[k].y);
								Single x3 = Mathf.Lerp(uv[0].x, uv[2].x, ouv[k].x);
								Single y3 = Mathf.Lerp(uv[0].y, uv[2].y, ouv[k].y);
								verts.Add(new Vector3(num9, num10, 0f));
								uvs.Add(new Vector2(x3, y3));
								cols.Add(item);
							}
						}
					}
				}
				return;
			}
			if (fillDirection == FillDirection.Radial360)
			{
				for (Int32 l = 0; l < 4; l++)
				{
					oxy[0] = new Vector2(0f, 0f);
					oxy[1] = new Vector2(0f, 1f);
					oxy[2] = new Vector2(1f, 1f);
					oxy[3] = new Vector2(1f, 0f);
					ouv[0] = new Vector2(0f, 0f);
					ouv[1] = new Vector2(0f, 1f);
					ouv[2] = new Vector2(1f, 1f);
					ouv[3] = new Vector2(1f, 0f);
					if (mInvert)
					{
						if (l > 0)
						{
							Rotate(oxy, l);
							Rotate(ouv, l);
						}
					}
					else if (l < 3)
					{
						Rotate(oxy, 3 - l);
						Rotate(ouv, 3 - l);
					}
					for (Int32 m = 0; m < 4; m++)
					{
						Int32 num11 = (!mInvert) ? (l * 4) : ((3 - l) * 4);
						Single from = matrix[num11];
						Single to = matrix[num11 + 1];
						Single from2 = matrix[num11 + 2];
						Single to2 = matrix[num11 + 3];
						oxy[m].x = Mathf.Lerp(from, to, oxy[m].x);
						oxy[m].y = Mathf.Lerp(from2, to2, oxy[m].y);
						ouv[m].x = Mathf.Lerp(from, to, ouv[m].x);
						ouv[m].y = Mathf.Lerp(from2, to2, ouv[m].y);
					}
					Single fill2 = mFillAmount * 4f - l;
					Boolean flag2 = l % 2 == 1;
					if (AdjustRadial(oxy, ouv, fill2, !flag2))
					{
						if (mInvert)
						{
							flag2 = !flag2;
						}
						verts.Reserve(4);
						uvs.Reserve(4);
						cols.Reserve(4);
						if (flag2)
						{
							for (Int32 n = 0; n < 4; n++)
							{
								Single x4 = Mathf.Lerp(xy[0].x, xy[2].x, oxy[n].x);
								Single y4 = Mathf.Lerp(xy[0].y, xy[2].y, oxy[n].y);
								Single x5 = Mathf.Lerp(uv[0].x, uv[2].x, ouv[n].x);
								Single y5 = Mathf.Lerp(uv[0].y, uv[2].y, ouv[n].y);
								verts.Add(new Vector3(x4, y4, 0f));
								uvs.Add(new Vector2(x5, y5));
								cols.Add(item);
							}
						}
						else
						{
							for (Int32 num12 = 3; num12 > -1; num12--)
							{
								Single x6 = Mathf.Lerp(xy[0].x, xy[2].x, oxy[num12].x);
								Single y6 = Mathf.Lerp(xy[0].y, xy[2].y, oxy[num12].y);
								Single x7 = Mathf.Lerp(uv[0].x, uv[2].x, ouv[num12].x);
								Single y7 = Mathf.Lerp(uv[0].y, uv[2].y, ouv[num12].y);
								verts.Add(new Vector3(x6, y6, 0f));
								uvs.Add(new Vector2(x7, y7));
								cols.Add(item);
							}
						}
					}
				}
				return;
			}
		}
		verts.Reserve(4);
		uvs.Reserve(4);
		cols.Reserve(4);
		for (Int32 num13 = 0; num13 < 4; num13++)
		{
			verts.Add(xy[num13]);
			uvs.Add(uv[num13]);
			cols.Add(item);
		}
	}

	protected void TiledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture mainTexture = material.mainTexture;
		if (mainTexture == null)
		{
			return;
		}
		Rect rect = mInner;
		if (atlas.coordinates == UIAtlas.Coordinates.TexCoords)
		{
			rect = NGUIMath.ConvertToPixels(rect, mainTexture.width, mainTexture.height, true);
		}
		Vector2 vector = cachedTransform.localScale;
		Single pixelSize = atlas.pixelSize;
		Single num = Mathf.Abs(rect.width / vector.x) * pixelSize;
		Single num2 = Mathf.Abs(rect.height / vector.y) * pixelSize;
		if (num < 0.01f || num2 < 0.01f)
		{
			Debug.LogWarning("The tiled sprite (" + NGUITools.GetHierarchy(gameObject) + ") is too small.\nConsider using a bigger one.");
			num = 0.01f;
			num2 = 0.01f;
		}
		Vector2 vector2 = new Vector2(rect.xMin / mainTexture.width, rect.yMin / mainTexture.height);
		Vector2 vector3 = new Vector2(rect.xMax / mainTexture.width, rect.yMax / mainTexture.height);
		Vector2 vector4 = vector3;
		Color color = this.color;
		color.a *= mPanel.alpha;
		Color32 item = (!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color);
		Single num3 = 0f;
		verts.Reserve((Int32)(1f / num2 * (1f / num)));
		uvs.Reserve((Int32)(1f / num2 * (1f / num)));
		cols.Reserve((Int32)(1f / num2 * (1f / num)));
		while (num3 < 1f)
		{
			Single num4 = 0f;
			vector4.x = vector3.x;
			Single num5 = num3 + num2;
			if (num5 > 1f)
			{
				vector4.y = vector2.y + (vector3.y - vector2.y) * (1f - num3) / (num5 - num3);
				num5 = 1f;
			}
			while (num4 < 1f)
			{
				Single num6 = num4 + num;
				if (num6 > 1f)
				{
					vector4.x = vector2.x + (vector3.x - vector2.x) * (1f - num4) / (num6 - num4);
					num6 = 1f;
				}
				verts.Add(new Vector3(num6, -num3, 0f));
				verts.Add(new Vector3(num6, -num5, 0f));
				verts.Add(new Vector3(num4, -num5, 0f));
				verts.Add(new Vector3(num4, -num3, 0f));
				uvs.Add(new Vector2(vector4.x, 1f - vector2.y));
				uvs.Add(new Vector2(vector4.x, 1f - vector4.y));
				uvs.Add(new Vector2(vector2.x, 1f - vector4.y));
				uvs.Add(new Vector2(vector2.x, 1f - vector2.y));
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
				num4 += num;
			}
			num3 += num2;
		}
	}

	public enum Type
	{
		Simple,
		Sliced,
		Tiled,
		Filled
	}

	public enum FillDirection
	{
		Horizontal,
		Vertical,
		Radial90,
		Radial180,
		Radial360
	}
}
