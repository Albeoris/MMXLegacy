using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("NGUI/UI/Camera")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class UICamera : MonoBehaviour
{
	public Boolean debug;

	public Boolean useMouse = true;

	public Boolean useTouch = true;

	public Boolean allowMultiTouch = true;

	public Boolean useKeyboard = true;

	public Boolean useController = true;

	public Boolean stickyPress = true;

	public LayerMask eventReceiverMask = -1;

	public Boolean clipRaycasts = true;

	public Single tooltipDelay = 1f;

	public Boolean stickyTooltip = true;

	public Single mouseDragThreshold = 4f;

	public Single mouseClickThreshold = 10f;

	public Single touchDragThreshold = 40f;

	public Single touchClickThreshold = 40f;

	public Single rangeDistance = -1f;

	public String scrollAxisName = "Mouse ScrollWheel";

	public String verticalAxisName = "Vertical";

	public String horizontalAxisName = "Horizontal";

	public KeyCode submitKey0 = KeyCode.Return;

	public KeyCode submitKey1 = KeyCode.JoystickButton0;

	public KeyCode cancelKey0 = KeyCode.Escape;

	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	public static OnCustomInput onCustomInput;

	public static Boolean showTooltips = true;

	public static Vector2 lastTouchPosition = Vector2.zero;

	public static RaycastHit lastHit;

	public static UICamera current = null;

	public static Camera currentCamera = null;

	public static Int32 currentTouchID = -1;

	public static MouseOrTouch currentTouch = null;

	public static Boolean inputHasFocus = false;

	public static GameObject genericEventHandler;

	public static GameObject fallThrough;

	private static List<UICamera> mList = new List<UICamera>();

	private static List<Highlighted> mHighlighted = new List<Highlighted>();

	private static GameObject mSel = null;

	private static MouseOrTouch[] mMouse = new MouseOrTouch[]
	{
		new MouseOrTouch(),
		new MouseOrTouch(),
		new MouseOrTouch()
	};

	private static GameObject mHover;

	private static MouseOrTouch mController = new MouseOrTouch();

	private static Single mNextEvent = 0f;

	private static Dictionary<Int32, MouseOrTouch> mTouches = new Dictionary<Int32, MouseOrTouch>();

	private GameObject mTooltip;

	private Camera mCam;

	private Single mTooltipTime;

	private Boolean mIsEditor;

	public static Boolean isDragging = false;

	public static GameObject hoveredObject;

	private GameObject mDragHover;

	private Boolean handlesEvents => eventHandler == this;

    public Camera cachedCamera
	{
		get
		{
			if (mCam == null)
			{
				mCam = camera;
			}
			return mCam;
		}
	}

	public static GameObject selectedObject
	{
		get => mSel;
	    set
		{
			if (mSel != value)
			{
				if (mSel != null)
				{
					UICamera uicamera = FindCameraForLayer(mSel.layer);
					if (uicamera != null)
					{
						current = uicamera;
						currentCamera = uicamera.mCam;
						Notify(mSel, "OnSelect", false);
						if (uicamera.useController || uicamera.useKeyboard)
						{
							Highlight(mSel, false);
						}
						current = null;
					}
				}
				mSel = value;
				if (mSel != null)
				{
					UICamera uicamera2 = FindCameraForLayer(mSel.layer);
					if (uicamera2 != null)
					{
						current = uicamera2;
						currentCamera = uicamera2.mCam;
						if (uicamera2.useController || uicamera2.useKeyboard)
						{
							Highlight(mSel, true);
						}
						Notify(mSel, "OnSelect", true);
						current = null;
					}
				}
			}
		}
	}

	public static Int32 touchCount
	{
		get
		{
			Int32 num = 0;
			for (Int32 i = 0; i < mTouches.Count; i++)
			{
				if (mTouches[i].pressed != null)
				{
					num++;
				}
			}
			for (Int32 j = 0; j < mMouse.Length; j++)
			{
				if (mMouse[j].pressed != null)
				{
					num++;
				}
			}
			if (mController.pressed != null)
			{
				num++;
			}
			return num;
		}
	}

	public static Int32 dragCount
	{
		get
		{
			Int32 num = 0;
			for (Int32 i = 0; i < mTouches.Count; i++)
			{
				if (mTouches[i].dragged != null)
				{
					num++;
				}
			}
			for (Int32 j = 0; j < mMouse.Length; j++)
			{
				if (mMouse[j].dragged != null)
				{
					num++;
				}
			}
			if (mController.dragged != null)
			{
				num++;
			}
			return num;
		}
	}

	private void OnApplicationQuit()
	{
		mHighlighted.Clear();
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera eventHandler = UICamera.eventHandler;
			return (!(eventHandler != null)) ? null : eventHandler.cachedCamera;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (Int32 i = 0; i < mList.Count; i++)
			{
				UICamera uicamera = mList[i];
				if (!(uicamera == null) && uicamera.enabled && NGUITools.GetActive(uicamera.gameObject))
				{
					return uicamera;
				}
			}
			return null;
		}
	}

	private static Int32 CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	public static Boolean Raycast(Vector3 inPos, ref RaycastHit hit)
	{
		for (Int32 i = 0; i < mList.Count; i++)
		{
			UICamera uicamera = mList[i];
			if (uicamera.enabled && NGUITools.GetActive(uicamera.gameObject))
			{
				currentCamera = uicamera.cachedCamera;
				Vector3 vector = currentCamera.ScreenToViewportPoint(inPos);
				if (vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f)
				{
					Ray ray = currentCamera.ScreenPointToRay(inPos);
					Int32 layerMask = currentCamera.cullingMask & uicamera.eventReceiverMask;
					Single distance = (uicamera.rangeDistance <= 0f) ? (currentCamera.farClipPlane - currentCamera.nearClipPlane) : uicamera.rangeDistance;
					if (uicamera.clipRaycasts)
					{
						RaycastHit[] array = Physics.RaycastAll(ray, distance, layerMask);
						if (array.Length > 1)
						{
							Array.Sort<RaycastHit>(array, (RaycastHit r1, RaycastHit r2) => r1.distance.CompareTo(r2.distance));
							Int32 j = 0;
							Int32 num = array.Length;
							while (j < num)
							{
								if (IsVisible(ref array[j]))
								{
									hit = array[j];
									return true;
								}
								j++;
							}
						}
						else if (array.Length == 1 && IsVisible(ref array[0]))
						{
							hit = array[0];
							return true;
						}
					}
					else if (Physics.Raycast(ray, out hit, distance, layerMask))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private static Boolean IsVisible(ref RaycastHit hit)
	{
		UIPanel uipanel = NGUITools.FindInParents<UIPanel>(hit.collider.gameObject);
		return uipanel == null || uipanel.IsVisible(hit.point);
	}

	public static UICamera FindCameraForLayer(Int32 layer)
	{
		Int32 num = 1 << layer;
		for (Int32 i = 0; i < mList.Count; i++)
		{
			UICamera uicamera = mList[i];
			Camera cachedCamera = uicamera.cachedCamera;
			if (cachedCamera != null && (cachedCamera.cullingMask & num) != 0)
			{
				return uicamera;
			}
		}
		return null;
	}

	private static Int32 GetDirection(KeyCode up, KeyCode down)
	{
		if (Input.GetKeyDown(up))
		{
			return 1;
		}
		if (Input.GetKeyDown(down))
		{
			return -1;
		}
		return 0;
	}

	private static Int32 GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (Input.GetKeyDown(up0) || Input.GetKeyDown(up1))
		{
			return 1;
		}
		if (Input.GetKeyDown(down0) || Input.GetKeyDown(down1))
		{
			return -1;
		}
		return 0;
	}

	private static Int32 GetDirection(String axis)
	{
		Single realtimeSinceStartup = Time.realtimeSinceStartup;
		if (mNextEvent < realtimeSinceStartup)
		{
			Single axis2 = Input.GetAxis(axis);
			if (axis2 > 0.75f)
			{
				mNextEvent = realtimeSinceStartup + 0.25f;
				return 1;
			}
			if (axis2 < -0.75f)
			{
				mNextEvent = realtimeSinceStartup + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	public static Boolean IsHighlighted(GameObject go)
	{
		Int32 i = mHighlighted.Count;
		while (i > 0)
		{
			Highlighted highlighted = mHighlighted[--i];
			if (highlighted.go == go)
			{
				return true;
			}
		}
		return false;
	}

	private static void Highlight(GameObject go, Boolean highlighted)
	{
		if (go != null)
		{
			Int32 i = mHighlighted.Count;
			while (i > 0)
			{
				Highlighted highlighted2 = mHighlighted[--i];
				if (highlighted2 == null || highlighted2.go == null)
				{
					mHighlighted.RemoveAt(i);
				}
				else if (highlighted2.go == go)
				{
					if (highlighted)
					{
						highlighted2.counter++;
					}
					else if (--highlighted2.counter < 1)
					{
						mHighlighted.Remove(highlighted2);
						Notify(go, "OnHover", false);
					}
					return;
				}
			}
			if (highlighted)
			{
				Highlighted highlighted3 = new Highlighted();
				highlighted3.go = go;
				highlighted3.counter = 1;
				mHighlighted.Add(highlighted3);
				Notify(go, "OnHover", true);
			}
		}
	}

	public static void Notify(GameObject go, String funcName, Object obj)
	{
		if (go != null)
		{
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			if (genericEventHandler != null && genericEventHandler != go)
			{
				genericEventHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static MouseOrTouch GetTouch(Int32 id)
	{
		MouseOrTouch mouseOrTouch = null;
		if (!mTouches.TryGetValue(id, out mouseOrTouch))
		{
			mouseOrTouch = new MouseOrTouch();
			mouseOrTouch.touchBegan = true;
			mTouches.Add(id, mouseOrTouch);
		}
		return mouseOrTouch;
	}

	public static void RemoveTouch(Int32 id)
	{
		mTouches.Remove(id);
	}

	private void Awake()
	{
		cachedCamera.eventMask = 0;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			useMouse = false;
			useTouch = true;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				useKeyboard = false;
				useController = false;
			}
		}
		else if (Application.platform == RuntimePlatform.PS3 || Application.platform == RuntimePlatform.XBOX360)
		{
			useMouse = false;
			useTouch = false;
			useKeyboard = false;
			useController = true;
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			mIsEditor = true;
		}
		mMouse[0].pos.x = Input.mousePosition.x;
		mMouse[0].pos.y = Input.mousePosition.y;
		lastTouchPosition = mMouse[0].pos;
		if (eventReceiverMask == -1)
		{
			eventReceiverMask = cachedCamera.cullingMask;
		}
	}

	private void Start()
	{
		mList.Add(this);
		mList.Sort(new Comparison<UICamera>(CompareFunc));
	}

	private void OnDestroy()
	{
		mList.Remove(this);
	}

	private void FixedUpdate()
	{
		if (useMouse && Application.isPlaying && handlesEvents)
		{
			hoveredObject = ((!Raycast(Input.mousePosition, ref lastHit)) ? fallThrough : lastHit.collider.gameObject);
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			for (Int32 i = 0; i < 3; i++)
			{
				mMouse[i].current = hoveredObject;
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying || !handlesEvents)
		{
			return;
		}
		current = this;
		if (useMouse || (useTouch && mIsEditor))
		{
			ProcessMouse();
		}
		if (useTouch)
		{
			ProcessTouches();
		}
		if (onCustomInput != null)
		{
			onCustomInput();
		}
		if (useMouse && mSel != null && ((cancelKey0 != KeyCode.None && Input.GetKeyDown(cancelKey0)) || (cancelKey1 != KeyCode.None && Input.GetKeyDown(cancelKey1))))
		{
			selectedObject = null;
		}
		if (mSel != null)
		{
			String text = Input.inputString;
			if (useKeyboard && Input.GetKeyDown(KeyCode.Delete))
			{
				text += "\b";
			}
			if (text.Length > 0)
			{
				if (!stickyTooltip && mTooltip != null)
				{
					ShowTooltip(false);
				}
				Notify(mSel, "OnInput", text);
			}
		}
		else
		{
			inputHasFocus = false;
		}
		if (mSel != null)
		{
			ProcessOthers();
		}
		if (useMouse && mHover != null)
		{
			Single axis = Input.GetAxis(scrollAxisName);
			if (axis != 0f)
			{
				Notify(mHover, "OnScroll", axis);
			}
			if (showTooltips && mTooltipTime != 0f && (mTooltipTime < Time.realtimeSinceStartup || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				mTooltip = mHover;
				ShowTooltip(true);
			}
		}
		current = null;
	}

	public void ProcessMouse()
	{
		Boolean flag = useMouse && Time.timeScale < 0.9f;
		if (!flag)
		{
			for (Int32 i = 0; i < 3; i++)
			{
				if (Input.GetMouseButton(i) || Input.GetMouseButtonUp(i))
				{
					flag = true;
					break;
				}
			}
		}
		mMouse[0].pos = Input.mousePosition;
		mMouse[0].delta = mMouse[0].pos - lastTouchPosition;
		Boolean flag2 = mMouse[0].pos != lastTouchPosition;
		lastTouchPosition = mMouse[0].pos;
		if (flag)
		{
			hoveredObject = ((!Raycast(Input.mousePosition, ref lastHit)) ? fallThrough : lastHit.collider.gameObject);
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			mMouse[0].current = hoveredObject;
		}
		for (Int32 j = 1; j < 3; j++)
		{
			mMouse[j].pos = mMouse[0].pos;
			mMouse[j].delta = mMouse[0].delta;
			mMouse[j].current = mMouse[0].current;
		}
		Boolean flag3 = false;
		for (Int32 k = 0; k < 3; k++)
		{
			if (Input.GetMouseButton(k))
			{
				flag3 = true;
				break;
			}
		}
		if (flag3)
		{
			mTooltipTime = 0f;
		}
		else if (useMouse && flag2 && (!stickyTooltip || mHover != mMouse[0].current))
		{
			if (mTooltipTime != 0f)
			{
				mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			}
			else if (mTooltip != null)
			{
				ShowTooltip(false);
			}
		}
		if (useMouse && !flag3 && mHover != null && mHover != mMouse[0].current)
		{
			if (mTooltip != null)
			{
				ShowTooltip(false);
			}
			Highlight(mHover, false);
			mHover = null;
		}
		if (useMouse && mDragHover != null && mDragHover != mMouse[0].current)
		{
			HighlightDragged(mDragHover, null);
			mDragHover = null;
		}
		if (useMouse)
		{
			for (Int32 l = 0; l < 3; l++)
			{
				Boolean mouseButtonDown = Input.GetMouseButtonDown(l);
				Boolean mouseButtonUp = Input.GetMouseButtonUp(l);
				currentTouch = mMouse[l];
				currentTouchID = -1 - l;
				if (mouseButtonDown)
				{
					currentTouch.pressedCam = currentCamera;
				}
				else if (currentTouch.pressed != null)
				{
					currentCamera = currentTouch.pressedCam;
				}
				ProcessTouch(mouseButtonDown, mouseButtonUp);
			}
			currentTouch = null;
		}
		if (useMouse && !flag3 && mHover != mMouse[0].current)
		{
			mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			mHover = mMouse[0].current;
			Highlight(mHover, true);
		}
		if (useMouse && flag3 && mHover != null && mDragHover != mMouse[0].current)
		{
			mDragHover = mMouse[0].current;
			HighlightDragged(mDragHover, mHover);
		}
	}

	public void ProcessTouches()
	{
		for (Int32 i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			currentTouchID = ((!allowMultiTouch) ? 1 : touch.fingerId);
			currentTouch = GetTouch(currentTouchID);
			Boolean flag = touch.phase == TouchPhase.Began || currentTouch.touchBegan;
			Boolean flag2 = touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended;
			currentTouch.touchBegan = false;
			if (flag)
			{
				currentTouch.delta = Vector2.zero;
			}
			else
			{
				currentTouch.delta = touch.position - currentTouch.pos;
			}
			currentTouch.pos = touch.position;
			hoveredObject = ((!Raycast(currentTouch.pos, ref lastHit)) ? fallThrough : lastHit.collider.gameObject);
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			currentTouch.current = hoveredObject;
			lastTouchPosition = currentTouch.pos;
			if (flag)
			{
				currentTouch.pressedCam = currentCamera;
			}
			else if (currentTouch.pressed != null)
			{
				currentCamera = currentTouch.pressedCam;
			}
			if (touch.tapCount > 1)
			{
				currentTouch.clickTime = Time.realtimeSinceStartup;
			}
			ProcessTouch(flag, flag2);
			if (flag2)
			{
				RemoveTouch(currentTouchID);
			}
			currentTouch = null;
			if (!allowMultiTouch)
			{
				break;
			}
		}
	}

	public void ProcessOthers()
	{
		currentTouchID = -100;
		currentTouch = mController;
		inputHasFocus = (mSel != null && mSel.GetComponent<UIInput>() != null);
		Boolean flag = (submitKey0 != KeyCode.None && Input.GetKeyDown(submitKey0)) || (submitKey1 != KeyCode.None && Input.GetKeyDown(submitKey1));
		Boolean flag2 = (submitKey0 != KeyCode.None && Input.GetKeyUp(submitKey0)) || (submitKey1 != KeyCode.None && Input.GetKeyUp(submitKey1));
		if (flag || flag2)
		{
			currentTouch.current = mSel;
			ProcessTouch(flag, flag2);
			currentTouch.current = null;
		}
		Int32 num = 0;
		Int32 num2 = 0;
		if (useKeyboard)
		{
			if (inputHasFocus)
			{
				num += GetDirection(KeyCode.UpArrow, KeyCode.DownArrow);
				num2 += GetDirection(KeyCode.RightArrow, KeyCode.LeftArrow);
			}
			else
			{
				num += GetDirection(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow);
				num2 += GetDirection(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow);
			}
		}
		if (useController)
		{
			if (!String.IsNullOrEmpty(verticalAxisName))
			{
				num += GetDirection(verticalAxisName);
			}
			if (!String.IsNullOrEmpty(horizontalAxisName))
			{
				num2 += GetDirection(horizontalAxisName);
			}
		}
		if (num != 0)
		{
			Notify(mSel, "OnKey", (num <= 0) ? KeyCode.DownArrow : KeyCode.UpArrow);
		}
		if (num2 != 0)
		{
			Notify(mSel, "OnKey", (num2 <= 0) ? KeyCode.LeftArrow : KeyCode.RightArrow);
		}
		if (useKeyboard && Input.GetKeyDown(KeyCode.Tab))
		{
			Notify(mSel, "OnKey", KeyCode.Tab);
		}
		if (cancelKey0 != KeyCode.None && Input.GetKeyDown(cancelKey0))
		{
			Notify(mSel, "OnKey", KeyCode.Escape);
		}
		if (cancelKey1 != KeyCode.None && Input.GetKeyDown(cancelKey1))
		{
			Notify(mSel, "OnKey", KeyCode.Escape);
		}
		currentTouch = null;
	}

	public void ProcessTouch(Boolean pressed, Boolean unpressed)
	{
		Boolean flag = currentTouch == mMouse[0] || currentTouch == mMouse[1] || currentTouch == mMouse[2];
		Single num = (!flag) ? touchDragThreshold : mouseDragThreshold;
		Single num2 = (!flag) ? touchClickThreshold : mouseClickThreshold;
		if (pressed)
		{
			if (mTooltip != null)
			{
				ShowTooltip(false);
			}
			currentTouch.pressStarted = true;
			Notify(currentTouch.pressed, "OnPress", false);
			currentTouch.pressed = currentTouch.current;
			currentTouch.dragged = currentTouch.current;
			currentTouch.clickNotification = ((!flag) ? ClickNotification.Always : ClickNotification.BasedOnDelta);
			currentTouch.totalDelta = Vector2.zero;
			currentTouch.dragStarted = false;
			Notify(currentTouch.pressed, "OnPress", true);
			if (currentTouch.pressed != mSel)
			{
				if (mTooltip != null)
				{
					ShowTooltip(false);
				}
				selectedObject = null;
			}
		}
		else
		{
			if (currentTouch.clickNotification != ClickNotification.None && !stickyPress && !unpressed && currentTouch.pressStarted && currentTouch.pressed != hoveredObject)
			{
				isDragging = true;
				Notify(currentTouch.pressed, "OnPress", false);
				currentTouch.pressed = hoveredObject;
				Notify(currentTouch.pressed, "OnPress", true);
				isDragging = false;
			}
			if (currentTouch.pressed != null)
			{
				Single magnitude = currentTouch.delta.magnitude;
				if (magnitude != 0f)
				{
					currentTouch.totalDelta += currentTouch.delta;
					magnitude = currentTouch.totalDelta.magnitude;
					if (!currentTouch.dragStarted && num < magnitude)
					{
						currentTouch.dragStarted = true;
						currentTouch.delta = currentTouch.totalDelta;
					}
					if (currentTouch.dragStarted)
					{
						if (mTooltip != null)
						{
							ShowTooltip(false);
						}
						isDragging = true;
						Boolean flag2 = currentTouch.clickNotification == ClickNotification.None;
						Notify(currentTouch.dragged, "OnDrag", currentTouch.delta);
						isDragging = false;
						if (flag2)
						{
							currentTouch.clickNotification = ClickNotification.None;
						}
						else if (currentTouch.clickNotification == ClickNotification.BasedOnDelta && num2 < magnitude)
						{
							currentTouch.clickNotification = ClickNotification.None;
						}
					}
				}
			}
		}
		if (unpressed)
		{
			currentTouch.pressStarted = false;
			if (mTooltip != null)
			{
				ShowTooltip(false);
			}
			if (currentTouch.pressed != null)
			{
				HighlightDragged(mDragHover, null);
				mDragHover = null;
				Notify(currentTouch.pressed, "OnPress", false);
				if (useMouse && currentTouch.pressed == mHover)
				{
					Notify(currentTouch.pressed, "OnHover", true);
				}
				if (currentTouch.dragged == currentTouch.current || (currentTouch.clickNotification != ClickNotification.None && currentTouch.totalDelta.magnitude < num))
				{
					if (currentTouch.pressed != mSel)
					{
						mSel = currentTouch.pressed;
						Notify(currentTouch.pressed, "OnSelect", true);
					}
					else
					{
						mSel = currentTouch.pressed;
					}
					if (currentTouch.clickNotification != ClickNotification.None)
					{
						Single realtimeSinceStartup = Time.realtimeSinceStartup;
						Notify(currentTouch.pressed, "OnClick", null);
						if (currentTouch.clickTime + 0.35f > realtimeSinceStartup && currentTouch.current == currentTouch.lastClicked)
						{
							Notify(currentTouch.pressed, "OnDoubleClick", null);
						}
						currentTouch.clickTime = realtimeSinceStartup;
						currentTouch.lastClicked = currentTouch.current;
					}
				}
				else
				{
					Notify(currentTouch.current, "OnDrop", currentTouch.dragged);
				}
			}
			currentTouch.dragStarted = false;
			currentTouch.pressed = null;
			currentTouch.dragged = null;
		}
	}

	public void ShowTooltip(Boolean val)
	{
		mTooltipTime = 0f;
		Notify(mTooltip, "OnTooltip", val);
		if (!val)
		{
			mTooltip = null;
		}
	}

	private static void HighlightDragged(GameObject receiver, GameObject dragger)
	{
		DragHoverEventArgs obj = DragHoverEventArgs.Empty;
		if (dragger != null)
		{
			obj = new DragHoverEventArgs(dragger, true);
		}
		Notify(receiver, "OnDragHover", obj);
	}

	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta
	}

	public class MouseOrTouch
	{
		public Vector2 pos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public GameObject lastClicked;

		public Single clickTime;

		public ClickNotification clickNotification = ClickNotification.Always;

		public Boolean touchBegan = true;

		public Boolean pressStarted;

		public Boolean dragStarted;
	}

	private class Highlighted
	{
		public GameObject go;

		public Int32 counter;
	}

	public delegate void OnCustomInput();
}
