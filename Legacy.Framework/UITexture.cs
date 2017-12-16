using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Texture")]
[ExecuteInEditMode]
public class UITexture : UIWidget
{
	[HideInInspector]
	[SerializeField]
	private Rect mRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private Shader mShader;

	[HideInInspector]
	[SerializeField]
	private Texture mTexture;

	[SerializeField]
	private Material mDynamicMat;

	private Boolean mCreatingMat;

	private Int32 mPMA = -1;

	public Rect uvRect
	{
		get => mRect;
	    set
		{
			if (mRect != value)
			{
				mRect = value;
				MarkAsChanged();
			}
		}
	}

	public Shader shader
	{
		get
		{
			if (mShader == null)
			{
				Material material = this.material;
				if (material != null)
				{
					mShader = material.shader;
				}
				if (mShader == null)
				{
					mShader = Shader.Find("Unlit/Texture");
				}
			}
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				mShader = value;
				Material material = this.material;
				if (material != null)
				{
					material.shader = value;
				}
				mPMA = -1;
			}
		}
	}

	public Boolean hasDynamicMaterial => mDynamicMat != null;

    public override Boolean keepMaterial => true;

    public override Material material
	{
		get
		{
			if (!mCreatingMat && mMat == null)
			{
				mCreatingMat = true;
				if (mainTexture != null)
				{
					if (mShader == null)
					{
						mShader = Shader.Find("Unlit/Texture");
					}
					mDynamicMat = new Material(mShader);
					mDynamicMat.hideFlags = HideFlags.DontSave;
					mDynamicMat.mainTexture = mainTexture;
					base.material = mDynamicMat;
					mPMA = 0;
				}
				mCreatingMat = false;
			}
			return mMat;
		}
		set
		{
			if (mDynamicMat != value && mDynamicMat != null)
			{
				DestroyImmediate(mDynamicMat);
				mDynamicMat = null;
			}
			base.material = value;
			mPMA = -1;
		}
	}

	public Boolean premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((!(material != null) || !(material.shader != null) || !material.shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return mPMA == 1;
		}
	}

	public override Texture mainTexture
	{
		get => (!(mTexture != null)) ? base.mainTexture : mTexture;
	    set
		{
			if (mPanel != null && mMat != null)
			{
				mPanel.RemoveWidget(this);
			}
			if (mMat == null)
			{
				mDynamicMat = new Material(shader);
				mDynamicMat.hideFlags = HideFlags.DontSave;
				mMat = mDynamicMat;
			}
			mPanel = null;
			mTex = value;
			mTexture = value;
			mMat.mainTexture = value;
			if (enabled)
			{
				CreatePanel();
			}
		}
	}

	private void OnDestroy()
	{
		DestroyImmediate(mDynamicMat);
	}

	public override void MakePixelPerfect()
	{
		Texture mainTexture = this.mainTexture;
		if (mainTexture != null)
		{
			Vector3 localScale = cachedTransform.localScale;
			localScale.x = mainTexture.width * uvRect.width;
			localScale.y = mainTexture.height * uvRect.height;
			localScale.z = 1f;
			cachedTransform.localScale = localScale;
		}
		base.MakePixelPerfect();
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Color color = this.color;
		color.a *= mPanel.alpha;
		Color32 item = (!premultipliedAlpha) ? color : NGUITools.ApplyPMA(color);
		verts.Reserve(4);
		uvs.Reserve(4);
		cols.Reserve(4);
		verts.Add(new Vector3(1f, 0f, 0f));
		verts.Add(new Vector3(1f, -1f, 0f));
		verts.Add(new Vector3(0f, -1f, 0f));
		verts.Add(new Vector3(0f, 0f, 0f));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
		uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
		uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
		uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
	}
}
