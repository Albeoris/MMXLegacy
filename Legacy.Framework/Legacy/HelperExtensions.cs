using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Legacy
{
	public static class HelperExtensions
	{
		public static Int32 GetSliderStep(this UISlider slider)
		{
			if (slider.numberOfSteps == 0)
			{
				return 0;
			}
			return (Int32)(slider.sliderValue * (slider.numberOfSteps - 1) + 0.5f);
		}

		public static void SetSliderStep(this UISlider slider, Int32 stepValue)
		{
			if (slider.numberOfSteps == 0)
			{
				return;
			}
			slider.sliderValue = stepValue / (slider.numberOfSteps - 1f);
		}

		public static StringBuilder Rest(this StringBuilder textBuilder)
		{
			textBuilder.Length = 0;
			return textBuilder;
		}

		public static void Clear<T>(this T[] array)
		{
			if (array != null && array.Length != 0)
			{
				Array.Clear(array, 0, array.Length);
			}
		}

		public static void CopyTo<T>(this T[] array, T[] other)
		{
			if (array != null && other != null)
			{
				Array.Copy(array, other, array.Length);
			}
		}

		public static T[] ToArray<T>(this ICollection<T> collection)
		{
			if (collection.Count == 0)
			{
				return Arrays<T>.Empty;
			}
			T[] array = new T[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			if (list != null)
			{
				Int32 i = list.Count;
				while (i > 1)
				{
					i--;
					Int32 index = Random.Range(0, i + 1);
					T value = list[index];
					list[index] = list[i];
					list[i] = value;
				}
			}
		}

		public static void Shuffle<T>(this T[] array)
		{
			if (array != null)
			{
				Int32 i = array.Length;
				while (i > 1)
				{
					i--;
					Int32 num = Random.Range(0, i + 1);
					Helper.Swap<T>(ref array[num], ref array[i]);
				}
			}
		}

		public static T RandomElement<T>(this IList<T> list)
		{
			return list.RandomElement(default(T));
		}

		public static T RandomElement<T>(this IList<T> list, T defaultObject)
		{
			if (list != null)
			{
				if (list.Count == 1)
				{
					return list[0];
				}
				if (list.Count > 1)
				{
					return list[Random.Range(0, list.Count)];
				}
			}
			return defaultObject;
		}

		public static T FirstElement<T>(this IList<T> array)
		{
			if (array != null && array.Count > 0)
			{
				return array[0];
			}
			return default(T);
		}

		public static T LastElement<T>(this IList<T> array)
		{
			if (array != null && array.Count > 0)
			{
				return array[array.Count - 1];
			}
			return default(T);
		}

		public static void UnloadAsset(this UnityEngine.Object obj)
		{
			if (obj != null)
			{
				Resources.UnloadAsset(obj);
			}
		}

		public static void AddChild(this Transform parent, Transform child, Vector3 localPosition, Quaternion localRotation)
		{
			child.parent = parent;
			child.localPosition = localPosition;
			child.localRotation = localRotation;
		}

		public static void AddChild(this Transform parent, Transform child, Vector3 localPosition)
		{
			child.parent = parent;
			child.localPosition = localPosition;
		}

		public static void AddChild(this Transform parent, Transform child)
		{
			child.parent = parent;
		}

		public static void AddChildAlignOrigin(this Transform parent, Transform child)
		{
			child.parent = parent;
			child.localPosition = Vector3.zero;
			child.localRotation = Quaternion.identity;
		}

		public static void AddChilds(this Transform parent, Transform[] childrens, Vector3 localPosition, Quaternion localRotation)
		{
			if (childrens != null)
			{
				for (Int32 i = 0; i < childrens.Length; i++)
				{
					childrens[i].parent = parent;
					childrens[i].localPosition = localPosition;
					childrens[i].localRotation = localRotation;
				}
			}
		}

		public static void RemoveChild(this Transform parent, Transform child)
		{
			if (child.parent == parent)
			{
				child.parent = null;
			}
		}

		public static void RemoveChilds(this Transform parent, Transform[] childrens)
		{
			if (childrens != null)
			{
				for (Int32 i = 0; i < childrens.Length; i++)
				{
					parent.RemoveChild(childrens[i]);
				}
			}
		}

		public static void DetachParent(this Transform child)
		{
			child.parent = null;
		}
	}
}
