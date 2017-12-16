using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Interaction/Draggable Panel")]
public class UIDraggablePanel : IgnoreTimeScale
{
	public Boolean restrictWithinPanel = true;

	public Boolean disableDragIfFits;

	public DragEffect dragEffect = DragEffect.MomentumAndSpring;

	public Boolean smoothDragStart = true;

	public Vector3 scale = Vector3.one;

	public Single scrollWheelFactor;

	public Single momentumAmount = 35f;

	public Vector2 relativePositionOnReset = Vector2.zero;

	public Boolean repositionClipping;

	public Boolean iOSDragEmulation = true;

	public UIScrollBar horizontalScrollBar;

	public UIScrollBar verticalScrollBar;

	public ShowCondition showScrollBars = ShowCondition.OnlyIfNeeded;

	public OnDragFinished onDragFinished;

	private Transform mTrans;

	private UIPanel mPanel;

	private Plane mPlane;

	private Vector3 mLastPos;

	private Boolean mPressed;

	private Vector3 mMomentum = Vector3.zero;

	private Single mScroll;

	private Bounds mBounds;

	private Boolean mCalculatedBounds;

	private Boolean mShouldMove;

	private Boolean mIgnoreCallbacks;

	private Int32 mDragID = -10;

	private Vector2 mDragStartOffset = Vector2.zero;

	private Boolean mDragStarted;

	public UIPanel panel => mPanel;

    public Bounds bounds
	{
		get
		{
			if (!mCalculatedBounds)
			{
				mCalculatedBounds = true;
				mBounds = NGUIMath.CalculateRelativeWidgetBounds(mTrans, mTrans);
			}
			return mBounds;
		}
	}

	public Boolean shouldMoveHorizontally
	{
		get
		{
			Single num = bounds.size.x;
			if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += mPanel.clipSoftness.x * 2f;
			}
			return num > mPanel.clipRange.z;
		}
	}

	public Boolean shouldMoveVertically
	{
		get
		{
			Single num = bounds.size.y;
			if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += mPanel.clipSoftness.y * 2f;
			}
			return num > mPanel.clipRange.w;
		}
	}

	private Boolean shouldMove
	{
		get
		{
			if (!disableDragIfFits)
			{
				return true;
			}
			if (mPanel == null)
			{
				mPanel = GetComponent<UIPanel>();
			}
			Vector4 clipRange = mPanel.clipRange;
			Bounds bounds = this.bounds;
			Single num = (clipRange.z != 0f) ? (clipRange.z * 0.5f) : Screen.width;
			Single num2 = (clipRange.w != 0f) ? (clipRange.w * 0.5f) : Screen.height;
			if (!Mathf.Approximately(scale.x, 0f))
			{
				if (bounds.min.x < clipRange.x - num)
				{
					return true;
				}
				if (bounds.max.x > clipRange.x + num)
				{
					return true;
				}
			}
			if (!Mathf.Approximately(scale.y, 0f))
			{
				if (bounds.min.y < clipRange.y - num2)
				{
					return true;
				}
				if (bounds.max.y > clipRange.y + num2)
				{
					return true;
				}
			}
			return false;
		}
	}

	public Vector3 currentMomentum
	{
		get => mMomentum;
	    set
		{
			mMomentum = value;
			mShouldMove = true;
		}
	}

	private void Awake()
	{
		mTrans = transform;
		mPanel = GetComponent<UIPanel>();
		UIPanel uipanel = mPanel;
		uipanel.onChange = (UIPanel.OnChangeDelegate)Delegate.Combine(uipanel.onChange, new UIPanel.OnChangeDelegate(OnPanelChange));
	}

	private void OnDestroy()
	{
		if (mPanel != null)
		{
			UIPanel uipanel = mPanel;
			uipanel.onChange = (UIPanel.OnChangeDelegate)Delegate.Remove(uipanel.onChange, new UIPanel.OnChangeDelegate(OnPanelChange));
		}
	}

	private void OnPanelChange()
	{
		UpdateScrollbars(true);
	}

	private void Start()
	{
		UpdateScrollbars(true);
		if (horizontalScrollBar != null)
		{
			UIScrollBar uiscrollBar = horizontalScrollBar;
			uiscrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(uiscrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnHorizontalBar));
			NGUITools.SetActiveSelf(horizontalScrollBar.gameObject, showScrollBars == ShowCondition.Always || shouldMoveHorizontally);
		}
		if (verticalScrollBar != null)
		{
			UIScrollBar uiscrollBar2 = verticalScrollBar;
			uiscrollBar2.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(uiscrollBar2.onChange, new UIScrollBar.OnScrollBarChange(OnVerticalBar));
			NGUITools.SetActiveSelf(verticalScrollBar.gameObject, showScrollBars == ShowCondition.Always || shouldMoveVertically);
		}
	}

	public Boolean RestrictWithinBounds(Boolean instant)
	{
		Vector3 vector = mPanel.CalculateConstrainOffset(bounds.min, bounds.max);
		if (vector.magnitude > 0.001f)
		{
			if (!instant && dragEffect == DragEffect.MomentumAndSpring)
			{
				SpringPanel.Begin(mPanel.gameObject, mTrans.localPosition + vector, 13f);
			}
			else
			{
				MoveRelative(vector);
				mMomentum = Vector3.zero;
				mScroll = 0f;
			}
			return true;
		}
		return false;
	}

	public void DisableSpring()
	{
		SpringPanel component = GetComponent<SpringPanel>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	public void UpdateScrollbars(Boolean recalculateBounds)
	{
		if (mPanel == null)
		{
			return;
		}
		if (horizontalScrollBar != null || verticalScrollBar != null)
		{
			if (recalculateBounds)
			{
				mCalculatedBounds = false;
				mShouldMove = shouldMove;
			}
			Bounds bounds = this.bounds;
			Vector2 a = bounds.min;
			Vector2 a2 = bounds.max;
			if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				Vector2 clipSoftness = mPanel.clipSoftness;
				a -= clipSoftness;
				a2 += clipSoftness;
			}
			if (horizontalScrollBar != null && a2.x > a.x)
			{
				Vector4 clipRange = mPanel.clipRange;
				Single num = clipRange.z * 0.5f;
				Single num2 = clipRange.x - num - bounds.min.x;
				Single num3 = bounds.max.x - num - clipRange.x;
				Single num4 = a2.x - a.x;
				num2 = Mathf.Clamp01(num2 / num4);
				num3 = Mathf.Clamp01(num3 / num4);
				Single num5 = num2 + num3;
				mIgnoreCallbacks = true;
				horizontalScrollBar.barSize = 1f - num5;
				horizontalScrollBar.scrollValue = ((num5 <= 0.001f) ? 0f : (num2 / num5));
				mIgnoreCallbacks = false;
			}
			if (verticalScrollBar != null && a2.y > a.y)
			{
				Vector4 clipRange2 = mPanel.clipRange;
				Single num6 = clipRange2.w * 0.5f;
				Single num7 = clipRange2.y - num6 - a.y;
				Single num8 = a2.y - num6 - clipRange2.y;
				Single num9 = a2.y - a.y;
				num7 = Mathf.Clamp01(num7 / num9);
				num8 = Mathf.Clamp01(num8 / num9);
				Single num10 = num7 + num8;
				mIgnoreCallbacks = true;
				verticalScrollBar.barSize = 1f - num10;
				verticalScrollBar.scrollValue = ((num10 <= 0.001f) ? 0f : (1f - num7 / num10));
				mIgnoreCallbacks = false;
			}
		}
		else if (recalculateBounds)
		{
			mCalculatedBounds = false;
		}
	}

	public void SetDragAmount(Single x, Single y, Boolean updateScrollbars)
	{
		DisableSpring();
		Bounds bounds = this.bounds;
		if (bounds.min.x == bounds.max.x || bounds.min.y == bounds.max.y)
		{
			return;
		}
		Vector4 clipRange = mPanel.clipRange;
		Single num = clipRange.z * 0.5f;
		Single num2 = clipRange.w * 0.5f;
		Single num3 = bounds.min.x + num;
		Single num4 = bounds.max.x - num;
		Single num5 = bounds.min.y + num2;
		Single num6 = bounds.max.y - num2;
		if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
		{
			num3 -= mPanel.clipSoftness.x;
			num4 += mPanel.clipSoftness.x;
			num5 -= mPanel.clipSoftness.y;
			num6 += mPanel.clipSoftness.y;
		}
		Single num7 = Mathf.Lerp(num3, num4, x);
		Single num8 = Mathf.Lerp(num6, num5, y);
		if (!updateScrollbars)
		{
			Vector3 localPosition = mTrans.localPosition;
			if (scale.x != 0f)
			{
				localPosition.x += clipRange.x - num7;
			}
			if (scale.y != 0f)
			{
				localPosition.y += clipRange.y - num8;
			}
			mTrans.localPosition = localPosition;
		}
		clipRange.x = num7;
		clipRange.y = num8;
		mPanel.clipRange = clipRange;
		if (updateScrollbars)
		{
			UpdateScrollbars(false);
		}
	}

	public void ResetPosition()
	{
		mCalculatedBounds = false;
		SetDragAmount(relativePositionOnReset.x, relativePositionOnReset.y, false);
		SetDragAmount(relativePositionOnReset.x, relativePositionOnReset.y, true);
	}

	private void OnHorizontalBar(UIScrollBar sb)
	{
		if (!mIgnoreCallbacks)
		{
			Single x = (!(horizontalScrollBar != null)) ? 0f : horizontalScrollBar.scrollValue;
			Single y = (!(verticalScrollBar != null)) ? 0f : verticalScrollBar.scrollValue;
			SetDragAmount(x, y, false);
		}
	}

	private void OnVerticalBar(UIScrollBar sb)
	{
		if (!mIgnoreCallbacks)
		{
			Single x = (!(horizontalScrollBar != null)) ? 0f : horizontalScrollBar.scrollValue;
			Single y = (!(verticalScrollBar != null)) ? 0f : verticalScrollBar.scrollValue;
			SetDragAmount(x, y, false);
		}
	}

	public void MoveRelative(Vector3 relative)
	{
		mTrans.localPosition += relative;
		Vector4 clipRange = mPanel.clipRange;
		clipRange.x -= relative.x;
		clipRange.y -= relative.y;
		mPanel.clipRange = clipRange;
		UpdateScrollbars(false);
	}

	public void MoveAbsolute(Vector3 absolute)
	{
		Vector3 a = mTrans.InverseTransformPoint(absolute);
		Vector3 b = mTrans.InverseTransformPoint(Vector3.zero);
		MoveRelative(a - b);
	}

	public void Press(Boolean pressed)
	{
		if (smoothDragStart && pressed)
		{
			mDragStarted = false;
			mDragStartOffset = Vector2.zero;
		}
		if (enabled && NGUITools.GetActive(gameObject))
		{
			if (!pressed && mDragID == UICamera.currentTouchID)
			{
				mDragID = -10;
			}
			mCalculatedBounds = false;
			mShouldMove = shouldMove;
			if (!mShouldMove)
			{
				return;
			}
			mPressed = pressed;
			if (pressed)
			{
				mMomentum = Vector3.zero;
				mScroll = 0f;
				DisableSpring();
				mLastPos = UICamera.lastHit.point;
				mPlane = new Plane(mTrans.rotation * Vector3.back, mLastPos);
			}
			else
			{
				if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None && dragEffect == DragEffect.MomentumAndSpring)
				{
					RestrictWithinBounds(false);
				}
				if (onDragFinished != null)
				{
					onDragFinished();
				}
			}
		}
	}

	public void Drag()
	{
		if (enabled && NGUITools.GetActive(gameObject) && mShouldMove)
		{
			if (mDragID == -10)
			{
				mDragID = UICamera.currentTouchID;
			}
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			if (smoothDragStart && !mDragStarted)
			{
				mDragStarted = true;
				mDragStartOffset = UICamera.currentTouch.totalDelta;
			}
			Ray ray = (!smoothDragStart) ? UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos) : UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos - mDragStartOffset);
			Single distance = 0f;
			if (mPlane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				Vector3 vector = point - mLastPos;
				mLastPos = point;
				if (vector.x != 0f || vector.y != 0f)
				{
					vector = mTrans.InverseTransformDirection(vector);
					vector.Scale(scale);
					vector = mTrans.TransformDirection(vector);
				}
				mMomentum = Vector3.Lerp(mMomentum, mMomentum + vector * (0.01f * momentumAmount), 0.67f);
				if (!iOSDragEmulation)
				{
					MoveAbsolute(vector);
				}
				else if (mPanel.CalculateConstrainOffset(bounds.min, bounds.max).magnitude > 0.001f)
				{
					MoveAbsolute(vector * 0.5f);
					mMomentum *= 0.5f;
				}
				else
				{
					MoveAbsolute(vector);
				}
				if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None && dragEffect != DragEffect.MomentumAndSpring)
				{
					RestrictWithinBounds(true);
				}
			}
		}
	}

	public void Scroll(Single delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && scrollWheelFactor != 0f)
		{
			DisableSpring();
			mShouldMove = shouldMove;
			if (Mathf.Sign(mScroll) != Mathf.Sign(delta))
			{
				mScroll = 0f;
			}
			mScroll += delta * scrollWheelFactor;
		}
	}

	private void LateUpdate()
	{
		if (repositionClipping)
		{
			repositionClipping = false;
			mCalculatedBounds = false;
			SetDragAmount(relativePositionOnReset.x, relativePositionOnReset.y, true);
		}
		if (!Application.isPlaying)
		{
			return;
		}
		Single deltaTime = UpdateRealTimeDelta();
		if (showScrollBars != ShowCondition.Always)
		{
			Boolean state = false;
			Boolean state2 = false;
			if (showScrollBars != ShowCondition.WhenDragging || mDragID != -10 || mMomentum.magnitude > 0.01f)
			{
				state = shouldMoveVertically;
				state2 = shouldMoveHorizontally;
			}
			if (verticalScrollBar)
			{
				NGUITools.SetActiveSelf(verticalScrollBar.gameObject, state);
			}
			if (horizontalScrollBar)
			{
				NGUITools.SetActiveSelf(horizontalScrollBar.gameObject, state2);
			}
		}
		if (mShouldMove && !mPressed)
		{
			mMomentum -= scale * (mScroll * 0.05f);
			if (mMomentum.magnitude > 0.0001f)
			{
				mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, deltaTime);
				Vector3 absolute = NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
				MoveAbsolute(absolute);
				if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None)
				{
					RestrictWithinBounds(false);
				}
				if (mMomentum.magnitude < 0.0001f && onDragFinished != null)
				{
					onDragFinished();
				}
				return;
			}
			mScroll = 0f;
			mMomentum = Vector3.zero;
		}
		else
		{
			mScroll = 0f;
		}
		NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
	}

	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring
	}

	public enum ShowCondition
	{
		Always,
		OnlyIfNeeded,
		WhenDragging
	}

	public delegate void OnDragFinished();
}
