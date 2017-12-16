using System;
using System.Threading;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Input (Basic)")]
public class UIInput : MonoBehaviour
{
	public static UIInput current;

	public UILabel label;

	public Int32 maxChars;

	public String caratChar = "|";

	public Validator validator;

	public KeyboardType type;

	public Boolean isPassword;

	public Boolean autoCorrect;

	public Boolean useLabelTextAtStart;

	public Color activeColor = Color.white;

	public GameObject selectOnTab;

	public GameObject eventReceiver;

	public String functionName = "OnSubmit";

	private String mText = String.Empty;

	private String mDefaultText = String.Empty;

	private Color mDefaultColor = Color.white;

	private UIWidget.Pivot mPivot = UIWidget.Pivot.Left;

	private Single mPosition;

	private String mLastIME = String.Empty;

	private Boolean mDoInit = true;

    public event OnSubmit onSubmit;

    public event OnSubmit onInputChanged;

	public virtual String text
	{
		get
		{
			if (mDoInit)
			{
				Init();
			}
			return mText;
		}
		set
		{
			if (mDoInit)
			{
				Init();
			}
			mText = value;
			if (label != null)
			{
				if (String.IsNullOrEmpty(value))
				{
					value = mDefaultText;
				}
				label.supportEncoding = false;
				label.text = ((!selected) ? value : (value + caratChar));
				label.showLastPasswordChar = selected;
				label.color = ((!selected && !(value != mDefaultText)) ? mDefaultColor : activeColor);
			}
		}
	}

	public Boolean selected
	{
		get => UICamera.selectedObject == gameObject;
	    set
		{
			if (!value && UICamera.selectedObject == gameObject)
			{
				UICamera.selectedObject = null;
			}
			else if (value)
			{
				UICamera.selectedObject = gameObject;
			}
		}
	}

	public String defaultText
	{
		get => mDefaultText;
	    set
		{
			if (label.text == mDefaultText)
			{
				label.text = value;
			}
			mDefaultText = value;
		}
	}

	protected void Init()
	{
		if (mDoInit)
		{
			mDoInit = false;
			if (label == null)
			{
				label = GetComponentInChildren<UILabel>();
			}
			if (label != null)
			{
				if (useLabelTextAtStart)
				{
					mText = label.text;
				}
				mDefaultText = label.text;
				mDefaultColor = label.color;
				label.supportEncoding = false;
				label.password = isPassword;
				mPivot = label.pivot;
				mPosition = label.cachedTransform.localPosition.x;
			}
			else
			{
				enabled = false;
			}
		}
	}

	private void OnEnable()
	{
		if (UICamera.IsHighlighted(gameObject))
		{
			OnSelect(true);
		}
	}

	private void OnDisable()
	{
		if (UICamera.IsHighlighted(gameObject))
		{
			OnSelect(false);
		}
	}

	private void OnSelect(Boolean isSelected)
	{
		if (mDoInit)
		{
			Init();
		}
		if (label != null && enabled && NGUITools.GetActive(gameObject))
		{
			if (isSelected)
			{
				mText = ((useLabelTextAtStart || !(label.text == mDefaultText)) ? label.text : String.Empty);
				label.color = activeColor;
				if (isPassword)
				{
					label.password = true;
				}
				Input.imeCompositionMode = IMECompositionMode.On;
				Transform cachedTransform = label.cachedTransform;
				Vector3 position = label.pivotOffset;
				position.y += label.relativeSize.y;
				position = cachedTransform.TransformPoint(position);
				Input.compositionCursorPos = UICamera.currentCamera.WorldToScreenPoint(position);
				UpdateLabel();
			}
			else
			{
				if (String.IsNullOrEmpty(mText))
				{
					label.text = mDefaultText;
					label.color = mDefaultColor;
					if (isPassword)
					{
						label.password = false;
					}
				}
				else
				{
					label.text = mText;
				}
				label.showLastPasswordChar = false;
				Input.imeCompositionMode = IMECompositionMode.Off;
				RestoreLabel();
			}
		}
	}

	private void Update()
	{
		if (selected)
		{
			if (selectOnTab != null && Input.GetKeyDown(KeyCode.Tab))
			{
				UICamera.selectedObject = selectOnTab;
			}
			if (Input.GetKeyDown(KeyCode.V) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
			{
				Append(NGUITools.clipboard);
			}
			if (mLastIME != Input.compositionString)
			{
				mLastIME = Input.compositionString;
				UpdateLabel();
			}
		}
	}

	private void OnInput(String input)
	{
		if (mDoInit)
		{
			Init();
		}
		if (selected && enabled && NGUITools.GetActive(gameObject))
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return;
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return;
			}
			Append(input);
		}
	}

	private void Append(String input)
	{
		mText = ((mText != null) ? mText : String.Empty);
		Int32 i = 0;
		Int32 length = input.Length;
		while (i < length)
		{
			Char c = input[i];
			if (c == '\b')
			{
				if (mText.Length > 0)
				{
					mText = mText.Substring(0, mText.Length - 1);
					if (onInputChanged != null)
					{
						onInputChanged(mText);
					}
					SendMessage("OnInputChanged", this, SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (c == '\r' || c == '\n')
			{
				if ((UICamera.current.submitKey0 == KeyCode.Return || UICamera.current.submitKey1 == KeyCode.Return) && (!label.multiLine || (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))))
				{
					current = this;
					if (onSubmit != null)
					{
						onSubmit(mText);
					}
					if (eventReceiver == null)
					{
						eventReceiver = gameObject;
					}
					eventReceiver.SendMessage(functionName, mText, SendMessageOptions.DontRequireReceiver);
					current = null;
					selected = false;
					return;
				}
				if (validator != null)
				{
					c = validator(mText, c);
				}
				if (c != '\0')
				{
					if (c == '\n' || c == '\r')
					{
						if (label.multiLine)
						{
							mText += "\n";
						}
					}
					else
					{
						mText += c;
					}
					if (onInputChanged != null)
					{
						onInputChanged(mText);
					}
					SendMessage("OnInputChanged", this, SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (c >= ' ')
			{
				if (validator != null)
				{
					c = validator(mText, c);
				}
				if (c != '\0')
				{
					mText += c;
					if (onInputChanged != null)
					{
						onInputChanged(mText);
					}
					SendMessage("OnInputChanged", this, SendMessageOptions.DontRequireReceiver);
				}
			}
			i++;
		}
		UpdateLabel();
	}

	private void UpdateLabel()
	{
		if (mDoInit)
		{
			Init();
		}
		if (maxChars > 0 && mText.Length > maxChars)
		{
			mText = mText.Substring(0, maxChars);
		}
		if (label.font != null)
		{
			String text;
			if (isPassword && selected)
			{
				text = String.Empty;
				Int32 i = 0;
				Int32 length = mText.Length;
				while (i < length)
				{
					text += "*";
					i++;
				}
				text = text + Input.compositionString + caratChar;
			}
			else
			{
				text = ((!selected) ? mText : (mText + Input.compositionString + caratChar));
			}
			label.supportEncoding = false;
			if (!label.shrinkToFit)
			{
				if (label.multiLine)
				{
					text = label.font.WrapText(text, label.lineWidth / label.cachedTransform.localScale.x, 0, false, UIFont.SymbolStyle.None);
				}
				else
				{
					String endOfLineThatFits = label.font.GetEndOfLineThatFits(text, label.lineWidth / label.cachedTransform.localScale.x, false, UIFont.SymbolStyle.None);
					if (endOfLineThatFits != text)
					{
						text = endOfLineThatFits;
						Vector3 localPosition = label.cachedTransform.localPosition;
						localPosition.x = mPosition + label.lineWidth;
						if (mPivot == UIWidget.Pivot.Left)
						{
							label.pivot = UIWidget.Pivot.Right;
						}
						else if (mPivot == UIWidget.Pivot.TopLeft)
						{
							label.pivot = UIWidget.Pivot.TopRight;
						}
						else if (mPivot == UIWidget.Pivot.BottomLeft)
						{
							label.pivot = UIWidget.Pivot.BottomRight;
						}
						label.cachedTransform.localPosition = localPosition;
					}
					else
					{
						RestoreLabel();
					}
				}
			}
			label.text = text;
			label.showLastPasswordChar = selected;
		}
	}

	private void RestoreLabel()
	{
		if (label != null)
		{
			label.pivot = mPivot;
			Vector3 localPosition = label.cachedTransform.localPosition;
			localPosition.x = mPosition;
			label.cachedTransform.localPosition = localPosition;
		}
	}

	public delegate Char Validator(String currentText, Char nextChar);

	public enum KeyboardType
	{
		Default,
		ASCIICapable,
		NumbersAndPunctuation,
		URL,
		NumberPad,
		PhonePad,
		NamePhonePad,
		EmailAddress
	}

	public delegate void OnSubmit(String inputString);
}
