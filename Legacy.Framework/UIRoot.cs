using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("NGUI/UI/Root")]
[ExecuteInEditMode]
public class UIRoot : MonoBehaviour
{
	private static List<UIRoot> mRoots = new List<UIRoot>();

	public Scaling scalingStyle = Scaling.FixedSize;

	[HideInInspector]
	public Boolean automatic;

	public Int32 manualHeight = 720;

	public Int32 minimumHeight = 320;

	public Int32 maximumHeight = 1536;

	private Transform mTrans;

	public static List<UIRoot> list => mRoots;

    public Int32 activeHeight
	{
		get
		{
			Int32 num = Mathf.Max(2, Screen.height);
			if (scalingStyle == Scaling.FixedSize)
			{
				return manualHeight;
			}
			if (num < minimumHeight)
			{
				return minimumHeight;
			}
			if (num > maximumHeight)
			{
				return maximumHeight;
			}
			return num;
		}
	}

	public Single pixelSizeAdjustment => GetPixelSizeAdjustment(Screen.height);

    public static Single GetPixelSizeAdjustment(GameObject go)
	{
		UIRoot uiroot = NGUITools.FindInParents<UIRoot>(go);
		return (!(uiroot != null)) ? 1f : uiroot.pixelSizeAdjustment;
	}

	public Single GetPixelSizeAdjustment(Int32 height)
	{
		height = Mathf.Max(2, height);
		if (scalingStyle == Scaling.FixedSize)
		{
			return manualHeight / (Single)height;
		}
		if (height < minimumHeight)
		{
			return minimumHeight / (Single)height;
		}
		if (height > maximumHeight)
		{
			return maximumHeight / (Single)height;
		}
		return 1f;
	}

	private void Awake()
	{
		mTrans = transform;
		mRoots.Add(this);
		if (automatic)
		{
			scalingStyle = Scaling.PixelPerfect;
			automatic = false;
		}
	}

	private void OnDestroy()
	{
		mRoots.Remove(this);
	}

	private void Start()
	{
		UIOrthoCamera componentInChildren = GetComponentInChildren<UIOrthoCamera>();
		if (componentInChildren != null)
		{
			Debug.LogWarning("UIRoot should not be active at the same time as UIOrthoCamera. Disabling UIOrthoCamera.", componentInChildren);
			Camera component = componentInChildren.gameObject.GetComponent<Camera>();
			componentInChildren.enabled = false;
			if (component != null)
			{
				component.orthographicSize = 1f;
			}
		}
	}

	private void Update()
	{
		if (mTrans != null)
		{
			Single num = activeHeight;
			if (num > 0f)
			{
				Single num2 = 2f / num;
				Vector3 localScale = mTrans.localScale;
				if (Mathf.Abs(localScale.x - num2) > 1.401298E-45f || Mathf.Abs(localScale.y - num2) > 1.401298E-45f || Mathf.Abs(localScale.z - num2) > 1.401298E-45f)
				{
					mTrans.localScale = new Vector3(num2, num2, num2);
				}
			}
		}
	}

	public static void Broadcast(String funcName)
	{
		Int32 i = 0;
		Int32 count = mRoots.Count;
		while (i < count)
		{
			UIRoot uiroot = mRoots[i];
			if (uiroot != null)
			{
				uiroot.BroadcastMessage(funcName, SendMessageOptions.DontRequireReceiver);
			}
			i++;
		}
	}

	public static void Broadcast(String funcName, Object param)
	{
		if (param == null)
		{
			Debug.LogError("SendMessage is bugged when you try to pass 'null' in the parameter field. It behaves as if no parameter was specified.");
		}
		else
		{
			Int32 i = 0;
			Int32 count = mRoots.Count;
			while (i < count)
			{
				UIRoot uiroot = mRoots[i];
				if (uiroot != null)
				{
					uiroot.BroadcastMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
				}
				i++;
			}
		}
	}

	public enum Scaling
	{
		PixelPerfect,
		FixedSize,
		FixedSizeOnMobiles
	}
}
