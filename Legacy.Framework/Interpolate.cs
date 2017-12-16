using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class Interpolate
{
	private static Vector3 Identity(Vector3 v)
	{
		return v;
	}

	private static Vector3 TransformDotPosition(Transform t)
	{
		return t.position;
	}

	private static IEnumerable<Single> NewTimer(Single duration)
	{
		Single elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			yield return elapsedTime;
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= duration)
			{
				yield return elapsedTime;
			}
		}
		yield break;
	}

	private static IEnumerable<Single> NewCounter(Int32 start, Int32 end, Int32 step)
	{
		for (Int32 i = start; i <= end; i += step)
		{
			yield return i;
		}
		yield break;
	}

	public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, Single duration)
	{
		IEnumerable<Single> driver = NewTimer(duration);
		return NewEase(ease, start, end, duration, driver);
	}

	public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, Int32 slices)
	{
		IEnumerable<Single> driver = NewCounter(0, slices + 1, 1);
		return NewEase(ease, start, end, slices + 1, driver);
	}

	private static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, Single total, IEnumerable<Single> driver)
	{
		Vector3 distance = end - start;
		foreach (Single num in driver)
		{
			Single i = num;
			yield return Ease(ease, start, distance, i, total);
		}
		yield break;
	}

	private static Vector3 Ease(Function ease, Vector3 start, Vector3 distance, Single elapsedTime, Single duration)
	{
		start.x = ease(start.x, distance.x, elapsedTime, duration);
		start.y = ease(start.y, distance.y, elapsedTime, duration);
		start.z = ease(start.z, distance.z, elapsedTime, duration);
		return start;
	}

	public static Function Ease(EaseType type)
	{
		Function result = null;
		switch (type)
		{
		case EaseType.Linear:
			result = new Function(Linear);
			break;
		case EaseType.EaseInQuad:
			result = new Function(EaseInQuad);
			break;
		case EaseType.EaseOutQuad:
			result = new Function(EaseOutQuad);
			break;
		case EaseType.EaseInOutQuad:
			result = new Function(EaseInOutQuad);
			break;
		case EaseType.EaseInCubic:
			result = new Function(EaseInCubic);
			break;
		case EaseType.EaseOutCubic:
			result = new Function(EaseOutCubic);
			break;
		case EaseType.EaseInOutCubic:
			result = new Function(EaseInOutCubic);
			break;
		case EaseType.EaseInQuart:
			result = new Function(EaseInQuart);
			break;
		case EaseType.EaseOutQuart:
			result = new Function(EaseOutQuart);
			break;
		case EaseType.EaseInOutQuart:
			result = new Function(EaseInOutQuart);
			break;
		case EaseType.EaseInQuint:
			result = new Function(EaseInQuint);
			break;
		case EaseType.EaseOutQuint:
			result = new Function(EaseOutQuint);
			break;
		case EaseType.EaseInOutQuint:
			result = new Function(EaseInOutQuint);
			break;
		case EaseType.EaseInSine:
			result = new Function(EaseInSine);
			break;
		case EaseType.EaseOutSine:
			result = new Function(EaseOutSine);
			break;
		case EaseType.EaseInOutSine:
			result = new Function(EaseInOutSine);
			break;
		case EaseType.EaseInExpo:
			result = new Function(EaseInExpo);
			break;
		case EaseType.EaseOutExpo:
			result = new Function(EaseOutExpo);
			break;
		case EaseType.EaseInOutExpo:
			result = new Function(EaseInOutExpo);
			break;
		case EaseType.EaseInCirc:
			result = new Function(EaseInCirc);
			break;
		case EaseType.EaseOutCirc:
			result = new Function(EaseOutCirc);
			break;
		case EaseType.EaseInOutCirc:
			result = new Function(EaseInOutCirc);
			break;
		}
		return result;
	}

	public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, Single duration)
	{
		IEnumerable<Single> steps = NewTimer(duration);
		return NewBezier<Transform>(ease, nodes, new ToVector3<Transform>(TransformDotPosition), duration, steps);
	}

	public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, Int32 slices)
	{
		IEnumerable<Single> steps = NewCounter(0, slices + 1, 1);
		return NewBezier<Transform>(ease, nodes, new ToVector3<Transform>(TransformDotPosition), slices + 1, steps);
	}

	public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, Single duration)
	{
		IEnumerable<Single> steps = NewTimer(duration);
		return NewBezier<Vector3>(ease, points, new ToVector3<Vector3>(Identity), duration, steps);
	}

	public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, Int32 slices)
	{
		IEnumerable<Single> steps = NewCounter(0, slices + 1, 1);
		return NewBezier<Vector3>(ease, points, new ToVector3<Vector3>(Identity), slices + 1, steps);
	}

	private static IEnumerable<Vector3> NewBezier<T>(Function ease, IList nodes, ToVector3<T> toVector3, Single maxStep, IEnumerable<Single> steps)
	{
		if (nodes.Count >= 2)
		{
			Vector3[] points = new Vector3[nodes.Count];
			foreach (Single num in steps)
			{
				Single step = num;
				for (Int32 i = 0; i < nodes.Count; i++)
				{
					points[i] = toVector3((T)nodes[i]);
				}
				yield return Bezier(ease, points, step, maxStep);
			}
		}
		yield break;
	}

	private static Vector3 Bezier(Function ease, Vector3[] points, Single elapsedTime, Single duration)
	{
		for (Int32 i = points.Length - 1; i > 0; i--)
		{
			for (Int32 j = 0; j < i; j++)
			{
				points[j].x = ease(points[j].x, points[j + 1].x - points[j].x, elapsedTime, duration);
				points[j].y = ease(points[j].y, points[j + 1].y - points[j].y, elapsedTime, duration);
				points[j].z = ease(points[j].z, points[j + 1].z - points[j].z, elapsedTime, duration);
			}
		}
		return points[0];
	}

	public static IEnumerable<Vector3> NewCatmullRom(Transform[] nodes, Int32 slices, Boolean loop)
	{
		return NewCatmullRom<Transform>(nodes, new ToVector3<Transform>(TransformDotPosition), slices, loop);
	}

	public static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, Int32 slices, Boolean loop)
	{
		return NewCatmullRom<Vector3>(points, new ToVector3<Vector3>(Identity), slices, loop);
	}

	private static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, ToVector3<T> toVector3, Int32 slices, Boolean loop)
	{
		if (nodes.Count >= 2)
		{
			yield return toVector3((T)nodes[0]);
			Int32 last = nodes.Count - 1;
			Int32 current = 0;
			while (loop || current < last)
			{
				if (loop && current > last)
				{
					current = 0;
				}
				Int32 previous = (current != 0) ? (current - 1) : ((!loop) ? current : last);
				Int32 start = current;
				Int32 end = (current != last) ? (current + 1) : ((!loop) ? current : 0);
				Int32 next = (end != last) ? (end + 1) : ((!loop) ? end : 0);
				Int32 stepCount = slices + 1;
				for (Int32 step = 1; step <= stepCount; step++)
				{
					yield return CatmullRom(toVector3((T)nodes[previous]), toVector3((T)nodes[start]), toVector3((T)nodes[end]), toVector3((T)nodes[next]), step, stepCount);
				}
				current++;
			}
		}
		yield break;
	}

	private static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, Single elapsedTime, Single duration)
	{
		Single num = elapsedTime / duration;
		Single num2 = num * num;
		Single num3 = num2 * num;
		return previous * (-0.5f * num3 + num2 - 0.5f * num) + start * (1.5f * num3 + -2.5f * num2 + 1f) + end * (-1.5f * num3 + 2f * num2 + 0.5f * num) + next * (0.5f * num3 - 0.5f * num2);
	}

	private static Single Linear(Single start, Single distance, Single elapsedTime, Single duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * (elapsedTime / duration) + start;
	}

	private static Single EaseInQuad(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime + start;
	}

	private static Single EaseOutQuad(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return -distance * elapsedTime * (elapsedTime - 2f) + start;
	}

	private static Single EaseInOutQuad(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 1f;
		return -distance / 2f * (elapsedTime * (elapsedTime - 2f) - 1f) + start;
	}

	private static Single EaseInCubic(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static Single EaseOutCubic(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return distance * (elapsedTime * elapsedTime * elapsedTime + 1f) + start;
	}

	private static Single EaseInOutCubic(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (elapsedTime * elapsedTime * elapsedTime + 2f) + start;
	}

	private static Single EaseInQuart(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static Single EaseOutQuart(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1f) + start;
	}

	private static Single EaseInOutQuart(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return -distance / 2f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2f) + start;
	}

	private static Single EaseInQuint(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static Single EaseOutQuint(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1f) + start;
	}

	private static Single EaseInOutQuint(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2f) + start;
	}

	private static Single EaseInSine(Single start, Single distance, Single elapsedTime, Single duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return -distance * Mathf.Cos(elapsedTime / duration * 1.57079637f) + distance + start;
	}

	private static Single EaseOutSine(Single start, Single distance, Single elapsedTime, Single duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * Mathf.Sin(elapsedTime / duration * 1.57079637f) + start;
	}

	private static Single EaseInOutSine(Single start, Single distance, Single elapsedTime, Single duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return -distance / 2f * (Mathf.Cos(3.14159274f * elapsedTime / duration) - 1f) + start;
	}

	private static Single EaseInExpo(Single start, Single distance, Single elapsedTime, Single duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * Mathf.Pow(2f, 10f * (elapsedTime / duration - 1f)) + start;
	}

	private static Single EaseOutExpo(Single start, Single distance, Single elapsedTime, Single duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * (-Mathf.Pow(2f, -10f * elapsedTime / duration) + 1f) + start;
	}

	private static Single EaseInOutExpo(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * Mathf.Pow(2f, 10f * (elapsedTime - 1f)) + start;
		}
		elapsedTime -= 1f;
		return distance / 2f * (-Mathf.Pow(2f, -10f * elapsedTime) + 2f) + start;
	}

	private static Single EaseInCirc(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return -distance * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) - 1f) + start;
	}

	private static Single EaseOutCirc(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return distance * Mathf.Sqrt(1f - elapsedTime * elapsedTime) + start;
	}

	private static Single EaseInOutCirc(Single start, Single distance, Single elapsedTime, Single duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return -distance / 2f * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) - 1f) + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) + 1f) + start;
	}

	public enum EaseType
	{
		Linear,
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseInQuart,
		EaseOutQuart,
		EaseInOutQuart,
		EaseInQuint,
		EaseOutQuint,
		EaseInOutQuint,
		EaseInSine,
		EaseOutSine,
		EaseInOutSine,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc
	}

	public delegate Vector3 ToVector3<T>(T v);

	public delegate Single Function(Single a, Single b, Single c, Single d);
}
