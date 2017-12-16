using System;
using UnityEngine;

public static class NGUIMath
{
	public static Single Lerp(Single from, Single to, Single factor)
	{
		return from * (1f - factor) + to * factor;
	}

	public static Int32 ClampIndex(Int32 val, Int32 max)
	{
		return (val >= 0) ? ((val >= max) ? (max - 1) : val) : 0;
	}

	public static Int32 RepeatIndex(Int32 val, Int32 max)
	{
		if (max < 1)
		{
			return 0;
		}
		while (val < 0)
		{
			val += max;
		}
		while (val >= max)
		{
			val -= max;
		}
		return val;
	}

	public static Single WrapAngle(Single angle)
	{
		while (angle > 180f)
		{
			angle -= 360f;
		}
		while (angle < -180f)
		{
			angle += 360f;
		}
		return angle;
	}

	public static Single Wrap01(Single val)
	{
		return val - Mathf.FloorToInt(val);
	}

	public static Int32 HexToDecimal(Char ch)
	{
		switch (ch)
		{
		case '0':
			return 0;
		case '1':
			return 1;
		case '2':
			return 2;
		case '3':
			return 3;
		case '4':
			return 4;
		case '5':
			return 5;
		case '6':
			return 6;
		case '7':
			return 7;
		case '8':
			return 8;
		case '9':
			return 9;
		default:
			switch (ch)
			{
			case 'a':
				break;
			case 'b':
				return 11;
			case 'c':
				return 12;
			case 'd':
				return 13;
			case 'e':
				return 14;
			case 'f':
				return 15;
			default:
				return 15;
			}
			break;
		case 'A':
			break;
		case 'B':
			return 11;
		case 'C':
			return 12;
		case 'D':
			return 13;
		case 'E':
			return 14;
		case 'F':
			return 15;
		}
		return 10;
	}

	public static Char DecimalToHexChar(Int32 num)
	{
		if (num > 15)
		{
			return 'F';
		}
		if (num < 10)
		{
			return (Char)(48 + num);
		}
		return (Char)(65 + num - 10);
	}

	public static String DecimalToHex(Int32 num)
	{
		num &= 16777215;
		return num.ToString("X6");
	}

	public static Int32 ColorToInt(Color c)
	{
		Int32 num = 0;
		num |= Mathf.RoundToInt(c.r * 255f) << 24;
		num |= Mathf.RoundToInt(c.g * 255f) << 16;
		num |= Mathf.RoundToInt(c.b * 255f) << 8;
		return num | Mathf.RoundToInt(c.a * 255f);
	}

	public static Color IntToColor(Int32 val)
	{
		Single num = 0.003921569f;
		Color black = Color.black;
		black.r = num * (val >> 24 & 255);
		black.g = num * (val >> 16 & 255);
		black.b = num * (val >> 8 & 255);
		black.a = num * (val & 255);
		return black;
	}

	public static String IntToBinary(Int32 val, Int32 bits)
	{
		String text = String.Empty;
		Int32 i = bits;
		while (i > 0)
		{
			if (i == 8 || i == 16 || i == 24)
			{
				text += " ";
			}
			text += (((val & 1 << --i) == 0) ? '0' : '1');
		}
		return text;
	}

	public static Color HexToColor(UInt32 val)
	{
		return IntToColor((Int32)val);
	}

	public static Rect ConvertToTexCoords(Rect rect, Int32 width, Int32 height)
	{
		Rect result = rect;
		if (width != 0f && height != 0f)
		{
			result.xMin = rect.xMin / width;
			result.xMax = rect.xMax / width;
			result.yMin = 1f - rect.yMax / height;
			result.yMax = 1f - rect.yMin / height;
		}
		return result;
	}

	public static Rect ConvertToPixels(Rect rect, Int32 width, Int32 height, Boolean round)
	{
		Rect result = rect;
		if (round)
		{
			result.xMin = Mathf.RoundToInt(rect.xMin * width);
			result.xMax = Mathf.RoundToInt(rect.xMax * width);
			result.yMin = Mathf.RoundToInt((1f - rect.yMax) * height);
			result.yMax = Mathf.RoundToInt((1f - rect.yMin) * height);
		}
		else
		{
			result.xMin = rect.xMin * width;
			result.xMax = rect.xMax * width;
			result.yMin = (1f - rect.yMax) * height;
			result.yMax = (1f - rect.yMin) * height;
		}
		return result;
	}

	public static Rect MakePixelPerfect(Rect rect)
	{
		rect.xMin = Mathf.RoundToInt(rect.xMin);
		rect.yMin = Mathf.RoundToInt(rect.yMin);
		rect.xMax = Mathf.RoundToInt(rect.xMax);
		rect.yMax = Mathf.RoundToInt(rect.yMax);
		return rect;
	}

	public static Rect MakePixelPerfect(Rect rect, Int32 width, Int32 height)
	{
		rect = ConvertToPixels(rect, width, height, true);
		rect.xMin = Mathf.RoundToInt(rect.xMin);
		rect.yMin = Mathf.RoundToInt(rect.yMin);
		rect.xMax = Mathf.RoundToInt(rect.xMax);
		rect.yMax = Mathf.RoundToInt(rect.yMax);
		return ConvertToTexCoords(rect, width, height);
	}

	public static Vector3 ApplyHalfPixelOffset(Vector3 pos)
	{
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsWebPlayer || platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.XBOX360)
		{
			pos.x -= 0.5f;
			pos.y += 0.5f;
		}
		return pos;
	}

	public static Vector3 ApplyHalfPixelOffset(Vector3 pos, Vector3 scale)
	{
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsWebPlayer || platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.XBOX360)
		{
			if (Mathf.RoundToInt(scale.x) == Mathf.RoundToInt(scale.x * 0.5f) * 2)
			{
				pos.x -= 0.5f;
			}
			if (Mathf.RoundToInt(scale.y) == Mathf.RoundToInt(scale.y * 0.5f) * 2)
			{
				pos.y += 0.5f;
			}
		}
		return pos;
	}

	public static Vector2 ConstrainRect(Vector2 minRect, Vector2 maxRect, Vector2 minArea, Vector2 maxArea)
	{
		Vector2 zero = Vector2.zero;
		Single num = maxRect.x - minRect.x;
		Single num2 = maxRect.y - minRect.y;
		Single num3 = maxArea.x - minArea.x;
		Single num4 = maxArea.y - minArea.y;
		if (num > num3)
		{
			Single num5 = num - num3;
			minArea.x -= num5;
			maxArea.x += num5;
		}
		if (num2 > num4)
		{
			Single num6 = num2 - num4;
			minArea.y -= num6;
			maxArea.y += num6;
		}
		if (minRect.x < minArea.x)
		{
			zero.x += minArea.x - minRect.x;
		}
		if (maxRect.x > maxArea.x)
		{
			zero.x -= maxRect.x - maxArea.x;
		}
		if (minRect.y < minArea.y)
		{
			zero.y += minArea.y - minRect.y;
		}
		if (maxRect.y > maxArea.y)
		{
			zero.y -= maxRect.y - maxArea.y;
		}
		return zero;
	}

	public static Vector3[] CalculateWidgetCorners(UIWidget w)
	{
		Vector2 relativeSize = w.relativeSize;
		Vector2 pivotOffset = w.pivotOffset;
		Vector4 relativePadding = w.relativePadding;
		Single num = pivotOffset.x * relativeSize.x - relativePadding.x;
		Single num2 = pivotOffset.y * relativeSize.y + relativePadding.y;
		Single x = num + relativeSize.x + relativePadding.x + relativePadding.z;
		Single y = num2 - relativeSize.y - relativePadding.y - relativePadding.w;
		Transform cachedTransform = w.cachedTransform;
		return new Vector3[]
		{
			cachedTransform.TransformPoint(num, num2, 0f),
			cachedTransform.TransformPoint(num, y, 0f),
			cachedTransform.TransformPoint(x, y, 0f),
			cachedTransform.TransformPoint(x, num2, 0f)
		};
	}

	public static Bounds CalculateAbsoluteWidgetBounds(Transform trans)
	{
		UIWidget[] componentsInChildren = trans.GetComponentsInChildren<UIWidget>();
		if (componentsInChildren.Length == 0)
		{
			return new Bounds(trans.position, Vector3.zero);
		}
		Vector3 vector = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
		Vector3 vector2 = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
		Int32 i = 0;
		Int32 num = componentsInChildren.Length;
		while (i < num)
		{
			UIWidget uiwidget = componentsInChildren[i];
			Vector2 a = uiwidget.relativeSize;
			Vector2 pivotOffset = uiwidget.pivotOffset;
			Single num2 = (pivotOffset.x + 0.5f) * a.x;
			Single num3 = (pivotOffset.y - 0.5f) * a.y;
			a *= 0.5f;
			Transform cachedTransform = uiwidget.cachedTransform;
			Vector3 lhs = cachedTransform.TransformPoint(new Vector3(num2 - a.x, num3 - a.y, 0f));
			vector2 = Vector3.Max(lhs, vector2);
			vector = Vector3.Min(lhs, vector);
			lhs = cachedTransform.TransformPoint(new Vector3(num2 - a.x, num3 + a.y, 0f));
			vector2 = Vector3.Max(lhs, vector2);
			vector = Vector3.Min(lhs, vector);
			lhs = cachedTransform.TransformPoint(new Vector3(num2 + a.x, num3 - a.y, 0f));
			vector2 = Vector3.Max(lhs, vector2);
			vector = Vector3.Min(lhs, vector);
			lhs = cachedTransform.TransformPoint(new Vector3(num2 + a.x, num3 + a.y, 0f));
			vector2 = Vector3.Max(lhs, vector2);
			vector = Vector3.Min(lhs, vector);
			i++;
		}
		Bounds result = new Bounds(vector, Vector3.zero);
		result.Encapsulate(vector2);
		return result;
	}

	public static Bounds CalculateRelativeWidgetBounds(Transform root, Transform child)
	{
		UIWidget[] componentsInChildren = child.GetComponentsInChildren<UIWidget>();
		if (componentsInChildren.Length == 0)
		{
			return new Bounds(Vector3.zero, Vector3.zero);
		}
		Vector3 vector = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
		Vector3 vector2 = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
		Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
		Int32 i = 0;
		Int32 num = componentsInChildren.Length;
		while (i < num)
		{
			UIWidget uiwidget = componentsInChildren[i];
			Vector2 a = uiwidget.relativeSize;
			Vector2 pivotOffset = uiwidget.pivotOffset;
			Transform cachedTransform = uiwidget.cachedTransform;
			Single num2 = (pivotOffset.x + 0.5f) * a.x;
			Single num3 = (pivotOffset.y - 0.5f) * a.y;
			a *= 0.5f;
			Vector3 vector3 = new Vector3(num2 - a.x, num3 - a.y, 0f);
			vector3 = cachedTransform.TransformPoint(vector3);
			vector3 = worldToLocalMatrix.MultiplyPoint3x4(vector3);
			vector2 = Vector3.Max(vector3, vector2);
			vector = Vector3.Min(vector3, vector);
			vector3 = new Vector3(num2 - a.x, num3 + a.y, 0f);
			vector3 = cachedTransform.TransformPoint(vector3);
			vector3 = worldToLocalMatrix.MultiplyPoint3x4(vector3);
			vector2 = Vector3.Max(vector3, vector2);
			vector = Vector3.Min(vector3, vector);
			vector3 = new Vector3(num2 + a.x, num3 - a.y, 0f);
			vector3 = cachedTransform.TransformPoint(vector3);
			vector3 = worldToLocalMatrix.MultiplyPoint3x4(vector3);
			vector2 = Vector3.Max(vector3, vector2);
			vector = Vector3.Min(vector3, vector);
			vector3 = new Vector3(num2 + a.x, num3 + a.y, 0f);
			vector3 = cachedTransform.TransformPoint(vector3);
			vector3 = worldToLocalMatrix.MultiplyPoint3x4(vector3);
			vector2 = Vector3.Max(vector3, vector2);
			vector = Vector3.Min(vector3, vector);
			i++;
		}
		Bounds result = new Bounds(vector, Vector3.zero);
		result.Encapsulate(vector2);
		return result;
	}

	public static Bounds CalculateRelativeInnerBounds(Transform root, UISprite sprite)
	{
		if (sprite.type == UISprite.Type.Sliced)
		{
			Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
			Vector2 a = sprite.relativeSize;
			Vector2 pivotOffset = sprite.pivotOffset;
			Transform cachedTransform = sprite.cachedTransform;
			Single num = (pivotOffset.x + 0.5f) * a.x;
			Single num2 = (pivotOffset.y - 0.5f) * a.y;
			a *= 0.5f;
			Single x = cachedTransform.localScale.x;
			Single y = cachedTransform.localScale.y;
			Vector4 border = sprite.border;
			if (x != 0f)
			{
				border.x /= x;
				border.z /= x;
			}
			if (y != 0f)
			{
				border.y /= y;
				border.w /= y;
			}
			Single x2 = num - a.x + border.x;
			Single x3 = num + a.x - border.z;
			Single y2 = num2 - a.y + border.y;
			Single y3 = num2 + a.y - border.w;
			Vector3 vector = new Vector3(x2, y2, 0f);
			vector = cachedTransform.TransformPoint(vector);
			vector = worldToLocalMatrix.MultiplyPoint3x4(vector);
			Bounds result = new Bounds(vector, Vector3.zero);
			vector = new Vector3(x2, y3, 0f);
			vector = cachedTransform.TransformPoint(vector);
			vector = worldToLocalMatrix.MultiplyPoint3x4(vector);
			result.Encapsulate(vector);
			vector = new Vector3(x3, y3, 0f);
			vector = cachedTransform.TransformPoint(vector);
			vector = worldToLocalMatrix.MultiplyPoint3x4(vector);
			result.Encapsulate(vector);
			vector = new Vector3(x3, y2, 0f);
			vector = cachedTransform.TransformPoint(vector);
			vector = worldToLocalMatrix.MultiplyPoint3x4(vector);
			result.Encapsulate(vector);
			return result;
		}
		return CalculateRelativeWidgetBounds(root, sprite.cachedTransform);
	}

	public static Bounds CalculateRelativeWidgetBounds(Transform trans)
	{
		return CalculateRelativeWidgetBounds(trans, trans);
	}

	public static Vector3 SpringDampen(ref Vector3 velocity, Single strength, Single deltaTime)
	{
		if (deltaTime > 1f)
		{
			deltaTime = 1f;
		}
		Single d = 1f - strength * 0.001f;
		Int32 num = Mathf.RoundToInt(deltaTime * 1000f);
		Vector3 vector = Vector3.zero;
		for (Int32 i = 0; i < num; i++)
		{
			vector += velocity * 0.06f;
			velocity *= d;
		}
		return vector;
	}

	public static Vector2 SpringDampen(ref Vector2 velocity, Single strength, Single deltaTime)
	{
		if (deltaTime > 1f)
		{
			deltaTime = 1f;
		}
		Single d = 1f - strength * 0.001f;
		Int32 num = Mathf.RoundToInt(deltaTime * 1000f);
		Vector2 vector = Vector2.zero;
		for (Int32 i = 0; i < num; i++)
		{
			vector += velocity * 0.06f;
			velocity *= d;
		}
		return vector;
	}

	public static Single SpringLerp(Single strength, Single deltaTime)
	{
		if (deltaTime > 1f)
		{
			deltaTime = 1f;
		}
		Int32 num = Mathf.RoundToInt(deltaTime * 1000f);
		deltaTime = 0.001f * strength;
		Single num2 = 0f;
		for (Int32 i = 0; i < num; i++)
		{
			num2 = Mathf.Lerp(num2, 1f, deltaTime);
		}
		return num2;
	}

	public static Single SpringLerp(Single from, Single to, Single strength, Single deltaTime)
	{
		if (deltaTime > 1f)
		{
			deltaTime = 1f;
		}
		Int32 num = Mathf.RoundToInt(deltaTime * 1000f);
		deltaTime = 0.001f * strength;
		for (Int32 i = 0; i < num; i++)
		{
			from = Mathf.Lerp(from, to, deltaTime);
		}
		return from;
	}

	public static Vector2 SpringLerp(Vector2 from, Vector2 to, Single strength, Single deltaTime)
	{
		return Vector2.Lerp(from, to, SpringLerp(strength, deltaTime));
	}

	public static Vector3 SpringLerp(Vector3 from, Vector3 to, Single strength, Single deltaTime)
	{
		return Vector3.Lerp(from, to, SpringLerp(strength, deltaTime));
	}

	public static Quaternion SpringLerp(Quaternion from, Quaternion to, Single strength, Single deltaTime)
	{
		return Quaternion.Slerp(from, to, SpringLerp(strength, deltaTime));
	}

	public static Single RotateTowards(Single from, Single to, Single maxAngle)
	{
		Single num = WrapAngle(to - from);
		if (Mathf.Abs(num) > maxAngle)
		{
			num = maxAngle * Mathf.Sign(num);
		}
		return from + num;
	}

	private static Single DistancePointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
	{
		Single sqrMagnitude = (b - a).sqrMagnitude;
		if (sqrMagnitude == 0f)
		{
			return (point - a).magnitude;
		}
		Single num = Vector2.Dot(point - a, b - a) / sqrMagnitude;
		if (num < 0f)
		{
			return (point - a).magnitude;
		}
		if (num > 1f)
		{
			return (point - b).magnitude;
		}
		Vector2 b2 = a + num * (b - a);
		return (point - b2).magnitude;
	}

	public static Single DistanceToRectangle(Vector2[] screenPoints, Vector2 mousePos)
	{
		Boolean flag = false;
		Int32 val = 4;
		for (Int32 i = 0; i < 5; i++)
		{
			Vector3 vector = screenPoints[RepeatIndex(i, 4)];
			Vector3 vector2 = screenPoints[RepeatIndex(val, 4)];
			if (vector.y > mousePos.y != vector2.y > mousePos.y && mousePos.x < (vector2.x - vector.x) * (mousePos.y - vector.y) / (vector2.y - vector.y) + vector.x)
			{
				flag = !flag;
			}
			val = i;
		}
		if (!flag)
		{
			Single num = -1f;
			for (Int32 j = 0; j < 4; j++)
			{
				Vector3 v = screenPoints[j];
				Vector3 v2 = screenPoints[RepeatIndex(j + 1, 4)];
				Single num2 = DistancePointToLineSegment(mousePos, v, v2);
				if (num2 < num || num < 0f)
				{
					num = num2;
				}
			}
			return num;
		}
		return 0f;
	}

	public static Single DistanceToRectangle(Vector3[] worldPoints, Vector2 mousePos, Camera cam)
	{
		Vector2[] array = new Vector2[4];
		for (Int32 i = 0; i < 4; i++)
		{
			array[i] = cam.WorldToScreenPoint(worldPoints[i]);
		}
		return DistanceToRectangle(array, mousePos);
	}
}
