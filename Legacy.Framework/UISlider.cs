﻿using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Slider")]
public class UISlider : IgnoreTimeScale
{
	public static UISlider current;

	public Transform foreground;

	public Transform thumb;

	public Direction direction;

	public GameObject eventReceiver;

	public String functionName = "OnSliderChange";

	public OnValueChange onValueChange;

	public Int32 numberOfSteps;

	[HideInInspector]
	[SerializeField]
	private Single rawValue = 1f;

	private BoxCollider mCol;

	private Transform mTrans;

	private Transform mFGTrans;

	private UIWidget mFGWidget;

	private UISprite mFGFilled;

	private Boolean mInitDone;

	private Vector2 mSize = Vector2.zero;

	private Vector2 mCenter = Vector3.zero;

	public Single sliderValue
	{
		get
		{
			Single num = rawValue;
			if (numberOfSteps > 1)
			{
				num = Mathf.Round(num * (numberOfSteps - 1)) / (numberOfSteps - 1);
			}
			return num;
		}
		set => Set(value, false);
	}

	public Vector2 fullSize
	{
		get => mSize;
	    set
		{
			if (mSize != value)
			{
				mSize = value;
				ForceUpdate();
			}
		}
	}

	private void Init()
	{
		mInitDone = true;
		if (foreground != null)
		{
			mFGWidget = foreground.GetComponent<UIWidget>();
			mFGFilled = ((!(mFGWidget != null)) ? null : (mFGWidget as UISprite));
			mFGTrans = foreground.transform;
			if (mSize == Vector2.zero)
			{
				mSize = foreground.localScale;
			}
			if (mCenter == Vector2.zero)
			{
				mCenter = foreground.localPosition + foreground.localScale * 0.5f;
			}
		}
		else if (mCol != null)
		{
			if (mSize == Vector2.zero)
			{
				mSize = mCol.size;
			}
			if (mCenter == Vector2.zero)
			{
				mCenter = mCol.center;
			}
		}
		else
		{
			Debug.LogWarning("UISlider expected to find a foreground object or a box collider to work with", this);
		}
	}

	private void Awake()
	{
		mTrans = transform;
		mCol = (collider as BoxCollider);
	}

	private void Start()
	{
		Init();
		if (Application.isPlaying && thumb != null && thumb.collider != null)
		{
			UIEventListener uieventListener = UIEventListener.Get(thumb.gameObject);
			UIEventListener uieventListener2 = uieventListener;
			uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(OnPressThumb));
			UIEventListener uieventListener3 = uieventListener;
			uieventListener3.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener3.onDrag, new UIEventListener.VectorDelegate(OnDragThumb));
		}
		Set(rawValue, true);
	}

	private void OnPress(Boolean pressed)
	{
		if (pressed && UICamera.currentTouchID != -100)
		{
			UpdateDrag();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		UpdateDrag();
	}

	private void OnPressThumb(GameObject go, Boolean pressed)
	{
		if (pressed)
		{
			UpdateDrag();
		}
	}

	private void OnDragThumb(GameObject go, Vector2 delta)
	{
		UpdateDrag();
	}

	private void OnKey(KeyCode key)
	{
		Single num = ((Single)numberOfSteps <= 1f) ? 0.125f : (1f / (numberOfSteps - 1));
		if (direction == Direction.Horizontal)
		{
			if (key == KeyCode.LeftArrow)
			{
				Set(rawValue - num, false);
			}
			else if (key == KeyCode.RightArrow)
			{
				Set(rawValue + num, false);
			}
		}
		else if (key == KeyCode.DownArrow)
		{
			Set(rawValue - num, false);
		}
		else if (key == KeyCode.UpArrow)
		{
			Set(rawValue + num, false);
		}
	}

	private void UpdateDrag()
	{
		if (mCol == null || UICamera.currentCamera == null || UICamera.currentTouch == null)
		{
			return;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		Plane plane = new Plane(mTrans.rotation * Vector3.back, mTrans.position);
		Single distance;
		if (!plane.Raycast(ray, out distance))
		{
			return;
		}
		Vector3 b = mTrans.localPosition + (Vector3)(mCenter - mSize * 0.5f);
		Vector3 b2 = mTrans.localPosition - b;
		Vector3 a = mTrans.InverseTransformPoint(ray.GetPoint(distance));
		Vector3 vector = a + b2;
		Set((direction != Direction.Horizontal) ? (vector.y / mSize.y) : (vector.x / mSize.x), false);
	}

	private void Set(Single input, Boolean force)
	{
		if (!mInitDone)
		{
			Init();
		}
		Single num = Mathf.Clamp01(input);
		if (num < 0.001f)
		{
			num = 0f;
		}
		Single sliderValue = this.sliderValue;
		rawValue = num;
		Single sliderValue2 = this.sliderValue;
		if (force || sliderValue != sliderValue2)
		{
			Vector3 localScale = mSize;
			if (direction == Direction.Horizontal)
			{
				localScale.x *= sliderValue2;
			}
			else
			{
				localScale.y *= sliderValue2;
			}
			if (mFGFilled != null && mFGFilled.type == UISprite.Type.Filled)
			{
				mFGFilled.fillAmount = sliderValue2;
			}
			else if (foreground != null)
			{
				mFGTrans.localScale = localScale;
				if (mFGWidget != null)
				{
					if (sliderValue2 > 0.001f)
					{
						mFGWidget.enabled = true;
						mFGWidget.MarkAsChanged();
					}
					else
					{
						mFGWidget.enabled = false;
					}
				}
			}
			if (thumb != null)
			{
				Vector3 localPosition = thumb.localPosition;
				if (mFGFilled != null && mFGFilled.type == UISprite.Type.Filled)
				{
					if (mFGFilled.fillDirection == UISprite.FillDirection.Horizontal)
					{
						localPosition.x = ((!mFGFilled.invert) ? localScale.x : (mSize.x - localScale.x));
					}
					else if (mFGFilled.fillDirection == UISprite.FillDirection.Vertical)
					{
						localPosition.y = ((!mFGFilled.invert) ? localScale.y : (mSize.y - localScale.y));
					}
					else
					{
						Debug.LogWarning("Slider thumb is only supported with Horizontal or Vertical fill direction", this);
					}
				}
				else if (direction == Direction.Horizontal)
				{
					localPosition.x = localScale.x;
				}
				else
				{
					localPosition.y = localScale.y;
				}
				thumb.localPosition = localPosition;
			}
			current = this;
			if (eventReceiver != null && !String.IsNullOrEmpty(functionName) && Application.isPlaying)
			{
				eventReceiver.SendMessage(functionName, sliderValue2, SendMessageOptions.DontRequireReceiver);
			}
			if (onValueChange != null)
			{
				onValueChange(sliderValue2);
			}
			current = null;
		}
	}

	public void ForceUpdate()
	{
		Set(rawValue, true);
	}

	public enum Direction
	{
		Horizontal,
		Vertical
	}

	public delegate void OnValueChange(Single val);
}
