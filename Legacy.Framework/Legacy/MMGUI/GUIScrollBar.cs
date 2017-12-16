using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	[AddComponentMenu("MM Legacy/GUI Misc/GUIScrollBar")]
	[ExecuteInEditMode]
	public class GUIScrollBar : MonoBehaviour
	{
		[HideInInspector]
		[SerializeField]
		private Single mTotalSize = 400f;

		[HideInInspector]
		[SerializeField]
		private Single mArrowButtonOffset = 10f;

		[HideInInspector]
		[SerializeField]
		private UIButton mButtonDown;

		[HideInInspector]
		[SerializeField]
		private UIButton mButtonUp;

		[SerializeField]
		[HideInInspector]
		private UISprite mOverlay;

		[HideInInspector]
		[SerializeField]
		private UISprite mBG;

		[SerializeField]
		[HideInInspector]
		private UISprite mFG;

		[SerializeField]
		[HideInInspector]
		private Direction mDir;

		[HideInInspector]
		[SerializeField]
		private Boolean mInverted;

		[SerializeField]
		[HideInInspector]
		private Single mScroll;

		[SerializeField]
		[HideInInspector]
		private Single mSize = 1f;

		[SerializeField]
		[HideInInspector]
		private Single mButtonScrollValue = 0.1f;

		private Transform mTrans;

		private Boolean mIsDirty;

		private Camera mCam;

		private Vector2 mScreenPos = Vector2.zero;

		public OnScrollBarChange onChange;

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

		public Camera cachedCamera
		{
			get
			{
				if (mCam == null)
				{
					mCam = NGUITools.FindCameraForLayer(gameObject.layer);
				}
				return mCam;
			}
		}

		public UISprite background
		{
			get => mBG;
		    set
			{
				if (mBG != value)
				{
					mBG = value;
					mIsDirty = true;
				}
			}
		}

		public UISprite foreground
		{
			get => mFG;
		    set
			{
				if (mFG != value)
				{
					mFG = value;
					mIsDirty = true;
				}
			}
		}

		public UIButton buttonDown
		{
			get => mButtonDown;
		    set
			{
				if (mButtonDown != value)
				{
					mButtonDown = value;
					mIsDirty = true;
				}
			}
		}

		public UIButton buttonUp
		{
			get => mButtonUp;
		    set
			{
				if (mButtonUp != value)
				{
					mButtonUp = value;
					mIsDirty = true;
				}
			}
		}

		public UISprite overlay
		{
			get => mOverlay;
		    set
			{
				if (mOverlay != value)
				{
					mOverlay = value;
					mIsDirty = true;
				}
			}
		}

		public Single arrowButtonOffset
		{
			get => mArrowButtonOffset;
		    set
			{
				if (mArrowButtonOffset != value)
				{
					mArrowButtonOffset = value;
					mIsDirty = true;
				}
			}
		}

		public Single buttonScrollValue
		{
			get => mButtonScrollValue;
		    set => mButtonScrollValue = value;
		}

		public Direction direction
		{
			get => mDir;
		    set
			{
				if (mDir != value)
				{
					mDir = value;
					mIsDirty = true;
					if (mBG != null)
					{
						Transform cachedTransform = mBG.cachedTransform;
						Vector3 localScale = cachedTransform.localScale;
						if ((mDir == Direction.Vertical && localScale.x > localScale.y) || (mDir == Direction.Horizontal && localScale.x < localScale.y))
						{
							Single x = localScale.x;
							localScale.x = localScale.y;
							localScale.y = x;
							cachedTransform.localScale = localScale;
							ForceUpdate();
							if (mBG.collider != null)
							{
								NGUITools.AddWidgetCollider(mBG.gameObject);
							}
							if (mFG.collider != null)
							{
								NGUITools.AddWidgetCollider(mFG.gameObject);
							}
						}
					}
				}
			}
		}

		public Boolean inverted
		{
			get => mInverted;
		    set
			{
				if (mInverted != value)
				{
					mInverted = value;
					mIsDirty = true;
				}
			}
		}

		public Single scrollValue
		{
			get => mScroll;
		    set
			{
				Single num = Mathf.Clamp01(value);
				if (mScroll != num)
				{
					mScroll = num;
					mIsDirty = true;
					if (onChange != null)
					{
						onChange(this);
					}
				}
			}
		}

		public Single barSize
		{
			get => mSize;
		    set
			{
				Single num = Mathf.Clamp01(value);
				if (mSize != num)
				{
					mSize = num;
					mIsDirty = true;
					if (onChange != null)
					{
						onChange(this);
					}
				}
			}
		}

		public Single totalSize
		{
			get => mTotalSize;
		    set
			{
				if (mTotalSize != value)
				{
					mTotalSize = value;
					mIsDirty = true;
				}
			}
		}

		public Single alpha
		{
			get
			{
				if (mFG != null)
				{
					return mFG.alpha;
				}
				if (mBG != null)
				{
					return mBG.alpha;
				}
				return 0f;
			}
			set
			{
				if (mFG != null)
				{
					mFG.alpha = value;
					NGUITools.SetActiveSelf(mFG.gameObject, mFG.alpha > 0.001f);
				}
				if (mBG != null)
				{
					mBG.alpha = value;
					NGUITools.SetActiveSelf(mBG.gameObject, mBG.alpha > 0.001f);
				}
			}
		}

		private void CenterOnPos(Vector2 localPos)
		{
			if (mBG == null || mFG == null)
			{
				return;
			}
			Bounds bounds = NGUIMath.CalculateRelativeInnerBounds(cachedTransform, mBG);
			Bounds bounds2 = NGUIMath.CalculateRelativeInnerBounds(cachedTransform, mFG);
			if (mDir == Direction.Horizontal)
			{
				Single num = bounds.size.x - bounds2.size.x;
				Single num2 = num * 0.5f;
				Single num3 = bounds.center.x - num2;
				Single num4 = (num <= 0f) ? 0f : ((localPos.x - num3) / num);
				scrollValue = ((!mInverted) ? num4 : (1f - num4));
			}
			else
			{
				Single num5 = bounds.size.y - bounds2.size.y;
				Single num6 = num5 * 0.5f;
				Single num7 = bounds.center.y - num6;
				Single num8 = (num5 <= 0f) ? 0f : (1f - (localPos.y - num7) / num5);
				scrollValue = ((!mInverted) ? num8 : (1f - num8));
			}
		}

		private void Reposition(Vector2 screenPos)
		{
			Transform cachedTransform = this.cachedTransform;
			Plane plane = new Plane(cachedTransform.rotation * Vector3.back, cachedTransform.position);
			Ray ray = cachedCamera.ScreenPointToRay(screenPos);
			Single distance;
			if (!plane.Raycast(ray, out distance))
			{
				return;
			}
			CenterOnPos(cachedTransform.InverseTransformPoint(ray.GetPoint(distance)));
		}

		private void OnScrollUpButtonClicked()
		{
			Int32 num = (!inverted) ? -1 : 1;
			scrollValue += buttonScrollValue * num;
		}

		private void OnScrollDownButtonClicked()
		{
			Int32 num = (!inverted) ? 1 : -1;
			scrollValue += buttonScrollValue * num;
		}

		private void OnPressBackground(GameObject go, Boolean isPressed)
		{
			mCam = UICamera.currentCamera;
			Reposition(UICamera.lastTouchPosition);
		}

		private void OnDragBackground(GameObject go, Vector2 delta)
		{
			mCam = UICamera.currentCamera;
			Reposition(UICamera.lastTouchPosition);
		}

		private void OnPressForeground(GameObject go, Boolean isPressed)
		{
			if (isPressed)
			{
				mCam = UICamera.currentCamera;
				Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(mFG.cachedTransform);
				mScreenPos = mCam.WorldToScreenPoint(bounds.center);
			}
		}

		private void OnDragForeground(GameObject go, Vector2 delta)
		{
			mCam = UICamera.currentCamera;
			Reposition(mScreenPos + UICamera.currentTouch.totalDelta);
		}

		private void Start()
		{
			if (background != null && background.collider != null)
			{
				UIEventListener uieventListener = UIEventListener.Get(background.gameObject);
				UIEventListener uieventListener2 = uieventListener;
				uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(OnPressBackground));
				UIEventListener uieventListener3 = uieventListener;
				uieventListener3.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener3.onDrag, new UIEventListener.VectorDelegate(OnDragBackground));
			}
			if (foreground != null && foreground.collider != null)
			{
				UIEventListener uieventListener4 = UIEventListener.Get(foreground.gameObject);
				UIEventListener uieventListener5 = uieventListener4;
				uieventListener5.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener5.onPress, new UIEventListener.BoolDelegate(OnPressForeground));
				UIEventListener uieventListener6 = uieventListener4;
				uieventListener6.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener6.onDrag, new UIEventListener.VectorDelegate(OnDragForeground));
			}
			ForceUpdate();
		}

		private void Update()
		{
			if (mIsDirty)
			{
				ForceUpdate();
			}
		}

		public void ForceUpdate()
		{
			mIsDirty = false;
			if (mBG != null && mFG != null)
			{
				mSize = Mathf.Clamp01(mSize);
				mScroll = Mathf.Clamp01(mScroll);
				Vector4 border = mFG.border;
				Single x = mBG.cachedTransform.localScale.x;
				Single y = mBG.cachedTransform.localScale.y;
				if (mDir == Direction.Horizontal)
				{
					x = mTotalSize;
				}
				else
				{
					y = mTotalSize;
				}
				Vector2 vector = new Vector2(x, y);
				Single num = (!mInverted) ? mScroll : (1f - mScroll);
				mBG.cachedTransform.localScale = new Vector3(x, y, mBG.cachedTransform.localScale.z);
				if (mDir == Direction.Horizontal)
				{
					Vector2 vector2 = new Vector2(vector.x * mSize, vector.y);
					if (vector2.x < border.x + border.z)
					{
						vector2.x = border.x + border.z;
					}
					mFG.pivot = UIWidget.Pivot.Left;
					mBG.pivot = UIWidget.Pivot.Left;
					mBG.cachedTransform.localPosition = Vector3.zero;
					mFG.cachedTransform.localPosition = new Vector3((vector.x - vector2.x) * num, 0f, 0f);
					mFG.cachedTransform.localScale = new Vector3(vector2.x, vector2.y, 1f);
					if (num < 0.999f && num > 0.001f)
					{
						mFG.MakePixelPerfect();
					}
					Quaternion localRotation = Quaternion.AngleAxis(90f, Vector3.forward);
					mButtonUp.transform.localPosition = new Vector3(mArrowButtonOffset, 0f, 0f);
					mButtonUp.transform.localRotation = localRotation;
					mButtonDown.transform.localPosition = new Vector3(-mBG.cachedTransform.localScale.x - mArrowButtonOffset, 0f, 0f);
					mButtonDown.transform.localRotation = localRotation;
					mOverlay.cachedTransform.localRotation = localRotation;
					Single x2 = mFG.cachedTransform.localPosition.x + mFG.cachedTransform.localScale.x * 0.5f;
					Single y2 = mFG.cachedTransform.localPosition.y;
					Single z = mFG.cachedTransform.localPosition.z;
					mOverlay.cachedTransform.localPosition = new Vector3(x2, y2, z);
				}
				else
				{
					Vector2 vector3 = new Vector2(vector.x, vector.y * mSize);
					if (vector3.y < border.y + border.w)
					{
						vector3.y = border.y + border.w;
					}
					mFG.pivot = UIWidget.Pivot.Top;
					mBG.pivot = UIWidget.Pivot.Top;
					mBG.cachedTransform.localPosition = Vector3.zero;
					mFG.cachedTransform.localPosition = new Vector3(0f, -(vector.y - vector3.y) * num, 0f);
					mFG.cachedTransform.localScale = new Vector3(vector3.x, vector3.y, 1f);
					if (num < 0.999f && num > 0.001f)
					{
						mFG.MakePixelPerfect();
					}
					mButtonUp.transform.localPosition = new Vector3(0f, mArrowButtonOffset, 0f);
					mButtonUp.transform.localRotation = Quaternion.identity;
					mButtonDown.transform.localPosition = new Vector3(0f, -mBG.cachedTransform.localScale.y - mArrowButtonOffset, 0f);
					mButtonDown.transform.localRotation = Quaternion.identity;
					mOverlay.cachedTransform.localRotation = Quaternion.identity;
					Single x3 = mFG.cachedTransform.localPosition.x;
					Single y3 = mFG.cachedTransform.localPosition.y - mFG.cachedTransform.localScale.y * 0.5f;
					Single z2 = mFG.cachedTransform.localPosition.z;
					mOverlay.cachedTransform.localPosition = new Vector3(x3, y3, z2);
				}
			}
		}

		public enum Direction
		{
			Horizontal,
			Vertical
		}

		public delegate void OnScrollBarChange(GUIScrollBar sb);
	}
}
