using System;
using UnityEngine;

public static class ArrayHelper
{
	public static T AddArrayElement<T>(ref T[] array) where T : new()
	{
		return AddArrayElement<T>(ref array, (default(T) == null) ? Activator.CreateInstance<T>() : default(T));
	}

	public static T AddArrayElement<T>(ref T[] array, T elToAdd)
	{
		if (array == null)
		{
			array = new T[1];
			array[0] = elToAdd;
			return elToAdd;
		}
		T[] array2 = new T[array.Length + 1];
		array.CopyTo(array2, 0);
		array2[array.Length] = elToAdd;
		array = array2;
		return elToAdd;
	}

	public static void DeleteArrayElement<T>(ref T[] array, Int32 index)
	{
		if (index >= array.Length || index < 0)
		{
			Debug.LogWarning("invalid index in DeleteArrayElement: " + index);
			return;
		}
		T[] array2 = new T[array.Length - 1];
		for (Int32 i = 0; i < index; i++)
		{
			array2[i] = array[i];
		}
		for (Int32 i = index + 1; i < array.Length; i++)
		{
			array2[i - 1] = array[i];
		}
		array = array2;
	}
}
