using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Legacy.Core.Map;
using Legacy.Utilities;
using UnityEngine;

namespace Legacy
{
	public static class Helper
	{
		private const Single BYTE_TO_COLOR = 0.003921569f;

		public static readonly Quaternion ForwardRotation = Quaternion.LookRotation(Vector3.forward);

		public static readonly Quaternion RightRotation = Quaternion.LookRotation(Vector3.right);

		public static readonly Quaternion BackRotation = Quaternion.LookRotation(Vector3.back);

		public static readonly Quaternion LeftRotation = Quaternion.LookRotation(Vector3.left);

		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		public static void Dispose<T>(ref T obj) where T : IDisposable
		{
			if (!ReferenceEquals(obj, null))
			{
				obj.Dispose();
				obj = default(T);
			}
		}

		public static void DisposeList<T>(IList<T> objs) where T : IDisposable
		{
			if (objs != null)
			{
				foreach (T t in objs)
				{
					t.Dispose();
				}
				objs.Clear();
			}
		}

		public static Boolean Is64BitOperatingSystem()
		{
			Boolean flag;
			return IntPtr.Size == 8 || (DoesWin32MethodExist("kernel32.dll", "IsWow64Process") && IsWow64Process(GetCurrentProcess(), out flag) && flag);
		}

		private static Boolean DoesWin32MethodExist(String moduleName, String methodName)
		{
			IntPtr moduleHandle = GetModuleHandle(moduleName);
			return !(moduleHandle == IntPtr.Zero) && GetProcAddress(moduleHandle, methodName) != IntPtr.Zero;
		}

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr GetModuleHandle(String moduleName);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] String procName);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean IsWow64Process(IntPtr hProcess, out Boolean wow64Process);

		public static T ResourcesLoad<T>(String p_path) where T : UnityEngine.Object
		{
			return ResourcesLoad<T>(p_path, true);
		}

		public static T ResourcesLoad<T>(String p_path, Boolean throwException) where T : UnityEngine.Object
		{
			T t = Resources.Load(p_path, typeof(T)) as T;
			if (throwException && t == null)
			{
				throw new Exception(String.Concat(new String[]
				{
					"Resources not found at '",
					p_path,
					"' [",
					typeof(T).Name,
					"]"
				}));
			}
			return t;
		}

		public static T ResourcesLoadLinked<T>(String p_path, Boolean throwException) where T : UnityEngine.Object
		{
			UnityEngine.Object @object = Resources.Load(p_path);
			if (@object != null && @object is ResourceLink)
			{
				@object = ((ResourceLink)@object).Link;
			}
			if (@object != null && @object is T)
			{
				return (T)@object;
			}
			if (throwException)
			{
				throw new Exception(String.Concat(new String[]
				{
					"Resources not found at '",
					p_path,
					"' [",
					typeof(T).Name,
					"]"
				}));
			}
			return null;
		}

		public static UnityEngine.Object[] ResourcesLoadAll<T>(String p_path) where T : UnityEngine.Object
		{
			return ResourcesLoadAll<T>(p_path, true);
		}

		public static UnityEngine.Object[] ResourcesLoadAll<T>(String p_path, Boolean throwException) where T : UnityEngine.Object
		{
			UnityEngine.Object[] array = Resources.LoadAll(p_path, typeof(T));
			if (throwException && array == null)
			{
				throw new Exception(String.Concat(new String[]
				{
					"Resources not found at '",
					p_path,
					"' [",
					typeof(T).Name,
					"]"
				}));
			}
			return array;
		}

		public static T Instantiate<T>(T p_original) where T : UnityEngine.Object
		{
			return UnityEngine.Object.Instantiate(p_original) as T;
		}

		public static T Instantiate<T>(T p_original, Vector3 p_position) where T : UnityEngine.Object
		{
			return Instantiate(p_original, p_position, Quaternion.identity);
		}

		public static T Instantiate<T>(T p_original, Vector3 p_position, Quaternion p_rotation) where T : UnityEngine.Object
		{
			return UnityEngine.Object.Instantiate(p_original, p_position, p_rotation) as T;
		}

		public static void DestroyImmediate<T>(T p_obj) where T : UnityEngine.Object
		{
			DestroyImmediate(ref p_obj);
		}

		public static void DestroyImmediate<T>(ref T p_obj) where T : UnityEngine.Object
		{
			if (!ReferenceEquals(p_obj, null))
			{
				UnityEngine.Object.DestroyImmediate(p_obj);
				p_obj = null;
			}
		}

		public static void DestroyImmediate<T>(ref T[] p_array) where T : UnityEngine.Object
		{
			if (p_array != null)
			{
				for (Int32 i = 0; i < p_array.Length; i++)
				{
					DestroyImmediate(ref p_array[i]);
				}
				p_array = null;
			}
		}

		public static void DestroyImmediateList<T>(IList<T> p_list) where T : UnityEngine.Object
		{
			if (p_list != null)
			{
				foreach (T t in p_list)
				{
					if (!ReferenceEquals(t, null))
					{
						UnityEngine.Object.DestroyImmediate(t);
					}
				}
				p_list.Clear();
			}
		}

		public static void DestroyGO<T>(T p_obj) where T : Component
		{
			DestroyGO(ref p_obj, 0f);
		}

		public static void DestroyGO<T>(ref T p_obj) where T : Component
		{
			DestroyGO(ref p_obj, 0f);
		}

		public static void DestroyGO<T>(T p_obj, Single delayTime) where T : Component
		{
			DestroyGO(ref p_obj, delayTime);
		}

		public static void DestroyGO<T>(ref T p_obj, Single delayTime) where T : Component
		{
			if (p_obj != null)
			{
				UnityEngine.Object.Destroy(p_obj.gameObject, delayTime);
			}
			p_obj = null;
		}

		public static void Destroy<T>(T p_obj) where T : UnityEngine.Object
		{
			Destroy(ref p_obj, 0f);
		}

		public static void Destroy<T>(ref T p_obj) where T : UnityEngine.Object
		{
			Destroy(ref p_obj, 0f);
		}

		public static void Destroy<T>(T p_obj, Single delayTime) where T : UnityEngine.Object
		{
			Destroy(ref p_obj, delayTime);
		}

		public static void Destroy<T>(ref T p_obj, Single delayTime) where T : UnityEngine.Object
		{
			if (!ReferenceEquals(p_obj, null))
			{
				UnityEngine.Object.Destroy(p_obj, delayTime);
				p_obj = null;
			}
		}

		public static void Destroy<T>(ref T[] p_array) where T : UnityEngine.Object
		{
			if (p_array != null)
			{
				for (Int32 i = 0; i < p_array.Length; i++)
				{
					Destroy(ref p_array[i]);
				}
				p_array = null;
			}
		}

		public static void DestroyList<T>(IList<T> p_list) where T : UnityEngine.Object
		{
			if (p_list != null)
			{
				if (p_list.IsReadOnly)
				{
					throw new ArgumentException("p_list is readonly");
				}
				for (Int32 i = 0; i < p_list.Count; i++)
				{
					T t = p_list[i];
					Destroy(ref t);
				}
				p_list.Clear();
			}
		}

		public static Color ToColor(UInt32 p_rgba)
		{
			Color result;
			result.r = ((p_rgba & 4278190080u) >> 24) * BYTE_TO_COLOR;
			result.g = ((p_rgba & 16711680u) >> 16) * BYTE_TO_COLOR;
			result.b = ((p_rgba & 65280u) >> 8) * BYTE_TO_COLOR;
			result.a = (p_rgba & 255u) * BYTE_TO_COLOR;
			return result;
		}

		public static Color ToColor(Int32 p_rgb)
		{
			Color result;
			result.r = ((p_rgb & 16711680) >> 16) * BYTE_TO_COLOR;
			result.g = ((p_rgb & 65280) >> 8) * BYTE_TO_COLOR;
			result.b = (p_rgb & 255) * BYTE_TO_COLOR;
			result.a = 1f;
			return result;
		}

		public static Color ToColor(Byte p_r, Byte p_g, Byte p_b, Byte p_a)
		{
			Color result;
			result.r = p_r * BYTE_TO_COLOR;
			result.g = p_g * BYTE_TO_COLOR;
			result.b = p_b * BYTE_TO_COLOR;
			result.a = p_a * BYTE_TO_COLOR;
			return result;
		}

		public static Color ToColor(Byte p_r, Byte p_g, Byte p_b)
		{
			Color result;
			result.r = p_r * BYTE_TO_COLOR;
			result.g = p_g * BYTE_TO_COLOR;
			result.b = p_b * BYTE_TO_COLOR;
			result.a = 1f;
			return result;
		}

		public static T GetComponent<T>(this GameObject go, Boolean throwError) where T : Component
		{
			T component = go.GetComponent<T>();
			if (throwError && component == null)
			{
				String message = String.Concat(new String[]
				{
					"Component '",
					typeof(T).Name,
					"' not found in '",
					go.name,
					"'."
				});
				throw new ComponentNotFoundException(message);
			}
			return component;
		}

		public static T GetComponentInChildren<T>(this GameObject go, Boolean throwError) where T : Component
		{
			T componentInChildren = go.GetComponentInChildren<T>();
			if (throwError && componentInChildren == null)
			{
				String message = String.Concat(new String[]
				{
					"Component '",
					typeof(T).Name,
					"' not found in '",
					go.name,
					"'."
				});
				throw new ComponentNotFoundException(message);
			}
			return componentInChildren;
		}

		public static T GetComponent<T>(this Component comp, Boolean throwError) where T : Component
		{
			T component = comp.GetComponent<T>();
			if (throwError && component == null)
			{
				String message = String.Concat(new String[]
				{
					"Component '",
					typeof(T).Name,
					"' not found in '",
					comp.name,
					"'."
				});
				throw new ComponentNotFoundException(message);
			}
			return component;
		}

		public static T GetComponentInChildren<T>(this Component comp, Boolean throwError) where T : Component
		{
			T componentInChildren = comp.GetComponentInChildren<T>();
			if (throwError && componentInChildren == null)
			{
				String message = String.Concat(new String[]
				{
					"Component '",
					typeof(T).Name,
					"' not found in '",
					comp.name,
					"'."
				});
				throw new ComponentNotFoundException(message);
			}
			return componentInChildren;
		}

		public static void SendBroadcastEvent(this GameObject go, String methodName, UnityEventArgs handler)
		{
			go.SendBroadcastEvent(methodName, handler, SendMessageOptions.DontRequireReceiver);
		}

		public static void SendBroadcastEvent(this GameObject go, String methodName, UnityEventArgs handler, SendMessageOptions options)
		{
			if (go == null)
			{
				throw new ArgumentNullException("go");
			}
			if (String.IsNullOrEmpty(methodName))
			{
				throw new ArgumentNullException("methodName");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			go.BroadcastMessage(methodName, handler, options);
		}

		public static void SendEvent(this GameObject go, String methodName, UnityEventArgs handler)
		{
			go.SendEvent(methodName, handler, SendMessageOptions.DontRequireReceiver);
		}

		public static void SendEvent(this GameObject go, String methodName, UnityEventArgs handler, SendMessageOptions options)
		{
			if (go == null)
			{
				throw new ArgumentNullException("go");
			}
			if (String.IsNullOrEmpty(methodName))
			{
				throw new ArgumentNullException("methodName");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			go.SendMessage(methodName, handler, options);
		}

		public static void SendEvent(this MonoBehaviour comp, String methodName, UnityEventArgs handler)
		{
			comp.SendEvent(methodName, handler, SendMessageOptions.DontRequireReceiver);
		}

		public static void SendEvent(this MonoBehaviour comp, String methodName, UnityEventArgs handler, SendMessageOptions options)
		{
			SendEvent((Component)comp, methodName, handler, options);
		}

		public static void SendEvent(this Component comp, String methodName, UnityEventArgs handler)
		{
			comp.SendEvent(methodName, handler, SendMessageOptions.DontRequireReceiver);
		}

		public static void SendEvent(this Component comp, String methodName, UnityEventArgs handler, SendMessageOptions options)
		{
			if (comp == null)
			{
				throw new ArgumentNullException("comp");
			}
			if (String.IsNullOrEmpty(methodName))
			{
				throw new ArgumentNullException("methodName");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			comp.SendMessage(methodName, handler, options);
		}

		public static Boolean Approximately(Single a, Single b, Single threshold)
		{
			return Math.Abs(a - b) <= threshold;
		}

		public static Boolean Approximately(ref Color a, ref Color b, Single threshold)
		{
			return Math.Abs(a.r - b.r) <= threshold && Math.Abs(a.g - b.g) <= threshold && Math.Abs(a.b - b.b) <= threshold && Math.Abs(a.a - b.a) <= threshold;
		}

		public static AnimationCurve AnimationCurveEaseInOut(Single timeStart, Single valueStart, Single timeEnd, Single valueEnd, WrapMode wrapMode)
		{
			AnimationCurve animationCurve = AnimationCurve.EaseInOut(timeStart, valueStart, timeEnd, valueEnd);
			animationCurve.postWrapMode = wrapMode;
			animationCurve.preWrapMode = wrapMode;
			return animationCurve;
		}

		public static Shader FindShader(String name)
		{
			return FindShader(name, true);
		}

		public static Shader FindShader(String name, Boolean throwException)
		{
			Shader shader = Shader.Find(name);
			if (shader == null && throwException)
			{
				throw new Exception("Requested shader not found! '" + name + "'");
			}
			return shader;
		}

		public static void AngleDeg(ref Quaternion a, ref Quaternion b, out Single degrees)
		{
			AngleRad(ref a, ref b, out degrees);
			degrees *= 57.29578f;
		}

		public static void AngleRad(ref Quaternion a, ref Quaternion b, out Single radiant)
		{
			Dot(ref a, ref b, out radiant);
			radiant = (Single)Math.Acos(Math.Min(Math.Abs(radiant), 1f)) * 2f;
		}

		public static void Dot(ref Quaternion a, ref Quaternion b, out Single dot)
		{
			dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		public static void ConstantSlerpDeg(ref Quaternion from, ref Quaternion to, Single maxDegreesDelta, out Quaternion result)
		{
			Single num;
			AngleDeg(ref from, ref to, out num);
			if (num > maxDegreesDelta && num != 0f)
			{
				result = Quaternion.Slerp(from, to, maxDegreesDelta / num);
			}
			else
			{
				result = to;
			}
		}

		public static void ConstantSlerpRad(ref Quaternion from, ref Quaternion to, Single maxRadiantsDelta, out Quaternion result)
		{
			Single num;
			AngleRad(ref from, ref to, out num);
			if (num > maxRadiantsDelta && num != 0f)
			{
				result = Quaternion.Slerp(from, to, maxRadiantsDelta / num);
			}
			else
			{
				result = to;
			}
		}

		public static Single Length(ref Quaternion a)
		{
			return (Single)Math.Sqrt(LengthSquared(ref a));
		}

		public static Single LengthSquared(ref Quaternion a)
		{
			return a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w;
		}

		public static void Normalize(ref Quaternion a)
		{
			Single num = (Single)Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w);
			if (num > 1.401298E-45f)
			{
				num = 1f / num;
				a.x *= num;
				a.y *= num;
				a.z *= num;
				a.w *= num;
			}
		}

		public static Boolean Equals(ref Quaternion a, ref Quaternion b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		}

		public static void Dot(ref Vector3 a, ref Vector3 b, out Single result)
		{
			result = a.x * b.x + a.y * b.y + a.z * b.z;
		}

		public static Single Dot(ref Vector3 a, ref Vector3 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		public static void Normalize(ref Vector3 a)
		{
			Single num = a.magnitude;
			if (num > 1.401298E-45f)
			{
				num = 1f / num;
				a.x *= num;
				a.y *= num;
				a.z *= num;
			}
			else
			{
				a.x = (a.y = (a.z = 0f));
			}
		}

		public static void Cross(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			Single x = left.y * right.z - left.z * right.y;
			Single y = left.z * right.x - left.x * right.z;
			Single z = left.x * right.y - left.y * right.x;
			result.x = x;
			result.y = y;
			result.z = z;
		}

		public static void Lerp(ref Vector3 start, ref Vector3 end, Single blend, out Vector3 result)
		{
			result.x = start.x + (end.x - start.x) * blend;
			result.y = start.y + (end.y - start.y) * blend;
			result.z = start.z + (end.z - start.z) * blend;
		}

		public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, Single blend, out Vector3 result)
		{
			Single num = blend * blend;
			Single num2 = blend * num;
			Single num3 = 2f * num2 - 3f * num + 1f;
			Single num4 = -2f * num2 + 3f * num;
			Single num5 = num2 - 2f * num + blend;
			Single num6 = num2 - num;
			result.x = value1.x * num3 + value2.x * num4 + tangent1.x * num5 + tangent2.x * num6;
			result.y = value1.y * num3 + value2.y * num4 + tangent1.y * num5 + tangent2.y * num6;
			result.z = value1.z * num3 + value2.z * num4 + tangent1.z * num5 + tangent2.z * num6;
		}

		public static void HermiteLerp(ref Vector3 start, ref Vector3 end, Single blend, out Vector3 result)
		{
			if (blend >= 1f)
			{
				result = end;
				return;
			}
			if (blend <= 0f)
			{
				result = start;
				return;
			}
			blend = blend * blend * (3f - 2f * blend);
			Lerp(ref start, ref end, blend, out result);
		}

		public static void SmoothStep(ref Vector3 start, ref Vector3 end, Single blend, out Vector3 result)
		{
			HermiteLerp(ref start, ref end, blend, out result);
		}

		public static Vector3 HermiteLerp(Vector3 start, Vector3 end, Single blend)
		{
			HermiteLerp(ref start, ref end, blend, out end);
			return end;
		}

		public static void Nlerp(ref Vector3 start, ref Vector3 end, Single blend, out Vector3 result)
		{
			Lerp(ref start, ref end, blend, out result);
			Normalize(ref result);
		}

		public static void ConstantLerp(ref Vector3 start, ref Vector3 end, Single maxDistanceDelta, out Vector3 result)
		{
			Vector3 vector;
			vector.x = end.x - start.x;
			vector.y = end.y - start.y;
			vector.z = end.z - start.z;
			Single num = vector.magnitude;
			if (num > maxDistanceDelta && num != 0f)
			{
				num = 1f / num;
				result.x = start.x + vector.x * num * maxDistanceDelta;
				result.y = start.y + vector.y * num * maxDistanceDelta;
				result.z = start.z + vector.z * num * maxDistanceDelta;
			}
			else
			{
				result = end;
			}
		}

		public static Single Distance(ref Vector3 left, ref Vector3 right)
		{
			return (Single)Math.Sqrt(DistanceSquared(ref left, ref right));
		}

		public static Single DistanceSquared(ref Vector3 left, ref Vector3 right)
		{
			Single num = left.x - right.x;
			Single num2 = left.y - right.y;
			Single num3 = left.z - right.z;
			return num * num + num2 * num2 + num3 * num3;
		}

		public static Boolean Equals(ref Vector3 a, ref Vector3 b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static Boolean Equals(Vector3 a, Vector3 b, Single tolerance)
		{
			return Equals(ref a, ref b, tolerance);
		}

		public static Boolean Equals(ref Vector3 a, ref Vector3 b, Single tolerance)
		{
			return Math.Abs(a.x - b.x) < tolerance && Math.Abs(a.y - b.y) < tolerance && Math.Abs(a.z - b.z) < tolerance;
		}

		public static void ConstantLerp(ref Vector2 start, ref Vector2 end, Single maxDistanceDelta, out Vector2 result)
		{
			Vector2 vector;
			vector.x = end.x - start.x;
			vector.y = end.y - start.y;
			Single num = vector.magnitude;
			if (num > maxDistanceDelta && num != 0f)
			{
				num = 1f / num;
				result.x = start.x + vector.x * num * maxDistanceDelta;
				result.y = start.y + vector.y * num * maxDistanceDelta;
			}
			else
			{
				result = end;
			}
		}

		public static void SpreadDirection(ref Vector3 directon, Single inaccuracy, out Vector3 spreadDir)
		{
			spreadDir = Vector3.Slerp(directon, UnityEngine.Random.onUnitSphere, inaccuracy);
		}

		public static void SpreadDirection(ref Vector3 directon, ref Vector3 rndDirection, Single inaccuracy, out Vector3 spreadDir)
		{
			spreadDir = Vector3.Slerp(directon, rndDirection, inaccuracy);
		}

		public static Single ClampAngle(Single angle, Single min, Single max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			else if (angle > 360f)
			{
				angle -= 360f;
			}
			if (angle < min)
			{
				return min;
			}
			if (angle > max)
			{
				return max;
			}
			return angle;
		}

		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0)
			{
				return min;
			}
			if (value.CompareTo(max) > 0)
			{
				return max;
			}
			return value;
		}

		public static Single ConstantLerp(Single from, Single to, Single maxDelta)
		{
			if (Math.Abs(to - from) <= maxDelta)
			{
				return to;
			}
			return from + Math.Sign(to - @from) * maxDelta;
		}

		public static Single SmoothStep(Single from, Single to, Single blend)
		{
			if (blend < 0f)
			{
				return from;
			}
			if (blend > 1f)
			{
				return to;
			}
			blend = blend * blend * (3f - 2f * blend);
			return (1f - blend) * from + blend * to;
		}

		public static Int32 AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
		{
			Cross(ref fwd, ref targetDir, out fwd);
			Single value;
			Dot(ref fwd, ref up, out value);
			return Math.Sign(value);
		}

		public static Vector3 GetOffset(this Grid p_grid)
		{
			return new Vector3(p_grid.OffsetX, p_grid.OffsetY, p_grid.OffsetZ);
		}

		public static Vector3 SlotLocalPosition(Position p_position, Single p_height)
		{
			Vector3 result;
			result.x = p_position.X * 10 + 5f;
			result.y = p_height;
			result.z = p_position.Y * 10 + 5f;
			return result;
		}

		public static Quaternion GridDirectionToQuaternion(EDirection p_direction)
		{
			switch (p_direction)
			{
			case EDirection.NORTH:
				break;
			case EDirection.EAST:
				return RightRotation;
			case EDirection.SOUTH:
				return BackRotation;
			case EDirection.WEST:
				return LeftRotation;
			default:
				Debug.LogWarning("Cannot convert direction to a rotation! Dir: " + p_direction);
				break;
			}
			return ForwardRotation;
		}
	}
}
