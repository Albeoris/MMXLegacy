using System;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Follow Target")]
public class UIFollowTarget : MonoBehaviour
{
	private Transform mTrans;

	private Boolean mIsVisible;

	public Transform target;

	public Boolean disableIfInvisible = true;

	public Camera GameCamera;

	public Camera UICamera;

	private void Awake()
	{
		mTrans = transform;
	}

	private void Start()
	{
		if (target != null)
		{
			if (GameCamera == null)
			{
				GameCamera = Camera.main;
				if (GameCamera == null)
				{
					GameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
				}
			}
			if (UICamera == null)
			{
				GameObject gameObject = GameObject.FindWithTag("UICamera");
				if (gameObject != null)
				{
					UICamera = gameObject.GetComponent<Camera>();
				}
				if (UICamera == null)
				{
					UICamera = NGUITools.FindCameraForLayer(this.gameObject.layer);
				}
			}
			SetVisible(false);
		}
		else
		{
			Debug.LogError("Expected to have 'target' set to a valid transform", this);
			enabled = false;
		}
	}

	private void SetVisible(Boolean visible)
	{
		mIsVisible = visible;
		Int32 i = 0;
		Int32 childCount = mTrans.childCount;
		while (i < childCount)
		{
			mTrans.GetChild(i).gameObject.SetActive(visible);
			i++;
		}
	}

	private void Update()
	{
		Vector3 vector = GameCamera.WorldToViewportPoint(target.position);
		Boolean flag = vector.z > 0f && vector.x >= 0f && vector.x <= 1f;
		if (disableIfInvisible && mIsVisible != flag)
		{
			SetVisible(flag);
		}
		if (flag)
		{
			mTrans.position = UICamera.ViewportToWorldPoint(vector);
			vector = mTrans.localPosition;
			vector.x = Mathf.RoundToInt(vector.x);
			vector.y = Mathf.RoundToInt(vector.y);
			vector.z = 0f;
			mTrans.localPosition = vector;
		}
	}
}
