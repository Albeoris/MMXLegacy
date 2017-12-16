using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Draggable Camera")]
[RequireComponent(typeof(Camera))]
public class UIDraggableCamera : IgnoreTimeScale
{
	public Transform rootForBounds;

	public Vector2 scale = Vector2.one;

	public Single scrollWheelFactor;

	public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;

	public Boolean smoothDragStart = true;

	public Single momentumAmount = 35f;

	private Camera mCam;

	private Transform mTrans;

	private Boolean mPressed;

	private Vector2 mMomentum = Vector2.zero;

	private Bounds mBounds;

	private Single mScroll;

	private UIRoot mRoot;

	private Boolean mDragStarted;

	public Vector2 currentMomentum
	{
		get => mMomentum;
	    set => mMomentum = value;
	}

	private void Awake()
	{
		mCam = camera;
		mTrans = transform;
		if (rootForBounds == null)
		{
			Debug.LogError(NGUITools.GetHierarchy(gameObject) + " needs the 'Root For Bounds' parameter to be set", this);
			enabled = false;
		}
	}

	private void Start()
	{
		mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
	}

	private Vector3 CalculateConstrainOffset()
	{
		if (rootForBounds == null || rootForBounds.childCount == 0)
		{
			return Vector3.zero;
		}
		Vector3 vector = new Vector3(mCam.rect.xMin * Screen.width, mCam.rect.yMin * Screen.height, 0f);
		Vector3 vector2 = new Vector3(mCam.rect.xMax * Screen.width, mCam.rect.yMax * Screen.height, 0f);
		vector = mCam.ScreenToWorldPoint(vector);
		vector2 = mCam.ScreenToWorldPoint(vector2);
		Vector2 minRect = new Vector2(mBounds.min.x, mBounds.min.y);
		Vector2 maxRect = new Vector2(mBounds.max.x, mBounds.max.y);
		return NGUIMath.ConstrainRect(minRect, maxRect, vector, vector2);
	}

	public Boolean ConstrainToBounds(Boolean immediate)
	{
		if (mTrans != null && rootForBounds != null)
		{
			Vector3 b = CalculateConstrainOffset();
			if (b.magnitude > 0f)
			{
				if (immediate)
				{
					mTrans.position -= b;
				}
				else
				{
					SpringPosition springPosition = SpringPosition.Begin(gameObject, mTrans.position - b, 13f);
					springPosition.ignoreTimeScale = true;
					springPosition.worldSpace = true;
				}
				return true;
			}
		}
		return false;
	}

	public void Press(Boolean isPressed)
	{
		if (isPressed)
		{
			mDragStarted = false;
		}
		if (rootForBounds != null)
		{
			mPressed = isPressed;
			if (isPressed)
			{
				mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(rootForBounds);
				mMomentum = Vector2.zero;
				mScroll = 0f;
				SpringPosition component = GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else if (dragEffect == UIDragObject.DragEffect.MomentumAndSpring)
			{
				ConstrainToBounds(false);
			}
		}
	}

	public void Drag(Vector2 delta)
	{
		if (smoothDragStart && !mDragStarted)
		{
			mDragStarted = true;
			return;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		if (mRoot != null)
		{
			delta *= mRoot.pixelSizeAdjustment;
		}
		Vector2 vector = Vector2.Scale(delta, -scale);
		mTrans.localPosition += (Vector3)vector;
		mMomentum = Vector2.Lerp(mMomentum, mMomentum + vector * (0.01f * momentumAmount), 0.67f);
		if (dragEffect != UIDragObject.DragEffect.MomentumAndSpring && ConstrainToBounds(true))
		{
			mMomentum = Vector2.zero;
			mScroll = 0f;
		}
	}

	public void Scroll(Single delta)
	{
		if (enabled && NGUITools.GetActive(gameObject))
		{
			if (Mathf.Sign(mScroll) != Mathf.Sign(delta))
			{
				mScroll = 0f;
			}
			mScroll += delta * scrollWheelFactor;
		}
	}

	private void Update()
	{
		Single deltaTime = UpdateRealTimeDelta();
		if (mPressed)
		{
			SpringPosition component = GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			mScroll = 0f;
		}
		else
		{
			mMomentum += scale * (mScroll * 20f);
			mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, deltaTime);
			if (mMomentum.magnitude > 0.01f)
			{
				mTrans.localPosition += (Vector3)NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
				mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(rootForBounds);
				if (!ConstrainToBounds(dragEffect == UIDragObject.DragEffect.None))
				{
					SpringPosition component2 = GetComponent<SpringPosition>();
					if (component2 != null)
					{
						component2.enabled = false;
					}
				}
				return;
			}
			mScroll = 0f;
		}
		NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
	}
}
