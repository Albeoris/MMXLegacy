using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class UIDragObject : IgnoreTimeScale
{
	public Transform target;

	public Vector3 scale = Vector3.one;

	public Single scrollWheelFactor;

	public Boolean restrictWithinPanel;

	public DragEffect dragEffect = DragEffect.MomentumAndSpring;

	public Single momentumAmount = 35f;

	private Plane mPlane;

	private Vector3 mLastPos;

	private UIPanel mPanel;

	private Boolean mPressed;

	private Vector3 mMomentum = Vector3.zero;

	private Single mScroll;

	private Bounds mBounds;

	private void FindPanel()
	{
		mPanel = ((!(target != null)) ? null : UIPanel.Find(target.transform, false));
		if (mPanel == null)
		{
			restrictWithinPanel = false;
		}
	}

	private void OnPress(Boolean pressed)
	{
		if (enabled && NGUITools.GetActive(gameObject) && target != null)
		{
			mPressed = pressed;
			if (pressed)
			{
				if (restrictWithinPanel && mPanel == null)
				{
					FindPanel();
				}
				if (restrictWithinPanel)
				{
					mBounds = NGUIMath.CalculateRelativeWidgetBounds(mPanel.cachedTransform, target);
				}
				mMomentum = Vector3.zero;
				mScroll = 0f;
				SpringPosition component = target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
				mLastPos = UICamera.lastHit.point;
				Transform transform = UICamera.currentCamera.transform;
				mPlane = new Plane(((!(mPanel != null)) ? transform.rotation : mPanel.cachedTransform.rotation) * Vector3.back, mLastPos);
			}
			else if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None && dragEffect == DragEffect.MomentumAndSpring)
			{
				mPanel.ConstrainTargetToBounds(target, ref mBounds, false);
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && target != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			Single distance = 0f;
			if (mPlane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				Vector3 vector = point - mLastPos;
				mLastPos = point;
				if (vector.x != 0f || vector.y != 0f)
				{
					vector = target.InverseTransformDirection(vector);
					vector.Scale(scale);
					vector = target.TransformDirection(vector);
				}
				if (dragEffect != DragEffect.None)
				{
					mMomentum = Vector3.Lerp(mMomentum, mMomentum + vector * (0.01f * momentumAmount), 0.67f);
				}
				if (restrictWithinPanel)
				{
					Vector3 localPosition = target.localPosition;
					target.position += vector;
					mBounds.center = mBounds.center + (target.localPosition - localPosition);
					if (dragEffect != DragEffect.MomentumAndSpring && mPanel.clipping != UIDrawCall.Clipping.None && mPanel.ConstrainTargetToBounds(target, ref mBounds, true))
					{
						mMomentum = Vector3.zero;
						mScroll = 0f;
					}
				}
				else
				{
					target.position += vector;
				}
			}
		}
	}

	private void LateUpdate()
	{
		Single deltaTime = UpdateRealTimeDelta();
		if (target == null)
		{
			return;
		}
		if (mPressed)
		{
			SpringPosition component = target.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			mScroll = 0f;
		}
		else
		{
			mMomentum += scale * (-mScroll * 0.05f);
			mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, deltaTime);
			if (mMomentum.magnitude > 0.0001f)
			{
				if (mPanel == null)
				{
					FindPanel();
				}
				if (mPanel != null)
				{
					target.position += NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
					if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None)
					{
						mBounds = NGUIMath.CalculateRelativeWidgetBounds(mPanel.cachedTransform, target);
						if (!mPanel.ConstrainTargetToBounds(target, ref mBounds, dragEffect == DragEffect.None))
						{
							SpringPosition component2 = target.GetComponent<SpringPosition>();
							if (component2 != null)
							{
								component2.enabled = false;
							}
						}
					}
					return;
				}
			}
			else
			{
				mScroll = 0f;
			}
		}
		NGUIMath.SpringDampen(ref mMomentum, 9f, deltaTime);
	}

	private void OnScroll(Single delta)
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

	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring
	}
}
