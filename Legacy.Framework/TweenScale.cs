using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Scale")]
public class TweenScale : UITweener
{
	public Vector3 from = Vector3.one;

	public Vector3 to = Vector3.one;

	public Boolean updateTable;

	private Transform mTrans;

	private UITable mTable;

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

	public Vector3 scale
	{
		get => cachedTransform.localScale;
	    set => cachedTransform.localScale = value;
	}

	protected override void OnUpdate(Single factor, Boolean isFinished)
	{
		cachedTransform.localScale = from * (1f - factor) + to * factor;
		if (updateTable)
		{
			if (mTable == null)
			{
				mTable = NGUITools.FindInParents<UITable>(gameObject);
				if (mTable == null)
				{
					updateTable = false;
					return;
				}
			}
			mTable.repositionNow = true;
		}
	}

	public static TweenScale Begin(GameObject go, Single duration, Vector3 scale)
	{
		TweenScale tweenScale = UITweener.Begin<TweenScale>(go, duration);
		tweenScale.from = tweenScale.scale;
		tweenScale.to = scale;
		if (duration <= 0f)
		{
			tweenScale.Sample(1f, true);
			tweenScale.enabled = false;
		}
		return tweenScale;
	}
}
