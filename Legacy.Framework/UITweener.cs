using System;
using AnimationOrTween;
using UnityEngine;

public abstract class UITweener : IgnoreTimeScale
{
	public OnFinished onFinished;

	public Method method;

	public Style style;

	[SerializeField]
	private AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 1f),
		new Keyframe(1f, 1f, 1f, 0f)
	});

	public Boolean ignoreTimeScale = true;

	public Single delay;

	public Single duration = 1f;

	public Boolean steeperCurves;

	public Int32 tweenGroup;

	public GameObject eventReceiver;

	public String callWhenFinished;

	private Boolean mStarted;

	private Single mStartTime;

	private Single mDuration;

	private Single mAmountPerDelta = 1f;

	private Single mFactor;

	private static AnimationCurve s_DEFAULT_CURVE = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 1f),
		new Keyframe(1f, 1f, 1f, 0f)
	});

	public AnimationCurve AnimationCurve
	{
		get
		{
			if (animationCurve == s_DEFAULT_CURVE)
			{
				animationCurve = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f, 0f, 1f),
					new Keyframe(1f, 1f, 1f, 0f)
				});
			}
			return animationCurve;
		}
		set => animationCurve = value;
	}

	public Single amountPerDelta
	{
		get
		{
			if (mDuration != duration)
			{
				mDuration = duration;
				mAmountPerDelta = Mathf.Abs((duration <= 0f) ? 1000f : (1f / duration));
			}
			return mAmountPerDelta;
		}
	}

	public Single tweenFactor => mFactor;

    public Direction direction => (mAmountPerDelta >= 0f) ? Direction.Forward : Direction.Reverse;

    private void Start()
	{
		Update();
	}

	private void Update()
	{
		Single num = (!ignoreTimeScale) ? Time.deltaTime : UpdateRealTimeDelta();
		Single num2 = (!ignoreTimeScale) ? Time.time : realTime;
		if (!mStarted)
		{
			mStarted = true;
			mStartTime = num2 + delay;
		}
		if (num2 < mStartTime)
		{
			return;
		}
		mFactor += amountPerDelta * num;
		if (style == Style.Loop)
		{
			if (mFactor > 1f)
			{
				mFactor -= Mathf.Floor(mFactor);
			}
		}
		else if (style == Style.PingPong)
		{
			if (mFactor > 1f)
			{
				mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
				mAmountPerDelta = -mAmountPerDelta;
			}
			else if (mFactor < 0f)
			{
				mFactor = -mFactor;
				mFactor -= Mathf.Floor(mFactor);
				mAmountPerDelta = -mAmountPerDelta;
			}
		}
		if (style == Style.Once && (mFactor > 1f || mFactor < 0f))
		{
			mFactor = Mathf.Clamp01(mFactor);
			Sample(mFactor, true);
			if (onFinished != null)
			{
				onFinished(this);
			}
			if (eventReceiver != null && !String.IsNullOrEmpty(callWhenFinished))
			{
				eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
			}
			if ((mFactor == 1f && mAmountPerDelta > 0f) || (mFactor == 0f && mAmountPerDelta < 0f))
			{
				enabled = false;
			}
		}
		else
		{
			Sample(mFactor, false);
		}
	}

	private void OnDisable()
	{
		mStarted = false;
	}

	public void Sample(Single factor, Boolean isFinished)
	{
		Single num = Mathf.Clamp01(factor);
		if (method == Method.EaseIn)
		{
			num = 1f - Mathf.Sin(1.57079637f * (1f - num));
			if (steeperCurves)
			{
				num *= num;
			}
		}
		else if (method == Method.EaseOut)
		{
			num = Mathf.Sin(1.57079637f * num);
			if (steeperCurves)
			{
				num = 1f - num;
				num = 1f - num * num;
			}
		}
		else if (method == Method.EaseInOut)
		{
			num -= Mathf.Sin(num * 6.28318548f) / 6.28318548f;
			if (steeperCurves)
			{
				num = num * 2f - 1f;
				Single num2 = Mathf.Sign(num);
				num = 1f - Mathf.Abs(num);
				num = 1f - num * num;
				num = num2 * num * 0.5f + 0.5f;
			}
		}
		else if (method == Method.BounceIn)
		{
			num = BounceLogic(num);
		}
		else if (method == Method.BounceOut)
		{
			num = 1f - BounceLogic(1f - num);
		}
		OnUpdate((animationCurve == null) ? num : animationCurve.Evaluate(num), isFinished);
	}

	private Single BounceLogic(Single val)
	{
		if (val < 0.363636f)
		{
			val = 7.5685f * val * val;
		}
		else if (val < 0.727272f)
		{
			val = 7.5625f * (val -= 0.545454f) * val + 0.75f;
		}
		else if (val < 0.90909f)
		{
			val = 7.5625f * (val -= 0.818181f) * val + 0.9375f;
		}
		else
		{
			val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f;
		}
		return val;
	}

	public void Play(Boolean forward)
	{
		mAmountPerDelta = Mathf.Abs(amountPerDelta);
		if (!forward)
		{
			mAmountPerDelta = -mAmountPerDelta;
		}
		enabled = true;
	}

	public void Reset()
	{
		mStarted = false;
		mFactor = ((mAmountPerDelta >= 0f) ? 0f : 1f);
		Sample(mFactor, false);
	}

	public void Toggle()
	{
		if (mFactor > 0f)
		{
			mAmountPerDelta = -amountPerDelta;
		}
		else
		{
			mAmountPerDelta = Mathf.Abs(amountPerDelta);
		}
		enabled = true;
	}

	protected abstract void OnUpdate(Single factor, Boolean isFinished);

	public static T Begin<T>(GameObject go, Single duration) where T : UITweener
	{
		T t = go.GetComponent<T>();
		if (t == null)
		{
			t = go.AddComponent<T>();
		}
		t.mStarted = false;
		t.duration = duration;
		t.mFactor = 0f;
		t.mAmountPerDelta = Mathf.Abs(t.mAmountPerDelta);
		t.style = Style.Once;
		t.animationCurve = s_DEFAULT_CURVE;
		t.eventReceiver = null;
		t.callWhenFinished = null;
		t.onFinished = null;
		t.enabled = true;
		return t;
	}

	public enum Method
	{
		Linear,
		EaseIn,
		EaseOut,
		EaseInOut,
		BounceIn,
		BounceOut
	}

	public enum Style
	{
		Once,
		Loop,
		PingPong
	}

	public delegate void OnFinished(UITweener tween);
}
