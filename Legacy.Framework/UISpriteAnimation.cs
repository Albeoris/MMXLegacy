using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/UI/Sprite Animation")]
[ExecuteInEditMode]
public class UISpriteAnimation : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	private Int32 mFPS = 30;

	[SerializeField]
	[HideInInspector]
	private String mPrefix = String.Empty;

	[SerializeField]
	[HideInInspector]
	private Boolean mLoop = true;

	private UISprite mSprite;

	private Single mDelta;

	private Int32 mIndex;

	private Boolean mActive = true;

	private List<String> mSpriteNames = new List<String>();

	public Int32 frames => mSpriteNames.Count;

    public Int32 framesPerSecond
	{
		get => mFPS;
        set => mFPS = value;
    }

	public String namePrefix
	{
		get => mPrefix;
	    set
		{
			if (mPrefix != value)
			{
				mPrefix = value;
				RebuildSpriteList();
			}
		}
	}

	public Boolean loop
	{
		get => mLoop;
	    set => mLoop = value;
	}

	public Boolean isPlaying => mActive;

    private void Start()
	{
		RebuildSpriteList();
	}

	private void Update()
	{
		if (mActive && mSpriteNames.Count > 1 && Application.isPlaying && mFPS > 0f)
		{
			mDelta += Time.deltaTime;
			Single num = 1f / mFPS;
			if (num < mDelta)
			{
				mDelta = ((num <= 0f) ? 0f : (mDelta - num));
				if (++mIndex >= mSpriteNames.Count)
				{
					mIndex = 0;
					mActive = loop;
				}
				if (mActive)
				{
					mSprite.spriteName = mSpriteNames[mIndex];
					mSprite.MakePixelPerfect();
				}
			}
		}
	}

	private void RebuildSpriteList()
	{
		if (mSprite == null)
		{
			mSprite = GetComponent<UISprite>();
		}
		mSpriteNames.Clear();
		if (mSprite != null && mSprite.atlas != null)
		{
			List<UIAtlas.Sprite> spriteList = mSprite.atlas.spriteList;
			Int32 i = 0;
			Int32 count = spriteList.Count;
			while (i < count)
			{
				UIAtlas.Sprite sprite = spriteList[i];
				if (String.IsNullOrEmpty(mPrefix) || sprite.name.StartsWith(mPrefix))
				{
					mSpriteNames.Add(sprite.name);
				}
				i++;
			}
			mSpriteNames.Sort();
		}
	}

	public void Reset()
	{
		mActive = true;
		mIndex = 0;
		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			mSprite.MakePixelPerfect();
		}
	}
}
