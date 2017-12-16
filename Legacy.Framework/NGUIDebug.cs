using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Debug")]
public class NGUIDebug : MonoBehaviour
{
	private static List<String> mLines = new List<String>();

	private static NGUIDebug mInstance = null;

	public static void Log(String text)
	{
		if (Application.isPlaying)
		{
			if (mLines.Count > 20)
			{
				mLines.RemoveAt(0);
			}
			mLines.Add(text);
			if (mInstance == null)
			{
				GameObject gameObject = new GameObject("_NGUI Debug");
				mInstance = gameObject.AddComponent<NGUIDebug>();
				DontDestroyOnLoad(gameObject);
			}
		}
		else
		{
			Debug.Log(text);
		}
	}

	public static void DrawBounds(Bounds b)
	{
		Vector3 center = b.center;
		Vector3 vector = b.center - b.extents;
		Vector3 vector2 = b.center + b.extents;
		Debug.DrawLine(new Vector3(vector.x, vector.y, center.z), new Vector3(vector2.x, vector.y, center.z), Color.red);
		Debug.DrawLine(new Vector3(vector.x, vector.y, center.z), new Vector3(vector.x, vector2.y, center.z), Color.red);
		Debug.DrawLine(new Vector3(vector2.x, vector.y, center.z), new Vector3(vector2.x, vector2.y, center.z), Color.red);
		Debug.DrawLine(new Vector3(vector.x, vector2.y, center.z), new Vector3(vector2.x, vector2.y, center.z), Color.red);
	}

	private void OnGUI()
	{
		Int32 i = 0;
		Int32 count = mLines.Count;
		while (i < count)
		{
			GUILayout.Label(mLines[i], new GUILayoutOption[0]);
			i++;
		}
	}
}
