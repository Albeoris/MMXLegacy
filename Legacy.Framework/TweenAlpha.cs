﻿using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Alpha")]
public class TweenAlpha : UITweener
{
	public Single from = 1f;

	public Single to = 1f;

	private UIWidget mWidget;

	private UIPanel mPanel;

	public Single alpha
	{
		get
		{
			if (mWidget != null)
			{
				return mWidget.alpha;
			}
			if (mPanel != null)
			{
				return mPanel.alpha;
			}
			return 0f;
		}
		set
		{
			if (mWidget != null)
			{
				mWidget.alpha = value;
			}
			else if (mPanel != null)
			{
				mPanel.alpha = value;
			}
		}
	}

	private void Awake()
	{
		mPanel = GetComponent<UIPanel>();
		if (mPanel == null)
		{
			mWidget = GetComponentInChildren<UIWidget>();
		}
	}

	protected override void OnUpdate(Single factor, Boolean isFinished)
	{
		alpha = Mathf.Lerp(from, to, factor);
	}

	public static TweenAlpha Begin(GameObject go, Single duration, Single alpha)
	{
		TweenAlpha tweenAlpha = UITweener.Begin<TweenAlpha>(go, duration);
		tweenAlpha.from = tweenAlpha.alpha;
		tweenAlpha.to = alpha;
		if (duration <= 0f)
		{
			tweenAlpha.Sample(1f, true);
			tweenAlpha.enabled = false;
		}
		return tweenAlpha;
	}
}
