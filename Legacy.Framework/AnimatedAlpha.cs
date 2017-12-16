using System;
using UnityEngine;

public class AnimatedAlpha : MonoBehaviour
{
	public Single alpha = 1f;

	private UIWidget mWidget;

	private UIPanel mPanel;

	private void Awake()
	{
		mWidget = GetComponent<UIWidget>();
		mPanel = GetComponent<UIPanel>();
		Update();
	}

	private void Update()
	{
		if (mWidget != null)
		{
			mWidget.alpha = alpha;
		}
		if (mPanel != null)
		{
			mPanel.alpha = alpha;
		}
	}
}
