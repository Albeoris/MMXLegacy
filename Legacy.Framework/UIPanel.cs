using System;
using UnityEngine;
using Object = System.Object;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Panel")]
public class UIPanel : MonoBehaviour
{
	public OnChangeDelegate onChange;

	public Boolean showInPanelTool = true;

	public Boolean generateNormals;

	public Boolean depthPass;

	public Boolean widgetsAreStatic;

	public Boolean cullWhileDragging;

	internal Matrix4x4 worldToLocal = Matrix4x4.identity;

	[SerializeField]
	[HideInInspector]
	private Single mAlpha = 1f;

	[HideInInspector]
	[SerializeField]
	private DebugInfo mDebugInfo = DebugInfo.Gizmos;

	[SerializeField]
	[HideInInspector]
	private UIDrawCall.Clipping mClipping;

	[HideInInspector]
	[SerializeField]
	private Vector4 mClipRange = Vector4.zero;

	[SerializeField]
	[HideInInspector]
	private Vector2 mClipSoftness = new Vector2(40f, 40f);

	private BetterList<UIWidget> mWidgets = new BetterList<UIWidget>();

	private BetterList<MaterialLayer> mChanged = new BetterList<MaterialLayer>();

	private BetterList<UIDrawCall> mDrawCalls = new BetterList<UIDrawCall>();

	private BetterList<Vector3> mVerts = new BetterList<Vector3>();

	private BetterList<Vector3> mNorms = new BetterList<Vector3>();

	private BetterList<Vector4> mTans = new BetterList<Vector4>();

	private BetterList<Vector2> mUvs = new BetterList<Vector2>();

	private BetterList<Color32> mCols = new BetterList<Color32>();

	private GameObject mGo;

	private Transform mTrans;

	private Camera mCam;

	private Int32 mLayer = -1;

	private Boolean mDepthChanged;

	private Single mCullTime;

	private Single mUpdateTime;

	private Single mMatrixTime;

	private static Single[] mTemp = new Single[4];

	private Vector2 mMin = Vector2.zero;

	private Vector2 mMax = Vector2.zero;

	private UIPanel[] mChildPanels;

	public GameObject cachedGameObject
	{
		get
		{
			if (mGo == null)
			{
				mGo = gameObject;
			}
			return mGo;
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

	public Single alpha
	{
		get => mAlpha;
	    set
		{
			Single num = Mathf.Clamp01(value);
			if (mAlpha != num)
			{
				mAlpha = num;
				for (Int32 i = 0; i < mDrawCalls.size; i++)
				{
					UIDrawCall uidrawCall = mDrawCalls[i];
					MarkMaterialAsChanged(uidrawCall.material, uidrawCall.layer, false);
				}
				for (Int32 j = 0; j < mWidgets.size; j++)
				{
					mWidgets[j].MarkAsChangedLite();
				}
			}
		}
	}

	public void SetAlphaRecursive(Single val, Boolean rebuildList)
	{
		if (rebuildList || mChildPanels == null)
		{
			mChildPanels = GetComponentsInChildren<UIPanel>(true);
		}
		Int32 i = 0;
		Int32 num = mChildPanels.Length;
		while (i < num)
		{
			mChildPanels[i].alpha = val;
			i++;
		}
	}

	public DebugInfo debugInfo
	{
		get => mDebugInfo;
	    set
		{
			if (mDebugInfo != value)
			{
				mDebugInfo = value;
				BetterList<UIDrawCall> drawCalls = this.drawCalls;
				HideFlags hideFlags = (mDebugInfo != DebugInfo.Geometry) ? HideFlags.HideAndDontSave : (HideFlags.DontSave | HideFlags.NotEditable);
				Int32 i = 0;
				Int32 size = drawCalls.size;
				while (i < size)
				{
					UIDrawCall uidrawCall = drawCalls[i];
					GameObject gameObject = uidrawCall.gameObject;
					NGUITools.SetActiveSelf(gameObject, false);
					gameObject.hideFlags = hideFlags;
					NGUITools.SetActiveSelf(gameObject, true);
					i++;
				}
			}
		}
	}

	public UIDrawCall.Clipping clipping
	{
		get => mClipping;
	    set
		{
			if (mClipping != value)
			{
				mClipping = value;
				mMatrixTime = 0f;
				UpdateDrawcalls();
			}
		}
	}

	public Vector4 clipRange
	{
		get => mClipRange;
	    set
		{
			if (mClipRange != value)
			{
				mCullTime = ((mCullTime != 0f) ? (Time.realtimeSinceStartup + 0.15f) : 0.001f);
				mClipRange = value;
				mMatrixTime = 0f;
				UpdateDrawcalls();
			}
		}
	}

	public Vector2 clipSoftness
	{
		get => mClipSoftness;
	    set
		{
			if (mClipSoftness != value)
			{
				mClipSoftness = value;
				UpdateDrawcalls();
			}
		}
	}

	public BetterList<UIWidget> widgets => mWidgets;

    public BetterList<UIDrawCall> drawCalls
	{
		get
		{
			Int32 i = mDrawCalls.size;
			while (i > 0)
			{
				UIDrawCall x = mDrawCalls[--i];
				if (x == null)
				{
					mDrawCalls.RemoveAt(i);
				}
			}
			return mDrawCalls;
		}
	}

	private Boolean IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		UpdateTransformMatrix();
		a = worldToLocal.MultiplyPoint3x4(a);
		b = worldToLocal.MultiplyPoint3x4(b);
		c = worldToLocal.MultiplyPoint3x4(c);
		d = worldToLocal.MultiplyPoint3x4(d);
		mTemp[0] = a.x;
		mTemp[1] = b.x;
		mTemp[2] = c.x;
		mTemp[3] = d.x;
		Single num = Mathf.Min(mTemp);
		Single num2 = Mathf.Max(mTemp);
		mTemp[0] = a.y;
		mTemp[1] = b.y;
		mTemp[2] = c.y;
		mTemp[3] = d.y;
		Single num3 = Mathf.Min(mTemp);
		Single num4 = Mathf.Max(mTemp);
		return num2 >= mMin.x && num4 >= mMin.y && num <= mMax.x && num3 <= mMax.y;
	}

	public Boolean IsVisible(Vector3 worldPos)
	{
		if (mAlpha < 0.001f)
		{
			return false;
		}
		if (mClipping == UIDrawCall.Clipping.None)
		{
			return true;
		}
		UpdateTransformMatrix();
		Vector3 vector = worldToLocal.MultiplyPoint3x4(worldPos);
		return vector.x >= mMin.x && vector.y >= mMin.y && vector.x <= mMax.x && vector.y <= mMax.y;
	}

	public Boolean IsVisible(UIWidget w)
	{
		if (mAlpha < 0.001f)
		{
			return false;
		}
		if (!w.enabled || !NGUITools.GetActive(w.cachedGameObject))
		{
			return false;
		}
		if (mClipping == UIDrawCall.Clipping.None)
		{
			return true;
		}
		Vector2 relativeSize = w.relativeSize;
		Vector2 vector = Vector2.Scale(w.pivotOffset, relativeSize);
		Vector2 v = vector;
		vector.x += relativeSize.x;
		vector.y -= relativeSize.y;
		Transform cachedTransform = w.cachedTransform;
		Vector3 a = cachedTransform.TransformPoint(vector);
		Vector3 b = cachedTransform.TransformPoint(new Vector2(vector.x, v.y));
		Vector3 c = cachedTransform.TransformPoint(new Vector2(v.x, vector.y));
		Vector3 d = cachedTransform.TransformPoint(v);
		return IsVisible(a, b, c, d);
	}

	public void MarkMaterialAsChanged(Material mat, Int32 layer, Boolean sort)
	{
		if (mat != null)
		{
			if (sort)
			{
				mDepthChanged = true;
			}
			MaterialLayer item = new MaterialLayer(mat, layer);
			if (!mChanged.Contains(item))
			{
				mChanged.Add(item);
			}
		}
	}

	public void AddWidget(UIWidget w)
	{
		if (w != null && !mWidgets.Contains(w))
		{
			mWidgets.Add(w);
			MaterialLayer item = new MaterialLayer(w.material, w.layer);
			if (!mChanged.Contains(item))
			{
				mChanged.Add(item);
			}
			mDepthChanged = true;
		}
	}

	public void RemoveWidget(UIWidget w)
	{
		if (w != null && w != null && mWidgets.Remove(w) && w.material != null)
		{
			MaterialLayer item = new MaterialLayer(w.material, w.layer);
			mChanged.Add(item);
		}
	}

	private UIDrawCall GetDrawCall(Material mat, Int32 layer, Boolean createIfMissing)
	{
		Int32 i = 0;
		Int32 size = drawCalls.size;
		while (i < size)
		{
			UIDrawCall uidrawCall = drawCalls.buffer[i];
			if (uidrawCall.layer == layer && uidrawCall.material == mat)
			{
				return uidrawCall;
			}
			i++;
		}
		UIDrawCall uidrawCall2 = null;
		if (createIfMissing)
		{
			GameObject gameObject = new GameObject(String.Concat(new Object[]
			{
				"_UIDrawCall [",
				layer,
				"][",
				mat.name,
				"]"
			}));
			DontDestroyOnLoad(gameObject);
			gameObject.layer = cachedGameObject.layer;
			uidrawCall2 = gameObject.AddComponent<UIDrawCall>();
			uidrawCall2.material = mat;
			uidrawCall2.layer = layer;
			mDrawCalls.Add(uidrawCall2);
		}
		return uidrawCall2;
	}

	private void Awake()
	{
		mGo = gameObject;
		mTrans = transform;
	}

	private void Start()
	{
		mLayer = mGo.layer;
		UICamera uicamera = UICamera.FindCameraForLayer(mLayer);
		mCam = ((!(uicamera != null)) ? NGUITools.FindCameraForLayer(mLayer) : uicamera.cachedCamera);
	}

	private void OnEnable()
	{
		Int32 i = 0;
		while (i < mWidgets.size)
		{
			UIWidget uiwidget = mWidgets.buffer[i];
			if (uiwidget != null)
			{
				MarkMaterialAsChanged(uiwidget.material, uiwidget.layer, true);
				i++;
			}
			else
			{
				mWidgets.RemoveAt(i);
			}
		}
	}

	private void OnDisable()
	{
		Int32 i = mDrawCalls.size;
		while (i > 0)
		{
			UIDrawCall uidrawCall = mDrawCalls.buffer[--i];
			if (uidrawCall != null)
			{
				NGUITools.DestroyImmediate(uidrawCall.gameObject);
			}
		}
		mDrawCalls.Clear();
		mChanged.Clear();
	}

	private void UpdateTransformMatrix()
	{
		if (mUpdateTime == 0f || mMatrixTime != mUpdateTime)
		{
			mMatrixTime = mUpdateTime;
			worldToLocal = cachedTransform.worldToLocalMatrix;
			if (mClipping != UIDrawCall.Clipping.None)
			{
				Vector2 a = new Vector2(mClipRange.z, mClipRange.w);
				if (a.x == 0f)
				{
					a.x = ((!(mCam == null)) ? mCam.pixelWidth : Screen.width);
				}
				if (a.y == 0f)
				{
					a.y = ((!(mCam == null)) ? mCam.pixelHeight : Screen.height);
				}
				a *= 0.5f;
				mMin.x = mClipRange.x - a.x;
				mMin.y = mClipRange.y - a.y;
				mMax.x = mClipRange.x + a.x;
				mMax.y = mClipRange.y + a.y;
			}
		}
	}

	public void UpdateDrawcalls()
	{
		Vector4 zero = Vector4.zero;
		if (mClipping != UIDrawCall.Clipping.None)
		{
			zero = new Vector4(mClipRange.x, mClipRange.y, mClipRange.z * 0.5f, mClipRange.w * 0.5f);
		}
		if (zero.z == 0f)
		{
			zero.z = Screen.width * 0.5f;
		}
		if (zero.w == 0f)
		{
			zero.w = Screen.height * 0.5f;
		}
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsWebPlayer || platform == RuntimePlatform.WindowsEditor)
		{
			zero.x -= 0.5f;
			zero.y += 0.5f;
		}
		Transform cachedTransform = this.cachedTransform;
		Int32 i = 0;
		Int32 size = mDrawCalls.size;
		while (i < size)
		{
			UIDrawCall uidrawCall = mDrawCalls.buffer[i];
			uidrawCall.clipping = mClipping;
			uidrawCall.clipRange = zero;
			uidrawCall.clipSoftness = mClipSoftness;
			uidrawCall.depthPass = (depthPass && mClipping == UIDrawCall.Clipping.None);
			Transform transform = uidrawCall.transform;
			transform.position = cachedTransform.position;
			transform.rotation = cachedTransform.rotation;
			transform.localScale = cachedTransform.lossyScale;
			i++;
		}
	}

	private void Fill(Material mat, Int32 layer)
	{
		Int32 i = 0;
		while (i < mWidgets.size)
		{
			UIWidget uiwidget = mWidgets.buffer[i];
			if (uiwidget == null)
			{
				mWidgets.RemoveAt(i);
			}
			else
			{
				if (uiwidget.material == mat && uiwidget.isVisible && uiwidget.layer == layer)
				{
					if (!(uiwidget.panel == this))
					{
						mWidgets.RemoveAt(i);
						continue;
					}
					if (generateNormals)
					{
						uiwidget.WriteToBuffers(mVerts, mUvs, mCols, mNorms, mTans);
					}
					else
					{
						uiwidget.WriteToBuffers(mVerts, mUvs, mCols, null, null);
					}
				}
				i++;
			}
		}
		if (mVerts.size > 0)
		{
			UIDrawCall drawCall = GetDrawCall(mat, layer, true);
			drawCall.depthPass = (depthPass && mClipping == UIDrawCall.Clipping.None);
			drawCall.Set(mVerts, (!generateNormals) ? null : mNorms, (!generateNormals) ? null : mTans, mUvs, mCols);
		}
		else
		{
			UIDrawCall drawCall2 = GetDrawCall(mat, layer, false);
			if (drawCall2 != null)
			{
				mDrawCalls.Remove(drawCall2);
				NGUITools.DestroyImmediate(drawCall2.gameObject);
			}
		}
		mVerts.Clear();
		mNorms.Clear();
		mTans.Clear();
		mUvs.Clear();
		mCols.Clear();
	}

	private void LateUpdate()
	{
		mUpdateTime = Time.realtimeSinceStartup;
		UpdateTransformMatrix();
		if (mLayer != cachedGameObject.layer)
		{
			mLayer = mGo.layer;
			UICamera uicamera = UICamera.FindCameraForLayer(mLayer);
			mCam = ((!(uicamera != null)) ? NGUITools.FindCameraForLayer(mLayer) : uicamera.cachedCamera);
			SetChildLayer(cachedTransform, mLayer);
			Int32 i = 0;
			Int32 size = drawCalls.size;
			while (i < size)
			{
				mDrawCalls.buffer[i].gameObject.layer = mLayer;
				i++;
			}
		}
		Boolean forceVisible = !cullWhileDragging && (clipping == UIDrawCall.Clipping.None || mCullTime > mUpdateTime);
		Int32 j = 0;
		Int32 size2 = mWidgets.size;
		while (j < size2)
		{
			UIWidget uiwidget = mWidgets[j];
			if (uiwidget.UpdateGeometry(this, forceVisible))
			{
				MaterialLayer item = new MaterialLayer(uiwidget.material, uiwidget.layer);
				if (!mChanged.Contains(item))
				{
					mChanged.Add(item);
				}
			}
			j++;
		}
		if (mChanged.size != 0 && onChange != null)
		{
			onChange();
		}
		if (mDepthChanged)
		{
			mDepthChanged = false;
			mWidgets.Sort(new Comparison<UIWidget>(UIWidget.CompareFunc));
		}
		Int32 k = 0;
		Int32 size3 = mChanged.size;
		while (k < size3)
		{
			Fill(mChanged.buffer[k].Material, mChanged.buffer[k].Layer);
			k++;
		}
		UpdateDrawcalls();
		mChanged.Clear();
	}

	public void Refresh()
	{
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
		Int32 i = 0;
		Int32 num = componentsInChildren.Length;
		while (i < num)
		{
			componentsInChildren[i].Update();
			i++;
		}
		LateUpdate();
	}

	public Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
	{
		Single num = clipRange.z * 0.5f;
		Single num2 = clipRange.w * 0.5f;
		Vector2 minRect = new Vector2(min.x, min.y);
		Vector2 maxRect = new Vector2(max.x, max.y);
		Vector2 minArea = new Vector2(clipRange.x - num, clipRange.y - num2);
		Vector2 maxArea = new Vector2(clipRange.x + num, clipRange.y + num2);
		if (clipping == UIDrawCall.Clipping.SoftClip)
		{
			minArea.x += clipSoftness.x;
			minArea.y += clipSoftness.y;
			maxArea.x -= clipSoftness.x;
			maxArea.y -= clipSoftness.y;
		}
		return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
	}

	public Boolean ConstrainTargetToBounds(Transform target, ref Bounds targetBounds, Boolean immediate)
	{
		Vector3 b = CalculateConstrainOffset(targetBounds.min, targetBounds.max);
		if (b.magnitude > 0f)
		{
			if (immediate)
			{
				target.localPosition += b;
				targetBounds.center += b;
				SpringPosition component = target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else
			{
				SpringPosition springPosition = SpringPosition.Begin(target.gameObject, target.localPosition + b, 13f);
				springPosition.ignoreTimeScale = true;
				springPosition.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	public Boolean ConstrainTargetToBounds(Transform target, Boolean immediate)
	{
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(cachedTransform, target);
		return ConstrainTargetToBounds(target, ref bounds, immediate);
	}

	private static void SetChildLayer(Transform t, Int32 layer)
	{
		for (Int32 i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			if (child.GetComponent<UIPanel>() == null)
			{
				if (child.GetComponent<UIWidget>() != null)
				{
					child.gameObject.layer = layer;
				}
				SetChildLayer(child, layer);
			}
		}
	}

	public static UIPanel Find(Transform trans, Boolean createIfMissing)
	{
		Transform y = trans;
		UIPanel uipanel = null;
		while (uipanel == null && trans != null)
		{
			uipanel = trans.GetComponent<UIPanel>();
			if (uipanel != null)
			{
				break;
			}
			if (trans.parent == null)
			{
				break;
			}
			trans = trans.parent;
		}
		if (createIfMissing && uipanel == null && trans != y)
		{
			uipanel = trans.gameObject.AddComponent<UIPanel>();
			SetChildLayer(uipanel.cachedTransform, uipanel.cachedGameObject.layer);
		}
		return uipanel;
	}

	public static UIPanel Find(Transform trans)
	{
		return Find(trans, true);
	}

	public enum DebugInfo
	{
		None,
		Gizmos,
		Geometry
	}

	public delegate void OnChangeDelegate();

	private struct MaterialLayer : IEquatable<MaterialLayer>
	{
		public Material Material;

		public Int32 Layer;

		public MaterialLayer(Material material, Int32 layer)
		{
			Material = material;
			Layer = layer;
		}

		public Boolean Equals(MaterialLayer other)
		{
			return Layer == other.Layer && Material == other.Material;
		}
	}
}
