using System;
using System.Collections.Generic;
using Legacy;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Popup List")]
[ExecuteInEditMode]
public class UIPopupList : MonoBehaviour
{
	private const Single animSpeed = 0.15f;

	public static UIPopupList current;

	public UIAtlas atlas;

	public UIFont font;

	public UILabel textLabel;

	public String backgroundSprite;

	public String highlightSprite;

	public Position position;

	public List<String> items = new List<String>();

	public Vector2 padding = new Vector3(4f, 4f);

	public Single textScale = 1f;

	public Color textColor = Color.white;

	public Color backgroundColor = Color.white;

	public Color highlightColor = new Color(0.596078455f, 1f, 0.2f, 1f);

	public Boolean isAnimated = true;

	public Boolean isLocalized;

	public Boolean isForcingDepth;

	public Int32 forcedDepth;

	public Int32 layer;

	public GameObject eventReceiver;

	public String functionName = "OnSelectionChange";

	public OnSelectionChange onSelectionChange;

	[SerializeField]
	[HideInInspector]
	private String mSelectedItem;

	private UIPanel mPanel;

	private GameObject mChild;

	private UISprite mBackground;

	private UISprite mHighlight;

	private UILabel mHighlightedLabel;

	private List<UILabel> mLabelList = new List<UILabel>();

	private Single mBgBorder;

	public Boolean isOpen => mChild != null;

    public String selection
	{
		get => mSelectedItem;
        set
		{
			if (mSelectedItem != value)
			{
				mSelectedItem = value;
				if (textLabel != null)
				{
					textLabel.text = ((!isLocalized) ? value : LocaManager.GetText(value));
				}
				current = this;
				if (onSelectionChange != null)
				{
					onSelectionChange(mSelectedItem);
				}
				if (eventReceiver != null && !String.IsNullOrEmpty(functionName) && Application.isPlaying)
				{
					eventReceiver.SendMessage(functionName, mSelectedItem, SendMessageOptions.DontRequireReceiver);
				}
				current = null;
				if (textLabel == null)
				{
					mSelectedItem = null;
				}
			}
		}
	}

	private Boolean handleEvents
	{
		get
		{
			UIButtonKeys component = GetComponent<UIButtonKeys>();
			return component == null || !component.enabled;
		}
		set
		{
			UIButtonKeys component = GetComponent<UIButtonKeys>();
			if (component != null)
			{
				component.enabled = !value;
			}
		}
	}

	private void Start()
	{
		if (textLabel != null)
		{
			if (String.IsNullOrEmpty(mSelectedItem))
			{
				if (items.Count > 0)
				{
					selection = items[0];
				}
			}
			else
			{
				String selection = mSelectedItem;
				mSelectedItem = null;
				this.selection = selection;
			}
		}
	}

	private void OnLocalize()
	{
		if (isLocalized && textLabel != null)
		{
			textLabel.text = LocaManager.GetText(mSelectedItem);
		}
	}

	private void Highlight(UILabel lbl, Boolean instant)
	{
		if (mHighlight != null)
		{
			TweenPosition component = lbl.GetComponent<TweenPosition>();
			if (component != null && component.enabled)
			{
				return;
			}
			mHighlightedLabel = lbl;
			UIAtlas.Sprite atlasSprite = mHighlight.GetAtlasSprite();
			if (atlasSprite == null)
			{
				return;
			}
			Single num = atlasSprite.inner.xMin - atlasSprite.outer.xMin;
			Single y = atlasSprite.inner.yMin - atlasSprite.outer.yMin;
			Vector3 vector = lbl.cachedTransform.localPosition + new Vector3(-num, y, 1f);
			if (instant || !isAnimated)
			{
				mHighlight.cachedTransform.localPosition = vector;
			}
			else
			{
				TweenPosition.Begin(mHighlight.gameObject, 0.1f, vector).method = UITweener.Method.EaseOut;
			}
		}
	}

	private void OnItemHover(GameObject go, Boolean isOver)
	{
		if (isOver)
		{
			UILabel component = go.GetComponent<UILabel>();
			Highlight(component, false);
		}
	}

	private void Select(UILabel lbl, Boolean instant)
	{
		Highlight(lbl, instant);
		UIEventListener component = lbl.gameObject.GetComponent<UIEventListener>();
		selection = (component.parameter as String);
		UIButtonSound[] components = GetComponents<UIButtonSound>();
		Int32 i = 0;
		Int32 num = components.Length;
		while (i < num)
		{
			UIButtonSound uibuttonSound = components[i];
			if (uibuttonSound.trigger == UIButtonSound.Trigger.OnClick)
			{
				NGUITools.PlaySound(uibuttonSound.audioClip, uibuttonSound.volume, 1f);
			}
			i++;
		}
	}

	private void OnItemPress(GameObject go, Boolean isPressed)
	{
		if (isPressed)
		{
			Select(go.GetComponent<UILabel>(), true);
		}
	}

	private void OnKey(KeyCode key)
	{
		if (enabled && NGUITools.GetActive(gameObject) && handleEvents)
		{
			Int32 num = mLabelList.IndexOf(mHighlightedLabel);
			if (key == KeyCode.UpArrow)
			{
				if (num > 0)
				{
					Select(mLabelList[num - 1], false);
				}
			}
			else if (key == KeyCode.DownArrow)
			{
				if (num + 1 < mLabelList.Count)
				{
					Select(mLabelList[num + 1], false);
				}
			}
			else if (key == KeyCode.Escape)
			{
				OnSelect(false);
			}
		}
	}

	private void OnSelect(Boolean isSelected)
	{
		if (!isSelected && mChild != null)
		{
			mLabelList.Clear();
			handleEvents = false;
			if (isAnimated)
			{
				UIWidget[] componentsInChildren = mChild.GetComponentsInChildren<UIWidget>();
				Int32 i = 0;
				Int32 num = componentsInChildren.Length;
				while (i < num)
				{
					UIWidget uiwidget = componentsInChildren[i];
					Color color = uiwidget.color;
					color.a = 0f;
					TweenColor.Begin(uiwidget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
					i++;
				}
				Collider[] componentsInChildren2 = mChild.GetComponentsInChildren<Collider>();
				Int32 j = 0;
				Int32 num2 = componentsInChildren2.Length;
				while (j < num2)
				{
					componentsInChildren2[j].enabled = false;
					j++;
				}
				Destroy(mChild, 0.15f);
			}
			else
			{
				Destroy(mChild);
			}
			mBackground = null;
			mHighlight = null;
			mChild = null;
		}
	}

	private void AnimateColor(UIWidget widget)
	{
		Color color = widget.color;
		widget.color = new Color(color.r, color.g, color.b, 0f);
		TweenColor.Begin(widget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
	}

	private void AnimatePosition(UIWidget widget, Boolean placeAbove, Single bottom)
	{
		Vector3 localPosition = widget.cachedTransform.localPosition;
		Vector3 localPosition2 = (!placeAbove) ? new Vector3(localPosition.x, 0f, localPosition.z) : new Vector3(localPosition.x, bottom, localPosition.z);
		widget.cachedTransform.localPosition = localPosition2;
		GameObject gameObject = widget.gameObject;
		TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
	}

	private void AnimateScale(UIWidget widget, Boolean placeAbove, Single bottom)
	{
		GameObject gameObject = widget.gameObject;
		Transform cachedTransform = widget.cachedTransform;
		Single num = font.size * textScale + mBgBorder * 2f;
		Vector3 localScale = cachedTransform.localScale;
		cachedTransform.localScale = new Vector3(localScale.x, num, localScale.z);
		TweenScale.Begin(gameObject, 0.15f, localScale).method = UITweener.Method.EaseOut;
		if (placeAbove)
		{
			Vector3 localPosition = cachedTransform.localPosition;
			cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y - localScale.y + num, localPosition.z);
			TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
		}
	}

	private void Animate(UIWidget widget, Boolean placeAbove, Single bottom)
	{
		AnimateColor(widget);
		AnimatePosition(widget, placeAbove, bottom);
	}

	private void OnClick()
	{
		if (mChild == null && atlas != null && font != null && items.Count > 0)
		{
			mLabelList.Clear();
			handleEvents = true;
			if (mPanel == null)
			{
				mPanel = UIPanel.Find(this.transform, true);
			}
			Transform transform = this.transform;
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform.parent, transform);
			mChild = new GameObject("Drop-down List");
			mChild.layer = gameObject.layer;
			Transform transform2 = mChild.transform;
			transform2.parent = transform.parent;
			transform2.localPosition = bounds.min;
			transform2.localRotation = Quaternion.identity;
			transform2.localScale = Vector3.one;
			mBackground = NGUITools.AddSprite(mChild, atlas, backgroundSprite);
			mBackground.pivot = UIWidget.Pivot.TopLeft;
			mBackground.layer = layer;
			if (isForcingDepth)
			{
				mBackground.depth = forcedDepth;
			}
			else
			{
				mBackground.depth = NGUITools.CalculateNextDepth(mPanel.gameObject);
			}
			mBackground.color = backgroundColor;
			Vector4 border = mBackground.border;
			mBgBorder = border.y;
			mBackground.cachedTransform.localPosition = new Vector3(0f, border.y, 0f);
			mHighlight = NGUITools.AddSprite(mChild, atlas, highlightSprite);
			mHighlight.pivot = UIWidget.Pivot.TopLeft;
			mHighlight.color = highlightColor;
			mHighlight.layer = layer;
			UIAtlas.Sprite atlasSprite = mHighlight.GetAtlasSprite();
			if (atlasSprite == null)
			{
				return;
			}
			Single num = atlasSprite.inner.yMin - atlasSprite.outer.yMin;
			Single num2 = font.size * font.pixelSize * textScale;
			Single num3 = 0f;
			Single num4 = -padding.y;
			List<UILabel> list = new List<UILabel>();
			Int32 i = 0;
			Int32 count = items.Count;
			while (i < count)
			{
				String text = items[i];
				UILabel uilabel = NGUITools.AddWidget<UILabel>(mChild);
				uilabel.pivot = UIWidget.Pivot.TopLeft;
				uilabel.font = font;
				uilabel.text = ((!isLocalized) ? text : LocaManager.GetText(text));
				uilabel.color = textColor;
				uilabel.cachedTransform.localPosition = new Vector3(border.x + padding.x, num4, -1f);
				uilabel.MakePixelPerfect();
				if (textScale != 1f)
				{
					Vector3 localScale = uilabel.cachedTransform.localScale;
					uilabel.cachedTransform.localScale = localScale * textScale;
				}
				uilabel.layer = layer;
				list.Add(uilabel);
				num4 -= num2;
				num4 -= padding.y;
				num3 = Mathf.Max(num3, uilabel.relativeSize.x * num2);
				UIEventListener uieventListener = UIEventListener.Get(uilabel.gameObject);
				uieventListener.onHover = new UIEventListener.BoolDelegate(OnItemHover);
				uieventListener.onPress = new UIEventListener.BoolDelegate(OnItemPress);
				uieventListener.parameter = text;
				if (mSelectedItem == text)
				{
					Highlight(uilabel, true);
				}
				mLabelList.Add(uilabel);
				i++;
			}
			num3 = Mathf.Max(num3, bounds.size.x - (border.x + padding.x) * 2f);
			Vector3 center = new Vector3(num3 * 0.5f / num2, -0.5f, 0f);
			Vector3 size = new Vector3(num3 / num2, (num2 + padding.y) / num2, 1f);
			Int32 j = 0;
			Int32 count2 = list.Count;
			while (j < count2)
			{
				UILabel uilabel2 = list[j];
				BoxCollider boxCollider = NGUITools.AddWidgetCollider(uilabel2.gameObject);
				center.z = boxCollider.center.z;
				boxCollider.center = center;
				boxCollider.size = size;
				j++;
			}
			num3 += (border.x + padding.x) * 2f;
			num4 -= border.y;
			mBackground.cachedTransform.localScale = new Vector3(num3, -num4 + border.y, 1f);
			mHighlight.cachedTransform.localScale = new Vector3(num3 - (border.x + padding.x) * 2f + (atlasSprite.inner.xMin - atlasSprite.outer.xMin) * 2f, num2 + num * 2f, 1f);
			Boolean flag = position == Position.Above;
			if (position == Position.Auto)
			{
				UICamera uicamera = UICamera.FindCameraForLayer(gameObject.layer);
				if (uicamera != null)
				{
					flag = (uicamera.cachedCamera.WorldToViewportPoint(transform.position).y < 0.5f);
				}
			}
			if (isAnimated)
			{
				Single bottom = num4 + num2;
				Animate(mHighlight, flag, bottom);
				Int32 k = 0;
				Int32 count3 = list.Count;
				while (k < count3)
				{
					Animate(list[k], flag, bottom);
					k++;
				}
				AnimateColor(mBackground);
				AnimateScale(mBackground, flag, bottom);
			}
			if (flag)
			{
				transform2.localPosition = new Vector3(bounds.min.x, bounds.max.y - num4 - border.y, bounds.min.z);
			}
		}
		else
		{
			OnSelect(false);
		}
	}

	public enum Position
	{
		Auto,
		Above,
		Below
	}

	public delegate void OnSelectionChange(String item);
}
