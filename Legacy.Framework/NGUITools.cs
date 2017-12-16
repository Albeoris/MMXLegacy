using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

public static class NGUITools
{
	private static AudioListener mListener;

	private static Boolean mLoaded;

	private static Single mGlobalVolume = 1f;

	private static PropertyInfo mSystemCopyBuffer;

	public static Single soundVolume
	{
		get
		{
			if (!mLoaded)
			{
				mLoaded = true;
				mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
			}
			return mGlobalVolume;
		}
		set
		{
			if (mGlobalVolume != value)
			{
				mLoaded = true;
				mGlobalVolume = value;
				PlayerPrefs.SetFloat("Sound", value);
			}
		}
	}

	public static Boolean fileAccess => Application.platform != RuntimePlatform.WindowsWebPlayer && Application.platform != RuntimePlatform.OSXWebPlayer;

    public static AudioSource PlaySound(AudioClip clip)
	{
		return PlaySound(clip, 1f, 1f);
	}

	public static AudioSource PlaySound(AudioClip clip, Single volume)
	{
		return PlaySound(clip, volume, 1f);
	}

	public static AudioSource PlaySound(AudioClip clip, Single volume, Single pitch)
	{
		volume *= soundVolume;
		if (clip != null && volume > 0.01f)
		{
			if (mListener == null)
			{
				mListener = (UnityEngine.Object.FindObjectOfType(typeof(AudioListener)) as AudioListener);
				if (mListener == null)
				{
					Camera camera = Camera.main;
					if (camera == null)
					{
						camera = (UnityEngine.Object.FindObjectOfType(typeof(Camera)) as Camera);
					}
					if (camera != null)
					{
						mListener = camera.gameObject.AddComponent<AudioListener>();
					}
				}
			}
			if (mListener != null && mListener.enabled && GetActive(mListener.gameObject))
			{
				AudioSource audioSource = mListener.audio;
				if (audioSource == null)
				{
					audioSource = mListener.gameObject.AddComponent<AudioSource>();
				}
				audioSource.pitch = pitch;
				audioSource.PlayOneShot(clip, volume);
				return audioSource;
			}
		}
		return null;
	}

	public static WWW OpenURL(String url)
	{
		WWW result = null;
		try
		{
			result = new WWW(url);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
		}
		return result;
	}

	public static WWW OpenURL(String url, WWWForm form)
	{
		if (form == null)
		{
			return OpenURL(url);
		}
		WWW result = null;
		try
		{
			result = new WWW(url, form);
		}
		catch (Exception ex)
		{
			Debug.LogError((ex == null) ? "<null>" : ex.Message);
		}
		return result;
	}

	public static Int32 RandomRange(Int32 min, Int32 max)
	{
		if (min == max)
		{
			return min;
		}
		return UnityEngine.Random.Range(min, max + 1);
	}

	public static String GetHierarchy(GameObject obj)
	{
		String text = obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			text = obj.name + "/" + text;
		}
		return "\"" + text + "\"";
	}

	public static Color ParseColor(String text, Int32 offset)
	{
		Int32 num = NGUIMath.HexToDecimal(text[offset]) << 4 | NGUIMath.HexToDecimal(text[offset + 1]);
		Int32 num2 = NGUIMath.HexToDecimal(text[offset + 2]) << 4 | NGUIMath.HexToDecimal(text[offset + 3]);
		Int32 num3 = NGUIMath.HexToDecimal(text[offset + 4]) << 4 | NGUIMath.HexToDecimal(text[offset + 5]);
		return new Color(0.003921569f * num, 0.003921569f * num2, 0.003921569f * num3);
	}

	public static Color32 ParseColor32(String text, Int32 offset)
	{
		Byte r = (Byte)(NGUIMath.HexToDecimal(text[offset]) << 4 | NGUIMath.HexToDecimal(text[offset + 1]));
		Byte g = (Byte)(NGUIMath.HexToDecimal(text[offset + 2]) << 4 | NGUIMath.HexToDecimal(text[offset + 3]));
		Byte b = (Byte)(NGUIMath.HexToDecimal(text[offset + 4]) << 4 | NGUIMath.HexToDecimal(text[offset + 5]));
		return new Color32(r, g, b, Byte.MaxValue);
	}

	public static Boolean IsColor(String text, Int32 offset)
	{
		Int32 i = offset;
		Int32 num = offset + 6;
		while (i < num)
		{
			Char c = text[i];
			switch (c)
			{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case 'A':
			case 'B':
			case 'C':
			case 'D':
			case 'E':
			case 'F':
				break;
			default:
				switch (c)
				{
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
					break;
				default:
					return false;
				}
				break;
			}
			i++;
		}
		return true;
	}

	public static String EncodeColor(Color c)
	{
		Int32 num = 16777215 & NGUIMath.ColorToInt(c) >> 8;
		return NGUIMath.DecimalToHex(num);
	}

	public static Int32 ParseSymbol(String text, Int32 index, List<Color32> colors, Boolean premultiply)
	{
		Int32 length = text.Length;
		if (index + 2 < length)
		{
			if (text[index + 1] == '-')
			{
				if (text[index + 2] == ']')
				{
					if (colors != null && colors.Count > 1)
					{
						colors.RemoveAt(colors.Count - 1);
					}
					return 3;
				}
			}
			else if (index + 7 < length && text[index + 7] == ']')
			{
				if (colors != null)
				{
					if (!IsColor(text, index + 1))
					{
						return 0;
					}
					Color32 item = ParseColor32(text, index + 1);
					item.a = colors[colors.Count - 1].a;
					if (premultiply && item.a != 255)
					{
						Single num = item.a * 0.003921569f;
						item.r = (Byte)(item.r * num);
						item.g = (Byte)(item.g * num);
						item.b = (Byte)(item.b * num);
					}
					colors.Add(item);
				}
				return 8;
			}
		}
		return 0;
	}

	public static String StripSymbols(String text)
	{
		if (text != null)
		{
			Int32 i = 0;
			Int32 length = text.Length;
			while (i < length)
			{
				Char c = text[i];
				if (c == '[')
				{
					Int32 num = ParseSymbol(text, i, null, false);
					if (num > 0)
					{
						text = text.Remove(i, num);
						length = text.Length;
						continue;
					}
				}
				i++;
			}
		}
		return text;
	}

	public static T[] FindActive<T>() where T : Component
	{
		return UnityEngine.Object.FindObjectsOfType(typeof(T)) as T[];
	}

	public static Camera FindCameraForLayer(Int32 layer)
	{
		Int32 num = 1 << layer;
		Camera[] array = FindActive<Camera>();
		Int32 i = 0;
		Int32 num2 = array.Length;
		while (i < num2)
		{
			Camera camera = array[i];
			if ((camera.cullingMask & num) != 0)
			{
				return camera;
			}
			i++;
		}
		return null;
	}

	public static BoxCollider AddWidgetCollider(GameObject go)
	{
		if (go != null)
		{
			Collider component = go.GetComponent<Collider>();
			BoxCollider boxCollider = component as BoxCollider;
			if (boxCollider == null)
			{
				if (component != null)
				{
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(component);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(component);
					}
				}
				boxCollider = go.AddComponent<BoxCollider>();
			}
			Int32 num = CalculateNextDepth(go);
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(go.transform);
			boxCollider.isTrigger = true;
			boxCollider.center = bounds.center + Vector3.back * (num * 0.25f);
			boxCollider.size = new Vector3(bounds.size.x, bounds.size.y, 0f);
			return boxCollider;
		}
		return null;
	}

	public static String GetName<T>() where T : Component
	{
		String text = typeof(T).ToString();
		if (text.StartsWith("UI"))
		{
			text = text.Substring(2);
		}
		else if (text.StartsWith("UnityEngine."))
		{
			text = text.Substring(12);
		}
		return text;
	}

	public static GameObject AddChild(GameObject parent)
	{
		GameObject gameObject = new GameObject();
		if (parent != null)
		{
			Transform transform = gameObject.transform;
			transform.parent = parent.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			gameObject.layer = parent.layer;
		}
		return gameObject;
	}

	public static GameObject AddChild(GameObject parent, GameObject prefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab) as GameObject;
		if (gameObject != null && parent != null)
		{
			Transform transform = gameObject.transform;
			transform.parent = parent.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			gameObject.layer = parent.layer;
		}
		return gameObject;
	}

	public static Int32 CalculateNextDepth(GameObject go)
	{
		Int32 num = -1;
		UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
		Int32 i = 0;
		Int32 num2 = componentsInChildren.Length;
		while (i < num2)
		{
			num = Mathf.Max(num, componentsInChildren[i].depth);
			i++;
		}
		return num + 1;
	}

	public static T AddChild<T>(GameObject parent) where T : Component
	{
		GameObject gameObject = AddChild(parent);
		gameObject.name = GetName<T>();
		return gameObject.AddComponent<T>();
	}

	public static T AddWidget<T>(GameObject go) where T : UIWidget
	{
		Int32 depth = CalculateNextDepth(go);
		T result = AddChild<T>(go);
		result.depth = depth;
		Transform transform = result.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = new Vector3(100f, 100f, 1f);
		result.gameObject.layer = go.layer;
		return result;
	}

	public static UISprite AddSprite(GameObject go, UIAtlas atlas, String spriteName)
	{
		UIAtlas.Sprite sprite = (!(atlas != null)) ? null : atlas.GetSprite(spriteName);
		UISprite uisprite = AddWidget<UISprite>(go);
		uisprite.type = ((sprite != null && !(sprite.inner == sprite.outer)) ? UISprite.Type.Sliced : UISprite.Type.Simple);
		uisprite.atlas = atlas;
		uisprite.spriteName = spriteName;
		return uisprite;
	}

	public static GameObject GetRoot(GameObject go)
	{
		Transform transform = go.transform;
		for (;;)
		{
			Transform parent = transform.parent;
			if (parent == null)
			{
				break;
			}
			transform = parent;
		}
		return transform.gameObject;
	}

	public static T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null)
		{
			return null;
		}
		T component = go.GetComponent<T>();
		if (component == null)
		{
			Transform parent = go.transform.parent;
			while (parent != null && component == null)
			{
				component = parent.gameObject.GetComponent<T>();
				parent = parent.parent;
			}
		}
		return component;
	}

	public static void Destroy(UnityEngine.Object obj)
	{
		if (obj != null)
		{
			if (Application.isPlaying)
			{
				if (obj is GameObject)
				{
					GameObject gameObject = obj as GameObject;
					gameObject.transform.parent = null;
				}
				UnityEngine.Object.Destroy(obj);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
		}
	}

	public static void DestroyImmediate(UnityEngine.Object obj)
	{
		if (obj != null)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			else
			{
				UnityEngine.Object.Destroy(obj);
			}
		}
	}

	public static void Broadcast(String funcName)
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		Int32 i = 0;
		Int32 num = array.Length;
		while (i < num)
		{
			array[i].SendMessage(funcName, SendMessageOptions.DontRequireReceiver);
			i++;
		}
	}

	public static void Broadcast(String funcName, Object param)
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		Int32 i = 0;
		Int32 num = array.Length;
		while (i < num)
		{
			array[i].SendMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
			i++;
		}
	}

	public static Boolean IsChild(Transform parent, Transform child)
	{
		if (parent == null || child == null)
		{
			return false;
		}
		while (child != null)
		{
			if (child == parent)
			{
				return true;
			}
			child = child.parent;
		}
		return false;
	}

	private static void Activate(Transform t)
	{
		SetActiveSelf(t.gameObject, true);
		Int32 i = 0;
		Int32 childCount = t.childCount;
		while (i < childCount)
		{
			Transform child = t.GetChild(i);
			if (child.gameObject.activeSelf)
			{
				return;
			}
			i++;
		}
		Int32 j = 0;
		Int32 childCount2 = t.childCount;
		while (j < childCount2)
		{
			Transform child2 = t.GetChild(j);
			Activate(child2);
			j++;
		}
	}

	private static void Deactivate(Transform t)
	{
		SetActiveSelf(t.gameObject, false);
	}

	public static void SetActive(GameObject go, Boolean state)
	{
		if (state)
		{
			Activate(go.transform);
		}
		else
		{
			Deactivate(go.transform);
		}
	}

	public static void SetActiveChildren(GameObject go, Boolean state)
	{
		Transform transform = go.transform;
		if (state)
		{
			Int32 i = 0;
			Int32 childCount = transform.childCount;
			while (i < childCount)
			{
				Transform child = transform.GetChild(i);
				Activate(child);
				i++;
			}
		}
		else
		{
			Int32 j = 0;
			Int32 childCount2 = transform.childCount;
			while (j < childCount2)
			{
				Transform child2 = transform.GetChild(j);
				Deactivate(child2);
				j++;
			}
		}
	}

	public static Boolean GetActive(GameObject go)
	{
		return go && go.activeInHierarchy;
	}

	public static void SetActiveSelf(GameObject go, Boolean state)
	{
		go.SetActive(state);
	}

	public static void SetLayer(GameObject go, Int32 layer)
	{
		go.layer = layer;
		Transform transform = go.transform;
		Int32 i = 0;
		Int32 childCount = transform.childCount;
		while (i < childCount)
		{
			Transform child = transform.GetChild(i);
			SetLayer(child.gameObject, layer);
			i++;
		}
	}

	public static Vector3 Round(Vector3 v)
	{
		v.x = Mathf.Round(v.x);
		v.y = Mathf.Round(v.y);
		v.z = Mathf.Round(v.z);
		return v;
	}

	public static void MakePixelPerfect(Transform t)
	{
		UIWidget component = t.GetComponent<UIWidget>();
		if (component != null)
		{
			component.MakePixelPerfect();
		}
		else
		{
			t.localPosition = Round(t.localPosition);
			t.localScale = Round(t.localScale);
			Int32 i = 0;
			Int32 childCount = t.childCount;
			while (i < childCount)
			{
				MakePixelPerfect(t.GetChild(i));
				i++;
			}
		}
	}

	public static Boolean Save(String fileName, Byte[] bytes)
	{
		if (!fileAccess)
		{
			return false;
		}
		String path = Application.persistentDataPath + "/" + fileName;
		if (bytes == null)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			return true;
		}
		FileStream fileStream = null;
		try
		{
			fileStream = File.Create(path);
		}
		catch (Exception ex)
		{
			NGUIDebug.Log(ex.Message);
			return false;
		}
		fileStream.Write(bytes, 0, bytes.Length);
		fileStream.Close();
		return true;
	}

	public static Byte[] Load(String fileName)
	{
		if (!fileAccess)
		{
			return null;
		}
		String path = Application.persistentDataPath + "/" + fileName;
		if (File.Exists(path))
		{
			return File.ReadAllBytes(path);
		}
		return null;
	}

	public static Color ApplyPMA(Color c)
	{
		if (c.a != 1f)
		{
			c.r *= c.a;
			c.g *= c.a;
			c.b *= c.a;
		}
		return c;
	}

	public static void MarkParentAsChanged(GameObject go)
	{
		UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
		Int32 i = 0;
		Int32 num = componentsInChildren.Length;
		while (i < num)
		{
			componentsInChildren[i].ParentHasChanged();
			i++;
		}
	}

	private static PropertyInfo GetSystemCopyBufferProperty()
	{
		if (mSystemCopyBuffer == null)
		{
			Type typeFromHandle = typeof(GUIUtility);
			mSystemCopyBuffer = typeFromHandle.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
		}
		return mSystemCopyBuffer;
	}

	public static String clipboard
	{
		get
		{
			PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
			return (systemCopyBufferProperty == null) ? null : ((String)systemCopyBufferProperty.GetValue(null, null));
		}
		set
		{
			PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
			if (systemCopyBufferProperty != null)
			{
				systemCopyBufferProperty.SetValue(null, value, null);
			}
		}
	}
}
