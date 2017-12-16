using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Tooltip")]
public class UITooltip : MonoBehaviour
{
	private static UITooltip mInstance;

	public Camera uiCamera;

	public UILabel text;

	public UISprite background;

	public Single appearSpeed = 10f;

	public Boolean scalingTransitions = true;

	private Transform mTrans;

	private Single mTarget;

	private Single mCurrent;

	private Vector3 mPos;

	private Vector3 mSize;

	private UIWidget[] mWidgets;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		mTrans = transform;
		mWidgets = GetComponentsInChildren<UIWidget>();
		mPos = mTrans.localPosition;
		mSize = mTrans.localScale;
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
		}
		SetAlpha(0f);
	}

	private void Update()
	{
		if (mCurrent != mTarget)
		{
			mCurrent = Mathf.Lerp(mCurrent, mTarget, Time.deltaTime * appearSpeed);
			if (Mathf.Abs(mCurrent - mTarget) < 0.001f)
			{
				mCurrent = mTarget;
			}
			SetAlpha(mCurrent * mCurrent);
			if (scalingTransitions)
			{
				Vector3 b = mSize * 0.25f;
				b.y = -b.y;
				Vector3 localScale = Vector3.one * (1.5f - mCurrent * 0.5f);
				Vector3 localPosition = Vector3.Lerp(mPos - b, mPos, mCurrent);
				mTrans.localPosition = localPosition;
				mTrans.localScale = localScale;
			}
		}
	}

	private void SetAlpha(Single val)
	{
		Int32 i = 0;
		Int32 num = mWidgets.Length;
		while (i < num)
		{
			UIWidget uiwidget = mWidgets[i];
			Color color = uiwidget.color;
			color.a = val;
			uiwidget.color = color;
			i++;
		}
	}

	private void SetText(String tooltipText)
	{
		if (text != null && !String.IsNullOrEmpty(tooltipText))
		{
			mTarget = 1f;
			if (text != null)
			{
				text.text = tooltipText;
			}
			mPos = Input.mousePosition;
			if (background != null)
			{
				Transform transform = background.transform;
				Transform transform2 = text.transform;
				Vector3 localPosition = transform2.localPosition;
				Vector3 localScale = transform2.localScale;
				mSize = text.relativeSize;
				mSize.x = mSize.x * localScale.x;
				mSize.y = mSize.y * localScale.y;
				mSize.x = mSize.x + (background.border.x + background.border.z + (localPosition.x - background.border.x) * 2f);
				mSize.y = mSize.y + (background.border.y + background.border.w + (-localPosition.y - background.border.y) * 2f);
				mSize.z = 1f;
				transform.localScale = mSize;
			}
			if (uiCamera != null)
			{
				mPos.x = Mathf.Clamp01(mPos.x / Screen.width);
				mPos.y = Mathf.Clamp01(mPos.y / Screen.height);
				Single num = uiCamera.orthographicSize / mTrans.parent.lossyScale.y;
				Single num2 = Screen.height * 0.5f / num;
				Vector2 vector = new Vector2(num2 * mSize.x / Screen.width, num2 * mSize.y / Screen.height);
				mPos.x = Mathf.Min(mPos.x, 1f - vector.x);
				mPos.y = Mathf.Max(mPos.y, vector.y);
				mTrans.position = uiCamera.ViewportToWorldPoint(mPos);
				mPos = mTrans.localPosition;
				mPos.x = Mathf.Round(mPos.x);
				mPos.y = Mathf.Round(mPos.y);
				mTrans.localPosition = mPos;
			}
			else
			{
				if (mPos.x + mSize.x > Screen.width)
				{
					mPos.x = Screen.width - mSize.x;
				}
				if (mPos.y - mSize.y < 0f)
				{
					mPos.y = mSize.y;
				}
				mPos.x = mPos.x - Screen.width * 0.5f;
				mPos.y = mPos.y - Screen.height * 0.5f;
			}
		}
		else
		{
			mTarget = 0f;
		}
	}

	public static void ShowText(String tooltipText)
	{
		if (mInstance != null)
		{
			mInstance.SetText(tooltipText);
		}
	}
}
