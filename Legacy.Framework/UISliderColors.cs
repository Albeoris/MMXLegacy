using System;
using UnityEngine;

[RequireComponent(typeof(UISlider))]
[AddComponentMenu("NGUI/Examples/Slider Colors")]
[ExecuteInEditMode]
public class UISliderColors : MonoBehaviour
{
	public UISprite sprite;

	public Color[] colors = new Color[]
	{
		Color.red,
		Color.yellow,
		Color.green
	};

	private UISlider mSlider;

	private void Start()
	{
		mSlider = GetComponent<UISlider>();
		Update();
	}

	private void Update()
	{
		if (sprite == null || colors.Length == 0)
		{
			return;
		}
		Single num = mSlider.sliderValue;
		num *= colors.Length - 1;
		Int32 num2 = Mathf.FloorToInt(num);
		Color color = colors[0];
		if (num2 >= 0)
		{
			if (num2 + 1 < colors.Length)
			{
				Single t = num - num2;
				color = Color.Lerp(colors[num2], colors[num2 + 1], t);
			}
			else if (num2 < colors.Length)
			{
				color = colors[num2];
			}
			else
			{
				color = colors[colors.Length - 1];
			}
		}
		color.a = sprite.color.a;
		sprite.color = color;
	}
}
