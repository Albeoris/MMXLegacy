using System;
using UnityEngine;

public abstract class UIWidget : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private Int32 mLayer;

	[HideInInspector]
	[SerializeField]
	protected Material mMat;

	[SerializeField]
	[HideInInspector]
	protected Texture mTex;

	[SerializeField]
	[HideInInspector]
	private Color mColor = Color.white;

	[HideInInspector]
	[SerializeField]
	private Pivot mPivot = Pivot.Center;

	[SerializeField]
	[HideInInspector]
	private Int32 mDepth;

	protected GameObject mGo;

	protected Transform mTrans;

	protected UIPanel mPanel;

	protected Boolean mChanged = true;

	protected Boolean mPlayMode = true;

	private Matrix4x4 mLocalToPanel;

	private Boolean mVisibleByPanel = true;

	private Single mLastAlpha;

	private UIGeometry mGeom = new UIGeometry();

	private Boolean mForceVisible;

	private Vector3 mOldV0;

	private Vector3 mOldV1;

	public Int32 layer
	{
		get => mLayer;
	    set
		{
			if (mLayer != value)
			{
				if (mPanel != null)
				{
					mPanel.MarkMaterialAsChanged(material, mLayer, false);
				}
				mLayer = value;
				if (mPanel != null)
				{
					mPanel.MarkMaterialAsChanged(material, mLayer, true);
				}
			}
		}
	}

	public new Transform transform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
		private set => mTrans = value;
	}

	public new GameObject gameObject
	{
		get
		{
			if (mGo == null)
			{
				mGo = base.gameObject;
			}
			return mGo;
		}
		private set => mGo = value;
	}

	public Boolean isVisible => mVisibleByPanel;

    public Color color
	{
		get => mColor;
        set
		{
			if (!ColorEquals(ref mColor, ref value))
			{
				mColor = value;
				mChanged = true;
			}
		}
	}

	public Single alpha
	{
		get => mColor.a;
	    set
		{
			Color color = mColor;
			color.a = value;
			this.color = color;
		}
	}

	public Single finalAlpha
	{
		get
		{
			if (mPanel == null)
			{
				CreatePanel();
			}
			return (!(mPanel != null)) ? mColor.a : (mColor.a * mPanel.alpha);
		}
	}

	public Pivot pivot
	{
		get => mPivot;
	    set
		{
			if (mPivot != value)
			{
				Vector3 vector = NGUIMath.CalculateWidgetCorners(this)[0];
				mPivot = value;
				mChanged = true;
				Vector3 vector2 = NGUIMath.CalculateWidgetCorners(this)[0];
				Transform cachedTransform = this.cachedTransform;
				Vector3 vector3 = cachedTransform.position;
				Single z = cachedTransform.localPosition.z;
				vector3.x += vector.x - vector2.x;
				vector3.y += vector.y - vector2.y;
				this.cachedTransform.position = vector3;
				vector3 = this.cachedTransform.localPosition;
				vector3.x = Mathf.Round(vector3.x);
				vector3.y = Mathf.Round(vector3.y);
				vector3.z = z;
				this.cachedTransform.localPosition = vector3;
			}
		}
	}

	public Int32 depth
	{
		get => mDepth;
	    set
		{
			if (mDepth != value)
			{
				mDepth = value;
				if (mPanel != null)
				{
					mPanel.MarkMaterialAsChanged(material, mLayer, true);
				}
			}
		}
	}

	public Vector2 pivotOffset
	{
		get
		{
			Vector2 zero = Vector2.zero;
			Vector4 relativePadding = this.relativePadding;
			Pivot pivot = this.pivot;
			if (pivot == Pivot.Top || pivot == Pivot.Center || pivot == Pivot.Bottom)
			{
				zero.x = (relativePadding.x - relativePadding.z - 1f) * 0.5f;
			}
			else if (pivot == Pivot.TopRight || pivot == Pivot.Right || pivot == Pivot.BottomRight)
			{
				zero.x = -1f - relativePadding.z;
			}
			else
			{
				zero.x = relativePadding.x;
			}
			if (pivot == Pivot.Left || pivot == Pivot.Center || pivot == Pivot.Right)
			{
				zero.y = (relativePadding.w - relativePadding.y + 1f) * 0.5f;
			}
			else if (pivot == Pivot.BottomLeft || pivot == Pivot.Bottom || pivot == Pivot.BottomRight)
			{
				zero.y = 1f + relativePadding.w;
			}
			else
			{
				zero.y = -relativePadding.y;
			}
			return zero;
		}
	}

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

	public virtual Material material
	{
		get => mMat;
	    set
		{
			if (mMat != value)
			{
				if (mMat != null && mPanel != null)
				{
					mPanel.RemoveWidget(this);
				}
				mPanel = null;
				mMat = value;
				mTex = null;
				if (mMat != null)
				{
					CreatePanel();
				}
			}
		}
	}

	public virtual Texture mainTexture
	{
		get
		{
			Material material = this.material;
			if (material != null)
			{
				if (material.mainTexture != null)
				{
					mTex = material.mainTexture;
				}
				else if (mTex != null)
				{
					if (mPanel != null)
					{
						mPanel.RemoveWidget(this);
					}
					mPanel = null;
					mMat.mainTexture = mTex;
					if (enabled)
					{
						CreatePanel();
					}
				}
			}
			return mTex;
		}
		set
		{
			Material material = this.material;
			if (material == null || material.mainTexture != value)
			{
				if (mPanel != null)
				{
					mPanel.RemoveWidget(this);
				}
				mPanel = null;
				mTex = value;
				material = this.material;
				if (material != null)
				{
					material.mainTexture = value;
					if (enabled)
					{
						CreatePanel();
					}
				}
			}
		}
	}

	public UIPanel panel
	{
		get
		{
			if (mPanel == null)
			{
				CreatePanel();
			}
			return mPanel;
		}
		set => mPanel = value;
	}

	public static BetterList<UIWidget> Raycast(GameObject root, Vector2 mousePos)
	{
		BetterList<UIWidget> betterList = new BetterList<UIWidget>();
		UICamera uicamera = UICamera.FindCameraForLayer(root.layer);
		if (uicamera != null)
		{
			Camera cachedCamera = uicamera.cachedCamera;
			foreach (UIWidget uiwidget in root.GetComponentsInChildren<UIWidget>())
			{
				Vector3[] worldPoints = NGUIMath.CalculateWidgetCorners(uiwidget);
				if (NGUIMath.DistanceToRectangle(worldPoints, mousePos, cachedCamera) == 0f)
				{
					betterList.Add(uiwidget);
				}
			}
			betterList.Sort((UIWidget w1, UIWidget w2) => w2.mDepth.CompareTo(w1.mDepth));
		}
		return betterList;
	}

	public static Int32 CompareFunc(UIWidget left, UIWidget right)
	{
		if (left.mDepth > right.mDepth)
		{
			return 1;
		}
		if (left.mDepth < right.mDepth)
		{
			return -1;
		}
		return 0;
	}

	public void MarkAsChangedLite()
	{
		mChanged = true;
	}

	public virtual void MarkAsChanged()
	{
		mChanged = true;
		if (mPanel != null && enabled && NGUITools.GetActive(gameObject) && !Application.isPlaying && material != null)
		{
			mPanel.AddWidget(this);
			CheckLayer();
		}
	}

	public void CreatePanel()
	{
		if (mPanel == null && enabled && NGUITools.GetActive(gameObject) && material != null)
		{
			mPanel = UIPanel.Find(cachedTransform);
			if (mPanel != null)
			{
				CheckLayer();
				mPanel.AddWidget(this);
				mChanged = true;
			}
		}
	}

	public void CheckLayer()
	{
		if (mPanel != null && mPanel.gameObject.layer != gameObject.layer)
		{
			Debug.LogWarning("You can't place widgets on a layer different than the UIPanel that manages them.\nIf you want to move widgets to a different layer, parent them to a new panel instead.", this);
			gameObject.layer = mPanel.gameObject.layer;
		}
	}

	[Obsolete("Use ParentHasChanged() instead")]
	public void CheckParent()
	{
		ParentHasChanged();
	}

	public void ParentHasChanged()
	{
		if (mPanel != null)
		{
			UIPanel y = UIPanel.Find(cachedTransform);
			if (mPanel != y)
			{
				mPanel.RemoveWidget(this);
				if (!keepMaterial || Application.isPlaying)
				{
					material = null;
				}
				mPanel = null;
				CreatePanel();
			}
		}
	}

	protected virtual void Awake()
	{
		transform = base.transform;
		gameObject = base.gameObject;
		mGo = gameObject;
		mPlayMode = Application.isPlaying;
	}

	protected virtual void OnEnable()
	{
		mChanged = true;
		if (!keepMaterial)
		{
			mMat = null;
			mTex = null;
		}
		mPanel = null;
	}

	private void Start()
	{
		OnStart();
		CreatePanel();
	}

	public virtual void Update()
	{
		if (mPanel == null)
		{
			CreatePanel();
		}
	}

	private void OnDisable()
	{
		if (!keepMaterial)
		{
			material = null;
		}
		else if (mPanel != null)
		{
			mPanel.RemoveWidget(this);
		}
		mPanel = null;
	}

	private void OnDestroy()
	{
		if (mPanel != null)
		{
			mPanel.RemoveWidget(this);
			mPanel = null;
		}
	}

	public Boolean UpdateGeometry(UIPanel p, Boolean forceVisible)
	{
		if (material != null && p != null)
		{
			mPanel = p;
			Boolean flag = false;
			Single finalAlpha = this.finalAlpha;
			Boolean flag2 = true;
			Boolean flag3 = forceVisible || mVisibleByPanel;
			if (cachedTransform.hasChanged)
			{
				mTrans.hasChanged = false;
				if (!mPanel.widgetsAreStatic)
				{
					Vector2 relativeSize = this.relativeSize;
					Vector2 pivotOffset = this.pivotOffset;
					Vector4 relativePadding = this.relativePadding;
					Single num = pivotOffset.x * relativeSize.x - relativePadding.x;
					Single num2 = pivotOffset.y * relativeSize.y + relativePadding.y;
					Single x = num + relativeSize.x + relativePadding.x + relativePadding.z;
					Single y = num2 - relativeSize.y - relativePadding.y - relativePadding.w;
					mLocalToPanel = p.worldToLocal * cachedTransform.localToWorldMatrix;
					flag = true;
					Vector3 vector = new Vector3(num, num2, 0f);
					Vector3 vector2 = new Vector3(x, y, 0f);
					vector = mLocalToPanel.MultiplyPoint3x4(vector);
					vector2 = mLocalToPanel.MultiplyPoint3x4(vector2);
					if (Vector3.SqrMagnitude(mOldV0 - vector) > 1E-06f || Vector3.SqrMagnitude(mOldV1 - vector2) > 1E-06f)
					{
						mChanged = true;
						mOldV0 = vector;
						mOldV1 = vector2;
					}
				}
				if (flag2 || mForceVisible != forceVisible)
				{
					mForceVisible = forceVisible;
					flag3 = (forceVisible || mPanel.IsVisible(this));
				}
			}
			else if (flag2 && mForceVisible != forceVisible)
			{
				mForceVisible = forceVisible;
				flag3 = mPanel.IsVisible(this);
			}
			if (mVisibleByPanel != flag3)
			{
				mVisibleByPanel = flag3;
				mChanged = true;
			}
			if (mVisibleByPanel && mLastAlpha != finalAlpha)
			{
				mChanged = true;
			}
			mLastAlpha = finalAlpha;
			if (mChanged)
			{
				mChanged = false;
				if (isVisible)
				{
					mGeom.Clear();
					OnFill(mGeom.verts, mGeom.uvs, mGeom.cols);
					if (mGeom.hasVertices)
					{
						Vector3 pivotOffset2 = pivotOffset;
						Vector2 relativeSize2 = relativeSize;
						pivotOffset2.x *= relativeSize2.x;
						pivotOffset2.y *= relativeSize2.y;
						if (!flag)
						{
							mLocalToPanel = p.worldToLocal * cachedTransform.localToWorldMatrix;
						}
						mGeom.ApplyOffset(pivotOffset2);
						mGeom.ApplyTransform(mLocalToPanel, p.generateNormals);
					}
					return true;
				}
				if (mGeom.hasVertices)
				{
					mGeom.Clear();
					return true;
				}
			}
		}
		return false;
	}

	public void WriteToBuffers(BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
	{
		mGeom.WriteToBuffers(v, u, c, n, t);
	}

	public virtual void MakePixelPerfect()
	{
		Vector3 localScale = cachedTransform.localScale;
		Int32 num = Mathf.RoundToInt(localScale.x);
		Int32 num2 = Mathf.RoundToInt(localScale.y);
		localScale.x = num;
		localScale.y = num2;
		localScale.z = 1f;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.z = Mathf.RoundToInt(localPosition.z);
		if (num % 2 == 1 && (pivot == Pivot.Top || pivot == Pivot.Center || pivot == Pivot.Bottom))
		{
			localPosition.x = Mathf.Floor(localPosition.x) + 0.5f;
		}
		else
		{
			localPosition.x = Mathf.Round(localPosition.x);
		}
		if (num2 % 2 == 1 && (pivot == Pivot.Left || pivot == Pivot.Center || pivot == Pivot.Right))
		{
			localPosition.y = Mathf.Ceil(localPosition.y) - 0.5f;
		}
		else
		{
			localPosition.y = Mathf.Round(localPosition.y);
		}
		cachedTransform.localPosition = localPosition;
		cachedTransform.localScale = localScale;
	}

	public virtual Vector2 relativeSize => Vector2.one;

    public virtual Vector4 relativePadding => Vector4.zero;

    public virtual Vector4 border => Vector4.zero;

    public virtual Boolean keepMaterial => false;

    public virtual Boolean pixelPerfectAfterResize => false;

    protected virtual void OnStart()
	{
	}

	public virtual void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
	}

	private static Boolean ColorEquals(ref Color a, ref Color b)
	{
		return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
	}

	public enum Pivot
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}
}
