using System;
using UnityEngine;

public class WindswordDashMotionBlur : MotionBlur
{
	protected override void Start()
	{
		blurAmount = 0.9f;
		shader = Shader.Find("Hidden/MotionBlur");
		base.Start();
	}
}
