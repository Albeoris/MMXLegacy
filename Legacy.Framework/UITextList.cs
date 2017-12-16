using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Text List")]
public class UITextList : MonoBehaviour
{
	public Style style;

	public UILabel textLabel;

	public Single maxWidth;

	public Single maxHeight;

	public Int32 maxEntries = 50;

	public Boolean supportScrollWheel = true;

	protected Char[] mSeparator = new Char[]
	{
		'\n'
	};

	protected List<Paragraph> mParagraphs = new List<Paragraph>();

	protected Single mScroll;

	protected Boolean mSelected;

	protected Int32 mTotalLines;

	public void Clear()
	{
		mParagraphs.Clear();
		UpdateVisibleText();
	}

	public void Add(String text)
	{
		Add(text, true);
	}

	protected void Add(String text, Boolean updateVisible)
	{
		Paragraph paragraph;
		if (mParagraphs.Count < maxEntries)
		{
			paragraph = new Paragraph();
		}
		else
		{
			paragraph = mParagraphs[0];
			mParagraphs.RemoveAt(0);
		}
		paragraph.text = text;
		mParagraphs.Add(paragraph);
		if (textLabel != null && textLabel.font != null)
		{
			paragraph.lines = textLabel.font.WrapText(paragraph.text, maxWidth / textLabel.transform.localScale.y, textLabel.maxLineCount, textLabel.supportEncoding, textLabel.symbolStyle).Split(mSeparator);
			mTotalLines = 0;
			Int32 i = 0;
			Int32 count = mParagraphs.Count;
			while (i < count)
			{
				mTotalLines += mParagraphs[i].lines.Length;
				i++;
			}
		}
		if (updateVisible)
		{
			UpdateVisibleText();
		}
	}

	private void Awake()
	{
		if (textLabel == null)
		{
			textLabel = GetComponentInChildren<UILabel>();
		}
		if (textLabel != null)
		{
			textLabel.lineWidth = 0;
		}
		Collider collider = this.collider;
		if (collider != null)
		{
			if (maxHeight <= 0f)
			{
				maxHeight = collider.bounds.size.y / transform.lossyScale.y;
			}
			if (maxWidth <= 0f)
			{
				maxWidth = collider.bounds.size.x / transform.lossyScale.x;
			}
		}
	}

	private void OnSelect(Boolean selected)
	{
		mSelected = selected;
	}

	protected void UpdateVisibleText()
	{
		if (textLabel != null)
		{
			UIFont font = textLabel.font;
			if (font != null)
			{
				Int32 num = 0;
				Int32 num2 = (maxHeight <= 0f) ? 100000 : Mathf.FloorToInt(maxHeight / textLabel.cachedTransform.localScale.y);
				Int32 num3 = Mathf.RoundToInt(mScroll);
				if (num2 + num3 > mTotalLines)
				{
					num3 = Mathf.Max(0, mTotalLines - num2);
					mScroll = num3;
				}
				if (style == Style.Chat)
				{
					num3 = Mathf.Max(0, mTotalLines - num2 - num3);
				}
				StringBuilder stringBuilder = new StringBuilder();
				Int32 i = 0;
				Int32 count = mParagraphs.Count;
				while (i < count)
				{
					Paragraph paragraph = mParagraphs[i];
					Int32 j = 0;
					Int32 num4 = paragraph.lines.Length;
					while (j < num4)
					{
						String value = paragraph.lines[j];
						if (num3 > 0)
						{
							num3--;
						}
						else
						{
							if (stringBuilder.Length > 0)
							{
								stringBuilder.Append("\n");
							}
							stringBuilder.Append(value);
							num++;
							if (num >= num2)
							{
								break;
							}
						}
						j++;
					}
					if (num >= num2)
					{
						break;
					}
					i++;
				}
				textLabel.text = stringBuilder.ToString();
			}
		}
	}

	private void OnScroll(Single val)
	{
		if (mSelected && supportScrollWheel)
		{
			val *= ((style != Style.Chat) ? -10f : 10f);
			mScroll = Mathf.Max(0f, mScroll + val);
			UpdateVisibleText();
		}
	}

	public enum Style
	{
		Text,
		Chat
	}

	protected class Paragraph
	{
		public String text;

		public String[] lines;
	}
}
