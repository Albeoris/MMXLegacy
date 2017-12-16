using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Draw Call")]
[ExecuteInEditMode]
public class UIDrawCall : MonoBehaviour
{
	private Transform mTrans;

	private Material mSharedMat;

	private Mesh mMesh0;

	private Mesh mMesh1;

	private MeshFilter mFilter;

	private MeshRenderer mRen;

	private Clipping mClipping;

	private Vector4 mClipRange;

	private Vector2 mClipSoft;

	private Material mClippedMat;

	private Material mDepthMat;

	private Int32[] mIndices;

	private BetterList<Vector3> mVerts = new BetterList<Vector3>();

	private BetterList<Vector2> mUvs = new BetterList<Vector2>();

	private BetterList<Color32> mCols = new BetterList<Color32>();

	private BetterList<Vector3> mNorms = new BetterList<Vector3>();

	private BetterList<Vector4> mTans = new BetterList<Vector4>();

	private Boolean mDepthPass;

	private Boolean mReset = true;

	private Boolean mEven = true;

	[SerializeField]
	public Int32 layer;

	public Boolean depthPass
	{
		get => mDepthPass;
	    set
		{
			if (mDepthPass != value)
			{
				mDepthPass = value;
				mReset = true;
			}
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = transform;
			}
			return mTrans;
		}
	}

	public Material material
	{
		get => mSharedMat;
	    set => mSharedMat = value;
	}

	public Int32 triangles
	{
		get
		{
			Mesh mesh = (!mEven) ? mMesh1 : mMesh0;
			return (!(mesh != null)) ? 0 : (mesh.vertexCount >> 1);
		}
	}

	public Boolean isClipped => mClippedMat != null;

    public Clipping clipping
	{
		get => mClipping;
        set
		{
			if (mClipping != value)
			{
				mClipping = value;
				mReset = true;
			}
		}
	}

	public Vector4 clipRange
	{
		get => mClipRange;
	    set => mClipRange = value;
	}

	public Vector2 clipSoftness
	{
		get => mClipSoft;
	    set => mClipSoft = value;
	}

	private Mesh GetMesh(ref Boolean rebuildIndices, Int32 vertexCount)
	{
		mEven = !mEven;
		if (mEven)
		{
			if (mMesh0 == null)
			{
				mMesh0 = new Mesh();
				mMesh0.hideFlags = HideFlags.DontSave;
				mMesh0.MarkDynamic();
				rebuildIndices = true;
			}
			else if (rebuildIndices || mMesh0.vertexCount != vertexCount)
			{
				rebuildIndices = true;
				mMesh0.Clear();
			}
			return mMesh0;
		}
		if (mMesh1 == null)
		{
			mMesh1 = new Mesh();
			mMesh1.hideFlags = HideFlags.DontSave;
			mMesh1.MarkDynamic();
			rebuildIndices = true;
		}
		else if (rebuildIndices || mMesh1.vertexCount != vertexCount)
		{
			rebuildIndices = true;
			mMesh1.Clear();
		}
		return mMesh1;
	}

	private void UpdateMaterials()
	{
		Boolean flag = mClipping != Clipping.None;
		if (flag)
		{
			Shader shader = null;
			if (mClipping != Clipping.None)
			{
				String text = mSharedMat.shader.name;
				text = text.Replace(" (AlphaClip)", String.Empty);
				text = text.Replace(" (SoftClip)", String.Empty);
				if (mClipping == Clipping.HardClip || mClipping == Clipping.AlphaClip)
				{
					shader = Shader.Find(text + " (AlphaClip)");
				}
				else if (mClipping == Clipping.SoftClip)
				{
					shader = Shader.Find(text + " (SoftClip)");
				}
				if (shader == null)
				{
					mClipping = Clipping.None;
				}
			}
			if (shader != null)
			{
				if (mClippedMat == null)
				{
					mClippedMat = new Material(mSharedMat);
					mClippedMat.hideFlags = HideFlags.DontSave;
				}
				mClippedMat.shader = shader;
				mClippedMat.CopyPropertiesFromMaterial(mSharedMat);
			}
			else if (mClippedMat != null)
			{
				DestroyImmediate(mClippedMat);
				mClippedMat = null;
			}
		}
		else if (mClippedMat != null)
		{
			DestroyImmediate(mClippedMat);
			mClippedMat = null;
		}
		if (mDepthPass)
		{
			if (mDepthMat == null)
			{
				Shader shader2 = Shader.Find("Unlit/Depth Cutout");
				mDepthMat = new Material(shader2);
				mDepthMat.hideFlags = HideFlags.DontSave;
			}
			mDepthMat.mainTexture = mSharedMat.mainTexture;
		}
		else if (mDepthMat != null)
		{
			DestroyImmediate(mDepthMat);
			mDepthMat = null;
		}
		if (mRen == null)
		{
			mRen = gameObject.GetComponent<MeshRenderer>();
		}
		if (mRen == null)
		{
			mRen = gameObject.AddComponent<MeshRenderer>();
		}
		Material material = (!(mClippedMat != null)) ? mSharedMat : mClippedMat;
		if (mDepthMat != null)
		{
			if (mRen.sharedMaterials != null && mRen.sharedMaterials.Length == 2 && mRen.sharedMaterials[1] == material)
			{
				return;
			}
			mRen.sharedMaterials = new Material[]
			{
				mDepthMat,
				material
			};
		}
		else if (mRen.sharedMaterial != material)
		{
			mRen.sharedMaterials = new Material[]
			{
				material
			};
		}
	}

	public void Set(BetterList<Vector3> verts, BetterList<Vector3> norms, BetterList<Vector4> tans, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Int32 size = verts.size;
		if (size > 0 && size == uvs.size && size == cols.size && size % 4 == 0)
		{
			if (mFilter == null)
			{
				mFilter = gameObject.GetComponent<MeshFilter>();
			}
			if (mFilter == null)
			{
				mFilter = gameObject.AddComponent<MeshFilter>();
			}
			if (mRen == null)
			{
				mRen = gameObject.GetComponent<MeshRenderer>();
			}
			if (mRen == null)
			{
				mRen = gameObject.AddComponent<MeshRenderer>();
				UpdateMaterials();
			}
			else if (mClippedMat != null && mClippedMat.mainTexture != mSharedMat.mainTexture)
			{
				UpdateMaterials();
			}
			if (verts.size < 65000)
			{
				Int32 num = (size >> 1) * 3;
				Boolean flag = mIndices == null || mIndices.Length != num;
				if (flag)
				{
					mIndices = new Int32[num];
					Int32 num2 = 0;
					for (Int32 i = 0; i < size; i += 4)
					{
						mIndices[num2++] = i;
						mIndices[num2++] = i + 1;
						mIndices[num2++] = i + 2;
						mIndices[num2++] = i + 2;
						mIndices[num2++] = i + 3;
						mIndices[num2++] = i;
					}
				}
				mVerts.Clear();
				mVerts.AddRanged(verts.buffer, 0, verts.size);
				if (norms != null)
				{
					mNorms.Clear();
					mNorms.AddRanged(norms.buffer, 0, norms.size);
				}
				if (tans != null)
				{
					mTans.Clear();
					mTans.AddRanged(tans.buffer, 0, tans.size);
				}
				mUvs.Clear();
				mUvs.AddRanged(uvs.buffer, 0, uvs.size);
				mCols.Clear();
				mCols.AddRanged(cols.buffer, 0, cols.size);
				Mesh mesh = GetMesh(ref flag, verts.size);
				mesh.vertices = mVerts.ToArray();
				if (norms != null)
				{
					mesh.normals = mNorms.ToArray();
				}
				if (tans != null)
				{
					mesh.tangents = mTans.ToArray();
				}
				mesh.uv = mUvs.ToArray();
				mesh.colors32 = mCols.ToArray();
				if (flag)
				{
					mesh.triangles = mIndices;
				}
				mesh.RecalculateBounds();
				mFilter.mesh = mesh;
			}
			else
			{
				if (mFilter.mesh != null)
				{
					mFilter.mesh.Clear();
				}
				Debug.LogError("Too many vertices on one panel: " + verts.size);
			}
		}
		else
		{
			if (mFilter.mesh != null)
			{
				mFilter.mesh.Clear();
			}
			Debug.LogError("UIWidgets must fill the buffer with 4 vertices per quad. Found " + size);
		}
	}

	private void OnWillRenderObject()
	{
		if (mReset)
		{
			mReset = false;
			UpdateMaterials();
		}
		if (mClippedMat != null)
		{
			mClippedMat.mainTextureOffset = new Vector2(-mClipRange.x / mClipRange.z, -mClipRange.y / mClipRange.w);
			mClippedMat.mainTextureScale = new Vector2(1f / mClipRange.z, 1f / mClipRange.w);
			Vector2 v = new Vector2(1000f, 1000f);
			if (mClipSoft.x > 0f)
			{
				v.x = mClipRange.z / mClipSoft.x;
			}
			if (mClipSoft.y > 0f)
			{
				v.y = mClipRange.w / mClipSoft.y;
			}
			mClippedMat.SetVector("_ClipSharpness", v);
		}
	}

	private void OnDestroy()
	{
		NGUITools.DestroyImmediate(mMesh0);
		NGUITools.DestroyImmediate(mMesh1);
		NGUITools.DestroyImmediate(mClippedMat);
		NGUITools.DestroyImmediate(mDepthMat);
	}

	public enum Clipping
	{
		None,
		HardClip,
		AlphaClip,
		SoftClip
	}
}
